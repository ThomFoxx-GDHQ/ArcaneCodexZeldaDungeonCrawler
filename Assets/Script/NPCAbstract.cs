using UnityEngine;

public abstract class NPCAbstract : MonoBehaviour
{
    protected bool _canEngage;
    protected InputSystem_Actions _input;
    [SerializeField] protected string _speakerName;
    [SerializeField] protected Sprite _portraitSprite;
    [SerializeField] protected DialogueSequence _dialogueSequence;

    protected virtual void OnEnable()
    {
        _input = new InputSystem_Actions();
        _input.Enable();
        _input.Player.Interact.performed += Interact_performed;
    }

    public virtual void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_canEngage)
        {
            Debug.Log("Intereacted with");
        }
    }

    public void SpawnAtPosition(GameObject go)
    {
        if (go == null) return;

        Instantiate(go, transform.position, transform.rotation, transform.parent);
    }

    private void OnDisable()
    {
        _input.Player.Interact.performed -= Interact_performed;
        _input.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _canEngage = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _canEngage = false;
    }
}
