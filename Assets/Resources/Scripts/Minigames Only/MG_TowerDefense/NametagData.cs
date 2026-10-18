using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Defense QnA", menuName = "Scriptable Objects/TD_Nametags")]
public class NametagsData : ScriptableObject
{
    public string question;
    public string[] correctAnswers;
    public string[] wrongAnswers;
}