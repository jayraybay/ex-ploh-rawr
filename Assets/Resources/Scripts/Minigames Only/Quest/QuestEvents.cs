using UnityEngine;
using UnityEngine.Events;

public class QuestEvents : MonoBehaviour
{
    public static UnityEvent<string> talkToEvent =
        new UnityEvent<string>();

    public static UnityEvent<string> goToEvent =
        new UnityEvent<string>();

    public static UnityEvent questUpdated =
        new UnityEvent();

    [Header("Quest Configuration")]
    [SerializeField] private QuestData questData;

    private Quest quest;

    public Quest CurrentQuest => quest;

    private void Awake()
    {
        if (questData == null)
        {
            Debug.LogError("Assign a QuestData asset in the Inspector.");
            return;
        }

        quest = new Quest(questData);

        talkToEvent.AddListener(TalkTo);
        goToEvent.AddListener(GoTo);
    }

    public void StartFirstQuest()
    {
        if (quest == null) return;

        quest.StartQuest();
        questUpdated.Invoke();
    }

    private void TalkTo(string npcName)
    {
        if (quest == null) return;

        quest.TalkTo(npcName);
        questUpdated.Invoke();
    }

    private void GoTo(string tripwireName)
    {
        if (quest == null) return;

        quest.GoTo(tripwireName);
        questUpdated.Invoke();
    }

    private void OnDestroy()
    {
        talkToEvent.RemoveListener(TalkTo);
        goToEvent.RemoveListener(GoTo);
    }
}