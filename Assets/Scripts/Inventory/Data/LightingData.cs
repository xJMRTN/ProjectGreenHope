using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LightingData
{
    public float TimeOfDay;

    public void GetData(){
       TimeOfDay = LightingManager.Instance.TimeOfDay;
    }
}