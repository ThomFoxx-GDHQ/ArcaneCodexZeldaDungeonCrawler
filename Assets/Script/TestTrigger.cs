using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    [SerializeField] FoxxTime _startEventTime;
    [SerializeField] FoxxTime _endEventTime;

    private void OnTriggerEnter(Collider other)
    {
        TimeOfDayManager.Instance.ChangeTime(_startEventTime);
    }

    private void OnTriggerExit(Collider other)
    {
        TimeOfDayManager.Instance.ChangeTime(_endEventTime);
    }
}
