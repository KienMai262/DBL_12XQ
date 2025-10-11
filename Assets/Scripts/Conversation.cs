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
    public Conversation nextConversation;
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

    public Conversation nextConversation;

    public bool isCanonical;
    [TextArea] public string canonical;
}
