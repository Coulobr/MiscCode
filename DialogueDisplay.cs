using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main component that listens to events and handles
/// displaying the dialogue 
/// </summary>

public class DialogueDisplay : MonoBehaviour
{
    [TextArea]
    public string DialogueNotes = "This goes in the canvas";
    [Header("Current Convorsation")]
    public ConversationData Conversation;
    public bool ClickToAdvance;
    [Header("Speaker Prefab")]
    public GameObject Speaker;
    private SpeakerUI speakerUI;
    private int activeLineIndex = 0;

    private const string START_TWEEN = "OnStartTween";
    private const string END_TWEEN = "OnEndTween";

    // Need to initialize the singleton 
    public TriggerDialogueHandler TriggerDialogueHandler;

    #region Init & Event Subs
    private void Awake()
    {
        TriggerDialogueHandler = TriggerDialogueHandler.Instance;
        speakerUI = Speaker.GetComponent<SpeakerUI>(); 
    }

    private void OnEnable()
    {
        TriggerDialogueHandler.Instance.OnTriggerConversation -= OnTriggerConvorsation;
        TriggerDialogueHandler.Instance.OnAdvanceConversation -= OnAdvanceConversation;
        TriggerDialogueHandler.Instance.OnTriggerConversation += OnTriggerConvorsation;
        TriggerDialogueHandler.Instance.OnAdvanceConversation += OnAdvanceConversation;
    }

    private void OnDestroy()
    {
        TriggerDialogueHandler.Instance.OnTriggerConversation -= OnTriggerConvorsation;
        TriggerDialogueHandler.Instance.OnAdvanceConversation -= OnAdvanceConversation;
    }
    private void OnDisable()
    {
        TriggerDialogueHandler.Instance.OnTriggerConversation -= OnTriggerConvorsation;
        TriggerDialogueHandler.Instance.OnAdvanceConversation -= OnAdvanceConversation;
    }

    #endregion
    private void Update()
    {
        if (ClickToAdvance && Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnAdvanceConversation();
        }
    }
    /// <summary>
    /// Listens to the ontriggerconversation event and starts a 
    /// new dialogue sequence
    /// </summary>
    /// <param name="m_conversation"></param>
    private void OnTriggerConvorsation(ConversationData m_conversation)
    {
        Conversation = m_conversation;
        OnAdvanceConversation();
    }

    /// <summary>
    /// Debug button in the scene view to manually test event calls
    /// and conversations
    /// </summary>
    /// <param name="convo"></param>
    public void Debug_Button(ConversationData convo)
    {
        TriggerDialogueHandler.Instance.TriggerConversation(convo);
    }

    public void OnAdvanceConversation()
    {
        StartCoroutine(OnAdvanceConversationRoutine());
    }

    /// <summary>
    /// Main dialogue method. Advances the conversation to the next wave
    /// and also checks if its the first or last line of the conversation
    /// to call events and broadcast messages
    /// </summary>
    public IEnumerator OnAdvanceConversationRoutine()
    {
        if(Conversation == null) { 
            StopAllCoroutines(); 
        }

        if (Conversation.IsAutoComplete)
        {
            Invoke("EndConversation", Conversation.AutoCompleteTime);
        }

        //If there are still lines left
        if (activeLineIndex < Conversation.lines.Length)
        {
            // START OF CONVO
            if (activeLineIndex == 0)
            {
                //Broadcast to children components to tween in
                BroadcastMessage("DialogueTween", START_TWEEN);
            }
            yield return new WaitForSeconds(.15f);
            DisplayLine();  //This is when we enable UI n stuff
            activeLineIndex++; //move to next sentence       
        }
        else // END OF CONVO
        {
            TriggerDialogueHandler.Instance.EndConversation();
            this.EndConversation();
        }
    }
    /// <summary>
    /// Handles Displaying a line
    /// </summary>
    private void DisplayLine()
    { 

        Line line = Conversation.lines[activeLineIndex];
        Character character = line.character;

        if (line.SpokeLast)
        {
            ContinueDialogue(speakerUI, line.Text);
        }
        else
        {
            SetNewSpeaker(speakerUI,
              line.Text,
              line.character.FullName,
              line.character.Portrait);
        }
    }
    /// <summary>
    /// Updates only the text
    /// </summary>
    /// <param name="activeSpeaker"></param>
    /// <param name="text"></param>
    public void ContinueDialogue(SpeakerUI activeSpeaker, string text)
    {
        _ = activeSpeaker ?? throw new System.ArgumentNullException(activeSpeaker.ToString());

        //Have to do this to re-trigger the typerwriter effect
        activeSpeaker.DialogueUIComponent.gameObject.SetActive(false);
        activeSpeaker.Dialogue = text;
        activeSpeaker.DialogueUIComponent.gameObject.SetActive(true);
    }

    /// <summary>
    /// When there is a new speaker, we call this function and set the 
    /// portrait, name, text, etc to that new speaker before we enable the UI
    /// </summary>
    /// <param name="activeSpeakerUI">Parent Obj of the Speaker Prefab</param>
    /// <param name="m_text">Text field within the speaker UI</param>
    /// <param name="m_dialogueBoxName">The Speakers Name</param>
    /// <param name="m_portrait">The Speakers IMG</param>
    private void SetNewSpeaker (
    SpeakerUI activeSpeakerUI,
    string m_text,
    string m_SpeakerName,
    Sprite m_portrait )
    {
        //Checking for null
        _ = activeSpeakerUI ?? throw new System.ArgumentNullException(activeSpeakerUI.name.ToString());
        _ = m_text ?? throw new System.ArgumentNullException();
        _ = m_SpeakerName ?? throw new System.ArgumentNullException();
        _ = m_portrait ?? throw new System.ArgumentNullException();

            //Have to do this to re-trigger the typerwriter effect
            activeSpeakerUI.DialogueUIComponent.gameObject.SetActive(false);
            activeSpeakerUI.PortraitUIComponent.gameObject.SetActive(false);

            //Set variables to the new active speaker
            activeSpeakerUI.NameUIComponent.text = m_SpeakerName;
            activeSpeakerUI.Dialogue = m_text;
            activeSpeakerUI.PortraitUIComponent.sprite = m_portrait;

            //Have to do this to re-trigger the typerwriter effect, 
            activeSpeakerUI.DialogueUIComponent.gameObject.SetActive(true);
            activeSpeakerUI.PortraitUIComponent.gameObject.SetActive(true);

            //show UI
            activeSpeakerUI.Show();
    }


    private void EndConversation()
    {
        //Broadcast to children to tween out
        BroadcastMessage("DialogueTween", END_TWEEN);
        activeLineIndex = 0;
        Conversation = null;
    }

    public void ChangePortrait(SpeakerUI speaker)
    {
        speaker.PortraitUIComponent.gameObject.SetActive(false);
        speaker.PortraitUIComponent.gameObject.SetActive(true);
    }

}
