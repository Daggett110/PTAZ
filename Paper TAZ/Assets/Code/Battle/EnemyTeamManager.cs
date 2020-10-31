using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTeamManager : TeamManager
{
    public Vector3[] EnemyPositions;

    #region Setup

    public override void InitializeTeam()
    {
        for(int i = 0; i < BattleManager.Instance.BattleData.EnemyData.Count; i++)
        {
            TeamMembers.Add(GameObject.Instantiate(BattleManager.Instance.BattleData.EnemyData[i].FighterPrefab).GetComponent<Fighter>());
            TeamMembers[i].FighterData = BattleManager.Instance.BattleData.EnemyData[i];
            TeamMembers[i].transform.SetParent(transform);
            TeamMembers[i].transform.localPosition = EnemyPositions[i];
        }

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
