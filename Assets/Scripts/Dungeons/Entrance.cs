using System.Collections;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            StartCoroutine(MovePlayerToDungeon());
        }
    }

    IEnumerator MovePlayerToDungeon(){
        StartCoroutine(UtilityManager.Instance.ScreenFade(0.6f, true, 0f));
        UtilityManager.Instance.FreezePlayer(true);
        yield return new WaitForSeconds(2f);
        UtilityManager.Instance.ChangeScene();
    }
}