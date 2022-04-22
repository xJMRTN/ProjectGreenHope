using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public sealed class BuildManager : MonoBehaviour
{
    public Buildable[] buildables;
    public Dictionary<string, Buildable> buildableList = new Dictionary<string, Buildable>();
    private static BuildManager instance = new BuildManager();
    private Buildable currentBuilding;
    public bool isBuilding = false;
    private GameObject building;
    public GameObject buildUI;
    public GameObject popupUI;
    public Transform layoutTransform;
    public GameObject slot;
     public float snapDistance;
    public Text itemName;
    public Text itemDesc;

    [SerializeField] PlayerMovement player;

    bool inUI = false;
    private Transform lastRotation;

    public List<GameObject> chests = new List<GameObject>();

     public List<Transform> itemsBuilt = new List<Transform>();
     private Transform[] currentPivots;
     private Transform buildingSnap;
     private Transform snappedPosition;
     private float pivotDistance;
     int currentScenePos;

     public List<Building> buildingsCreated = new List<Building>();
     public List<GameObject> buildingsCreatedObj = new List<GameObject>();

    bool snapped = false;
    bool canSnap = false;

    private List<Building> buildList = new List<Building>();
    private List<ChestData> chestList = new List<ChestData>();

    void Awake(){
        if(instance == null) {
            instance = this;    
            SetupDictionaries();               
        }
    }

    void Start(){ 
        currentScenePos = SaveLoad.GetCurrentScene() - 1;
        if(GameData.current != null){
            LoadWorld() ;       
            LoadBlueprints(GameData.current.buildingData.blueprints);
        }
        SetupMenu();   
    }

    void LoadWorld(){
        LoadLists();

        if(buildList.Count > 0) {           
            BuildLoadedWorld(buildList);
        }

        if(chestList.Count > 0) {
            LoadChestData(chestList);             
        }      
    }

    void LoadLists(){
        if(GameData.current.buildingData.buildingsCreated[currentScenePos] != null) {           
            buildList = GameData.current.buildingData.buildingsCreated[currentScenePos];
        }
        if(GameData.current.inventoryData.chestsCreated[currentScenePos] != null) {
            chestList = GameData.current.inventoryData.chestsCreated[currentScenePos];             
        }  
    }

    static BuildManager(){
        
    }

    private BuildManager(){
        
    }

    public static BuildManager Instance{
        get{return instance;}
    }

    public void SetupDictionaries(){
        foreach(Buildable item in buildables){
            string key = item.BuildItem.displayName;
            if(!buildableList.ContainsKey(key)) {
                buildableList.Add(key, item);
            }
        }
    }

    public void SetupMenu(){
        for(int x = 0; x < buildables.Length; x++){
            if(buildables[x].unlocked){
                GameObject newSlot = Instantiate(slot, layoutTransform);
                newSlot.GetComponent<CraftingSlot>().Set(buildables[x]);
            }        
        }
    }

    public void Update(){
        if(isBuilding){
            UpdateBuildingPosition();
            if(canSnap) LookForSnaps();
            if(Input.GetKeyDown(KeyCode.R)){
                RotateItem();
            }
            if(Input.GetKeyDown(KeyCode.B)){
                ToggleUI();
           } 
           if(Input.GetKeyDown(KeyCode.Escape)){
                ToggleBuilding();
           } 
        }else{
           if(Input.GetKeyDown(KeyCode.B)){
                ToggleUI();
           } 
        }
    }

    public void ToggleUI(){
        inUI = !inUI;
        if(inUI) {
            player.ToggleMouse(false);
            LeanTween.moveLocal(buildUI, new Vector3(-710, 0, 0), .9f).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutElastic);
            LeanTween.moveLocal(popupUI, new Vector3(375, 223, 0), .9f).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutElastic).setDelay(1.5f);
        }
        else{
            player.ToggleMouse(true);           
            LeanTween.moveLocal(popupUI, new Vector3(45, 223, 0), .8f).setIgnoreTimeScale(true);
            LeanTween.moveLocal(buildUI, new Vector3(-1250, 0, 0), .8f).setIgnoreTimeScale(true).setDelay(1.5f);
        } 
        if(isBuilding )ToggleBuilding();
    }

    public void LookForSnaps(){
        if(!snapped){
            foreach(Transform t in currentPivots ){
                buildingSnap = t;
                foreach(Transform _t in itemsBuilt ){
                    if(_t == null) break;
                    pivotDistance = Vector3.Distance(t.position, _t.position);
                    if(pivotDistance < snapDistance - .5f){                       
                        snappedPosition = _t;
                        SnapItem();
                        return;
                    }
                }
            }
        }       
    }

    public void SnapItem(){
        snapped = true;
        float xDifference = buildingSnap.position.x - snappedPosition.position.x;
        float zDifference = buildingSnap.position.z - snappedPosition.position.z;
        building.transform.Translate(-xDifference, 0, -zDifference, Space.World);
    }

    public void UnSnap(){
        snapped = false;
        snappedPosition = null;    
    }

    public void UpdateBuildingPosition(){
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit)) {
            if(snapped){
                if(Vector3.Distance(snappedPosition.position, hit.point) > snapDistance) {
                    building.transform.position = new Vector3(hit.point.x, -1f ,hit.point.z);
                    UnSnap();
                }
            }else{
                building.transform.position = new Vector3(hit.point.x, -1f ,hit.point.z);
            }           
         }
    }

    public bool HasResources(){
        int length = currentBuilding.BuildResources.Length;
        bool canBuild = true;
        for(int x = 0; x < length; x++){
            if(!InventoryManager.Instance.HasEnoughResources(currentBuilding.BuildResources[x].item, currentBuilding.BuildResources[x].amount)) {            
                canBuild = false;
            }
        }
        return canBuild;
    }

    public void TakeResources(){
        int length = currentBuilding.BuildResources.Length;
        for(int x = 0; x < length; x++){
            InventoryManager.Instance.Remove(currentBuilding.BuildResources[x].item, currentBuilding.BuildResources[x].amount);
        }
    }

    public void BuildItem(){
        if(HasResources()) {
            TakeResources();
            PlaceItem();
            UnSnap();
        }
    }

    public void RotateItem(){
        building.transform.Rotate(0, 45.0f, 0.0f);
        if(snapped) SnapItem();
    }

    public void PlaceItem(){
        if(building.tag != "Floor") building.layer = 0;

        lastRotation = building.transform;

        if(building.GetComponent<MeshRenderer>()) building.GetComponent<MeshRenderer>().material = currentBuilding.BuildItem.itemMaterial;

        foreach(Transform t in building.GetComponent<PivotPoints>().Pivots){
            itemsBuilt.Add(t);
        }    

        float[] temp = new float[3];
        temp[0] = building.transform.position.x;
        temp[1] = building.transform.position.y;
        temp[2] = building.transform.position.z;

        float[] rot = new float[3];
        rot[0] = building.transform.eulerAngles.x;
        rot[1] = building.transform.eulerAngles.y;
        rot[2] = building.transform.eulerAngles.z;

        Building newBuilding = new Building(currentBuilding.BuildItem.displayName, temp, rot);
        buildingsCreated.Add(newBuilding);
        buildingsCreatedObj.Add(building);

        if(building.GetComponent<Chest>()){
            chests.Add(building);
        }

        building = null;
        CreateBuilding(currentBuilding);
    }

    public void SetNewItem(string itemName){
        if(buildableList.TryGetValue(itemName, out Buildable value)){
            currentBuilding = value;
            CreateBuilding(currentBuilding);       
        }
    }

    public void CreateBuilding(Buildable value){      
        if(building) Destroy(building);
        building = Instantiate(currentBuilding.BuildItem.prefab);
        if(lastRotation) building.transform.rotation = lastRotation.rotation;
        currentPivots = building.GetComponent<PivotPoints>().Pivots;
        if(!HasResources()){
           if(building.GetComponent<MeshRenderer>()) building.GetComponent<MeshRenderer>().material = value.BuildItem.blockedMaterial;
            canSnap = false;
        } 
        else {
            if(building.GetComponent<MeshRenderer>())building.GetComponent<MeshRenderer>().material = value.BuildItem.goodMaterial;
            canSnap = true;
        }
        if(!isBuilding) isBuilding = ToggleBuilding();
    }

    // public void SetCostText(Buildable value, Transform pos){
    //     if(value != null){
    //         itemYield.text = "";
    //         popup.transform.position = pos.position;
    //         itemName.text =value.BuildItem.displayName;
    //         itemDesc.text = value.BuildItem.description;
    //         int length = value.BuildResources.Length;
    //         for(int x = 0; x < length; x++){
    //             itemYield.text +=  value.BuildResources[x].amount + "x " + value.BuildResources[x].item.displayName;
    //             if(x < length - 1)itemYield.text += " + ";
    //         }
    //          popup.SetActive(true);
    //     }   else  popup.SetActive(false);  
    // }

    public bool ToggleBuilding(){      
        isBuilding = !isBuilding;
        if(!isBuilding && building) {
            Destroy(building);
            lastRotation = null;
        }
        return isBuilding;
    }

    public void BuildLoadedWorld(List<Building> _create){
        int chestsCreatedInt = 0;
        foreach(Building b in _create){    
            if(!b.isDestroyed) {     
                if(buildableList.TryGetValue(b.id, out Buildable value)){
                    currentBuilding = value;
                GameObject newBuilding = Instantiate(currentBuilding.BuildItem.prefab);
                newBuilding.transform.position = new Vector3(b.position[0],b.position[1],b.position[2]);
                newBuilding.transform.rotation = Quaternion.Euler(b.rotation[0],b.rotation[1],b.rotation[2]);

                    foreach(Transform t in newBuilding.GetComponent<PivotPoints>().Pivots){
                        itemsBuilt.Add(t);
                    }  

                    if(newBuilding.GetComponent<Chest>()){
                        chests.Add(newBuilding);
                        chestsCreatedInt++;
                    }   

                    if(newBuilding.tag != "Floor") newBuilding.layer = 0;

                    Building _building = new Building(value.BuildItem.displayName, b.position, b.rotation);
                    buildingsCreated.Add(_building);
                    buildingsCreatedObj.Add(newBuilding);
                    
                }
            }
            else{
                //handle building that was destroyed
                if(buildableList.TryGetValue(b.id, out Buildable value)){
                    currentBuilding = value;

                    if(currentBuilding.BuildItem.prefab.GetComponent<Chest>()){
                        chestList.RemoveAt(chestsCreatedInt);
                    } 
                }  
            }
        }
    }

    public void LoadChestData(List<ChestData> _create){
        int x = 0;
        foreach(GameObject chestObject in chests){       
            Chest _chest  = chestObject.GetComponent<Chest>(); 
            _chest.LoadChestData(_create[x].inventory);    
            x++;
        }
    }

    public void LoadBlueprints(List<Blueprint> blueprints){
        foreach(Blueprint blueprint in blueprints){
             if(buildableList.TryGetValue(blueprint.BuildingID, out Buildable value)){
                 value.unlocked = blueprint.unlocked;
             }
        }
    }

    public List<Blueprint> SaveBlueprints(){
        List<Blueprint> bp = new List<Blueprint>();
        foreach(KeyValuePair<string,Buildable>  buildable in buildableList){
            Blueprint newBP = new Blueprint(buildable.Key, buildable.Value.unlocked);
            bp.Add(newBP);
        }
        return bp;
    }

    public void BuyBlueprint(string ID){
        if(buildableList.TryGetValue(ID, out Buildable value)){
            value.unlocked = true;
            GameObject newSlot = Instantiate(slot, layoutTransform);
        }
    }
}

[System.Serializable]
public class Buildable{
    public InventoryItem BuildItem;
    public BuildCost[] BuildResources;
    public bool unlocked = false;
    public int blueprintCost;
}

[System.Serializable]
public class BuildCost{
    public InventoryItem item;
    public int amount;
}