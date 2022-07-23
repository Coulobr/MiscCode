using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Contains all system event calls
/// </summary>

[CreateAssetMenu(fileName = "DialogueHandler", menuName = "Singletons/DialogueHandler")]
public class TriggerDialogueHandler : SingletonScriptableObject<TriggerDialogueHandler>
{
    public string note = "Please read class summary! Contains valueable info on how to properly use this system.";

    /// <summary>
    /// Prompts the dialogue UI controller to activate and 
    /// begin the dialogue conversation
    /// </summary>
    public event Action<ConversationData> OnTriggerConversation;
    public void TriggerConversation(ConversationData ConvoToTrigger)
    {
        OnTriggerConversation?.Invoke(ConvoToTrigger);
    }

    /// <summary>
    /// Prompts the dialogue UI controller continue the
    /// conversation if able
    /// </summary>
    public event Action OnAdvanceConversation;
    public void AdvanceConversation()
    {
        OnAdvanceConversation?.Invoke();
    }

    /// <summary>
    /// Prompts the dialogue UI controller to activate and 
    /// begin the dialogue conversation
    /// </summary>
    public event Action OnEndConversation;
    public void EndConversation()
    {
        OnEndConversation?.Invoke();
    }
}
