using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RepairSyntaxData",menuName = "Scriptable Objects/Repair Syntax Data")]
public class RepairSyntaxData : ScriptableObject // The entire code
{
    [Header("Challenge Name")]
    public string Name;

    [Header("Waves")]
    public List<RepairSyntaxWave> waves = new List<RepairSyntaxWave>();
}

[Serializable] // This creates a dropdown
public class RepairSyntaxWave // Its lines
{
    public string waveName;

    [Tooltip("Code lines that appear during this wave.")]
    public List<RepairSyntaxLineData> codeLines = new List<RepairSyntaxLineData>();
}

[Serializable]
public class RepairSyntaxLineData
{
    [Tooltip("Does this line contain a syntax error?")]
    public bool hasSyntaxError;

    [TextArea(2, 5)]
    [Tooltip("Leave blank is no syntax error.")]
    public string brokenCode; 
    [TextArea(2, 5)]
    [Tooltip("What gets displayed after fixing the syntax")]
    public string originalCode; 

    [Tooltip("Maximum of 4 only. One should MATCH incorrect token if an error exists.")]
    public List<string> tokenChoices = new List<string>();
    [Tooltip("The exact incorrect token. Leave empty if the line is valid.")] public string incorrectToken;

    [Tooltip("The correct replacement for the incorrect token.")]
    public string correctAnswer;
}


