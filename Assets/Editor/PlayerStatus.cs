using UnityEngine;


[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Assets/NewPlayerData")]
public class PlayerStatus : ScriptableObject
{
    public string id, engName, jpName;
    public int hp, atk, def, spd;
    public string skill1, skill2, skill3, skill4;
}