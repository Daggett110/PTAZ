using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles SSelecting Attacker, selecting target, and monitoring team status
/// </summary>
public class TeamManager : MonoBehaviour {

    public delegate void FighterSelection(Fighter fighter);
    public static event FighterSelection OnTargetedFighterChanged;
    public static event FighterSelection OnCurrentFighterChanged;

    public List<Fighter> TeamMembers = new List<Fighter>();
    public bool FightersArrangedLeftToRight;
    public List<Fighter> ActiveTeamMembers { get; private set; }

    [HideInInspector]
    public TeamManager opposingTeam;
    public List<Fighter> TargetableFighters;
    public int ActiveFighterIndex { get; private set; }
    public int TargetedFighterIndex { get; private set; }

    private int ActionPoints;

    #region Setup and Initialization
    
    public virtual void InitializeTeam()
    {
        ActiveTeamMembers = new List<Fighter>();
        for (int i = 0; i < TeamMembers.Count; i++)
        {
            TeamMembers[i].FighterTeam = this;
            if (TeamMembers[i].CurrentHealth > 0)
                ActiveTeamMembers.Add(TeamMembers[i]);
        }

        SetTeamIndexes();
    }

    public void WalkInFighters()
    {
        // TODO Delay First Strike Fighter if there is one

        // TODO have incapacitated Fighters start incapacitated on the floor rather than walking out
        for(int i = 0; i < TeamMembers.Count; i++)
        {
            TeamMembers[i].WalkInFighter();
        }
    }

    #endregion

    #region Team Composition

    private void SetTeamIndexes()
    {
        for(int i = 0; i < TeamMembers.Count; i++)
        {
            TeamMembers[i].SetTeamIndex(i);
        }
    }

    private void RefreshActiveTeamList()
    {
        if (ActiveTeamMembers == null)
            ActiveTeamMembers = new List<Fighter>();
        else
            ActiveTeamMembers.Clear();
        for (int i = 0; i < TeamMembers.Count; i++)
        {
            if (TeamMembers[i].CurrentHealth > 0)
                ActiveTeamMembers.Add(TeamMembers[i]);
        }
    }

    #endregion

    #region Targetting
    
    protected void TargetFighterAt(int i)
    {
        ActiveTeamMembers[TargetedFighterIndex].SetAsTargeted(false);
        TargetedFighterIndex = i;
        ActiveTeamMembers[TargetedFighterIndex].SetAsTargeted(true);
        ActiveTeamMembers[ActiveFighterIndex].SetTarget(ActiveTeamMembers[TargetedFighterIndex]);
        if (OnTargetedFighterChanged != null)
            OnTargetedFighterChanged(ActiveTeamMembers[TargetedFighterIndex]);
    }

    /// <summary>
    /// Checks if the passed in fighter index is able to be targeted
    /// </summary>
    /// <param name="i">The index of the desired fighter</param>
    /// <returns></returns>
    protected bool FighterIsTargetableAt(int i)
    {
        // TODO Take attack and Statuses/Attributes into account Move into a fighter manager/Utility?
        if (ActiveTeamMembers.Count > i && i >= 0)
        {
            if (ActiveTeamMembers[i].CurrentHealth > 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Finds the first targetable fighter on the team this method exstends from
    /// </summary>
    /// <returns>The fighter found matching the constraints, or null if none match</returns>
    public Fighter GetFirstTargetableFighter()
    {
        return ActiveTeamMembers.Count > 0 ? ActiveTeamMembers[0] : null;
    }

    /// <summary>
    /// Finds a random targetable fighter on the team this method exstends from
    /// </summary>
    /// <returns>The fighter found matching the constraints, or null if none match</returns>
    public Fighter GetRandomTargetableFighter()
    {
        if(ActiveTeamMembers.Count > 0)
        {
            return ActiveTeamMembers[Random.Range(0, ActiveTeamMembers.Count)];
        }
        return null;
    }


    #endregion

    #region BattleUpdates

    public IEnumerator FighterFinished()
    {
        yield return new WaitForSeconds(1f);
        OnTargetedFighterChanged?.Invoke(null);
        if (BattleManager.Instance.CurrentBattleState != BattleManager.BattleState.Lose && BattleManager.Instance.CurrentBattleState != BattleManager.BattleState.Win)
        {
            bool nextFighterFound = false;
            for (int i = 0; i < ActiveTeamMembers.Count; i++)
            {
                if (ActiveTeamMembers[i].FighterCanBeActive())
                {
                    SetActiveFighter(i);
                    nextFighterFound = true;
                    break;
                }
            }
            if (!nextFighterFound)
                BattleManager.Instance.SetState(BattleManager.Instance.CurrentBattleState == BattleManager.BattleState.PlayerTurn ? BattleManager.BattleState.OpponentTurn : BattleManager.BattleState.PlayerTurn);
        }
    }

    protected virtual void SetActiveFighter(int fighterIndex)
    {
        ActiveFighterIndex = fighterIndex;
        ActiveTeamMembers[ActiveFighterIndex].SetAsActiveFighter();
        OnCurrentFighterChanged?.Invoke(ActiveTeamMembers[fighterIndex]);
    }

    public virtual void FighterDefeated(Fighter defeatedFighter)
    {
        ActiveTeamMembers.Remove(defeatedFighter);

        if (ActiveTeamMembers.Count == 0)
            BattleManager.Instance.SetState(BattleManager.BattleState.Lose);
    }

    public virtual void SetupActive()
    {
        RefreshActiveTeamList();

        for (int i = 0; i < ActiveTeamMembers.Count; i++)
            ActiveTeamMembers[i].ResetTurn();
        
        SetActiveFighter(0);
    }

    // TODO Is this necessary?
    public virtual void SetupDefensive()
    {
        // TODO Override in player/AI teams to allow for different setup
    }

    #endregion
}
