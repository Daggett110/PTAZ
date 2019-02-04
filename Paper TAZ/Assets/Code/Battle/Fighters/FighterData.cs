using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FighterData", menuName = "PTAZ/Battle/FighterData", order = 1)]
public class FighterData : ScriptableObject
{
    [Header("Aesthetic")]
    public string FighterName = "DEFAULT NAME";
    [Header("Stats")]
    public int MaxHealth = 1;
    public int Defense = 0;
    public int AP = 5;
    public int AttackTurns = 1;
    public int ItemTurns = 1;

    public List<AttackData> AttackList = new List<AttackData>();
}

public class AttackData
{
    [Header("Attack")]
    public string AttackName;
    public int Power = 1;
    public BattleTargetting.TargetSelection TargetSelection = BattleTargetting.TargetSelection.Enemy;
    public BattleTargetting.TargetPosition TargetPosition = BattleTargetting.TargetPosition.Any;
    public BattleTargetting.TargetCount TargetCount = BattleTargetting.TargetCount.Single;
    public BattleTargetting.AttackAttribute Attributes = BattleTargetting.AttackAttribute.None;
}
