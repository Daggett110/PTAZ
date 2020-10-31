using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that Handles Targetting for all Battle Scenarios in terms of what targets what
/// </summary>
public static class BattleTargetting
{
    private const string LOG_PREFIX = "Battle Targetting: ";
    #region Enums and Flags
    
    public enum TargetSelection
    {
        Enemy,          // Targets Enemy Team
        Ally,           // Targets Allied Team
        Self,           // Targets targetting Fighter
    }

    public enum TargetPosition
    {
        Any,
        Front,          // Target the Front Available Fighter
        Rear           // Target the Rear Available Fighter
    }

    public enum TargetElevation
    {
        Any,
        Floor,
        Air,
        Ceiling,
        Grounded
    }

    // Might be flag
    public enum TargetStatus
    {
        Any = 0,
        Active = 1,
        Unconscious = 2
    }

    public enum TargetCount
    {
        Single,         // Target One Fighter
        EntireTeam,     // Target One Entire Team
        AllFighters,    // Target All Fighters
    }

    #region Flag Utility Functions

    public static TargetStatus SetFlag(TargetStatus a, TargetStatus b)
    {
        return a | b;
    }

    public static TargetStatus UnsetFlag(TargetStatus a, TargetStatus b)
    {
        return a & (~b);
    }

    // Works with "None" as well
    public static bool HasFlag(TargetStatus a, TargetStatus b)
    {
        return (a & b) == b;
    }

    public static TargetStatus ToogleFlag(TargetStatus a, TargetStatus b)
    {
        return a ^ b;
    }

    public static ActionAttribute SetFlag(ActionAttribute a, ActionAttribute b)
    {
        return a | b;
    }

    public static ActionAttribute UnsetFlag(ActionAttribute a, ActionAttribute b)
    {
        return a & (~b);
    }

    // Works with "None" as well
    public static bool HasFlag(ActionAttribute a, ActionAttribute b)
    {
        return b != ActionAttribute.None && (a & b) == b;
    }

    public static ActionAttribute ToogleFlag(ActionAttribute a, ActionAttribute b)
    {
        return a ^ b;
    }

    public static FighterAttribute SetFlag(FighterAttribute a, FighterAttribute b)
    {
        return a | b;
    }

    public static FighterAttribute UnsetFlag(FighterAttribute a, FighterAttribute b)
    {
        return a & (~b);
    }

    // Works with "None" as well
    public static bool HasFlag(FighterAttribute a, FighterAttribute b)
    {
        return b != FighterAttribute.None && (a & b) == b;
    }

    public static FighterAttribute ToogleFlag(FighterAttribute a, FighterAttribute b)
    {
        return a ^ b;
    }

    #endregion

