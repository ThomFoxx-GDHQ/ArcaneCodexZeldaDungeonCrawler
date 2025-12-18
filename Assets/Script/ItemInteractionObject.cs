using UnityEngine;
using UnityEngine.Events;

public class ItemInteractionObject : MonoBehaviour
{
    [SerializeField, Tooltip("ID for Item required to activate")] int _itemID;
    [SerializeField] int _numberOfItemsNeeded = 1;
    [SerializeField] bool _removeItemOnUse;

    [Space(10)]
    [SerializeField] UnityEvent _event;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int amountOnHand = FindFirstObjectByType<InventoryManager>().InventoryAmount(_itemID);

            if (amountOnHand >= _numberOfItemsNeeded)
            {
                _event?.Invoke();
                if (_removeItemOnUse) FindFirstObjectByType<InventoryManager>().RemoveFromInventory(_itemID, _numberOfItemsNeeded);
            }
        }
    }
}
