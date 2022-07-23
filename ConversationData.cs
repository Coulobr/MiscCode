using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct Line
{
    [Header("Character Who Will Speak")]
    public Character character;
    [Header("Did They Speak Last Also?")]
    public bool SpokeLast;
    [Header("Max 150 Chars Plz Ty")]
    [TextArea(2, 5)]
    public string Text;

}

[CreateAssetMenu(fileName = "Convorsation", menuName = "Dialogue/New Conversation")]
public class ConversationData : ScriptableObject
{ 
    [Header("Specific Convorsation Variables (Read Mouseover)")]
    [Tooltip("Lock Controls During Convorsation?")]
    public bool ShouldLockControls;
    [Tooltip("Only Trigger Dialogue Sequence Once")]
    public bool ShouldTriggerOnce;
    [Tooltip("Auto End the conversation?")]
    public bool IsAutoComplete;
    public float AutoCompleteTime;
    //[Tooltip("Plays dialogue automatically")]
    //public bool AutomaticConversation;
    [Header("Generic Convorsation Variables")]
    public Line[] lines;


}
