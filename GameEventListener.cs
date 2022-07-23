using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows for game events to be neatly contained.
/// Registers each event and raises each event 
/// as they are activated.
/// </summary>
public class GameEventListener : MonoBehaviour {

    #region Instance Variables
    [TextArea(4, 3)]
    public string note;
    [Header("Event Variables")]
    [Tooltip("The event you are listening for.")]
    public GameEvent Event;
    [Tooltip("The response fired when the event is triggered")]
    public UnityEvent Response;
    public List<GameEvent> events;

    private int caller; // used for audio SFX switch
    #endregion 

    #region OnEnable
    /// <summary>
    /// When this script is enabled :
    ///     -Registers this listener to GameEvents listed in the inspector.
    /// Allows systems to communicate and remain independant
    /// </summary>
    private void OnEnable()
    {
        if(Event != null)
        {
            Event.RegisterListener(this);
        }
        foreach(GameEvent e in events)
        {
            e.RegisterListener(this);
        }
    }
    #endregion

    #region OnDisable
    /// <summary>
    /// When this script is disabled :
    ///     -Unregisters this listener to GameEvents listed in the inspector.
    /// Allows systems to communicate and remain independant
    /// </summary>
    private void OnDisable()
    {
        if(Event != null)
        {
            Event.UnregisterListener(this);
        }
        foreach(GameEvent e in events)
        {
            e.UnregisterListener(this);
        }

        caller = -1;
    }
    #endregion

    #region OnEventRaised
    /// <summary>
    /// Called by a GameEvent to trigger a response.
    /// </summary>
    public void OnEventRaised()
    {
        Response.Invoke();
    }
    #endregion

    #region Set Caller Methods
    public void SetCaller(int caller)
    {
        this.caller = caller;
    }
    #endregion
}
