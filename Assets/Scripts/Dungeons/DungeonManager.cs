using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{   
    void Start()
    {
        StartCoroutine(UtilityManager.Instance.ScreenFade(0.3f, false, 2f));
    }
}