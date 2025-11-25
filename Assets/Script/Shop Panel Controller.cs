using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ShopPanelController : MonoBehaviour
{
    [SerializeField] private GameObject _shopItemPanelPrefab;
    [SerializeField] private int[] _shopItemIDs = new int[5];
    [SerializeField] private int[] _shopItemQuantities = new int[5];
    [SerializeField, Tooltip("0 = Default Value Otherwise Cost is Overrides")] 
    private int[] _shopItemCosts = new int[5];
    [SerializeField] private Transform _itemPanel;
    [SerializeField] private TMP_Text _shopNameText;
    [SerializeField] private TMP_Text _totalAmountText;
    [SerializeField] private string _currencyCharater = "₲";
    private int[] _purchaseAmounts = new int[5] { 0, 0, 0, 0, 0 };
    [SerializeField] private Button _purchaseButton;
    private int _purchaseTotalAmount;
    private int _coinID = 2;
    private InventoryManager _inventoryManager;

    private void OnEnable()
    {
        _inventoryManager = FindFirstObjectByType<InventoryManager>();
        PopulateShopItems();
        _totalAmountText.text = $"{_currencyCharater}0";
        UpdatePurchaseButton();
        _purchaseAmounts = new int[5] { 0, 0, 0, 0, 0 };
    }

    private void PopulateShopItems()
    {
        //Error Checking
        if (_shopItemPanelPrefab == null || _itemPanel == null)
        {
            Debug.LogError("Shop item panel prefab or item panel transform is not assigned.");
            return;
        }
        //Clear previous Useage
        if (_itemPanel.childCount > 0)
        {
            foreach (Transform child in _itemPanel)
            {
                Destroy(child.gameObject);
            }
        }

        //Populate Shop Items
        for (int i = 0;  i < _shopItemIDs.Length; i++)
        {
            if (_shopItemIDs[i] == 0) continue; // Skip empty slots
            ShopItemPanelUIController shopItemPanel = Instantiate(_shopItemPanelPrefab, _itemPanel).GetComponent<ShopItemPanelUIController>();
            shopItemPanel.LoadPanel(_shopItemIDs[i], _shopItemQuantities[i], _shopItemCosts[i], i);

            if (_shopItemCosts[i] == 0)
            {
                if (FindFirstObjectByType<InventoryManager>().MasterItemList.TryGetValue(_shopItemIDs[i], out Item item))
                {
                    _shopItemCosts[i] = item.value;
                }
            }
        }
    }

    public void UpdatePurchaseAmount(int index, int value)
    {
        _purchaseAmounts[index] = value;
        UpdateTotalPriceDisplay();
        UpdatePurchaseButton();
    }

    private void UpdateTotalPriceDisplay()
    {
        int totalPrice = 0;
        for (int i = 0; i<_purchaseAmounts.Length; i++)
        {
            if (_purchaseAmounts[i] <= 0) continue;

            totalPrice += _purchaseAmounts[i] * _shopItemCosts[i];
        }
        _totalAmountText.text = $"{_currencyCharater}{totalPrice}";
        _purchaseTotalAmount = totalPrice;
    }

    private void UpdatePurchaseButton()
    {
        if (_inventoryManager.InventoryAmount(_coinID) >= _purchaseTotalAmount)
            _purchaseButton.interactable = true;
        else _purchaseButton.interactable = false;
        if (_purchaseTotalAmount == 0)
            _purchaseButton.interactable = false;
    }

    public void MakePurchase()
    {
        for (int i = 0;i<_purchaseAmounts.Length;i++)
        {
            if (_purchaseAmounts[i] > 0)
            {
                _inventoryManager.AddToInventory(_shopItemIDs[i], _purchaseAmounts[i]);
                _shopItemQuantities[i] -= _purchaseAmounts[i];
            }
        }
        gameObject.SetActive(false);
    }
}
