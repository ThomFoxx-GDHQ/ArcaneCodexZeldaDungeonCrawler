using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class QuestNPCBehavior : NPCAbstract
{
    [Header("Quest Information")]
    [SerializeField] QuestObject _questObjectSO;
    [SerializeField] int _questRewardID;
    [SerializeField] int _questRewardAmount;
    [SerializeField] int _questCompleteDialogueID;
    [SerializeField] bool _dropItemOnComplete;
    [SerializeField] GameObject _droppedItemed;
    ConfirmationPanelBehavior _confirmationPanel;
    int _questingItemID;
    int _questinItemAmount;

    TMP_Text _confirmationPanelText;
    Button _confirmButton;
    InventoryManager _invManager;

    // === Layout Note ===
    // Confirmation Panel should be laid out as follows:
    // Confirmation Dialogue Panel 
    //  |- Dialogue Text
    //  |- Confirm Button
    //  |   |- Text
    //  |- Cancel Button
    //      |- Text
    // ===================

    protected override void OnEnable()
    {
        base.OnEnable();
        _confirmationPanel = FindFirstObjectByType<ConfirmationPanelBehavior>(FindObjectsInactive.Include);
        _confirmationPanelText = _confirmationPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        _confirmButton = _confirmationPanel.transform.GetChild(1).GetComponent<Button>();
        _invManager = FindFirstObjectByType<InventoryManager>();

        if (_questObjectSO != null)
        {
            _questingItemID = _questObjectSO.itemID;
            _questinItemAmount = _questObjectSO.questCount;
        }

        //Debug.Log("OnEnable Ran");
    }

    public override void Interact_performed(InputAction.CallbackContext obj)
    {
        if (_canEngage)
        {
            Debug.Log("Interacting");
            DialogueHandler.Instance.ClearOnCompleteEvent();
            DialogueHandler.OnDialogueComplete += DialogueHandler_OnDialogueComplete;
            UIManager.Instance.StartDialogueSequence(_dialogueSequence, _speakerName, _portraitSprite);
        }
    }

    private void DialogueHandler_OnDialogueComplete()
    {
        //open Confrimation panel
        _confirmationPanel.ActivatePanel(true);
        _confirmationPanel.transform.parent.gameObject.SetActive(true);
        string confirmationText = DialogueService.Instance.GetLine(_questCompleteDialogueID);
        //_confirmationPanelText.text = confirmationText;
        _confirmationPanel.ConfigurePanel(confirmationText);

        if (_invManager.InventoryAmount(_questingItemID) >= _questinItemAmount)
        {
            _confirmButton.interactable = true;
            _confirmButton.onClick.AddListener(() => QuestRewardEarned());
        }
        else _confirmButton.interactable = false;
        //allow choice
        //Act on choice
    }

    public void QuestRewardEarned()
    {
        _invManager.RemoveFromInventory(_questingItemID, _questinItemAmount);
        _invManager.AddToInventory(_questRewardID, _questRewardAmount);

        if (_questingItemID == UIManager.Instance.CoinID || _questRewardID == UIManager.Instance.CoinID)
            UIManager.Instance.UpdateCoins();

        _confirmationPanel.ActivatePanel(false);
        _confirmationPanel.transform.parent.gameObject.SetActive(false);
        Transform dropPosition = FindFirstObjectByType<PlayerInformation>().transform;

        Instantiate(_droppedItemed, dropPosition.position, Quaternion.identity);

        QuestManager.Instance.RemoveQuest(_questObjectSO);
        QuestManager.Instance.CheckQuestsForUpdate(_questingItemID, -_questinItemAmount);
    }    
}
