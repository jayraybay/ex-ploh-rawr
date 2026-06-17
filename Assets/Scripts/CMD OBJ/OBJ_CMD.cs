using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OBJ_CMD : NPC
{
    int index = 0;

    public QA[] QAs;
    public int score;


    public new void Start()
    {
        isEntity = false;
    }

    public async override void Interact() {
        await QuizTime();
    }

    public async Task QuizTime()
    {
        score = 0;
        animator.SetBool("isAnswering", true);


        Canvas player = GameObject.FindGameObjectWithTag("Canvas_Player").GetComponent<Canvas>();
        player.enabled = false;

        Canvas ui = GameObject.FindGameObjectWithTag("Canvas_Terminal").GetComponent<Canvas>();
        ui.enabled = true;

        TMP_InputField question = ui.transform.Find("Question").GetComponent<TMP_InputField>();
        Button b1 = ui.transform.Find("Choice A").GetComponent<Button>();
        Button b2 = ui.transform.Find("Choice B").GetComponent<Button>();
        Button b3 = ui.transform.Find("Choice C").GetComponent<Button>();
        Button b4 = ui.transform.Find("Choice D").GetComponent<Button>();

        Button[] buttons = new Button[4];
        buttons[0] = b1;
        buttons[1] = b2;
        buttons[2] = b3;
        buttons[3] = b4;


        foreach (QA qa in QAs) {
            //Debug.Log(qa.QUESTION);
            question.text = qa.QUESTION;
            
            b1.interactable = true;
            b2.interactable = true;
            b3.interactable = true;
            b4.interactable = true;
            b1.GetComponent<Image>().color = Color.white;
            b2.GetComponent<Image>().color = Color.white;
            b3.GetComponent<Image>().color = Color.white;
            b4.GetComponent<Image>().color = Color.white;
            b1.transform.Find("Answer 1").GetComponent<TMP_Text>().text = ((MultipleChoices)qa).choices[0];
            b2.transform.Find("Answer 2").GetComponent<TMP_Text>().text = ((MultipleChoices)qa).choices[1];
            b3.transform.Find("Answer 3").GetComponent<TMP_Text>().text = ((MultipleChoices)qa).choices[2];
            b4.transform.Find("Answer 4").GetComponent<TMP_Text>().text = ((MultipleChoices)qa).choices[3];

            string ans = await ChooseAnswer(((MultipleChoices)qa).choices);
            b1.interactable = false;
            b2.interactable = false;
            b3.interactable = false;
            b4.interactable = false;

            if (qa.Answer(ans)) {
                score++;
            }

            foreach (Button b in buttons) {
                if (b.GetComponentInChildren<TMP_Text>().text == qa.ANSWER)
                {
                    b.GetComponent<Image>().color = Color.green;
                }
                else if (b.GetComponentInChildren<TMP_Text>().text == ans) {
                    b.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    b.GetComponent<Image>().color = Color.white;
                }
            }

            
            await Awaitable.WaitForSecondsAsync(1.0f);

        }

        animator.SetBool("isAnswering", false);
        //Debug.Log(score >= (QAs.Length / 2));
        animator.SetBool("isPassed", this.score >= (QAs.Length / 2));

        //Debug.Log("Quiz Done");
        ui.enabled = false;
        player.enabled = true;
    }

    TaskCompletionSource<string> buttonClickTask;
    public Task<string> ChooseAnswer(string[] choices) {
        buttonClickTask = new TaskCompletionSource<string>();

        void OnClick(Button btn) {
            string text = btn.GetComponentInChildren<TMP_Text>().text;

            GameObject.Find("Choice A").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Choice B").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Choice C").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Choice D").GetComponent<Button>().onClick.RemoveAllListeners();

            buttonClickTask.SetResult(text);
        }

        GameObject.Find("Choice A").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("Choice A").GetComponent<Button>()));
        GameObject.Find("Choice B").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("Choice B").GetComponent<Button>()));
        GameObject.Find("Choice C").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("Choice C").GetComponent<Button>()));
        GameObject.Find("Choice D").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("Choice D").GetComponent<Button>()));
            
        return buttonClickTask.Task;
    }

    IEnumerator RevealAnswer() {
        yield return new WaitForSeconds(5.0f);
        Debug.Log("Finised");
    }


}
