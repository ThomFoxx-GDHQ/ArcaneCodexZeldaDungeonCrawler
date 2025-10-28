using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text _healthText;
    [SerializeField] TMP_Text _coinText;
    [SerializeField] private int _coinID = 2;
    [SerializeField] Image _heartContainerImage;
    [SerializeField] private int _heartContainerID = 1;
    [SerializeField] Sprite[] _heartContainersSprites;

    private PlayerInformation _playerInformation;
    private InventoryManager _inventoryManager;

    private void Start()
    {
        _inventoryManager = FindFirstObjectByType<InventoryManager>();
        _playerInformation = FindFirstObjectByType<PlayerInformation>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _healthText.SetText("{0}/{1} hp", _playerInformation.CurrentHealth, _playerInformation.MaxHealth);
        _coinText.SetText(_inventoryManager.InventoryAmount(_coinID).ToString());
        int hearts = _inventoryManager.InventoryAmount(_heartContainerID);
        if (hearts >= 0 &&  hearts < _heartContainersSprites.Length)
            _heartContainerImage.sprite = _heartContainersSprites[hearts];
    }
}
