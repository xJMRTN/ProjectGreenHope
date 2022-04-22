using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public sealed class SaveLoad : MonoBehaviour
{
    public static List<GameData> savedGames = new List<GameData>();
    private static SaveLoad instance = new SaveLoad();    
    public static int buildIndex;

    void Awake(){
        if(instance == null) {
            instance = this;         
        }
        buildIndex = GetCurrentScene();
    }

    static SaveLoad(){
        
    }

    private SaveLoad(){
        
    }

    public static int GetCurrentScene(){  
        int sceneID =  UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        return sceneID;
    }

    public static SaveLoad Instance{
        get{return instance;}
    }

    public static string GetDate(){
        DateTimeOffset offsetDate;
        DateTime regularDate;

        offsetDate = DateTimeOffset.Now;
        regularDate = offsetDate.DateTime;
        return regularDate.ToString();
    }

    public static void Save(){              
        GameData.current.GameDate = GetDate();
        savedGames.Add(GameData.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SkiesAbove.qf");
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
        savedGames.Remove(GameData.current);
    }

    public static void Load(){
        if(File.Exists(Application.persistentDataPath + "/SkiesAbove.qf")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SkiesAbove.qf", FileMode.Open);
            SaveLoad.savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void Load(bool menu){
        if(File.Exists(Application.persistentDataPath + "/SkiesAbove.qf")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SkiesAbove.qf", FileMode.Open);
            SaveLoad.savedGames = (List<GameData>)bf.Deserialize(file);
            file.Close();
        }
    }
}   