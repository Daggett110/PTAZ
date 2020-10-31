using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FighterData", menuName = "PTAZ/Battle/FighterData", order = 2)]
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

    [Header("Statuses and Attributes")]
    public BattleTargetting.TargetElevation startingElevation = BattleTargetting.TargetElevation.Floor;

    [Header("References")]
    public GameObject FighterPrefab;

    public BattleTargetting.FighterAttribute fighterAttributes;

    public List<AttackData> AttackList = new List<AttackData>();
}

/// <summary>
/// Actions are things fighters can use to attack enemies/help allies (sword/heal/buff/etc)
/// </summary>
[System.Serializable]
public class Action : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public TargetingData TargetData;
}

[System.Serializable]
public class TargetingData
{
    public BattleTargetting.TargetSelection TargetSelection = BattleTargetting.TargetSelection.Enemy;
    public BattleTargetting.TargetPosition TargetPosition = BattleTargetting.TargetPosition.Front;
    public BattleTargetting.TargetElevation TargetElevation = BattleTargetting.TargetElevation.Floor;
    public BattleTargetting.TargetStatus TargetStatus = BattleTargetting.TargetStatus.Active;
    public BattleTargetting.ActionAttribute Attributes = BattleTargetting.ActionAttribute.None;
    public BattleTargetting.TargetCount TargetCount = BattleTargetting.TargetCount.Single;
    public bool ExcludeSelf = false;
}