using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingName : MonoBehaviour
{
    public Transform Target;
    
    void Update()
    {
        Vector3 namePos = Camera.main.WorldToScreenPoint(Target.position + new Vector3(0, 0.5f, 0));
        this.transform.position = namePos;
    }
}