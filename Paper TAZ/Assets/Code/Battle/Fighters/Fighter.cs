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

    [Header("Aesthetics")]
    public string FighterName;

    [Header("Stats")]
    public int MaxHealth;
    public int CurrentHealth;

    [Header("Mechanics")]
    public TeamManager FighterTeam;
    public bool actionUsed = false;

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
    
    private Fighter targetedFighter;

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
        CurrentHealth = FighterData.MaxHealth;
    }

    #endregion

    #region Fighter Status

    public bool FighterCanBeActive()
    {
        return CurrentHealth <= 0 || actionUsed ? false : true;
    }

    #endregion

    #region Battle Updates

    public void SetTeamIndex(int i)
    {
        TeamIndex = i;
    }

    #endregion

    #region Attacks

    public void SetTarget(Fighter targetFighter)
    {
        targetedFighter = targetFighter;
    }

    public void Attack()
    {
        // TODO Replace with selected attacks once multiple are supported
        if (FighterData.AttackList.Count > 0)
        {
            targetedFighter.Damage(FighterData.AttackList[0].Power);
        }
        targetedFighter.SetTargetted(false);
        actionUsed = true;
        StartCoroutine(FighterTeam.FighterFinished());
    }

    public void Damage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
            CurrentHealth = 0;
        UpdateHealthUI();
        StartCoroutine(IndicateDamage(damage));

        if(CurrentHealth == 0)
            FighterTeam.FighterDefeated(this);
    }

    #endregion

    #region UI

    public void UpdateHealthUI()
    {
        HealthText.text = CurrentHealth + "/" + MaxHealth;
        healthBar.fillAmount = (float)CurrentHealth / MaxHealth;
    }

    public virtual void SetTargetted(bool targeted)
    {
        // TODO IDEA What if a nervous animation was started when targeted?
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
