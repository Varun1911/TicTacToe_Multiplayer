using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI buttonText;

    private ushort index;

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
        BoardManager.Instance.ButtonClicked(index);
    }


    public ushort GetIndex()
    {
        return index;
    }


    public void SetIndex(ushort index)
    {
        this.index = index;
    }


    public void UpdateText(string text)
    {
        buttonText.text = text;
    }

    public string GetText()
    {
        return buttonText.text;
    }


    public void DisableButton()
    {
        button.interactable = false;
    }


    public void ResetButton()
    {
        button.interactable = true;
        buttonText.text = string.Empty;
    }
}
