using UnityEngine;

public class CoinBehaviour : MonoBehaviour, ICollidable
{
    InventoryManager _invManager;
    [SerializeField] int _coinID = 2;
    [SerializeField] int _coinAmount = 1;
    [SerializeField] bool _isRandom = false;
    [SerializeField] Vector2Int _randomAmount;

    void Start()
    {
        _invManager = FindAnyObjectByType<InventoryManager>();

        if (_invManager.InventoryAmount(_coinID) == -1)
            Debug.LogWarning("This Id does not exist in the Inventory Master List", this.gameObject);

        if (_isRandom)
            _coinAmount = Random.Range(_randomAmount.x, _randomAmount.y);

        
    }

    public void OnCollide(Transform other)
    {
        if(other.CompareTag("Player"))
        {
            if (_invManager != null)
            {
                _invManager.AddToInventory(_coinID, _coinAmount);
                Debug.Log($"{_coinAmount} Coins Collected");
                UIManager.Instance.UpdateCoins();
                
            }

            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(this.gameObject);
        }
    }
}
