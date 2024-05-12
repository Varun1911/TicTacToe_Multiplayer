using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


public enum GameState
{
    Start,
    WaitingForPlayers,
    Game,
    GameOver
}


public class BoardManager : NetworkBehaviour
{

    public enum PlayerTurn
    {
        P1 = 0,
        P2 = 1
    }


    public static BoardManager Instance { get; private set; }


    //Network Varibales 
    private NetworkVariable<ushort> turnCount_NV = new NetworkVariable<ushort>(0);
    private NetworkVariable<ulong> currPlayer_NV = new NetworkVariable<ulong>(0);
    private NetworkVariable<GameState> currState_NV = new NetworkVariable<GameState>(GameState.Game);


    //only for the use of server
    //server Varibales
    private PlayerTurn currPlayer_S;
    private Dictionary<PlayerTurn , ulong> playerTurnMap_S = new Dictionary<PlayerTurn , ulong>();

    
    //Events
    [SerializeField] private UnityEvent<bool> OnTurnChanged;
    [SerializeField] private UnityEvent OnSpawnNetwork;
    [SerializeField] private UnityEvent<string> OnXOTextSet;
    [SerializeField] private UnityEvent<GameState> OnGameStateChange;


    [SerializeField] private BoardButton boardButtonPrefab;


    private bool isMyTurn = false;
    private List<BoardButton> boardButtonList;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsHost)
        {
            SetRandomPlayers_Server();

            //Set Intial player turn
            currPlayer_S = PlayerTurn.P1;
            currPlayer_NV.Value = playerTurnMap_S[currPlayer_S];
        }

        GetXOTextServerRPC();
        CheckTurn();

        turnCount_NV.OnValueChanged += (ushort prevVal, ushort newVal) => Debug.Log("turn count = " + newVal);
        currPlayer_NV.OnValueChanged += OnCurrentPlayerChange;
        currState_NV.OnValueChanged += OnGameStateUpdated;


        OnSpawnNetwork.Invoke();
    }



    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        turnCount_NV.OnValueChanged -= (ushort prevVal, ushort newVal) => Debug.Log("turn count = " + newVal);
        currPlayer_NV.OnValueChanged -= OnCurrentPlayerChange;
        currState_NV.OnValueChanged -= OnGameStateUpdated;

    }

    private void OnCurrentPlayerChange(ulong previousValue, ulong newValue)
    {
        CheckTurn();
    }


    private void OnGameStateUpdated(GameState previousValue, GameState newValue)
    {
        OnGameStateChange.Invoke(newValue);
    }


    private void CheckTurn()
    {
        if (currPlayer_NV.Value == NetworkManager.Singleton.LocalClientId)
        {
            isMyTurn = true;
        }

        else
        {
            isMyTurn = false;
        }


        OnTurnChanged.Invoke(isMyTurn);
    }


    private void Start()
    {
        boardButtonList = new List<BoardButton>();
        SpawnButtons();
    }



    private void SpawnButtons()
    {
        for(int i = 0; i < 9; i++)
        {
            BoardButton boardButton = Instantiate(boardButtonPrefab, transform);
            boardButton.name = $"BoardButton {(i + 1)}";
            boardButton.SetIndex((ushort)i);
            boardButtonList.Add(boardButton);
        }
    }


    public void ButtonClicked(ushort index)
    {
        if(isMyTurn)
            HandleButtonClickServerRPC(index);
    }


    private void UpdateButtonText(ushort index, string str)
    {
        boardButtonList[index].UpdateText(str);  
    }


    private void DisableButton(ushort index)
    {
        boardButtonList[index].DisableButton();
    }


    private void HandleXOText(ulong clientIdX, ulong clientIdO)
    {
        if(NetworkManager.Singleton.LocalClientId == clientIdX)
        {
            OnXOTextSet.Invoke("X");
        }

        else
        {
            OnXOTextSet.Invoke("O");
        }
    }


    public bool IsCurrentPlayer()
    {
        return isMyTurn;
    }


    public bool IsDraw()
    {
        return turnCount_NV.Value == 9;
    }



    public void ResetBoard()
    {
        for(int i= 0; i < boardButtonList.Count; i++)
        {
            boardButtonList[i].ResetButton();
        }

        GetXOTextServerRPC();
        CheckTurn();

        if (IsHost)
        {
            ResetBoard_Server();
        }
    }

    //For Server Only
    private void SetRandomPlayers_Server()
    {
        //Improve this logic, this should run when all players have connected and you
        //shouold take their ids from the network manager and set them randomly

        //Set random players
        if(UnityEngine.Random.value > 0.5)
        {
            playerTurnMap_S[PlayerTurn.P1] = 0;
            playerTurnMap_S[PlayerTurn.P2] = 1;
        }

        else
        {
            playerTurnMap_S[PlayerTurn.P1] = 1;
            playerTurnMap_S[PlayerTurn.P2] = 0;
        }

    }


    private void NextPlayer_Server()
    {
        currPlayer_S = (PlayerTurn)(((int)currPlayer_S + 1) % Enum.GetNames(typeof(PlayerTurn)).Length);
        currPlayer_NV.Value = playerTurnMap_S[currPlayer_S];
    }


    private void HandleButtonText_Server(ushort index)
    {
        string buttonText = playerTurnMap_S[PlayerTurn.P1] == currPlayer_NV.Value ? "X" : "O";
        HandleButtonClickClientRPC(index, buttonText);
    }

    private bool CheckWinner_Server()
    {

        if(turnCount_NV.Value < 5)
            return false;

        //for rows
        for(int i = 0; i < 7; i+=3)
        {
            if (!CheckValues_Server(i, i + 1))
                continue;

            if (!CheckValues_Server(i, i + 2))
                continue;

            return true;
        }


        //for columns
        for(int i = 0; i < 3; i++ )
        {
            if (!CheckValues_Server(i, i + 3))
                continue;

            if (!CheckValues_Server(i, i + 6)) 
                continue;

            return true;
        }


        if(CheckValues_Server(0,4) && CheckValues_Server(0,8))
            return true;

        if (CheckValues_Server(2, 4) && CheckValues_Server(2, 6))
            return true;


        return false;
    }


    private bool CheckValues_Server(int ind_1, int ind_2)
    {
        string str1 = boardButtonList[ind_1].GetText();
        string str2 = boardButtonList[ind_2].GetText();


        if(string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            return false;

        return (str1 == str2);
    }


    private bool CheckDraw_Server()
    {
        if(turnCount_NV.Value == 9)
        {
            return true;
        }

        return false;
    }


    private void UpdateGameState_Server(GameState state)
    {
        currState_NV.Value = state;
    }



    private void ResetBoard_Server()
    {
        SetRandomPlayers_Server();

        //Set Intial player turn
        currPlayer_S = PlayerTurn.P1;
        currPlayer_NV.Value = playerTurnMap_S[currPlayer_S];

        turnCount_NV.Value = 0;
        UpdateGameState_Server(GameState.Game);
    }


    //=============RPCs=====================

    [ServerRpc(RequireOwnership = false)]
    private void HandleButtonClickServerRPC(ushort index)
    {
        turnCount_NV.Value++;
        HandleButtonText_Server(index);
        DisableButtonClientRPC(index);

        if(CheckWinner_Server() || CheckDraw_Server())
        {
            UpdateGameState_Server(GameState.GameOver);
            return;
        }

        NextPlayer_Server();
    }


    [ServerRpc(RequireOwnership = false)]
    private void GetXOTextServerRPC()
    {
        HandleXOTextClientRPC(playerTurnMap_S[PlayerTurn.P1], playerTurnMap_S[PlayerTurn.P2]);
    }


    [ClientRpc]
    private void HandleButtonClickClientRPC(ushort index, string buttonText)
    {
        UpdateButtonText(index, buttonText);
    }


    [ClientRpc]
    private void DisableButtonClientRPC(ushort index)
    {
        DisableButton(index);
    }


    [ClientRpc] 
    private void HandleXOTextClientRPC(ulong clientIdX, ulong clientIdO)
    {
        HandleXOText(clientIdX, clientIdO);
    }

}
