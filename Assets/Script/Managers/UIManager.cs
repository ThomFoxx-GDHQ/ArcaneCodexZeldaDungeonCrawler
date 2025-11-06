using UnityEngine;
using TMPro;

public class UIManager : MonoSingleton<UIManager>
{
    private HealthUI _healthUI;
    private InventoryManager _inventoryManager;
    private MenuUIManager _menuUIManager;
    private SwordSelectUI _swordSelectUI;
    private SettingsMenu _settingsMenu;
    private DialogueHandler _dialogueHandler;

    [Header("Coin Overlay Display")]
    [SerializeField] private int _coinID = 2;
    [SerializeField] private GameObject _coinPanel;
    [SerializeField] private TMP_Text _coinAmountText;
    [SerializeField] private Animator _coinAnim;

    public int CoinID => _coinID;

    private void Start()
    {
        _healthUI = FindFirstObjectByType<HealthUI>();
        _inventoryManager = FindFirstObjectByType<InventoryManager>();
        _menuUIManager = FindFirstObjectByType<MenuUIManager>(FindObjectsInactive.Include);
        _swordSelectUI = FindFirstObjectByType<SwordSelectUI>(FindObjectsInactive.Include);
        _settingsMenu = FindFirstObjectByType<SettingsMenu>(FindObjectsInactive.Include);
        _dialogueHandler = FindFirstObjectByType<DialogueHandler>(FindObjectsInactive.Include);

        if (_inventoryManager != null)
        {
            _coinPanel?.SetActive(false);
            _coinAmountText.SetText("0");
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        Debug.Log($"UIManager: Current Health = {currentHealth}");
        _healthUI.CalculateHearts(currentHealth);
        //_menuUIManager.UpdateHealth;
    }

    public void UpdateCoins()
    {
        if (_inventoryManager == null) return;

        int amount = _inventoryManager.InventoryAmount(_coinID);

        if (amount > 0)
        {
            _coinAmountText.SetText("{0}", amount);
            _coinPanel.SetActive(true);
            _coinAnim.SetTrigger("Collect");
        }
        else
        {
            _coinAmountText.SetText("0");
            _coinPanel.SetActive(false);            
        }
    }

    public void UpdateSwordDisplay()
    {
        _swordSelectUI.SetSwordDisplay();
    }

    public void ActivateSettingsMenu(bool state)
    {
        _settingsMenu.transform.parent.gameObject.SetActive(state);
        _settingsMenu.gameObject.SetActive(state);
    }

    public void StartDialogueSequence(DialogueSequence sequence, string name, Sprite portrait)
    {
        _dialogueHandler.transform.parent.gameObject.SetActive(true);
        _dialogueHandler.LoadSpeakerInfo(name, portrait);
        _dialogueHandler.LoadDialogue(sequence);
    }

}
