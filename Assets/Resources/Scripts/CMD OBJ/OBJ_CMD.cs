using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[Serializable]
public enum QuestionSeverity {
    Activity,
    Quiz,
    Exam
}


[Serializable]
public enum QuestionType {
    Multiple,
    Identify,
    Fact
}

[Serializable]
public class Content {
    public int index;
    public QuestionType type;
}

public class OBJ_CMD : NPC
{

    // Inspector elements
    public bool isTimed = false;
    /*
        When Timed:
            Choices = 7.5s
            Identify = 10s
            Fact = 5s
     */
    public QuestionSeverity severity;

    [SerializeReference]
    [SubclassSelector]
    public List<QA> QAs = new List<QA>(); // Don't use this in Inspector! Use the list "contents" instead.

    [SerializeReference]
    [SubclassSelector]
    public List<Content> contents = new List<Content>();
    /*
        ^^^ Explanation ^^^
        Index is used for getting the question from Database (namely "Indicate", not "Index" because it's a MySQL reserved-keyword)
        Type is used what type of question, especially useful when it comes to Multiple-Choices. More on later when it comes to Tags.
        Severity is just Activity/Quiz/Exam..
     */

    // Don't touch it
    public int score = 0;

    public new void Start()
    {
        isEntity = false;
        if (contents.Count >= 1) StartCoroutine(GetQuestions());
    }

    public new void Update() {
        transform.Find("Square").GetComponent<SpriteRenderer>().transform.rotation = Quaternion.LookRotation(PlayerInteract.PLAYER_CAMERA.transform.forward);
    }

    IEnumerator GetQuestions()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", "PlayerOne");


        // http://localhost/Explorer/php%20stuffs/game_get_questions.php
        DBC.IP_ADDRESS = PlayerPrefs.GetString("IPaddress");
        Debug.Log(DBC.IP_ADDRESS);

