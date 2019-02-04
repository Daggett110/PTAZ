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
    public List<Fighter> ActiveTeamMembers { get; private set; }

    [HideInInspector]
    public TeamManager opposingTeam;
    public int ActiveFighterIndex { get; private set; }
    public int targetedFighterIndex { get; private set; }

    #region Setup and Initialization

    private void Start()
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

    public void BeginTargetingFighters(TeamManager targetTeam)
    {
        for (int i = 0; i < targetTeam.ActiveTeamMembers.Count; i++)
            targetTeam.ActiveTeamMembers[i].SetTargetted(false);

        for (int i = 0; i < targetTeam.ActiveTeamMembers.Count; i++)
        {
            if (targetTeam.FighterIsTargetableAt(i))
            {
                TargetFighterAt(i);
                return;
            }
        }
    }

    public void CancelTargetingFighters(TeamManager targetTeam)
    {
        for (int i = 0; i < targetTeam.ActiveTeamMembers.Count; i++)
            opposingTeam.ActiveTeamMembers[i].SetTargetted(false);
    }

    public void TargetNextValidFighter()
    {
        int i = targetedFighterIndex + 1;
        while (!FighterIsTargetableAt(i))
        {
            i++;
            if (i >= ActiveTeamMembers.Count)
                i = 0;
            if (i == targetedFighterIndex)
                return;
        }
        TargetFighterAt(i);
    }

    public void TargetPreviousValidFighter()
    {
        int i = targetedFighterIndex - 1;
        while (!FighterIsTargetableAt(i))
        {
            i--;
            if (i <= -1)
                i = ActiveTeamMembers.Count - 1;
            if (i == targetedFighterIndex)
                return;
        }
        TargetFighterAt(i);
    }

    protected void TargetFighterAt(int i)
    {
        ActiveTeamMembers[targetedFighterIndex].SetTargetted(false);
        targetedFighterIndex = i;
        ActiveTeamMembers[targetedFighterIndex].SetTargetted(true);
        ActiveTeamMembers[ActiveFighterIndex].SetTarget(ActiveTeamMembers[targetedFighterIndex]);
        if (OnTargetedFighterChanged != null)
            OnTargetedFighterChanged(ActiveTeamMembers[targetedFighterIndex]);
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
        if (OnTargetedFighterChanged != null)
            OnTargetedFighterChanged(null);
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
        if (OnCurrentFighterChanged != null)
            OnCurrentFighterChanged(ActiveTeamMembers[fighterIndex]);
    }

    public virtual void FighterDefeated(Fighter defeatedFighter)
    {
        // TODO Remove Targeted Fighter from pool
        ActiveTeamMembers.Remove(defeatedFighter);
        // TODO Test for loss with no fighters remaining
        if (ActiveTeamMembers.Count == 0)
            BattleManager.Instance.SetState(BattleManager.BattleState.Lose);
    }

    public virtual void SetupActive()
    {
        RefreshActiveTeamList();

        for (int i = 0; i < ActiveTeamMembers.Count; i++)
            ActiveTeamMembers[i].actionUsed = false;
        
        SetActiveFighter(0);
    }

    public virtual void SetupDefensive()
    {
        // TODO Override in player/AI teams to allow for different setup
    }

    #endregion
}
