using System;
using UnityEngine;


/**
 * 
 * S  : 100%
 * A+ : 95
 * A  : 
 * A- : 
 * B+ : 
 * B  : 
 * B- : 60%
 * C+ : 40%
 * C  : 
 * C- : 
 * D+ : 
 * D  : 
 * D- : 
 * F  : 70%
 * 
 * 
 * 
 */

// Abstract class, don't use the QA class!
[Serializable]
public abstract class QA
{
    public string QUESTION;
    public string ANSWER;
    public string savedAnswer; // dont use in the Inspector

    public virtual bool Answer(string answer) {
        this.savedAnswer = answer;
        return (this.ANSWER == answer);
    }
}

/** Types of questions below... */

[Serializable]
public class MultipleChoices : QA
{
    public string[] choices;
    public MultipleChoices() {}
    public MultipleChoices(string question, string[] choices, int correct)
    {
        base.QUESTION = question;
        this.choices = choices;
        base.ANSWER = choices[correct];
    }

    public bool AnswerChoice(int choice)
    {
        return Answer(this.choices[choice]);
    }

    // TODO test later
    public static string[] Shuffle(string[] here)
    {
        string[] res = new string[here.Length];

        for (int i = 0; i < res.Length; i++)
        {
            System.Random r = new System.Random();
            int ri = r.Next(0, here.Length);

            string e1 = here[i];
            string e2 = here[ri];

            here[i] = e2;
            here[ri] = e1;
        }

        return res;
    }
}

[Serializable]
public class Trivia : QA {
    private bool isFact; // unused in Inspector
    public Trivia() {}
    public Trivia(string question, bool trueOrFalse) {
        this.QUESTION = question;
        this.isFact = trueOrFalse;
        base.ANSWER = isFact.ToString().ToLower();
    }
    public bool AnswerFact(bool answer) {
        return Answer(answer.ToString());
    }
    public bool AnswerFact(string answer)
    {
        return Answer(answer.ToLower());
    }
    
    public static bool ParseBool(string here) {
        here = here.ToLower();
        return bool.Parse(here);
    }
}

[Serializable]
public class Identify : QA
{
    public bool isCaseSensitive;

    public Identify() {}
    public Identify(string question, string answer, bool isCaseSensitive)
    {
        base.QUESTION = question;
        base.ANSWER = answer;
        this.isCaseSensitive = isCaseSensitive;
    }

    public bool AnswerIdentity(string answer)
    {
        if (isCaseSensitive) return Answer(answer);
        return base.ANSWER.ToLower() == answer.ToLower();
    }


}
