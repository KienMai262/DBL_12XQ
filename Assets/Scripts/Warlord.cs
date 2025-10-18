using UnityEngine;
using System.Collections.Generic;

public enum WarlordStatus { Undefeated, Defeated, Ally }

[CreateAssetMenu(fileName = "New Warlord", menuName = "Warlord")]
public class Warlord : ScriptableObject
{
    public string warlordName;
    [TextArea] public string description;
    public Sprite portrait;

    public WarlordStatus status = WarlordStatus.Undefeated;

    // Các chỉ số của sứ quân
    public Parameter stats; 

    // Điều kiện để có thể tấn công sứ quân này
    public List<Warlord> prerequisites; 

    // Các Conversation liên quan
    public Conversation battleStartConversation; // Hội thoại khi bắt đầu trận đánh
    public Conversation victoryConversation;      // Hội thoại khi chiến thắng
    public Conversation defeatConversation;       // Hội thoại khi thất bại
}