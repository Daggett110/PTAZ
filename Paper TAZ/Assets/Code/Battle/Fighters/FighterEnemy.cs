using UnityEngine;

/// <summary>
/// EnemyFighter is a Fighter is under AI control
/// EnemyFighter submits input via AI, loads all stats from EnemyData, perhaps 
/// some alternative data based on previous battles for important enemies.
/// </summary>
public class FighterEnemy : Fighter
{
    //[Header("Fighter - Enemy")]
    private FighterDataEnemy EnemyData;

    protected override void SetupFighterStats()
    {
        base.SetupFighterStats();

        EnemyData = FighterData as FighterDataEnemy;
    }

    public override void SetTargetted(bool targeted)
    {
        TargetedArrow.gameObject.SetActive(targeted);
        base.SetTargetted(targeted);
    }
}