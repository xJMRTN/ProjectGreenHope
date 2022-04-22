using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class BalloonLever : MonoBehaviour
{
    bool holdingLever = false;
    [SerializeField] float leverMoveSpeed;
    [SerializeField] GameObject floatingText;
    [SerializeField] Transform progressText;
    [SerializeField] Transform canvas;
    public float power;
    bool hasText = false;
    GameObject _text;
    TextMeshProUGUI progress;

    void Start(){
        SetupText();
        power = CalculatePower();
    }

    void OnMouseDown(){
        if(!EventSystem.current.IsPointerOverGameObject()){
            if(UtilityManager.Instance.isPlayerCloseEnough(transform)){
                 if(!hasText) SetupText();
                 holdingLever = true;
                 _text.SetActive(holdingLever); 
            }               
        }
    }

    void OnMouseUp(){
       holdingLever = false;
       _text.SetActive(holdingLever); 
    }

    void Update(){
        if(holdingLever) ManageInput();
    }

    void ManageInput(){
        if(Input.GetAxis("Mouse Y")>0){
            transform.Rotate(0, 0, -leverMoveSpeed * Time.deltaTime);
        }

        if(Input.GetAxis("Mouse Y")<0){
            transform.Rotate(0, 0, leverMoveSpeed * Time.deltaTime);
        }
        
        if(transform.eulerAngles.z < 30f)transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,30f);
        if(transform.eulerAngles.z > 150f)transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,150f);

        power = CalculatePower();
        progress.SetText(power.ToString("F0") + "%"); 
    }

    float CalculatePower(){
        float currentValue = transform.eulerAngles.z - 30f;
        currentValue = currentValue/120f;
        currentValue *= 100;
        currentValue = 100- currentValue;       
        return currentValue;
    }

    void SetupText(){
        hasText = true;
        _text = Instantiate(floatingText, canvas);
        progress = _text.GetComponent<TextMeshProUGUI>();
        _text.GetComponent<FloatingName>().Target = progressText;
        _text.SetActive(holdingLever);
    }
}