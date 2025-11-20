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

    private void OnEnable()
    {
        PopulateShopItems();
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
            shopItemPanel.LoadPanel(_shopItemIDs[i], _shopItemQuantities[i], _shopItemCosts[i]);

        }
    }
}
