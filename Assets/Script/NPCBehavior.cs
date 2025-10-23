using UnityEngine;

public class NPCBehavior : NPCAbstract
{

    public override void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_canEngage)
        {
            UIManager.Instance.StartDialogueSequence(_dialogueSequence, _speakerName, _portraitSprite);
        }
    }
}
