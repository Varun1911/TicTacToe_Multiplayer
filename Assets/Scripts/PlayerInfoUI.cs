using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI XOText;
    [SerializeField] private string yourTurnText;
    [SerializeField] private string opponentTurnText;


    private void Start()
    {
        HideUI();
    }

    public void OnTurnChange(bool isMyTurn)
    {
        if(isMyTurn)
        {
            turnText.text = yourTurnText;
        }

        else
        {
            
            turnText.text = opponentTurnText;
        }
    }


    public void HideUI()
    {
        turnText.gameObject.SetActive(false);
        XOText.gameObject.SetActive(false);
    }


    public void ShowUI()
    {
        turnText.gameObject.SetActive(true);
        //XOText.gameObject.SetActive(true);
    }


    public void SetXOText(string str)
    {
        XOText.text = "You are playing : " + str;
        XOText.gameObject.SetActive(true);
    }
}
