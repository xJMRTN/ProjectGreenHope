using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WindManager : MonoBehaviour
{
    private static WindManager instance = new WindManager();
    [SerializeField] GameObject WindParticle;
    [SerializeField] float WindAmount;
    [SerializeField] float StormAmount;
    public float WindStrength;
    public float StormStrength;
    [SerializeField] float TimeBetweenWindChanges;
    [SerializeField] float TimeBetweenStormWindChanges;
    [SerializeField] float windSpawnRadius;
    public Transform WindDirection;       
    [SerializeField] Transform PlayerPosition;

    [SerializeField] Material stormWeather;
    [SerializeField] Material normalWeather;
    [SerializeField] MeshRenderer clouds;

    float maxTimeBetweenWindChanges;
    float maxTimeBetweenStormWindChanges;
    bool canSpawnWind = true;
    public bool isStorm;

    public void Awake(){
        if(instance == null) {
            instance = this;        
        }
    }

    static WindManager(){
    }

    private WindManager(){

    }

    public static WindManager Instance{
        get{return instance;}   
    }

    void Start(){
        maxTimeBetweenWindChanges = TimeBetweenWindChanges;
        maxTimeBetweenStormWindChanges = TimeBetweenStormWindChanges;
    }

    void Update(){
        if(canSpawnWind) StartCoroutine(SpawnWind());
        CheckWindDirection();
    }

    IEnumerator SpawnWind(){
        canSpawnWind = false;
        Instantiate(WindParticle, RandomPositionAroundPlayer(), WindDirection.rotation * Quaternion.Euler(0, -90f, 0));
        if(isStorm)yield return new WaitForSeconds(StormAmount);    
        else yield return new WaitForSeconds(WindAmount);     
        canSpawnWind = true;
    }

    Vector3 RandomPositionAroundPlayer(){
        Vector3 positionToSpawn = new Vector3();
        positionToSpawn = (Vector3)Random.insideUnitCircle * windSpawnRadius;  
        positionToSpawn = new Vector3 (transform.position.x +positionToSpawn.x, transform.position.y + Random.Range(-10f, 10f), transform.position.z +positionToSpawn.y);        
        return positionToSpawn;
    }

    void CheckWindDirection(){
        if(isStorm){        
            TimeBetweenStormWindChanges-= Time.deltaTime;
            if(TimeBetweenStormWindChanges < 0f) ChangeRotation();
        }else{
            TimeBetweenWindChanges-= Time.deltaTime;
            if(TimeBetweenWindChanges < 0f) ChangeRotation();
        }
    }

    void ChangeRotation(){
        WindDirection.Rotate(Random.Range(-30f, 30f), Random.Range(0f, 360f), Random.Range(-30f, 30f));
        if(isStorm)TimeBetweenStormWindChanges = Random.Range(maxTimeBetweenStormWindChanges - 0.2f, maxTimeBetweenStormWindChanges + 0.2f);
        else TimeBetweenWindChanges = Random.Range(maxTimeBetweenWindChanges - 5f, maxTimeBetweenWindChanges + 5f);
    }

    public void IntroMovement(){
        clouds.material = stormWeather;
        isStorm = true;
    }

    public void Reset(){
        clouds.material = normalWeather;
        isStorm = false;
    }
}