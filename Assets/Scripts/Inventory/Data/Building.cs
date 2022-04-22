using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building
{
    public string id;
    public float[] position;
    public float[] rotation;
    public float respawnTime;
    public bool isPooled;
    public bool isDestroyed;

    public Building(string _value, float[] _pos, float[] _rot){
        id = _value;
        position = _pos;
        rotation = _rot;
    }

    public Building(string _value, float[] _pos, float _respawn, bool _pool, bool _destroyed){
        id = _value;
        position = _pos;
        respawnTime = _respawn;
        isPooled = _pool;
        isDestroyed = _destroyed;
    }

    public Building(string _value, float[] _pos){
        id = _value;
        position = _pos;
    }
}