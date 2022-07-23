using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Allow the creation of a GameEvent asset withing the Unity Editor
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "System/Game Event")]

public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners = new List<GameEventListener>();
    public int caller;

    #region Enable Disable Functions
    private void OnDisable()
    {
        caller = -1;
    }
    #endregion

    #region Raise
    /// <summary>
    /// Call Raise() to alert all listeners that have registered.
    /// Used to allow systems interact without being dependant.
    /// 
    /// </summary>
    public void Raise()
    {
        for(int i=listeners.Count-1; i>=0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }
    /// <summary>
    /// Call Raise(GameObject caller) to alert all listeners that have registered.
    /// Used to allow systems interact without being dependant.
    /// Overload allows for a refrence to the caller to be stored
    /// </summary>
    public void Raise(int caller)
    {
        this.caller = caller;

        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
            listeners[i].SetCaller(this.caller);
        }
    }
    #endregion

    #region Register Listener
    /// <summary>
    /// Registers a listener to this GameEvent.
    /// Used to allow systems interact without being dependant
    /// </summary>
    /// <param name="listener">The listener to be registered</param>
    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }
    #endregion

    #region Unregister Listener
    /// <summary>
    /// Unregisters a listener from this GameEvent.
    /// </summary>
    /// <param name="listener">The listener to be unregistered</param>
    public void UnregisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
    #endregion
}
