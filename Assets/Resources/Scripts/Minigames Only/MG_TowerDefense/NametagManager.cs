using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NametagManager : MonoBehaviour
{
    [Header("References")]
    // The nametag of the four evil bun prefabs (other) references
    // The tower's nametag
    ScriptableObject nametagScriptableObject;
    [SerializeField] private List <NametagsData> nametags = new List<NametagsData>();
    [SerializeField] private NametagsData winningSO; // winning SO (scriptable object)
    NametagsData ntd;
    int ntdIndex = 0;

    // Void start() defines them
    void Start()
    {
        nametags = new List<NametagsData>(Resources.LoadAll<NametagsData>("Minigames/TowerDefense/QnAs"));
        Debug.Log("Loaded " + nametags.Count + " Nametags from Resources/Minigames/TowerDefense/QnAs");
        foreach (NametagsData ntd in nametags)
        {
            Debug.Log("Loaded Nametag: " + ntd.name);
            ntdIndex++;
        }

        winningSO = GetRandomNametagSO();

        /*
        Debug.Log("Selected Winning Nametag: " + winningSO.name);
        Debug.Log("Question: " + winningSO.question );
        Debug.Log("Correct Answers: " + string.Join(", ", winningSO.correctAnswers)  );
        Debug.Log("Wrong Answers: " + string.Join(", ", winningSO.wrongAnswers) );
        */
    }

    public void rollNewSet()
    {
        winningSO = GetRandomNametagSO();
    }

    public NametagsData GetRandomNametagSO() // Pull a random nametag dataset from list
    {
        return nametags[Random.Range(0, nametags.Count)];
    }

    public NametagsData GetWinningSO()
    {
        return winningSO;
    }

}
