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
    
    #region Events

    public delegate void ClickEvent();
    public static event ClickEvent OnAttackClicked;
    public static event ClickEvent OnItemClicked;
    public static event ClickEvent OnSpecialClicked;
    public static event ClickEvent OnTacticsClicked;

    #endregion

    #region Enums

    private enum MenuSelectionLevel
    {
        Category,
        Action,
        Target
    }

    private enum CategorySelection
    {
        Attack,
        Item,
        Special,
        Tactics
    }

    #endregion

    #region Exposed Variables

    public static BattleUIManager Instance = null;

    // UI References
    public Canvas PlayerUI;
    public TextMeshProUGUI StateText;
    public TextMeshProUGUI TurnCountText;
    public TextMeshProUGUI CurrentFighterText;
    public TextMeshProUGUI CurrentTargetedFighterText;
    public TextMeshProUGUI descriptionText;

    [Header ("Action Selection")]
    public Button AttackButton;
    public Button ItemButton;
    public Button SpecialButton;
    public Button TacticsButton;
    public Button ConfirmTargetButton;

    public ActionSelectionPanel ActionSelectionPanel;
    public Canvas CategorySelectionCanvas;
    public Canvas ActionSelectionCanvas;
    public Canvas TargetSelectionCanvas;

    private bool canNavigateTargets
    {
        get
        {
            return currentMenuSelectionLevel == MenuSelectionLevel.Target &&
                currentTargetingAction != null &&
                currentTargetingAction.TargetData.TargetCount == BattleTargetting.TargetCount.Single &&
                targetableFighters.Count > 1;
        }
    }

    private List<Fighter> targetableFighters = new List<Fighter>();
    private Fighter currentFighter;
    private Action currentTargetingAction;
    private MenuSelectionLevel currentMenuSelectionLevel;
    private CategorySelection currentCategorySelection;
    private int currentActionSelectionIndex;
    private int currentTargetSelectionIndex;

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
            Back();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SelectNextTarget();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SelectPreviousTarget();
    }

    #region UI Update Events

    public void UpdateState(BattleManager.BattleState state)
    {
        StateText.text = "State: " + state;
    }

    public void UpdateCurrentFighter(Fighter fighter)
    {
        CurrentFighterText.text = "Current Fighter: " + fighter.FighterData.FighterName;
        currentFighter = fighter;
        if(currentFighter.FighterTeam == PlayerTeam)
            ShowPlayerUI();
    }

    public void UpdateTargetedFighter(Fighter fighter)
    {
        if (fighter != null)
            CurrentTargetedFighterText.text = "Targeted Fighter: " + fighter.FighterData.FighterName;
        else
            CurrentTargetedFighterText.text = "Targeted Fighter:";
    }

    public void UpdateTargetedFighter(List<Fighter> fighter)
    {
        string fighterNames = "";
        for(int i = 0; i < fighter.Count; i++)
        {
            if (i > 0)
                fighterNames += ", ";
            fighterNames += fighter[i].FighterData.FighterName;
        }
        if (fighter != null)
            CurrentTargetedFighterText.text = "Targeted Fighter: " + fighterNames;
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
        // TODO Hook player up to animations readying to fight
        if (OnAttackClicked != null)
            OnAttackClicked();

        ActionSelectionPanel.LoadAttacks(currentFighter);

        SetSelectionLevel(MenuSelectionLevel.Action);
        currentCategorySelection = CategorySelection.Attack;
    }

    public void OnClickItem()
    {
        if (OnItemClicked != null)
            OnItemClicked();
        // TODO Load Different Items Into next selection System

        SetSelectionLevel(MenuSelectionLevel.Action);
        currentCategorySelection = CategorySelection.Item;
    }

    public void OnClickSpecial()
    {
        if (OnSpecialClicked != null)
            OnSpecialClicked();
        // TODO Load Different Specials Into next selection System

        SetSelectionLevel(MenuSelectionLevel.Action);
        currentCategorySelection = CategorySelection.Special;
    }

    public void OnClickTactics()
    {
        if (OnTacticsClicked != null)
            OnTacticsClicked();
        // TODO Load Different Tactics into next Selection System

        SetSelectionLevel(MenuSelectionLevel.Action);
        currentCategorySelection = CategorySelection.Tactics;
    }

    public void OnClickConfirmTarget()
    {
        ConfirmTarget();
    }

    #endregion

    #region MenuTransitions

    public void ShowPlayerUI()
    {
        PlayerUI.enabled = true;
        EventSystem.current.SetSelectedGameObject(null);

        SetSelectionLevel(MenuSelectionLevel.Category, true);
    }

    public void HideActionUI()
    {
        PlayerUI.enabled = false;
    }

    private void SetSelectionLevel(MenuSelectionLevel selectionLevel, bool resetToTopSeleciton = false)
    {
        switch(selectionLevel)
        {
            case MenuSelectionLevel.Category:
                CategorySelectionCanvas.enabled = true;
                ActionSelectionCanvas.enabled = false;
                TargetSelectionCanvas.enabled = false;

                if (resetToTopSeleciton)
                {
                    AttackButton.Select();
                }
                else
                {
                    switch(currentCategorySelection)
                    {
                        case CategorySelection.Attack: AttackButton.Select(); break;
                        case CategorySelection.Item: ItemButton.Select(); break;
                        case CategorySelection.Special: SpecialButton.Select(); break;
                        case CategorySelection.Tactics: TacticsButton.Select(); break;
                    }
                }
                break;
            case MenuSelectionLevel.Action:
                CategorySelectionCanvas.enabled = false;
                ActionSelectionCanvas.enabled = true;
                TargetSelectionCanvas.enabled = false;

                if (resetToTopSeleciton)
                    ActionSelectionPanel.SelectAction(0);
                else
                    ActionSelectionPanel.SelectAction(currentActionSelectionIndex);
                break;
            case MenuSelectionLevel.Target:
                // TODO Hide menus and prep targets
                CategorySelectionCanvas.enabled = false;
                ActionSelectionCanvas.enabled = false;
                TargetSelectionCanvas.enabled = true;

                ConfirmTargetButton.Select();
                break;
        }

        currentMenuSelectionLevel = selectionLevel;
    }

    public void SetActionSelectionIndex(int index)
    {
        currentActionSelectionIndex = index;
    }

    private void Back()
    {
        switch (currentMenuSelectionLevel)
        {
            case MenuSelectionLevel.Category:
                // TODO Nothing, maybe play sound?
                break;
            case MenuSelectionLevel.Action:
                SetSelectionLevel(MenuSelectionLevel.Category);
                break;
            case MenuSelectionLevel.Target:
                SetSelectionLevel(MenuSelectionLevel.Action);
                currentTargetingAction = null;
                for(int i = 0; i < targetableFighters.Count; i++)
                {
                    targetableFighters[i].SetAsTargeted(false);
                }
                break;
        }
    }

    #endregion

    #region ActionTargetting

    public void BeginTargettingForAction(int actionIndex)
    {
        SetSelectionLevel(MenuSelectionLevel.Target, true);

        switch(currentCategorySelection)
        {
            case CategorySelection.Attack:
                currentTargetingAction = currentFighter.FighterData.AttackList[actionIndex];
                break;
            case CategorySelection.Item:
                break;
            case CategorySelection.Special:
                break;
            case CategorySelection.Tactics:
                break;
        }

        targetableFighters = BattleTargetting.GetTargettableFighters(currentFighter, currentTargetingAction.TargetData);
        if (currentTargetingAction.TargetData.TargetCount == BattleTargetting.TargetCount.Single) // single target
        {
            targetableFighters[0].SetAsTargeted(true);
            UpdateTargetedFighter(targetableFighters[0]);
            currentTargetSelectionIndex = 0;
        }
        else // Target all at once
        {
            for(int i = 0; i < targetableFighters.Count; i++)
            {
                targetableFighters[i].SetAsTargeted(true);
            }
            UpdateTargetedFighter(targetableFighters);
        }

    }
    
    private void ConfirmTarget()
    {
        if (currentMenuSelectionLevel != MenuSelectionLevel.Target)
            return;

        if(currentTargetingAction.TargetData.TargetCount == BattleTargetting.TargetCount.Single)
        {
            currentFighter.SetTarget(targetableFighters[currentTargetSelectionIndex]);
            targetableFighters[currentTargetSelectionIndex].SetAsTargeted(false);
        }
        else
        {
            currentFighter.SetTargets(targetableFighters);
            for(int i = 0; i < targetableFighters.Count; i++)
            {
                targetableFighters[i].SetAsTargeted(false);
            }
        }

        switch(currentCategorySelection)
        {
            case CategorySelection.Attack:
                currentFighter.Attack();
                break;
            case CategorySelection.Item:
                currentFighter.UseItem();
                break;
            case CategorySelection.Special:
                break;
            case CategorySelection.Tactics:
                break;
        }
    }

    private void SelectPreviousTarget()
    {
        if(canNavigateTargets)
        {
            targetableFighters[currentTargetSelectionIndex].SetAsTargeted(false);
            currentTargetSelectionIndex--;
            if (currentTargetSelectionIndex < 0)
            {
                currentTargetSelectionIndex = targetableFighters.Count - 1;
            }
            targetableFighters[currentTargetSelectionIndex].SetAsTargeted(true);
            UpdateTargetedFighter(targetableFighters[currentTargetSelectionIndex]);
        }
    }

    private void SelectNextTarget()
    {
        if (canNavigateTargets)
        {
            targetableFighters[currentTargetSelectionIndex].SetAsTargeted(false);
            currentTargetSelectionIndex++;
            if (currentTargetSelectionIndex == targetableFighters.Count)
            {
                currentTargetSelectionIndex = 0;
            }
            targetableFighters[currentTargetSelectionIndex].SetAsTargeted(true);
            UpdateTargetedFighter(targetableFighters[currentTargetSelectionIndex]);
        }
    }

    #endregion

}