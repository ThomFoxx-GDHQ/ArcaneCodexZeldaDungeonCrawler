using UnityEngine;

[CreateAssetMenu(fileName = "QuestObject", menuName = "Scriptable Objects/QuestObject")]
public class QuestObject : ScriptableObject
{
    public string questObjective;
    public int itemID;
    public int questCount;
}
