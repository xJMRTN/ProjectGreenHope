using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public sealed class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
     public float TimeOfDay;
    [SerializeField] private float speed;

    private static LightingManager instance = new LightingManager();

    public void Awake(){
        if(instance == null) {
            instance = this;        
        }

        if(DirectionalLight != null)
        return;
        

        if(RenderSettings.sun != null){
            DirectionalLight = RenderSettings.sun;
        }else{
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights){
                if(light.type == LightType.Directional){
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

    static LightingManager(){
    }

    private LightingManager(){

    }

    public static LightingManager Instance{
        get{return instance;}   
    }

    private void UpdateLighting(float timePercent){
        RenderSettings.ambientLight = Preset.AmbientColour.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColour.Evaluate(timePercent);
        if(DirectionalLight != null){
            DirectionalLight.color = Preset.DirectionalColour.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void Update(){
        if(Preset == null)
            return;

        TimeOfDay += Time.deltaTime * speed;
        TimeOfDay %= 24;
        UpdateLighting(TimeOfDay / 24f);
    }

    public void Intro(){
        //maybe TOD 4f
        TimeOfDay = 19f;
        UpdateLighting(TimeOfDay / 24f);
        speed = 0f;
    }

    public void Reset(){
        TimeOfDay = 12f;
        UpdateLighting(TimeOfDay / 24f);
        speed = 0.02f;
    }
}