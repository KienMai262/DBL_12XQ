using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogDescription
{
    public string nameNPC;
    [TextArea] public List<string> Lines;
    public List<Sprite> images;
}

public enum DialogType { Chat, Narrator }

[System.Serializable]
public class ChoiceBranch
{
    [TextArea] public string choiceText;

    [Header("Logic Điều Kiện")]
    public Parameter requiredStats;
    public Conversation successConversation;
    public Conversation failureConversation;

    [Header("Hệ quả")]
    public Parameter parameterChange;
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public DialogType type;
    [TextArea] public string description;
    public DialogDescription sentences;

    [Header("Branching Logic")]
    public List<ChoiceBranch> choices;

    [Header("Game Logic")]
    public bool triggersBattleResolution;

    public bool isCompleteParenthetical;
    public Parameter onCompletionParameterChange;

    public Conversation nextConversation;

    public bool isCanonical;
    [TextArea] public string canonical;
}
