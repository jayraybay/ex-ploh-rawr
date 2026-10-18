
using UnityEngine;
using TMPro;
using System.Collections;

public class QuestUIManager : MonoBehaviour
{
    [Header("Quest UI")]
    [SerializeField] private GameObject questUI;

    private TextMeshProUGUI questNameText;
    private TextMeshProUGUI questDescriptionText;
    private TextMeshProUGUI questObjectiveText;

    [Header("Quest Events")]
    [SerializeField] private QuestEvents questEvents;

    [Header("Completion Feedback")]
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private float feedbackDuration = 1f;

    private int previousObjectiveIndex = -1;
    private Coroutine completionCoroutine;

    private void Awake()
    {
        if (questUI == null)
        {
            Debug.LogError("QuestUI is not assigned!");
            return;
        }

        questNameText = questUI.transform.Find("Q_Name")
            .GetComponent<TextMeshProUGUI>();

        questDescriptionText = questUI.transform.Find("Q_Description")
            .GetComponent<TextMeshProUGUI>();

        questObjectiveText = questUI.transform.Find("Q_Objective")
            .GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        QuestEvents.questUpdated.AddListener(UpdateQuestUI);
    }

    private void OnDisable()
    {
        QuestEvents.questUpdated.RemoveListener(UpdateQuestUI);

        if (completionCoroutine != null)
            StopCoroutine(completionCoroutine);
    }

    private void Start()
    {
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        if (questEvents == null ||
            questEvents.CurrentQuest == null ||
            questEvents.CurrentQuest.qd == null ||
            questObjectiveText == null)
            return;

        QuestData data = questEvents.CurrentQuest.qd;
        int currentIndex = data.currentObjIndex;

        bool objectiveAdvanced =
            previousObjectiveIndex >= 0 &&
            currentIndex > previousObjectiveIndex;

        if (objectiveAdvanced)
        {
            if (completionCoroutine != null)
                StopCoroutine(completionCoroutine);

            completionCoroutine = StartCoroutine(
                ShowCompletionFeedback(data, currentIndex)
            );
        }
        else
        {
            DisplayCurrentObjective(data);
        }

        previousObjectiveIndex = currentIndex;
    }

    private IEnumerator ShowCompletionFeedback(
        QuestData data, int newIndex)
    {
        // Temporarily show the objective that was just completed.
        int completedIndex = newIndex - 1;

        if (completedIndex >= 0 &&
            completedIndex < data.objectives.Length)
        {
            if (data.objectiveDescriptions != null &&
                completedIndex < data.objectiveDescriptions.Length &&
                !string.IsNullOrEmpty(
                    data.objectiveDescriptions[completedIndex]))
            {
                questObjectiveText.text =
                    data.objectiveDescriptions[completedIndex];
            }
            else
            {
                questObjectiveText.text =
                    data.objectives[completedIndex];
            }
        }

        questObjectiveText.color = completedColor;

        yield return new WaitForSeconds(feedbackDuration);

        // After one second, display the next objective.
        DisplayCurrentObjective(data);
        completionCoroutine = null;
    }

    private void DisplayCurrentObjective(QuestData data)
    {
        questNameText.text = data.questName;
        questDescriptionText.text = data.questDescription;

        if (data.isCompleted)
        {
            questObjectiveText.text = "Quest completed!";
        }
        else if (data.isActive &&
                 data.currentObjIndex >= 0 &&
                 data.currentObjIndex < data.objectives.Length)
        {
            int index = data.currentObjIndex;

            if (data.objectiveDescriptions != null &&
                index < data.objectiveDescriptions.Length &&
                !string.IsNullOrEmpty(
                    data.objectiveDescriptions[index]))
            {
                questObjectiveText.text =
                    data.objectiveDescriptions[index];
            }
            else
            {
                questObjectiveText.text = data.objectives[index];
            }
        }
        else
        {
            questObjectiveText.text = "No active objective.";
        }

        questObjectiveText.color = Color.white;
    }
}
