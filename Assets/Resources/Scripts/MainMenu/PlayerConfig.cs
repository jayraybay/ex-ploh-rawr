using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerConfig : MonoBehaviour
{

    SpriteRenderer maleMarker, femaleMarker, nonbinaryMarker;
    void Start()
    {
        maleMarker = transform.GetChild(6).GetComponent<SpriteRenderer>();
        femaleMarker = transform.GetChild(7).GetComponent<SpriteRenderer>();
        nonbinaryMarker = transform.GetChild(8).GetComponent<SpriteRenderer>();
        maleMarker.enabled = PlayerPrefs.GetString("PlayerGender") == "M";
        femaleMarker.enabled = PlayerPrefs.GetString("PlayerGender") == "F";
        nonbinaryMarker.enabled = PlayerPrefs.GetString("PlayerGender") != "M" && PlayerPrefs.GetString("PlayerGender") != "F";
    }

    // Update is called once per frame
    void Update() {}

    public void ClickBack() {
        SceneManager.LoadScene("MainMenu");
    }

    public void SelectMale() {
        //Debug.Log("Male Selected!");
        PlayerPrefs.SetString("PlayerGender", "M");
        PlayerPrefs.Save();

        maleMarker.enabled = true;
        femaleMarker.enabled = false;
        nonbinaryMarker.enabled = false;
    }

    public void SelectFemale()
    {
        //Debug.Log("Female Selected!");
        PlayerPrefs.SetString("PlayerGender", "F");
        PlayerPrefs.Save();

        maleMarker.enabled = false;
        femaleMarker.enabled = true;
        nonbinaryMarker.enabled = false;
    }

    public void SelectNonbinary()
    {
        //Debug.Log("Nonbinary Selected!");
        PlayerPrefs.SetString("PlayerGender", "N");
        PlayerPrefs.Save();

        maleMarker.enabled = false;
        femaleMarker.enabled = false;
        nonbinaryMarker.enabled = true;
    }

}
