using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData : SaveData
{
   public int hp { get; set; }
    public Transform playerTransform;
   public PlayerSaveData()
    {

    }
    public PlayerSaveData(int index, Player player)
    {
        this.index = index;
        hp = player.hp;
        playerTransform = player.transform;
    }
    public PlayerSaveData(int index,int hp)
    {
        this.index = index;
        this.hp = hp;
    }
 }

public class SaveManager : Singleton<SaveManager>
{
    [SerializeField] Player player;
    private PlayerSaveData jsonSave;
    private PlayerSaveData jsonLoad;
    public void SaveJson1()
    {
        jsonSave = new(0,player);
        SaveDataController.Save(jsonSave,0);
    }
    public void SaveJson2()
    {
        jsonSave = new(1, player);
        SaveDataController.Save(jsonSave,1);
    }
    public void SaveJson3()
    {
        jsonSave = new(2, player);
        SaveDataController.Save(jsonSave,2);
    }
    public void LoadJson1()
    {
        jsonLoad = new(0, 0);
        SaveDataController.Load(ref jsonLoad);
    }
    public void LoadJson2()
    {
        jsonLoad = new(1, 0);
        SaveDataController.Load(ref jsonLoad);
    }
    public void LoadJson3()
    {
        jsonLoad = new(2, 0);
        SaveDataController.Load(ref jsonLoad);
    }
    public void PlayerDataLoad()
    {
        player.hp = jsonLoad.hp;
        player.transform.position = jsonLoad.playerTransform.position;
        player.transform.rotation = jsonLoad.playerTransform.rotation;
    }
    public bool CheckSaveNull()
    {
        if(jsonSave==null)
        {
            return true;
        }
        return false;
    }
    public bool CheckLoadNull()
    {
        if(jsonLoad==null)
        {
            return true;
        }
        return false;
    }
}
