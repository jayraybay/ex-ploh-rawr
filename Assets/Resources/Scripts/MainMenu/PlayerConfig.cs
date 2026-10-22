using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerConfig : MonoBehaviour
{

    Animator maleAnim, femaleAnim, nonBinaryAnim;
    Image shad1, shad2, shad3;
    TMP_InputField enterName;
    void Start()
    {
        enterName = GameObject.Find("InputField User").GetComponent<TMP_InputField>();

        maleAnim = GameObject.Find("Select Male").GetComponentInChildren<Animator>();
        femaleAnim = GameObject.Find("Select Female").GetComponentInChildren<Animator>();
        nonBinaryAnim = GameObject.Find("Select Nonbinary").GetComponentInChildren<Animator>();
        shad1 = GameObject.Find("Shadow Blob 1").GetComponent<Image>();
        shad2 = GameObject.Find("Shadow Blob 2").GetComponent<Image>();
        shad3 = GameObject.Find("Shadow Blob 3").GetComponent<Image>();

        shad1.enabled = PlayerPrefs.GetString("PlayerGender") == "M";
        shad2.enabled = PlayerPrefs.GetString("PlayerGender") == "F";
        shad3.enabled = PlayerPrefs.GetString("PlayerGender") != "M" && PlayerPrefs.GetString("PlayerGender") != "F";

        maleAnim.runtimeAnimatorController = (PlayerPrefs.GetString("PlayerGender") == "M") ? (Resources.Load<RuntimeAnimatorController>("Animations/PlayerMale_Controller")) : (null);
        femaleAnim.runtimeAnimatorController = (PlayerPrefs.GetString("PlayerGender") == "F") ? (Resources.Load<RuntimeAnimatorController>("Animations/PlayerFemale_Controller")) : (null);
        nonBinaryAnim.runtimeAnimatorController = (PlayerPrefs.GetString("PlayerGender") != "M" && PlayerPrefs.GetString("PlayerGender") != "F") ? (Resources.Load<RuntimeAnimatorController>("Animations/PlayerNonbinary_Controller")) : (null);

        enterName.text = PlayerPrefs.GetString("PlayerName");
        if (enterName.text == "")
        {
            if (PlayerPrefs.GetString("PlayerGender") == "M") enterName.text = "John";
            else if (PlayerPrefs.GetString("PlayerGender") == "F") enterName.text = "Joy";
            else enterName.text = "Jay";
        }
        else {
            enterName.text = PlayerPrefs.GetString("PlayerName");
        }
    }

    void Update() {}

    public void ClickBack() {
        SceneManager.LoadScene("MainMenu");
        if (enterName.text == "")
        { // empty
            string gender = PlayerPrefs.GetString("PlayerGender");
            if (gender == "M") PlayerPrefs.SetString("PlayerName", "John");
            else if (gender == "F") PlayerPrefs.SetString("Username", "Joy");
            else PlayerPrefs.SetString("Username", "Jay");

        }
        else {
            PlayerPrefs.SetString("Username", enterName.text);
        }
        PlayerPrefs.Save();
    }

    public void SelectMale() {
        //Debug.Log("Male Selected!");
        PlayerPrefs.SetString("PlayerGender", "M");
        PlayerPrefs.Save();
        maleAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerMale_Controller");
        femaleAnim.runtimeAnimatorController = null;
        nonBinaryAnim.runtimeAnimatorController = null;
        shad1.enabled = true;
        shad2.enabled = false;
        shad3.enabled = false;
    }

    public void SelectFemale()
    {
        //Debug.Log("Female Selected!");
        PlayerPrefs.SetString("PlayerGender", "F");
        PlayerPrefs.Save();
        maleAnim.runtimeAnimatorController = null;
        femaleAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerFemale_Controller");
        nonBinaryAnim.runtimeAnimatorController = null;
        shad1.enabled = false;
        shad2.enabled = true;
        shad3.enabled = false;
    }

    public void SelectNonbinary()
    {
        //Debug.Log("Nonbinary Selected!");
        PlayerPrefs.SetString("PlayerGender", "N");
        PlayerPrefs.Save();
        maleAnim.runtimeAnimatorController = null;
        femaleAnim.runtimeAnimatorController = null;
        nonBinaryAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/PlayerNonbinary_Controller");
        shad1.enabled = false;
        shad2.enabled = false;
        shad3.enabled = true;
    }

}
