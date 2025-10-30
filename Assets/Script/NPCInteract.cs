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
            DialogueHandler.OnDialogueComplete += () => { _OnDialogueComplete.Invoke(); };
            UIManager.Instance.StartDialogueSequence(_dialogueSequence, _speakerName, _portraitSprite);
        }
    }
}
