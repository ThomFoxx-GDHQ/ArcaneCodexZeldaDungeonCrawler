using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemKeyPair
{
    public int key;
    public Item value;

    public ItemKeyPair(int key, Item value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class ItemDictionaryWrapper
{
    public List<ItemKeyPair> items = new List<ItemKeyPair>();
}

public class InventoryManager : MonoBehaviour
{
    //Master Item List ( ID, Item Class)
    private Dictionary<int, Item> _masterItemList = new Dictionary<int, Item>();
    //Player Inventory ( ID, Count)
    private Dictionary<int, int> _playerInventory = new Dictionary<int, int>();

    public Dictionary<int, Item> MasterItemList => _masterItemList;

    private void Awake()
    {
        ReadInJsonList();
    }

    public void AddToInventory(int id, int count)
    {
        if (!_masterItemList.ContainsKey(id))
        {
            Debug.LogWarning($"Item Id:{id} does not exist in Master Item List.");
            return;
        }

        if (_playerInventory.ContainsKey(id))
        {
            _playerInventory[id] += count;
        }
        else
            _playerInventory.Add(id, count);

        QuestManager.Instance.CheckQuestsForUpdate(id, count);
    }

    public void RemoveFromInventory(int id, int count)
    {
        if (!_masterItemList.ContainsKey(id))
        {
            Debug.LogWarning($"Item Id:{id} does not exist in Master Item List.");
            return;
        }
        if (!_playerInventory.ContainsKey(id))
        {
            //May report to somewhere that the player doesn't have the item
            Debug.LogWarning($"Item ID:{id} does not exist in the Player Inventory.");
            return;
        }
        if (_playerInventory[id] > count)
        {
            _playerInventory[id] -= count;
        }
        else
        {
            _playerInventory.Remove(id);
        }

    }

    public void RemoveFromMasterList(int key)
    {
        if (!_masterItemList.ContainsKey(key))
        {
            Debug.LogWarning($"Item Id:{key} does not exist in Master Item List.");
            return;
        }

        _masterItemList.Remove(key);
        WriteOutToJson();
    }

    public int InventoryAmount(int id)
    {
        if (!_masterItemList.ContainsKey(id))
        {
            Debug.LogWarning($"Item Id:{id} does not exist in Master Item List.");
            return -1;
        }
        if (!_playerInventory.ContainsKey(id) ) return 0;
        else return _playerInventory[id];
    }

    [ContextMenu("Next Key ID")]
    public int GetNextKeyID()
    {
        Debug.Log($"Next Key Value = {_masterItemList.Count}");
        return _masterItemList.Count;
    }

    private bool AddToMasterList(Item item)
    {
        if (_masterItemList.All( i => i.Value.id != item.id))
        {
                _masterItemList.Add(item.id, item);
                return true;
        }
        return false;
    }

    public bool AddItemToMasterList(Item item, bool saveAfter = true)
    {
        bool added = AddToMasterList(item);
        if (added && saveAfter)
        {
            WriteOutToJson();
        }
        return added;
    }

    public void UpdateItemInMasterList(Item item)
    {
        _masterItemList[item.id] = item;
        WriteOutToJson();
    }

    [ContextMenu("Add Test Item")]
    private void AddTestItem()
    {
        Item test = new Item("Test", ItemType.Weapon, 34);
        if (!AddToMasterList(test)) Debug.LogWarning("Load Failed");
        else Debug.Log("Load Success");
    }

    [ContextMenu("Load Master List")]
    public void ReadInJsonList()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "MasterItems.json");
        if (File.Exists(filePath))
        {
            string masterList = File.ReadAllText(filePath);
            var list = JsonUtility.FromJson<ItemDictionaryWrapper>(masterList);
            _masterItemList = ItemDictionaryConverter.ToDictionary(list);
        }
    }

    [ContextMenu("Save Master List")]
    public void WriteOutToJson()
    {
        //Debug.Log(_masterItemList.Count);

        var list = ItemDictionaryConverter.ToWrapper(_masterItemList);

        string masterList = JsonUtility.ToJson(list, true);

        //Debug.Log(masterList);

        string filePath = Path.Combine(Application.persistentDataPath, "MasterItems.json");
        File.WriteAllText(filePath, masterList);
        
    }
}
