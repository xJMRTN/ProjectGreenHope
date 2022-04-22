using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] RaftController raftController;
    [SerializeField] RaftSteering raftSteering;
    [SerializeField] SailControls sailControls;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject _camera;
    [SerializeField] GameObject CrashSite;
    [SerializeField] float cameraShakeAmount;
    [SerializeField] Transform island;
    [SerializeField] Transform RespawnPoint;
    [SerializeField] float distanceToIsland;

    bool introOver = false;
    bool movementSet = false;

    void Start(){
        WindManager.Instance.IntroMovement();
        raftController.anchor = true;
        LightingManager.Instance.Intro();
       
    }

    void FixedUpdate(){
        if(introOver) return;
        if(!movementSet) SetPlayerMovement();
        raftSteering.IntroMovement();
        sailControls.IntroMovement();
        raftController.IntroMovement();
        UtilityManager.Instance.ShakeCamera(0.05f);
        CheckIslandDistance();
    }

    void SetPlayerMovement(){
        movementSet = true;
        playerMovement.Intro();
    }

    void CheckIslandDistance(){
        float distance = Vector3.Distance(playerMovement.gameObject.transform.position, island.position);
        if(distance < distanceToIsland){
            EndIntro();
        }
    }

    void EndIntro(){
        introOver = true;
        UtilityManager.Instance.BlackScreen();
        island.Translate(0f, 40f, 0f);       
        CrashSite.SetActive(true);
        playerMovement.Reset();
        LightingManager.Instance.Reset();
        WindManager.Instance.Reset();
        playerMovement.transform.parent.transform.position = RespawnPoint.position;
        playerMovement.transform.parent.transform.parent = null;
        Destroy(raftController.gameObject);      
        StartCoroutine(UtilityManager.Instance.ScreenFade(0.2f, false, 2f));
    }
}