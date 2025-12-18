using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField] UnityEvent _triggerEvent;
    Collider _collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            _triggerEvent?.Invoke();
        }
    }
}
