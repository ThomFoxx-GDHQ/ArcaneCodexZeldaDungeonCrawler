using UnityEngine;
using UnityEngine.InputSystem;

public class QuestNPCBehavior : NPCAbstract
{
    ConfirmationPanelBehavior _confirmationPanel;
    [SerializeField] int _questingItemID;
    [SerializeField] int _questinItemAmount;
    [SerializeField] int _questRewardID;
    [SerializeField] int _questRewardAmount;

    protected override void OnEnable()
    {
        base.OnEnable();
        _confirmationPanel = FindFirstObjectByType<ConfirmationPanelBehavior>(FindObjectsInactive.Include);
        Debug.Log("OnEnable Ran");
    }

    public override void Interact_performed(InputAction.CallbackContext obj)
    {
        if (_canEngage)
        {
            DialogueHandler.OnDialogueComplete += DialogueHandler_OnDialogueComplete;
            UIManager.Instance.StartDialogueSequence(_dialogueSequence, _speakerName, _portraitSprite);
        }
    }

    private void DialogueHandler_OnDialogueComplete()
    {
        //open Confrimation panel
        _confirmationPanel.ActivatePanel(true);
        _confirmationPanel.transform.parent.gameObject.SetActive(true);
        //allow choice
        //Act on choice
    }


}
