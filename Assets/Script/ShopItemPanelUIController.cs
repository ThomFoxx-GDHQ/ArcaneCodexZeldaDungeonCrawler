using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;

public class ShopItemPanelUIController : MonoBehaviour
{
    private int _itemID;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _availableText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _quantityText;
    [SerializeField] private Button _increaseQuantityButton;
    [SerializeField] private Button _decreaseQuantityButton;
    private int _available = 0;
    private int _cost = 0;
    private int _quantity = 0;
    [SerializeField] private Sprite _defaultIcon;
    private Item _item;
    private int _index;

    public void LoadPanel(int id, int available, int cost, int index)
    {
        _index = index;
        _itemID = id;
        _available = available;
        _cost = cost;

        if (FindFirstObjectByType<InventoryManager>().MasterItemList.TryGetValue(_itemID, out _item))
        {
            _itemNameText.text = _item.name;
            if (_item.icon != null)
            {
                _iconImage.sprite = _item.icon;
            }
            else
                _iconImage.sprite = _defaultIcon;

            if (cost == 0)
                _cost = _item.value;
        }
        else Debug.LogError("Item not found or InventoryManager is Null", FindFirstObjectByType<InventoryManager>());

        _availableText.text = _available.ToString();
        _costText.text = _cost.ToString();
        _quantityText.text = _quantity.ToString();
    }

    public void ChangePurchaseQuantity(int quantity)
    {
        if (_quantity <= _available && _quantity >= 0)
        {
            _quantity += quantity;
            if (_quantity < 0) _quantity = 0;
            if (_quantity > _available) _quantity = _available;
        }
        _quantityText.text = _quantity.ToString();
        GetComponentInParent<ShopPanelController>().UpdatePurchaseAmount(_index, _quantity);
    }
}
