using UnityEngine;

public class cmd1 : OBJ_CMD
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QAs = new QA[4];

        QAs[0] = new MultipleChoices("What do you use for single-line comments in Java?", new string[] {"/*","//","#","--"}, 1);
        QAs[1] = new MultipleChoices("Which data type is used for texts such as names, etc.?", new string[] {"Integer", "Double", "String", "Boolean"}, 2);
        QAs[2] = new MultipleChoices("What is the correct code for printing \"Hello World\"?", new string[] { "Console.Write(\"Hello World\");", "print('Hello World')", "console.log(\"Hello World\")", "System.out.print(\"Hello World\");" }, 3);
        QAs[3] = new MultipleChoices("Which data type is used for conditions using True or False?", new string[] { "Boolean", "Byte", "Integer", "Long" }, 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
