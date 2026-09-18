using UnityEngine;

public class cmd2 : OBJ_CMD
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        QAs.Add(new MultipleChoices("Which selection-flow that compares each value?", new string[] {"Ternary operator", "If statement", "Switch-case", "=="}, 2));
        QAs.Add(new Identify("Who created Java?", "James Gosling", false));
        QAs.Add(new Identify("What does OOP stands for?", "Object Oriented Programming", false));
        QAs.Add(new Trivia("Is Java a shortened name for JavaScript?", false));
        QAs.Add(new Trivia("Strings are surrounded by double-quotes.", true));

        //QAs[2] = new MultipleChoices("What is the output below?\nint x = 5;\nif (x > 2) System.out.print(\"Great!\");\nelse System.out.print(\"Lesser...\");", new string[] { "Great!", "Lesser...", "Compilation Error", "No Output" }, 0);
        //QAs[3] = new MultipleChoices("Which control-flow guarantees execution at least once?", new string[] { "Do-While Statement", "For statement", "Foreach statement", "While statement" }, 0);
    }
}
