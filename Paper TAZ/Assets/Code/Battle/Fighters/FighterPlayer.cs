using UnityEngine;

/// <summary>
/// PlayerFighter is a Fighter is under player control
/// PlayerFighter takes input via UI, loads certain stats from player save data, some from a playerData Object,
/// and utilizes only certain parts of that data based on the player save state
/// </summary>
public class FighterPlayer : Fighter
{
    //[Header("Fighter - Player")]
    private FighterDataPlayer playerData;
    
    protected override void SetupFighterStats()
    {
        base.SetupFighterStats();

        // TODO Load in persistent stats (i.e. health)
    }
}