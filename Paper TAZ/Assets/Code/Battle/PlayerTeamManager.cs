using UnityEngine;

// In charge of handiling tying the UI System to the action input
public class PlayerTeamManager : TeamManager
{
    #region Setup and Initialization

    private void OnEnable()
    {
        // TODO IMMEDIATE FIX
        //BattleUIManager.OnAttackClicked += BeginTargetingFighters;
        BattleUIManager.OnItemClicked += UseItem;
        BattleUIManager.OnSpecialClicked += UseSpecial;
        BattleUIManager.OnTacticsClicked += UseTactic;
    }

    private void OnDisable()
    {
        // TODO IMMEDIATE FIX
        //BattleUIManager.OnAttackClicked -= BeginTargetingFighters;
        BattleUIManager.OnItemClicked -= UseItem;
        BattleUIManager.OnSpecialClicked -= UseSpecial;
        BattleUIManager.OnTacticsClicked -= UseTactic;
    }

    #endregion

    #region Battle Updates

    public override void SetupActive()
    {
        base.SetupActive();

        BattleUIManager.Instance.ShowActionUI();
    }

    public override void SetupDefensive()
    {
        base.SetupDefensive();
    }

    public override void FighterDefeated(Fighter defeatedFighter)
    {
        base.FighterDefeated(defeatedFighter);

        if (ActiveTeamMembers.Count == 0)
            BattleManager.Instance.SetState(BattleManager.BattleState.Lose);
    }

    #endregion

    #region Actions (Maybe move to team manager?)

    public void UseItem()
    {
        // TODO Setup Item Data Object and hold until target is selected
        Debug.Log("Item Selected");
    }

    public void UseSpecial()
    {
        // TODO Setup Special Data Object and hold until target is selected
        Debug.Log("Special Selected");
    }

    public void UseTactic()
    {
        // TODO Setup Tactic Data Object and hold until target is selected
        Debug.Log("Tactic Selected");
    }

    #endregion
}
