using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that Handles Targetting for all Battle Scenarios in terms of what targets what
/// </summary>
public static class BattleTargetting
{
    #region Enums and Flags
    
    public enum TargetSelection
    {
        Ally,           // Targets Allied Team
        Enemy,          // Targets Enemy Team
        Self,           // Targets targetting Fighter
        ExcludeSelf,    // Cannot target targetting Fighter
    }
    public enum TargetPosition
    {
        Front,          // Target the Front Available Fighter
        Rear,           // Target the Rear Available Fighter
        Grounded,       // Target Available Fighters on the Floor
        Any
    }

    public enum TargetCount
    {
        Single,         // Target One Fighter
        Multiple,       // Target Multiple Fighters
        EntireTeam,     // Target One Entire Team
        AllFighters,    // Target All Fighters
    }

    /*public static TargetSelection SetFlag(TargetSelection a, TargetSelection b)
    {
        return a | b;
    }

    public static TargetSelection UnsetFlag(TargetSelection a, TargetSelection b)
    {
        return a & (~b);
    }

    // Works with "None" as well
    public static bool HasFlag(TargetSelection a, TargetSelection b)
    {
        return (a & b) == b;
    }

    public static TargetSelection ToogleFlag(TargetSelection a, TargetSelection b)
    {
        return a ^ b;
    }*/

    [System.Flags]
    public enum AttackAttribute
    {
        None,
        Rock,
        Paper,
        Scissors
    }

    [System.Flags]
    public enum FighterAttribute
    {
        None,
        Rock,
        Paper,
        Scissors
    }

    #endregion

    public static List<Fighter> GetTargettableFighters(TeamManager originTeam, TargetSelection targetSelection, AttackAttribute attackAttribute)
    {
        List<Fighter> targettableFighters = new List<Fighter>();

        switch(targetSelection)
        {
            case TargetSelection.Ally:

                break;
            case TargetSelection.Enemy:
                break;
            case TargetSelection.Self:
                break;
            case TargetSelection.ExcludeSelf:
                break;
        }

        return targettableFighters;
    }


}
