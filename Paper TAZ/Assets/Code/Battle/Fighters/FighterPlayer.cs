using UnityEngine;

/// <summary>
/// PlayerFighter is a Fighter is under player control
/// PlayerFighter takes input via UI, loads certain stats from player save data, some from a playerData Object,
/// and utilizes only certain parts of that data based on the player save state
/// </summary>
public class FighterPlayer : Fighter
{
    private FighterDataPlayer playerData;

    #region Setup

    protected override void SetupFighterStats()
    {
        base.SetupFighterStats();

        // TODO Load in persistent stats (i.e. health)
    }

    #endregion

    #region Battle Updates

    public override void SetAsActiveFighter()
    {
        BattleUIManager.Instance.UpdateCurrentFighter(this);
    }

    #endregion
}