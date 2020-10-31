using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleData_", menuName = "PTAZ/Battle/BattleData", order = 1)]
public class BattleData : ScriptableObject
{
    public List<FighterData> EnemyData = new List<FighterData>();
    public string locationID;
    public string specialConditionDescription;

}
