using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour {

    #region Classes, Enums, Structs

    public enum BattleState
    {
        Invalid,
        Start,
        PlayerTurn,
        OpponentTurn,
        Hold,
        Lose,
        Win
    }

    #endregion

    #region Events
    public delegate void StateChange(BattleState newState);
    public static event StateChange OnStateChanged;
    public delegate void NumberChanged(int count);
    public static event NumberChanged OnTurnCountUpdated;
    #endregion
    #region Exposed Variables

    public static BattleManager Instance { get; private set; }
    
    // Component References
    public TeamManager PlayerTeam;
    public TeamManager OpponentTeam;

    #endregion

    #region Public Variables

    private BattleState currentBattleState;
    public BattleState CurrentBattleState
    {
        get { return currentBattleState; }
        private set
        {
            if (OnStateChanged != null)
                OnStateChanged(value);
            currentBattleState = value;
        }
    }


    #endregion

    #region Private Variables
    
    private int turnCount = 0;
    private TeamManager activeTeam;
    private TeamManager defendingTeam;

    #endregion

    #region Initialization

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CurrentBattleState = BattleState.Start;

        PlayerTeam.opposingTeam = OpponentTeam;
        OpponentTeam.opposingTeam = PlayerTeam;
        SetState(BattleState.PlayerTurn);
    }

    #endregion

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            SetState(BattleState.PlayerTurn);
        if (Input.GetKeyDown(KeyCode.O))
            SetState(BattleState.OpponentTurn);
        if (Input.GetKeyDown(KeyCode.Space))
            activeTeam.ActiveTeamMembers[activeTeam.ActiveFighterIndex].Attack();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            activeTeam.TargetNextValidFighter();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            activeTeam.TargetPreviousValidFighter();
    }

    #region Battle State Changes PARSE TASKS TO TEAM MANAGER

    public void SetState(BattleState state)
    {
        CurrentBattleState = state;
        switch(CurrentBattleState)
        {
            case BattleState.PlayerTurn:
                SetupTurn(PlayerTeam, OpponentTeam);
                break;
            case BattleState.OpponentTurn:
                SetupTurn(OpponentTeam, PlayerTeam);
                break;
            case BattleState.Lose:
                // TODO Take away any control maybe add restart button
                Lose();
                break;
            case BattleState.Win:
                Win();
                break;
        }
    }

    private void SetupTurn(TeamManager newActiveTeam, TeamManager newDefendingTeam)
    {
        activeTeam = newActiveTeam;
        defendingTeam = newDefendingTeam;
        activeTeam.SetupActive();
        defendingTeam.SetupDefensive();
        if (activeTeam == PlayerTeam)
            IncrementTurnCount();
    }

    void Win()
    {

        // TODO Make Win presentation
    }

    void Lose()
    {
        // TODO Make Loss presentation
    }

    public void IncrementTurnCount()
    {
        turnCount++;
        if (OnTurnCountUpdated != null)
            OnTurnCountUpdated(turnCount);
    }

    #endregion
    
}
