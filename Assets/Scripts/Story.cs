using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Story", menuName = "Story")]
public class Story : ScriptableObject
{
    public List<ListConver> story;
}
[Serializable]
public class ListConver
{
    public string nameStory;
    public string nameList;
    public List<Conversation> conversations;
}