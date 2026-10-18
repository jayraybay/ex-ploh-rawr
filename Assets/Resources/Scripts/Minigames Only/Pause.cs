using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject playerUI;

    void Start()
    {
        playerUI = GameObject.Find("UI Player");
        GetComponent<Button>().onClick.AddListener(PauseGame);  // Pause button
        pauseMenuUI = GameObject.Find("PauseUI");
        pauseMenuUI.GetComponent<Canvas>().enabled = true;
        pauseMenuUI.SetActive(false); // Hide pause menu initially
    }


    public void PauseGame()
    {
        playerUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");
        pauseMenuUI.SetActive(false);
        playerUI.SetActive(true);
        Time.timeScale = 1f;

    }
}
