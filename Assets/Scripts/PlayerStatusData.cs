using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStatusData : ScriptableObject
{
    public List<PlayerStatus> PlayerStatusList = new List<PlayerStatus>();
}


[System.Serializable]
public class PlayerStatus
{
    public string name = "名前";
    public int HP = 100, atk = 10, def = 10, spd = 10;

}
