using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : MonoBehaviour
{
    [SerializeField] ButtonCombo buttonCombo;

    [SerializeField] enum Puzzle{
        ButtonCombo
    }

    [SerializeField] Puzzle puzzle;

    [SerializeField] int id;

    void Interact(){          
        switch(puzzle){
            case Puzzle.ButtonCombo:
                buttonCombo.ButtonPressed(id);
                break;
        }
    }
}