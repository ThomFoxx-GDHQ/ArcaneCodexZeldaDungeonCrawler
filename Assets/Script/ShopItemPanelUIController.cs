using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemPanelUIController : MonoBehaviour
{
    private int _itemID;
    private Image _iconImage;
    private TMP_Text _itemNameText;
    private TMP_Text _availableText;
    private TMP_Text _costText;
    private TMP_Text _quantityText;
    private Button _increaseQuantityButton;
    private Button _decreaseQuantityButton;
    private int _available = 0;
    private int _cost = 0;
    private int _quantity = 0;
    [SerializeField] private Sprite _defaultIcon;

    public void LoadPanel(int id, int available, int cost)
    {
        _itemID = id;
        _available = available;
        _cost = cost;

        _availableText.text = _available.ToString();
        _costText.text = _cost.ToString();

        FindFirstObjectByType<InventoryManager>().MasterItemList.TryGetValue(_itemID, out Item item);

        _itemNameText.text = item.name;
        if (item.icon != null)
        {
            _iconImage.sprite = item.icon;
        }
        else
            _iconImage.sprite = _defaultIcon;
    }
}
