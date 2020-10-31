using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fighter : MonoBehaviour {

    #region Enums

    public enum FighterState
    {
        Alive,
        Incapacitated,
        Dead
    }

    #endregion

    #region Exposed Variables

    public FighterData FighterData;
    
    [Header("Stats")]
    public int MaxHealth;
    public int CurrentHealth;

    [Header("Mechanics")]
    public TeamManager FighterTeam;
    public bool ActionUsed = false;
    public bool FirstStrike { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI HealthText;
    public Image healthBar;
    public TextMeshProUGUI HitText;
    public int DamageIndicationLength;
    public Image TargetedArrow;

    #endregion

    #region Properties

    public FighterState State { get; private set; }
    public int TeamIndex { get; private set; }

    #endregion

    #region Private Variables
    // TODO Organize
    private WaitForSeconds DamageIndicationWait;
    
    private List<Fighter> targetedFighters = new List<Fighter>();

    public BattleTargetting.TargetElevation currentElevation { get; protected set; }

    #endregion

    #region Initialization

    void Awake()
    {
        SetupFighterStats();
        DamageIndicationWait = new WaitForSeconds(DamageIndicationLength);

        // Turn on/off appropriate things
        HitText.gameObject.SetActive(false);
        TargetedArrow.gameObject.SetActive(false);
        UpdateHealthUI();
    }

    protected virtual void SetupFighterStats()
    {
        // Set up variables
        CurrentHealth = MaxHealth = FighterData.MaxHealth;
    }

    public void WalkInFighter()
    {
        // TODO Play walk on animation for fighter
    }

    #endregion

    #region Fighter Status

    public bool FighterCanBeActive()
    {
        return CurrentHealth <= 0 || ActionUsed ? false : true;
    }

    public void Damage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
            CurrentHealth = 0;
        UpdateHealthUI();
        StartCoroutine(IndicateDamage(damage));

        if (CurrentHealth == 0)
            FighterTeam.FighterDefeated(this);
    }

    #endregion

    #region Battle Updates

    public void SetTeamIndex(int i)
    {
        TeamIndex = i;
    }

    public void ResetTurn()
    {
        ActionUsed = false;

        // TODO Decrment buffs/debuffs make sure these work each turn
    }

    public virtual void SetAsActiveFighter()
    {
        // Override in Child classes
    }

    #endregion

    #region Targetting

    public void SetTarget(Fighter targetFighter)
    {
        targetedFighters.Clear();
        targetedFighters.Add(targetFighter);
    }

    public void SetTargets(List<Fighter> targetFighter)
    {
        targetedFighters = targetFighter;
    }

    // TODO Figure out when a target should stop acting targetted
    public virtual void SetAsTargeted(bool targeted)
    {
        TargetedArrow.gameObject.SetActive(targeted);
        // TODO IDEA What if a nervous animation was started when targeted?
    }

    #endregion

    #region Actions
    
    // TODO Rework to use action list/Action Command
    public void Attack()
    {
        // TODO Replace with selected attacks once multiple are supported
        if (FighterData.AttackList.Count > 0)
        {
            for(int i = 0; i < targetedFighters.Count; i++)
                targetedFighters[i].Damage(FighterData.AttackList[0].Power);
        }
        //targetedFighter.SetTargetted(false);
        ActionUsed = true;
        StartCoroutine(FighterTeam.FighterFinished());
    }

    public void UseItem()
    {

    }

    #endregion

    #region UI

    public void UpdateHealthUI()
    {
        HealthText.text = CurrentHealth + "/" + MaxHealth;
        healthBar.fillAmount = (float)CurrentHealth / MaxHealth;
    }

    public IEnumerator IndicateDamage(int damage)
    {
        HitText.text = damage.ToString();
        HitText.gameObject.SetActive(true);
        yield return DamageIndicationWait;
        HitText.gameObject.SetActive(false);
    }

    #endregion
}
