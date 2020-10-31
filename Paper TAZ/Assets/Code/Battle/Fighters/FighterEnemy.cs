using UnityEngine;

/// <summary>
/// EnemyFighter is a Fighter is under AI control
/// EnemyFighter submits input via AI, loads all stats from EnemyData, perhaps 
/// some alternative data based on previous battles for important enemies.
/// </summary>
public class FighterEnemy : Fighter
{
    private FighterDataEnemy EnemyData;

    protected override void SetupFighterStats()
    {
        base.SetupFighterStats();

        EnemyData = FighterData as FighterDataEnemy;

        currentElevation = EnemyData.startingElevation;
    }

    public override void SetAsTargeted(bool targeted)
    {
        base.SetAsTargeted(targeted);
    }
}