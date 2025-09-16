using UnityEngine;
using TMPro;

public class NumpadController : MonoBehaviour
{
    public static WeightController Instance { get; private set; }
    [Header("References")]
    public GameObject numpadPanel;          // assign in Inspector
    public TMP_InputField targetInputField; // assign in Inspector

    void Start()
    {
        numpadPanel.SetActive(false);

        // hook up show/hide in code
        targetInputField.onSelect.AddListener(ShowNumpad);
    }

    void ShowNumpad(string _)
    {
        numpadPanel.SetActive(true);
    }

    void HideNumpad(string _)
    {
        numpadPanel.SetActive(false);
    }

    // called from buttons
    public void AddDigit(string digit)
    {
        targetInputField.text += digit;
    }

    public void Backspace()
    {
        if (targetInputField.text.Length > 0)
        {
            targetInputField.text = targetInputField.text.Substring(0, targetInputField.text.Length - 1);
        }
    }

    public void Clear()
    {
        targetInputField.text = "";
    }

    public void Enter()
    {
        WeightController.Instance.SetWeight(targetInputField.text);
        HideNumpad(""); // closes numpad
    }

    public void Close()
    {
        Clear();
        HideNumpad("");
    }
}
