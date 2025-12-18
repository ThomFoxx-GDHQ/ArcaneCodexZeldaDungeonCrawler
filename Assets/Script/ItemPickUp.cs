using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickUp : MonoBehaviour
{
    [SerializeField] int _itemId;
    [SerializeField] int _itemAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            FindFirstObjectByType<InventoryManager>().AddToInventory(_itemId, _itemAmount);
            Destroy(this.gameObject);
        }
    }
}
