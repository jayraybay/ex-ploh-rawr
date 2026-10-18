using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class RSRepairUI : MonoBehaviour
{
    // Note: This class is responsible for the Repair Syntax minigame's UI. 

    [Header("UI Parents")]
    [SerializeField] private GameObject repairUI;
    [SerializeField] private GameObject inputUI;

    // REFERENCES
    private Canvas repairCanvas;
    private Canvas inputCanvas;

    private TMP_Text codeText;
    private TMP_Text identifyText;
    private TMP_Text questionText;
    private TMP_Text feedbackText;
    private TMP_InputField answerInput;

    private Button[] choiceButtons = new Button[4];
    private TMP_Text[] choiceTexts = new TMP_Text[4];
    private Button submitButton;

    private RepairSyntaxLineData currentLineData;
    private CodeLine currentCodeLine; // The physical code line in the world that got hit

    void Start()
    {
        // 1. Find the Repair UI components
        if (repairUI != null)
        {
            repairCanvas = repairUI.GetComponent<Canvas>();

            Transform codeTransform = repairUI.transform.Find("Code");
            Transform identifyTransform = repairUI.transform.Find("Identify");

            if (codeTransform != null) codeText = codeTransform.GetComponent<TMP_Text>();
            if (identifyTransform != null) identifyText = identifyTransform.GetComponent<TMP_Text>();

            if (repairCanvas != null)
            {
                repairCanvas.enabled = false; // Hidden until a code line gets hit
            }
            else
            {
                Debug.LogError("RSRepairUI: Repair UI has no Canvas! (Did you add a Canvas to the Repair UI object?)");
            }
        }
        else
        {
            Debug.LogError("RSRepairUI: Repair UI reference is missing! (Did you assign it in the Inspector?)");
        }

        // 2. Find the Input UI components
        if (inputUI != null)
        {
            inputCanvas = inputUI.GetComponent<Canvas>();

            Transform questionTransform = inputUI.transform.Find("Question");
            Transform feedbackTransform = inputUI.transform.Find("Feedback");
            Transform answerTransform = inputUI.transform.Find("Input Box");
            Transform submitTransform = inputUI.transform.Find("Submit Answer");

            if (questionTransform != null) questionText = questionTransform.GetComponent<TMP_Text>();
            if (feedbackTransform != null) feedbackText = feedbackTransform.GetComponent<TMP_Text>();
            if (answerTransform != null) answerInput = answerTransform.GetComponent<TMP_InputField>();
            if (submitTransform != null) submitButton = submitTransform.GetComponent<Button>();

            if (inputCanvas != null)
            {
                inputCanvas.enabled = false;
            }
            else
            {
                Debug.LogError("RSRepairUI: Input UI has no Canvas! (Did you add a Canvas to the Input UI object?)");
            }
        }
        else
        {
            Debug.LogError("RSRepairUI: Input UI reference is missing! (Did you assign it in the Inspector?)");
        }

        // 3. Validate the required components in one check instead of one error each
        if (codeText == null || identifyText == null || questionText == null || feedbackText == null || answerInput == null || submitButton == null)
        {
            Debug.LogError("RSRepairUI: A UI child is missing! (Are Code, Identify, Question, Feedback, Input Box, and Submit Answer named exactly?)");
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(submitAnswer);
        }

        // 4. Find the four token-choice buttons and connect them
        string[] choiceNames = { "Choice A", "Choice B", "Choice C", "Choice D" };

        for (int i = 0; i < choiceNames.Length; i++)
        {
            if (repairUI == null)
                break;

            Transform choiceTransform = repairUI.transform.Find(choiceNames[i]);

            if (choiceTransform == null)
            {
                Debug.LogError($"RSRepairUI: Could not find {choiceNames[i]}! (Is it a child of the Repair UI?)");
                continue;
            }

            choiceButtons[i] = choiceTransform.GetComponent<Button>();

            Transform textTransform = choiceTransform.Find("Text");
            if (textTransform != null) choiceTexts[i] = textTransform.GetComponent<TMP_Text>();

            if (choiceButtons[i] != null)
            {
                int buttonIndex = i; // Copy i, otherwise every button would use the last value
                choiceButtons[i].onClick.AddListener(() => selectToken(buttonIndex));
            }
        }
    }

    string cleanAnswer(string text) // obeys Java-style whitespace: a space only counts if it separates two words (int i =1; int i= 1;), everything else is ignored
    {
        text = text.Trim();

        StringBuilder result = new StringBuilder();
        bool insideString = false;
        char quoteChar = '\0';

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            // 1. Inside "quotes" or 'quotes', spaces are part of the text, so keep everything as typed
            if (insideString)
            {
                result.Append(c);
                if (c == '\\' && i + 1 < text.Length) // Escaped character like \" so it doesn't end the string
                {
                    result.Append(text[i + 1]);
                    i++;
                }
                else if (c == quoteChar)
                {
                    insideString = false;
                }
                continue;
            }

            if (c == '"' || c == '\'')
            {
                insideString = true;
                quoteChar = c;
                result.Append(c);
                continue;
            }

            // 2. Skip the whole run of spaces, and keep ONE space only if it sits between two word characters
            if (char.IsWhiteSpace(c))
            {
                while (i + 1 < text.Length && char.IsWhiteSpace(text[i + 1]))
                {
                    i++;
                }

                if (result.Length > 0 && i + 1 < text.Length && isWordChar(result[result.Length - 1]) && isWordChar(text[i + 1]))
                {
                    result.Append(' ');
                }
                continue;
            }

            result.Append(c);
        }

        return result.ToString();
    }

    bool isWordChar(char c) // Letters, digits, _ and $ can be part of a Java word
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '$';
    }

    public void ShowRepairUI(RepairSyntaxLineData lineData) // Opens the token-picking screen for a broken line
    {
        if (lineData == null)
        {
            Debug. LogError("RSRepairUI: Line data is missing!");
            return;
        }

        if (repairCanvas == null)
            return;

        currentLineData = lineData;
        currentCodeLine = null;

        // 1. Remember which physical code line was hit
        CodeLine[] codeLines = FindObjectsByType<CodeLine>(FindObjectsSortMode.None);

        for (int i = 0; i < codeLines.Length; i++)
        {
            if (codeLines[i].IsUsingData(lineData))
            {
                currentCodeLine = codeLines[i];
                break;
            }
        }

        // 2. Show the Repair UI and hide the Input UI
        if (inputCanvas != null) inputCanvas.enabled = false;
        repairCanvas.enabled = true;

        // 3. Fill in the broken code and the instruction text
        if (codeText != null) codeText.text = lineData.brokenCode;
        if (identifyText != null) identifyText.text = "Select the incorrect token.";

        // 4. Fill in the choices, and hide the buttons that have no choice
        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < lineData.tokenChoices.Count)
            {
                if (choiceTexts[i] != null) choiceTexts[i].text = lineData.tokenChoices[i];
                if (choiceButtons[i] != null) choiceButtons[i].gameObject.SetActive(true);
            }
            else
            {
                if (choiceButtons[i] != null) choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void selectToken(int index) // Called when the player clicks one of the four choices
    {
        if (currentLineData == null || index < 0 || index >= currentLineData.tokenChoices.Count)
            return;

        string selectedToken = currentLineData.tokenChoices[index];

        if (selectedToken == currentLineData.incorrectToken)
        {
            if (questionText != null) questionText.text = "Enter the correct replacement for: " + selectedToken;

            // Clear leftovers from the last attempt, then swap screens
            if (feedbackText != null) feedbackText.text = "";
            if (answerInput != null) answerInput.text = "";

            if (repairCanvas != null) repairCanvas.enabled = false;
            if (inputCanvas != null) inputCanvas.enabled = true;
        }
        else
        {
            if (identifyText != null) identifyText.text = "Incorrect token. Try again.";

            RSManager manager = FindFirstObjectByType<RSManager>();

            if (manager != null)
            {
                manager.ApplyWrongTokenPenalty(); // -10% of the starting time
            }
        }
    }

    public void submitAnswer() // Called when the player clicks Submit Answer
    {
        if (currentLineData == null || answerInput == null)
            return;

        string playerAnswer = cleanAnswer(answerInput.text);

        if (playerAnswer == cleanAnswer(currentLineData.correctAnswer))
        {
            if (currentCodeLine != null)
            {
                currentCodeLine.SetCorrectedCode(currentLineData.originalCode);
                currentCodeLine.ShowRepairedFeedback();
            }

            RSManager manager = FindFirstObjectByType<RSManager>();

            if (manager != null)
            {
                manager.RegisterSyntaxRepair();
            }

            if (inputCanvas != null) inputCanvas.enabled = false;

            currentLineData = null;
            currentCodeLine = null;
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "Incorrect answer. Try again.";
        }
    }
}