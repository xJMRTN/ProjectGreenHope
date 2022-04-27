using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class UtilityManager : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] PlayerMovement PlayerMovement;
    [SerializeField] Camera _camera;
    [SerializeField] float PlayerReach;
    public RawImage blackBox;

    private static UtilityManager instance = new UtilityManager();

    public void Awake(){
        if(instance == null) {
            instance = this;        
        }
    }

    static UtilityManager(){
    }

    private UtilityManager(){

    }

    public static UtilityManager Instance{
        get{return instance;}   
    }

    public bool isPlayerCloseEnough(Transform _object){        
        float temp = Vector3.Distance(Player.position, _object.position);
        Debug.Log("Distance: " + temp);
        return (temp <= PlayerReach);
    }

    public void RaycastFromPlayer(){
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity)){      
            Debug.Log("Hit: " + hit.transform.name);  
            if(isPlayerCloseEnough(hit.transform)){
                Debug.Log("Hit: " + hit.transform.name);
                hit.transform.gameObject.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
            }
        }else{
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 10, Color.blue);
        }
    }

    public Vector3 ShootRayCastDown(Vector3 startPos){
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(startPos, -Vector3.up, out hit)){
            if(hit.transform.tag == "Ground"){
                return hit.point;
            }
        }
        return Vector3.zero;
    }

    public void ShakeCamera(float ShakeAmount){
        _camera.transform.localPosition = new Vector3(
            Random.Range(-1f, 1f) * ShakeAmount,
            Random.Range(-1f, 1f) * ShakeAmount,
            _camera.transform.localPosition.z
        );
    }

    public IEnumerator ScreenFade(float FadeRate, bool fadeIn, float delay){
        float targetAlpha;
        if(fadeIn) targetAlpha = 1.0f;
        else targetAlpha = 0.0f;

        yield return new WaitForSeconds(delay);
          
        Color curColor = blackBox.color;
        while(Mathf.Abs(curColor.a - targetAlpha) > 0.0001f) {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
            blackBox.color = curColor;
            yield return null;
        }
    }

    public void BlackScreen(){
        Color curColor = blackBox.color;
        curColor.a = 1f;
        blackBox.color = curColor;
    }

    public void FreezePlayer(bool state){
        PlayerMovement.Frozen = state;
    }

    public void ChangeScene(){
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}