using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }


    public void ClickPlay() {
        Debug.Log("Clicked Play");
        SceneManager.LoadScene("SampleScene");
    }
    public void ClickUser() {
        Debug.Log("Clicked User");
    }
    public void ClickSettings() {
        Debug.Log("Clicked Setttings");
    }
}
