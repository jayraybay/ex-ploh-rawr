using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Scriptable Objects/Quest")]
public class QuestData : ScriptableObject
{
    public bool isActive;
    public bool isCompleted;

    public string questName;
    public string questDescription;

    public string[] objectives;
    public string[] objectiveDescriptions;

    public int currentObjIndex;
    public ObjectiveType[] objectiveTypes;

    private void OnValidate() // if the indexes are not the same amount, send a log to console.
    {
        int objectivesCount = objectives != null ? objectives.Length : 0;
        int typesCount = objectiveTypes != null ? objectiveTypes.Length : 0;
        int descriptionsCount = objectiveDescriptions != null ? objectiveDescriptions.Length : 0;

        if (objectivesCount != typesCount ||
            objectivesCount != descriptionsCount)
        {
            Debug.LogWarning(
                $"[QuestData] Quest: '{questName}' has mismatched array sizes!\n" +
                $"Objectives: {objectivesCount}\n" +
                $"Objective Types: {typesCount}\n" +
                $"Objective Descriptions: {descriptionsCount}",
                this
            );
        }
        else
        {
            Debug.Log(
                $"[QuestData] Quest: '{questName}' is valid. " +
                $"All arrays contain {objectivesCount} elements.",
                this
            );
        }
    }
}

public enum ObjectiveType
{
    Kill,
    GoTo,
    TalkTo,
    Solve,
    TeleportTo
}