using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTeamManager : TeamManager
{
    
    #region Setup

    public override void InitializeTeam()
    {
        SpawnFighters(BattleManager.Instance.BattleData.EnemyData);

        base.InitializeTeam();
    }

    #endregion

    #region Battle Updates

    protected override void SetActiveFighter(int fighterIndex)
    {
        base.SetActiveFighter(fighterIndex);

        // TODO replace with actual AI
        // TODO Make sure selected fighter is targetable with attack
        // TODO Grab and use random attack
        ActiveTeamMembers[ActiveFighterIndex].SetTarget(opposingTeam.GetRandomTargetableFighter());
        ActiveTeamMembers[ActiveFighterIndex].Attack();
    }

    public override void SetupActive()
    {
        base.SetupActive();
    }

    public override void SetupDefensive()
    {
        base.SetupDefensive();
    }

    public override void FighterDefeated(Fighter defeatedFighter)
    {
        base.FighterDefeated(defeatedFighter);


        if (ActiveTeamMembers.Count == 0)
            BattleManager.Instance.SetState(BattleManager.BattleState.Win);
    }

    #endregion
}
