using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private float resetTime;
    [SerializeField] private Image fillImage;


    private bool isTimerStarted = false;
    private float timer = 0;

    private void Start()
    {
        Hide();
    }


    public void OnGameEnd(GameState state)
    {
        string text;

        if(state == GameState.GameOver)
        {
            if(BoardManager.Instance.IsDraw())
            {
                text = "It's a Draw !!";
            }

            else if (BoardManager.Instance.IsCurrentPlayer())
            {
                text = "You Won !!";
            }

            else
            {
                text = "You Lost :(";
            }


            gameOverText.text = text;

            Show();

            isTimerStarted = true;
        }

    }



    private void Update()
    {
        if(isTimerStarted)
        {
            timer += Time.deltaTime;

            fillImage.fillAmount = timer / resetTime;

            if(timer >= resetTime)
            {
                isTimerStarted = false;
                timer = 0;
                Hide();
                BoardManager.Instance.ResetBoard();
            }
        }
    }

    public void Show()
    {
        panel.SetActive(true);
    }


    public void Hide()
    {
        panel.SetActive(false);
    }
}
