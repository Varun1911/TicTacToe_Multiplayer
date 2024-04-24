using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }


    private void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ButtonClick);
    }


    private void ButtonClick()
    {
        Debug.Log("Clicked");
        buttonText.text = "X";
    }
}
