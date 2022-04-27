using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCombo : MonoBehaviour
{
    [SerializeField] ButtonControls[] buttons;
    [SerializeField] Light[] lights;
    [SerializeField] GameObject ladder;
    [SerializeField] Color offColour;
    [SerializeField] Color onColour;

    [SerializeField] Transform ladderPos;
    [SerializeField] float ladderSpeed;
    
    bool complete = false;

    void Update(){
        if(complete){
            float step =  ladderSpeed * Time.deltaTime;
            ladder.transform.position = Vector3.MoveTowards(ladder.transform.position, ladderPos.position, step);
        }
    }

    public void ButtonPressed(int buttonValue){
        if(complete) return;
        
        foreach(Light l in buttons[buttonValue].lights){
            if(l.color == offColour) l.color = onColour;
            else l.color = offColour;
        }

        if(CheckIfComplete())
            PuzzleComplete();

    }

    void PuzzleComplete(){
        complete = true;
    }

    bool CheckIfComplete(){
        foreach(Light l in lights){
            if(l.color != onColour) return false;
        }
        return true;
    }
}

[System.Serializable]
public class ButtonControls{
    public GameObject button;
    public Light[] lights;
}