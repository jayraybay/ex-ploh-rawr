using UnityEngine;

public class cmd2 : OBJ_CMD
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        QAs = new QA[4];
        QAs[0] = new MultipleChoices("Which selection-flow that compares each value?", new string[] {"Ternary operator", "If statement", "Switch-case", "=="}, 2);
        QAs[1] = new MultipleChoices("Which control-flow that has iterations?", new string[] { "Do-While statement", "Recursion", "For statement", "While statement" }, 2);
        QAs[2] = new MultipleChoices("What is the output below?\nint x = 5;\nif (x > 2) System.out.print(\"Great!\");\nelse System.out.print(\"Lesser...\");", new string[] { "Great!", "Lesser...", "Compilation Error", "No Output" }, 0);
        QAs[3] = new MultipleChoices("Which control-flow guarantees execution at least once?", new string[] { "Do-While Statement", "For statement", "Foreach statement", "While statement" }, 0);
    }
}
