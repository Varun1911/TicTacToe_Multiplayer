using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    StartMenu,
    WaitingForPlayers,
    Game,
    GameEnd
}


public class BoardManager : MonoBehaviour
{

    public static BoardManager instance {  get; private set; }

    [SerializeField] private BoardButton boardButtonPrefab;
    private List<BoardButton> boardButtonList;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
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
            boardButtonList.Add(boardButton);
        }
    }
}
