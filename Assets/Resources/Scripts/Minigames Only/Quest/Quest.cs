using UnityEngine;

public class Quest
{
    public QuestData qd { get; private set; }

    public Quest(QuestData questData)
    {
        if (questData == null)
        {
            Debug.LogError("QuestData is not assigned.");
            return;
        }

        qd = questData;
    }

    public void StartQuest()
    {
        if (qd == null) return;

        qd.isActive = true;
        qd.isCompleted = false;
        qd.currentObjIndex = 0;

        Debug.Log("Quest started: " + qd.questName);
    }

    public void TalkTo(string npcName)
    {
        if (!CanProgress(ObjectiveType.TalkTo)) return;

        if (qd.objectives[qd.currentObjIndex] == npcName)
        {
            Debug.Log("TalkTo objective completed!");
            AdvanceObjective();
        }
    }

    public void GoTo(string tripwireName)
    {
        if (!CanProgress(ObjectiveType.GoTo)) return;

        if (qd.objectives[qd.currentObjIndex] == tripwireName)
        {
            Debug.Log("GoTo objective completed!");
            AdvanceObjective();
        }
    }

    private bool CanProgress(ObjectiveType type)    
    {
        return qd != null
            && qd.isActive
            && !qd.isCompleted
            && qd.currentObjIndex >= 0
            && qd.currentObjIndex < qd.objectives.Length
            && qd.currentObjIndex < qd.objectiveTypes.Length
            && qd.objectiveTypes[qd.currentObjIndex] == type;
    }

    private void AdvanceObjective()
    {
        qd.currentObjIndex++;

        Debug.Log(
            $"[Quest] Advanced to index: {qd.currentObjIndex} " +
            $"| Total objectives: {qd.objectives.Length}"
        );

        if (qd.currentObjIndex >= qd.objectives.Length)
        {
            CompleteQuest();
        }
        else
        {
            Debug.Log(
                $"[Quest] Next objective: " +
                $"{qd.objectiveDescriptions[qd.currentObjIndex]}"
            );
        }
    }

    public void CompleteQuest()
    {
        qd.isCompleted = true;
        qd.isActive = false;

        Debug.Log("Quest completed: " + qd.questName);
    }

    public string getQuestName()
    {
        return qd != null ? qd.questName : "";
    }

    public string getQuestDescription()
    {
        return qd != null ? qd.questDescription : "";
    }

    public string getObjective(int index)
    {
        if (qd == null || index < 0 ||
            index >= qd.objectives.Length)
        {
            return null;
        }

        return qd.objectives[index];
    }

    public ObjectiveType getCurrentObjType(int index)
    {
        if (qd == null || index < 0 ||
            index >= qd.objectiveTypes.Length)
        {
            return ObjectiveType.Kill;
        }

        return qd.objectiveTypes[index];
    }
}