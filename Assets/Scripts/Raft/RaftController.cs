using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftController : MonoBehaviour
{
    [SerializeField] Balloon[] balloons;
    [SerializeField] float thrust = 2f;
    [SerializeField] float forwardSpeed = 2f;
    [SerializeField] float maxTurningSpeed = 4f;
    [SerializeField] float turningSpeed = 1f;
    [SerializeField] GameObject Raft;
    [SerializeField] GameObject Player;

    public bool anchor = false;

    float maxThrust;
    float balloonThrust;


    float rotationAmount = 0f;

    void FixedUpdate (){
        if(anchor) return;
        maxThrust = GetMaxThrust();
        balloonThrust = GetAverageBalloonPower();
        ApplyRotation();
        ForwardForce();
        UpwardsForce();
        ApplyWindForce();
    }

    void ApplyRotation(){
        Raft.transform.Rotate(0,rotationAmount * Time.deltaTime ,0, Space.World);
        Player.transform.Rotate(0,-rotationAmount * Time.deltaTime ,0, Space.World);
    }

    void ForwardForce(){
        Raft.transform.Translate(-Raft.transform.forward * Time.deltaTime * forwardSpeed, Space.World);
    }

    void ApplyWindForce(){
        if(WindManager.Instance.isStorm)Raft.transform.Translate(WindManager.Instance.WindDirection.forward * Time.deltaTime * WindManager.Instance.StormStrength, Space.World);
        else Raft.transform.Translate(WindManager.Instance.WindDirection.forward * Time.deltaTime * WindManager.Instance.WindStrength, Space.World);       
    }

    void UpwardsForce(){
        Raft.transform.Translate(Vector3.up * Time.deltaTime * balloonThrust, Space.World);
    }

    public void ChangeRotation(float amount){
        amount /= 500f;
        amount *= turningSpeed;
        if(amount > maxTurningSpeed) amount = maxTurningSpeed;
        if(amount < -maxTurningSpeed) amount = -maxTurningSpeed;
        rotationAmount = amount;
    }

    float GetAverageBalloonPower(){
        float value = 0;
        foreach(Balloon balloon in balloons){
            value += balloon.lever.power;
        }
        value /= balloons.Length; 
        value = maxThrust * (value/ 100f); //% of all balloons power.
        
        value = value - (maxThrust /2) ; 
          Debug.Log("Max Thrust: " + maxThrust/2 + "... Thrust using: " + value);  
        return value;
    }

    float GetMaxThrust(){
        return balloons.Length * thrust;
    }

    public void MoveTowardsAnchor(Transform targetPos, float _speed){        
        Vector3 direction = (targetPos.position - Raft.transform.position).normalized;
        Raft.transform.Translate(direction * Time.deltaTime * _speed, Space.World);
    }

    public void IntroMovement(){
        Raft.transform.Translate(new Vector3(-0.2f, -5f, -2f) * Time.deltaTime * 2f, Space.World);
    }
}