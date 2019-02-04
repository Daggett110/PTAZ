using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Battle UI for player interaction. References player team because Enemy's will use AI without the need for UI
/// </summary>
public class BattleUIManager : MonoBehaviour {

    public enum BattleUISState
    {
        Off,
        ActionSelect,
        Targetting,
    }

    #region Events

    public delegate void ClickEvent();
    public static event ClickEvent OnAttackClicked;
    public static event ClickEvent OnItemClicked;
    public static event ClickEvent OnSpecialClicked;
    public static event ClickEvent OnTacticsClicked;
    
    #endregion

    #region Exposed Variables

    public static BattleUIManager Instance = null;

    // UI References
    public GameObject PlayerActionPanel;
    public TextMeshProUGUI StateText;
    public TextMeshProUGUI TurnCountText;
    public TextMeshProUGUI CurrentFighterText;
    public TextMeshProUGUI CurrentTargetedFighterText;
    public Button AttackButton;
    public Button ItemButton;
    public Button SpecialButton;
    public Button TacticsButton;

    public BattleUISState state;

    #endregion

    #region Private Variables
    private TeamManager PlayerTeam;
    #endregion

    #region Setup and Initialization

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PlayerTeam = BattleManager.Instance.PlayerTeam;
    }

    public void OnEnable()
    {
        BattleManager.OnStateChanged += UpdateState;
        TeamManager.OnCurrentFighterChanged += UpdateCurrentFighter;
        TeamManager.OnTargetedFighterChanged += UpdateTargetedFighter;
        BattleManager.OnTurnCountUpdated += UpdateTurnCount;
    }

    public void OnDisable()
    {
        BattleManager.OnStateChanged -= UpdateState;
        TeamManager.OnCurrentFighterChanged -= UpdateCurrentFighter;
        TeamManager.OnTargetedFighterChanged -= UpdateTargetedFighter;
        BattleManager.OnTurnCountUpdated -= UpdateTurnCount;
    }

    #endregion

    private void Update()
    {
        // TODO Replace with key mapping system
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            switch(state)
            {
                case BattleUISState.ActionSelect:
                    break;
                case BattleUISState.Targetting:
                    // TODO Stop Targetting and return to action select, preferably at the selection you just selected
                    // TODO IMMEDIATE FIX
                    //BattleManager.Instance.PlayerTeam.CancelTargetingFighters();
                    ShowActionUI();
                    break;
                default:
                    break;
            }
        }
    }

    #region UI Update Events

    public void UpdateState(BattleManager.BattleState state)
    {
        StateText.text = "State: " + state;
    }

    public void UpdateCurrentFighter(Fighter fighter)
    {
        CurrentFighterText.text = "Current Fighter: " + fighter.FighterName;
        if(BattleManager.Instance.CurrentBattleState == BattleManager.BattleState.PlayerTurn)
            ShowActionUI();
    }

    public void UpdateTargetedFighter(Fighter fighter)
    {
        if (fighter != null)
            CurrentTargetedFighterText.text = "Targeted Fighter: " + fighter.FighterName;
        else
            CurrentTargetedFighterText.text = "Targeted Fighter:";
    }

    public void UpdateTurnCount(int count)
    {
        TurnCountText.text = "Turn Count: " + count;
    }

    #endregion

    #region OnClick Events

    public void OnClickAttack()
    {
        if (OnAttackClicked != null)
            OnAttackClicked();
        state = BattleUISState.Targetting;
        // TODO Load Different Attacks Into next selection System
        HideActionUI();
    }

    public void OnClickItem()
    {
        if (OnItemClicked != null)
            OnItemClicked();
        state = BattleUISState.Targetting;
        // TODO Load Different Items Into next selection System
        HideActionUI();
    }

    public void OnClickSpecial()
    {
        if (OnSpecialClicked != null)
            OnSpecialClicked();
        // TODO Load Different Specials Into next selection System
        HideActionUI();
    }

    public void OnClickTactics()
    {
        if (OnTacticsClicked != null)
            OnTacticsClicked();
        // TODO Load Different Tactics into next Selection System
        HideActionUI();
    }

    #endregion

    #region State Updates

    private void Back()
    {
        switch (state)
        {
            case BattleUISState.ActionSelect:
                break;
            case BattleUISState.Targetting:
                // TODO IMMEDIATE FIX
                //BattleManager.Instance.PlayerTeam.CancelTargetingFighters();
                state = BattleUISState.ActionSelect;
                break;
            default:
                break;
        }
    }

    public void ShowActionUI()
    {
        // TODO Get Possible Actions of Current Fighter, Populate Accordingly
        // TODO select what was just selected when returning
        PlayerActionPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        AttackButton.Select();
        state = BattleUISState.ActionSelect;
    }

    public void HideActionUI()
    {
        PlayerActionPanel.SetActive(false);
    }


    #endregion

}