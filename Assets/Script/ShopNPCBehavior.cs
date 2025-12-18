using UnityEngine;

public class ShopNPCBehavior : NPCAbstract
{
    [Header("Shop Information")]
    [SerializeField] GameObject _shopPrefab;
    [SerializeField] private int[] _shopItemIDs = new int[5];
    [SerializeField] private int[] _shopItemQuantities = new int[5];
    [SerializeField, Tooltip("0 = Default Value Otherwise Cost is Overrides")]
    private int[] _shopItemCosts = new int[5];

    public override void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_canEngage)
        {
            var shop = Instantiate(_shopPrefab).GetComponent<ShopPanelController>();
            shop.SetupShop(_shopItemIDs, _shopItemQuantities, _shopItemCosts);
            shop.gameObject.SetActive(true);
        }
    }
}
