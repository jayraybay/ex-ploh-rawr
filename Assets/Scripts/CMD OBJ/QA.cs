using UnityEngine;

public class QA
{
    public string QUESTION;
    public string ANSWER;

    private string savedAnswer;

    public virtual bool Answer(string answer) {
        this.savedAnswer = answer;
        return (this.ANSWER == answer);
    }

}

public class MultipleChoices : QA {
    public string[] choices;
    public MultipleChoices(string question, string[] choices, int correct)
    {
        base.QUESTION = question;
        this.choices = choices;
        base.ANSWER = choices[correct];
    }

    public bool AnswerChoice(int choice) {
        return Answer(this.choices[choice]);
    }

}
