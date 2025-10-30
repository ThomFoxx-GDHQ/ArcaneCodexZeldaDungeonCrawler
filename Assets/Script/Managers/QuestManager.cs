using System.Collections.Generic;
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

    private void Awake()
    {
        if (_quests.Count > 0)
            _OverlayQuestPanel.gameObject.SetActive(true);
        else
            _OverlayQuestPanel.gameObject.SetActive(false);
    }

    public void AddQuest(QuestObject questSO)
    {
        var overlayObject = Instantiate(_questDisplayPrefab, _OverlayQuestPanel).GetComponent<QuestDisplayItem>();
        var menuObject = Instantiate(overlayObject, _MenuDisplayQuestPanel).GetComponent<QuestDisplayItem>();

        overlayObject.AssignQuest(questSO.questObjective, questSO.questCount);
        menuObject.AssignQuest(questSO.questObjective, questSO.questCount);

        var quest = new Quest(questSO, overlayObject.gameObject, menuObject.gameObject);

        _quests.Add(quest);

        if (_quests.Count > 0)
            _OverlayQuestPanel.gameObject.SetActive(true);
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