        using (UnityWebRequest www = UnityWebRequest.Post($"http://192.168.1.8/Explorer/php%20stuffs/game_get_questions.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Server Response: " + www.downloadHandler.text);
                ParseContent(www.downloadHandler.text);
            }
        }

    }


    public void ParseContent(string data)
    {
        // get all questions
        string[] questions = data.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        List<QA> newQAs = new List<QA>();

        /* Highly inefficient but okay ... */

        // Get all questions based on provided list of Index
        List<string> potential_questions = new List<string>();
        for (int i = 0; i < questions.Length; i++) {
            if (contents.Any(u => u.index - 1 == i)) { // minus one because the AUTO_INCREMENT starts at one (1)
                potential_questions.Add(questions[i]);
            }
        
        }

        foreach (string q in potential_questions)
        {
            string[] fields = q.Split('|');
            Debug.Log(q); // FIXED! MAKE SURE YOUR VARCHAR DOESN'T HAVE NEW LINES FOR NO REASON
            //Debug.Log(fields[0] + " | " + fields[1] + " | " + fields[2] + " | " + fields[3]);

            if (fields.Length >= 4) // [0] Index, [1] Activity, [2] Quiz, [3] Exam, [4] Answer, [5] Tag
            {
                //int index = Convert.ToInt32(fields[0]);
                string question, answer;
                switch (this.severity)
                {
                    case QuestionSeverity.Activity: question = fields[1]; break;
                    case QuestionSeverity.Quiz:     question = fields[2]; break;
                    case QuestionSeverity.Exam:     question = fields[3]; break;
                    default: return; // never happens
                }
                answer = fields[4];
                //tag = fields[5];

                // Implement when it comes to Choice/Fact
                newQAs.Add( new Identify(question, answer, false) );
            }

        }
        //return newQA;
        QAs = newQAs;
    }


    TaskCompletionSource<bool> quizRespond = new TaskCompletionSource<bool>();

    public async override void Interact() {

        Canvas player = GameObject.FindGameObjectWithTag("Canvas_Player").GetComponent<Canvas>();
        player.enabled = false;
        Canvas readypage = GameObject.Find("Terminal Ready").GetComponent<Canvas>();
        readypage.enabled = true;
        Button buttonReady = readypage.transform.Find("Button Ready").GetComponent<Button>();
        Button buttonClose = readypage.transform.Find("Button Close").GetComponent<Button>();

        TMP_Text count = readypage.transform.Find("Count Value").GetComponent<TMP_Text>();
        TMP_Text timed = readypage.transform.Find("Timed Value").GetComponent<TMP_Text>(); // timed question
        TMP_Text score = readypage.transform.Find("Saved Score").GetComponent<TMP_Text>();
        count.text = $"{QAs.Count}";
        timed.text = (isTimed) ? ("YES") : ("NO");
        score.text = $"Saved Score {this.score}/{QAs.Count}";

        bool isReady = await ReadyQuiz(); // ask before proceeding
        if (isReady)
        {
            await CloseReady();
            await QuizTime(); // take the quiz
        }
        else {
            await CloseReady();
        }
    }  

    public Task<bool> ReadyQuiz()
    {
        quizRespond = new TaskCompletionSource<bool>();
        void OnClick(Button btn) {
            GameObject.Find("Button Ready").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Button Close").GetComponent<Button>().onClick.RemoveAllListeners();
            if (btn.GetComponentInChildren<TMP_Text>().text == "START") quizRespond.SetResult(true);
            else                                                        quizRespond.SetResult(false);
        }
        GameObject.Find("Button Ready").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("Button Ready").GetComponent<Button>()));
        GameObject.Find("Button Close").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("Button Close").GetComponent<Button>()));
        return quizRespond.Task;
    }

    public Task CloseReady()
    {
        Canvas readypage = GameObject.Find("Terminal Ready").GetComponent<Canvas>();
        readypage.enabled = false;
        Canvas player = GameObject.FindGameObjectWithTag("Canvas_Player").GetComponent<Canvas>();
        player.enabled = true;
        return Task.CompletedTask;
    }

    /**
     * Don't get lost, here's this and that
     *  SetMultipleQuestion() : ChooseAnswer()
     *  SetIdentifyQuestion() : IdentifyAnswer()
     *  SetTriviaQuestion()   : FactAnswer()
     * 
     */

    // Starting the quiz
    public async Task QuizTime()
    {
        score = 0;
        animator.SetBool("isAnswering", true);

        Canvas ui = null;
        Canvas player = GameObject.FindGameObjectWithTag("Canvas_Player").GetComponent<Canvas>();
        player.enabled = false;

        foreach (QA qa in QAs) {
            //Debug.Log(qa.QUESTION);
            //question.text = qa.QUESTION;

            if (qa.GetType() == typeof(MultipleChoices)) {
                ui = GameObject.Find("Terminal Multiple").GetComponent<Canvas>();
                await SetMultipleQuestion(qa);
            } else if (qa.GetType() == typeof(Trivia)) {
                ui = GameObject.Find("Terminal Trivia").GetComponent<Canvas>();
                await SetTriviaQuestion(qa);
            } else if (qa.GetType() == typeof(Identify)) {
                ui = GameObject.Find("Terminal Identify").GetComponent<Canvas>();
                await SetIdentifyQuestion(qa);
            } else {
                Debug.Log("No questions asked!");
                ui = null;
                break;
            }
            ui.enabled = true;
            
            await Awaitable.WaitForSecondsAsync(1.0f);
            ui.enabled = false;
        }

        animator.SetBool("isAnswering", false);
        //Debug.Log(score >= (QAs.Count / 2));
        animator.SetBool("isPassed", this.score > (QAs.Count / 2));

        ShowResult();

        //Debug.Log("Quiz Done");
        //if (ui != null) { ui.enabled = false; }
        //player.enabled = true;
    }

    public void ShowResult() {
        Canvas res = GameObject.Find("Terminal Results").GetComponent<Canvas>();
        res.enabled = true;

        TMP_InputField grade = res.transform.Find("Result Score").GetComponent<TMP_InputField>();
        grade.text = $"{score}/{QAs.Count}";

        TMP_InputField state = res.transform.Find("Result State").GetComponent<TMP_InputField>();
        state.text = (this.score > (QAs.Count / 2)) ? ("PASSED") : ("FAILED");

        Button done = res.transform.Find("Button Done").GetComponent<Button>();
        done.onClick.AddListener(DoneResult);
    }

    public void DoneResult() {
        Canvas res = GameObject.Find("Terminal Results").GetComponent<Canvas>();
        Canvas player = GameObject.FindGameObjectWithTag("Canvas_Player").GetComponent<Canvas>();
        res.enabled = false;
        player.enabled = true;
    }

    // forgot what it is but it looks important for later... unused and unaffected
    //IEnumerator RevealAnswer() {
    //    yield return new WaitForSeconds(5.0f);
    //    Debug.Log("Finised");
    //}

    /* Initialize each question */
    public async Task SetMultipleQuestion(QA qa) {
        Canvas ui = GameObject.Find("Terminal Multiple").GetComponent<Canvas>();
        ui.enabled = true;

        TMP_InputField question = ui.transform.Find("Question").GetComponent<TMP_InputField>();
        Button b1 = ui.transform.Find("Choice A").GetComponent<Button>();
        Button b2 = ui.transform.Find("Choice B").GetComponent<Button>();
        Button b3 = ui.transform.Find("Choice C").GetComponent<Button>();
        Button b4 = ui.transform.Find("Choice D").GetComponent<Button>();
        question.text = qa.QUESTION;

        Button[] buttons = new Button[4];
        buttons[0] = b1;
        buttons[1] = b2;
        buttons[2] = b3;
        buttons[3] = b4;

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
        if (qa.Answer(ans)) { score++; }

        foreach (Button b in buttons) {
            if      (b.GetComponentInChildren<TMP_Text>().text == qa.ANSWER) b.GetComponent<Image>().color = Color.green;
            else if (b.GetComponentInChildren<TMP_Text>().text == ans)       b.GetComponent<Image>().color = Color.red;
            else                                                             b.GetComponent<Image>().color = Color.white;
        }
    }

    public async Task SetIdentifyQuestion(QA qa) {
        Canvas ui = GameObject.Find("Terminal Identify").GetComponent<Canvas>();
        //Debug.Log(ui.name);
        ui.enabled = true;

        TMP_InputField question = ui.transform.Find("Question").GetComponent<TMP_InputField>();
        TMP_InputField identify = ui.transform.Find("Identify Answer").GetComponent<TMP_InputField>();
        Button submit = ui.transform.Find("Submit Answer").GetComponent<Button>();
        question.text = qa.QUESTION;
        identify.text = string.Empty;
        submit.GetComponent<Image>().color = Color.white;

        string ans = await IdentifyAnswer();

        if (((Identify) qa).AnswerIdentity(ans)) {
            score++;
            submit.GetComponent<Image>().color = Color.green;
        } else {
            submit.GetComponent<Image>().color = Color.red;
        }
    }
    public async Task SetTriviaQuestion(QA qa) {
        Canvas ui = GameObject.Find("Terminal Trivia").GetComponent<Canvas>();
        ui.enabled = true;

        TMP_InputField question = ui.transform.Find("Question").GetComponent<TMP_InputField>();
        question.text = qa.QUESTION;
        Button bt = ui.transform.Find("True Answer").GetComponent<Button>();
        Button bf = ui.transform.Find("False Answer").GetComponent<Button>();
        bt.GetComponent<Image>().color = Color.white;
        bf.GetComponent<Image>().color = Color.white;

        string ans = await FactAnswer();
        if (((Trivia)qa).AnswerFact(ans))
        {
            score++;
            if      (qa.savedAnswer == "true")  bt.GetComponent<Image>().color = Color.green;
            else if (qa.savedAnswer == "false") bf.GetComponent<Image>().color = Color.green;
        } else {
            if (qa.savedAnswer == "true") {
                bt.GetComponent<Image>().color = Color.red;
                bf.GetComponent<Image>().color = Color.green;
            } else {
                bt.GetComponent<Image>().color = Color.green;
                bf.GetComponent<Image>().color = Color.red;
            }
        }             
    }

    /* Getting the answer */
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

    public Task<string> IdentifyAnswer() {
        buttonClickTask = new TaskCompletionSource<string>();

        void OnClick(Button btn) {
            string text = GameObject.Find("Identify Answer").GetComponent<TMP_InputField>().text;
            GameObject.Find("Submit Answer").GetComponent<Button>().onClick.RemoveAllListeners();
            buttonClickTask.SetResult(text);
        }
        GameObject.Find("Submit Answer").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("Submit Answer").GetComponent<Button>()));
        return buttonClickTask.Task;
    }

    public Task<string> FactAnswer()
    {
        buttonClickTask = new TaskCompletionSource<string>();

        void OnClick(Button btn)
        {
            GameObject.Find("True Answer").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("False Answer").GetComponent<Button>().onClick.RemoveAllListeners();
            buttonClickTask.SetResult(btn.GetComponentInChildren<TMP_Text>().text);
        }
        GameObject.Find("True Answer").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("True Answer").GetComponent<Button>()));
        GameObject.Find("False Answer").GetComponent<Button>().onClick.AddListener(() => OnClick(GameObject.Find("False Answer").GetComponent<Button>()));
        return buttonClickTask.Task;
    }

}
