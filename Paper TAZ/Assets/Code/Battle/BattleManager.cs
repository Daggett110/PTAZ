using System;
using System.Collections.Generic;
using UnityEngine;

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
    //public static event Action OnBeginFirstStrike;
    #endregion

    #region Exposed Variables

    public static BattleManager Instance { get; private set; }
    
    // Component References
    public PlayerTeamManager PlayerTeam;
    public EnemyTeamManager OpponentTeam;
    public BattleData overrideBattleData;
    public List<FighterData> PlayerFighters;

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

    public BattleData BattleData { get; private set; }

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

        if (overrideBattleData != null)
            InitializeBattle(overrideBattleData);
        else
            Debug.LogError("Battle Manager: No Battle override Data has been added to the inspector, if GameManager is now passing data, Horray! and remove this error.");
    }

    // TODO Call from Game Manager with appropriate data passed in (BattleData?)
    private void InitializeBattle(BattleData battleData)
    {
        BattleData = battleData;

        // TODO Load Location into scene
        Debug.Log("Loading Location: " + BattleData.locationID);

        // Initialize Teams
        PlayerTeam.opposingTeam = OpponentTeam;
        OpponentTeam.opposingTeam = PlayerTeam;
        PlayerTeam.InitializeTeam();
        OpponentTeam.InitializeTeam();

        // Reveal Battle Stage
        // TODO Play stage reveal animation
        PlayerTeam.WalkInFighters();
        OpponentTeam.WalkInFighters();

        // Resolve First Strike
        //if (OnBeginFirstStrike != null)
            //OnBeginFirstStrike();

        // Initialize Starting Team, entering the battle loop
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
    }

    #region Battle State Changes

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
