using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ItemType
{ 
    Weapon,
    Equipment,
    Consumable
}


[System.Serializable]
public class Item
{
    [SerializeField] public string name;
    [SerializeField] public ItemType type;
    [SerializeField] public int id;
    [SerializeField] public int value;
    [SerializeField] public Sprite icon;

    public Item (string name, ItemType type, int id)
    {
        this.name = name;
        this.type = type;
        this.id = id;         
        this.value = 0;        

        Debug.Log($"{name} {type} {id}");
    }
}
