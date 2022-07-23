using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Handles global UI setup and general 
/// gameplay interactions with finger input
/// </summary>
public class UIBehavior : MonoBehaviour
{
    [Tooltip("The UI scale factor, copied here for easy reference")]
    public float CanvasScaleFactor = 1f;

    [Tooltip("Determines whether the towers can be selected")]
    public bool Locked = false;

    [Tooltip("Whether the game is paused")]
    private bool gamePaused = false;

    // Used for OnClick dialouge advances - disabled for now
    //[Tooltip("Whether the game is in dialogue mode")]
    //private bool activeConversation = false;

    private TriggerDialogueHandler triggerDialogueHandler;
    private void Awake()
    {
        triggerDialogueHandler = TriggerDialogueHandler.Instance;
        CanvasScaleFactor = transform.GetChild(0).GetComponent<Canvas>().scaleFactor;
    }
    private void Start()
    {
        triggerDialogueHandler.OnEndConversation += ConvoInactive;
        triggerDialogueHandler.OnTriggerConversation += ConvoActive;
    }
    private void OnDestroy()
    {
        triggerDialogueHandler.OnEndConversation -= ConvoInactive;
        triggerDialogueHandler.OnTriggerConversation -= ConvoActive;
    }
    /// <summary>
    /// Handles input for selecting and deselecting towers
    /// </summary>
    private void Update()
    {
        // ignore input if game is paused
        if (gamePaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Advance dialogue on-click
            //if (activeConversation)
            //{
            //    triggerDialogueHandler.AdvanceConversation();
            //}

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                if (hit.collider.CompareTag("Tower"))
                {
                    if (!Locked && hit.collider.gameObject != SelectedTower.Instance.CurrentTower)
                    {
                        SelectedTower.Instance.SelectTower(hit.collider.gameObject);
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                if (!hit.collider.CompareTag("Tower"))
                {
                    EventManager.Instance.RaiseGameEvent(EventConstants.DESELECT_TOWERS);
                }
            }
            // Notify that inventory updated
            EventManager.Instance.RaiseGameEvent(EventConstants.UPDATE_INVENTORY);
        }
    }

    /// <summary>
    /// Locks tower interaction
    /// </summary>
    public void LockTowers()
    {
        Locked = true;
    }

    /// <summary>
    /// Unlocks tower interaction
    /// </summary>
    public void UnlockTowers()
    {
        Locked = false;
    }

    /// <summary>
    /// Enter pause mode (blocks tower interaction)
    /// </summary>
    public void GamePause()
    {
        gamePaused = true;
    }

    /// <summary>
    /// Exit pause mode
    /// </summary>
    public void GameResume()
    {
        gamePaused = false;
    }

    /// <summary>
    /// Enter conversation mode
    /// </summary>
    private void ConvoActive(ConversationData _ )
    {
        Debug.Log("convo active");
        //activeConversation = true;
    }

    /// <summary>
    /// Exit conversation mode
    /// </summary>
    private void ConvoInactive()
    {
       // activeConversation = false;
    }
}
