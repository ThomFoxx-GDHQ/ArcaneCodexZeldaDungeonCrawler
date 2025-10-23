using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConfirmationPanelBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text _messageText;
    [SerializeField] Button _confirmationButton;
    [SerializeField] Button _cancelButton;

    public void ActivatePanel(bool state)
    {
        gameObject.SetActive(state);
    }

    public void ConfigurePanel(string message, string confirm = "Yes", string  cancel = "No")
    {
        _messageText.text = message;
        _confirmationButton.GetComponentInChildren<TMP_Text>(true).text = confirm;
        _cancelButton.GetComponentInChildren<TMP_Text>(true).text = cancel;

        //Configure what confirmation button does
    }
}
