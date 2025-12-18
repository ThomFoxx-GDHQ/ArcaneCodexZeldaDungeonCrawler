using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    bool _canInteract = false;
    [SerializeField] UnityEvent _interactEvent;
    [SerializeField] InputActionReference _interactionInput;
    [SerializeField] float _actionDelayTime = 0;
    Coroutine _delayRoutine;

    private void Start()
    {
        _interactionInput.action.Enable();
        _interactionInput.action.performed += OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext obj)
    {
        if (_canInteract && _delayRoutine == null)
        {
            if (TryGetComponent<Animator>(out Animator anim))
                anim.SetTrigger("IsOpen");

            _delayRoutine = StartCoroutine(ActionDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _canInteract = false;
    }

    IEnumerator ActionDelay()
    {
        yield return new WaitForSeconds(_actionDelayTime);
        _interactEvent?.Invoke();
        _delayRoutine = null;
    }
}
