using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    //List  Quests
    //Update Quests
    //Remove Quests
    //Track UI Elements

    private List<Quest> _quests = new List<Quest>();
    [SerializeField] Transform _OverlayQuestPanel;
    [SerializeField] Transform _MenuDisplayQuestPanel;
    [SerializeField] GameObject _questDisplayPrefab;

    public override void Init()
    {
        if (_quests.Count > 0)
            _OverlayQuestPanel.gameObject.SetActive(true);
        else
            _OverlayQuestPanel.gameObject.SetActive(false);
    }

    public void AddQuest(QuestObject questSO)
    {
        if (_quests.Exists(q => q.SO == questSO)) return;

        var overlayObject = Instantiate(_questDisplayPrefab, _OverlayQuestPanel).GetComponent<QuestDisplayItem>();
        var menuObject = Instantiate(overlayObject, _MenuDisplayQuestPanel).GetComponent<QuestDisplayItem>();

        overlayObject.AssignQuest(questSO.questObjective, questSO.questCount);
        menuObject.AssignQuest(questSO.questObjective, questSO.questCount);

        var quest = new Quest(questSO, overlayObject.gameObject, menuObject.gameObject);

        _quests.Add(quest);

        if (_quests.Count > 0)
            _OverlayQuestPanel.gameObject.SetActive(true);
    }

    public void RemoveQuest(QuestObject questSO)
    {
        var quest = _quests.FirstOrDefault(q => q.SO == questSO);
        if (quest != null)
        {
            Destroy(quest.OverlayDisplayObject);
            Destroy(quest.MenuDisplayObject);
            _quests.Remove(quest);
        }
    }

    public void CheckQuestsForUpdate(int id, int count)
    {
        foreach (var quest in _quests)
        {
            if(quest.SO.itemID == id)
            {
                quest.OverlayDisplayObject.GetComponent<QuestDisplayItem>().UpdateCount(count);
                quest.MenuDisplayObject.GetComponent<QuestDisplayItem>().UpdateCount(count);
            }
        }

        //Linq method
        /*var quest1 = _quests.Where(s => s.SO.itemID == id);

        foreach (var q in quest1)
        {
            q.OverlayDisplayObject.GetComponent<QuestDisplayItem>().UpdateCount(count);
            q.MenuDisplayObject.GetComponent<QuestDisplayItem>().UpdateCount(count);
        }*/
    }
}

[System.Serializable]
public class Quest
{
    public QuestObject SO;
    public GameObject OverlayDisplayObject;
    public GameObject MenuDisplayObject;

    public Quest (QuestObject so, GameObject overlayDisplayObject, GameObject menuDisplayObject)
    {
        this.SO = so;
        this.OverlayDisplayObject = overlayDisplayObject;
        this.MenuDisplayObject = menuDisplayObject;
    }
}

