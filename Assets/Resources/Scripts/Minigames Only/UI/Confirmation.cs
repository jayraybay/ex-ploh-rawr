using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Confirmation : MonoBehaviour
{
    [SerializeField] private GameObject confirmationUI;
    private Canvas confirmCanvas;
    private Button buttonNo;
    private Button buttonYes;
    private TextMeshProUGUI descriptionText; 
    [SerializeField] private int descriptionTextMaxLength = 100;


    void Start()
    {
        confirmationUI = GameObject.Find("ConfirmationUI");
        confirmCanvas = confirmationUI.GetComponent<Canvas>();

        buttonNo = confirmationUI.transform.Find("Button No").GetComponent<Button>();
        buttonNo.onClick.AddListener(CloseConfirmationUI);

        buttonYes = confirmationUI.transform.Find("Button Yes").GetComponent<Button>();

        descriptionText = confirmationUI.transform.Find("Description").GetComponent<TextMeshProUGUI>();
    }

    public void EditDescription(string text)
    {
        if (descriptionText == null)
        {
            descriptionText = confirmationUI.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        } else if (descriptionText.text.Length < descriptionTextMaxLength)
            descriptionText.text = text + "\nWould you like to proceed?";
    }

    public void CloseConfirmationUI()
    {
        confirmCanvas.enabled = false;
        buttonYes.onClick.RemoveAllListeners(); // Remove all listeners to prevent multiple invocations
    }
}
