using TMPro;
using UnityEngine;

public class QuestDisplayItem : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    string _questObjective;
    int _questCounter;
    int _questCount;

    public void UpdateText()
    {
        _text.text = $"{_questObjective}: {_questCounter}/{_questCount}";
    }

    public void UpdateCount(int value)
    {
        _questCounter += value;
        UpdateText();
    }

    public void AssignQuest(string objective,int count, int counter = 0)
    {
        _questObjective = objective;
        _questCount = count;
        _questCounter = counter;
        UpdateText();
    }
}
