using UnityEngine;

// In charge of handiling tying the UI System to the action input
public class PlayerTeamManager : TeamManager
{
    public int ActionPointMaximum = 5;
    public int AvailableActionPoints { get; private set; }

    #region Setup and Initialization
    
    public override void InitializeTeam()
    {
        base.InitializeTeam();

        // TODO LOAD PLAYERS AND PERSISTENT DATA
        AvailableActionPoints = ActionPointMaximum;

        // TODO PASS PERSISTENT DATA TO UI MANAGER
    }

    #endregion

    #region Battle Updates

    public override void SetupActive()
    {
        base.SetupActive();

        // TODO Passive fighter ability?
    }

    public override void FighterDefeated(Fighter defeatedFighter)
    {
        base.FighterDefeated(defeatedFighter);

        if (ActiveTeamMembers.Count == 0)
            BattleManager.Instance.SetState(BattleManager.BattleState.Lose);
    }

    #endregion

    #region Actions

    public void SwapOrder()
    {
        // TODO Change order with other active team member
    }

    public void TagOut()
    {
        // TODO Tag out with passive team member
    }
    
    public void UseSpecial()
    {
        // TODO Setup Special Data Object and hold until target is selected
        Debug.Log("Special Selected");
    }

    #endregion
}