    [System.Flags]
    public enum ActionAttribute
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scissors = 4
    }

    [System.Flags]
    public enum FighterAttribute
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scissors = 4
    }

    #endregion

    public static bool ValidTargetsFoundForAction(Fighter originator, TargetingData targetData)
    {
        if (targetData.TargetSelection == TargetSelection.Self)
            return true;
        
        // Take care of front/rear options seperately, as they can only be one player if not, loop through possible options later
        switch(targetData.TargetPosition)
        {
            case TargetPosition.Front: // single target
            case TargetPosition.Rear: // single target
                Fighter consideredFighter = null;
                TeamManager targetTeam = null;
                // Ignore self, stupid to add self 
                switch(targetData.TargetSelection)
                {
                    case TargetSelection.Ally: targetTeam = originator.FighterTeam; break;
                    case TargetSelection.Enemy: targetTeam = originator.FighterTeam.opposingTeam; break;
                }
                
                for (int i = 0; i < targetTeam.TeamMembers.Count; i++)
                {
                    if (FighterIsOnTargetableElevation(targetTeam.TeamMembers[i], targetData.TargetElevation))
                    {
                        consideredFighter = targetTeam.TeamMembers[i];
                        break;
                    }
                }

                // Ther are no fighters at the necessary elevation, no targets available
                if(consideredFighter != null)
                {
                    if (targetData.ExcludeSelf && consideredFighter == originator)
                        return false;

                    // test status
                    if (!FighterHasTargetableStatuses(consideredFighter, targetData.TargetStatus))
                        return false;

                    return FighterHasTargetableAttributes(consideredFighter, targetData.Attributes);
                }
                return false;
            case TargetPosition.Any: // possible multiple targets
                switch(targetData.TargetSelection)
                {
                    case TargetSelection.Self:
                        return true;
                    case TargetSelection.Ally:
                        for(int i = 0; i < originator.FighterTeam.TeamMembers.Count; i++)
                        {
                            if (targetData.ExcludeSelf && originator.FighterTeam.TeamMembers[i] == originator)
                                continue;

                            if (FighterIsOnTargetableElevation(originator.FighterTeam.TeamMembers[i], targetData.TargetElevation) &&
                                FighterHasTargetableStatuses(originator.FighterTeam.TeamMembers[i], targetData.TargetStatus) &&
                                FighterHasTargetableAttributes(originator.FighterTeam.TeamMembers[i], targetData.Attributes))
                            {
                                return true;
                            }
                        }
                        return false;
                    case TargetSelection.Enemy:
                        for (int i = 0; i < originator.FighterTeam.opposingTeam.TeamMembers.Count; i++)
                        {
                            if (FighterIsOnTargetableElevation(originator.FighterTeam.opposingTeam.TeamMembers[i], targetData.TargetElevation) &&
                                FighterHasTargetableStatuses(originator.FighterTeam.opposingTeam.TeamMembers[i], targetData.TargetStatus) &&
                                FighterHasTargetableAttributes(originator.FighterTeam.opposingTeam.TeamMembers[i], targetData.Attributes))
                            {
                                return true;
                            }
                        }
                        return false;
                }
                break;
        }
        return false;
    }

    public static List<Fighter> GetTargettableFighters(Fighter originator, TargetingData targetingData)
    {
        return GetTargettableFighters(originator, targetingData.TargetCount, targetingData.TargetSelection, targetingData.ExcludeSelf, targetingData.TargetStatus, targetingData.Attributes, targetingData.TargetPosition, targetingData.TargetElevation);
    }

    public static List<Fighter> GetTargettableFighters(Fighter originator, TargetCount targetCount, TargetSelection targetSelection, bool excludeSelf, TargetStatus targetStatus, ActionAttribute attackAttribute, TargetPosition targetPosition, TargetElevation targetElevation)
    {
        List<Fighter> targettableFighters = new List<Fighter>();
        if (targetCount == TargetCount.AllFighters)
        {
            // Allys first
            targettableFighters.AddRange(originator.FighterTeam.TeamMembers);
            // Reverse ally order for navigation
            targettableFighters.Reverse();

            if (excludeSelf)
                targettableFighters.Remove(originator);
            targettableFighters.AddRange(originator.FighterTeam.opposingTeam.TeamMembers);
        }
        else
        {
            switch (targetSelection)
            {
                case TargetSelection.Ally:
                    targettableFighters.AddRange(originator.FighterTeam.TeamMembers);
                    if (excludeSelf)
                        targettableFighters.Remove(originator);
                    break;
                case TargetSelection.Enemy:
                    targettableFighters.AddRange(originator.FighterTeam.opposingTeam.TeamMembers);
                    break;
                case TargetSelection.Self:
                    targettableFighters.Add(originator);
                    break;
            }
        }
        
        for(int i = targettableFighters.Count - 1; i >= 0; i--)
        {
            if (!FighterIsOnTargetableElevation(targettableFighters[i], targetElevation))
            {
                targettableFighters.RemoveAt(i);
                continue;
            }

            // Positional actions will never focus on all teams, so lets do this conditionally
            switch(targetPosition)
            {
                case TargetPosition.Front:
                    if (i > 0)
                    {
                        targettableFighters.RemoveAt(i);
                        continue;
                    }
                    break;
                case TargetPosition.Rear:
                    if(i < targettableFighters.Count - 1)
                    {
                        targettableFighters.RemoveAt(i);
                    }
                    break;
            }

            if (!FighterHasTargetableStatuses(targettableFighters[0], targetStatus))
            {
                targettableFighters.RemoveAt(i);
            }

            if (!FighterHasTargetableAttributes(targettableFighters[0], attackAttribute))
            {
                targettableFighters.RemoveAt(i);
            }
        }

        return targettableFighters;
    }

    private static bool FighterHasTargetableAttributes(Fighter fighterCandidate, ActionAttribute attackingAttribute)
    {
        if (HasFlag(ActionAttribute.Rock, attackingAttribute) && HasFlag(FighterAttribute.Paper, fighterCandidate.FighterData.fighterAttributes))
            return false;
        if (HasFlag(ActionAttribute.Paper, attackingAttribute) && HasFlag(FighterAttribute.Scissors, fighterCandidate.FighterData.fighterAttributes))
            return false;
        if (HasFlag(ActionAttribute.Scissors, attackingAttribute) && HasFlag(FighterAttribute.Rock, fighterCandidate.FighterData.fighterAttributes))
            return false;   
        return true;
    }

    private static bool FighterIsOnTargetableElevation(Fighter targetFighter, TargetElevation targetElevation)
    {
        if (targetElevation == TargetElevation.Any)
            return true;

        switch(targetFighter.currentElevation)
        {
            case TargetElevation.Any:
                return true;
            case TargetElevation.Air:
                return targetElevation == TargetElevation.Air;
            case TargetElevation.Ceiling:
                return targetElevation == TargetElevation.Ceiling || targetElevation == TargetElevation.Grounded;
            case TargetElevation.Floor:
                return targetElevation == TargetElevation.Floor || targetElevation == TargetElevation.Air || targetElevation == TargetElevation.Grounded;
        }
        return false;
    }

    private static bool FighterHasTargetableStatuses(Fighter targetFighter, TargetStatus targetStatus)
    {
        switch(targetStatus)
        {
            case TargetStatus.Any:
                return true;
            case TargetStatus.Active:
                return targetFighter.CurrentHealth > 0;
            case TargetStatus.Unconscious:
                return targetFighter.CurrentHealth == 0;
        }
        return false;
    }
}
