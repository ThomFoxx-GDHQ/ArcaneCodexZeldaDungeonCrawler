using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NPCInteract : NPCAbstract
{
    [SerializeField] UnityEvent _OnDialogueComplete;

    public override void Interact_performed(InputAction.CallbackContext obj)
    {
        if (_canEngage)
        {
            DialogueHandler.OnDialogueComplete += () => { 
                _OnDialogueComplete.Invoke(); 
                DialogueHandler.Instance.transform.parent.gameObject.SetActive(false);
            };
            UIManager.Instance.StartDialogueSequence(_dialogueSequence, _speakerName, _portraitSprite);
        }
    }

    private void OnDisable()
    {
        _OnDialogueComplete.RemoveAllListeners();
        FindFirstObjectByType<DialogueHandler>().ClearOnCompleteEvent();
    }
}
