using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour { //TODO: make generic
    static readonly Dictionary<string, Action> dictionary = new();
    static readonly Dictionary<string, Action<int>> integerDictionary = new();
    static readonly Dictionary<string, Action<float>> floatDictionary = new();


    #region Normal
    public static void Subscribe(string _eventName, Action _action) {
        if (dictionary.ContainsKey(_eventName))
            dictionary[_eventName] += _action;
        else
            dictionary[_eventName] = _action;
    }
    public static void Unsubscribe(string _eventName, Action _action) {
        if (dictionary.ContainsKey(_eventName))
            dictionary[_eventName] -= _action;
    }

    public static void Trigger(string _eventName, bool logDebug = true) {
        if (dictionary.ContainsKey(_eventName)) {
            dictionary[_eventName]?.Invoke();
            
            if (logDebug) Debug.Log("Event Triggered: " + _eventName);
        }
    }
    #endregion

    #region Int
    public static void SubscribeInteger(string _eventName, Action<int> _action) {
        if (integerDictionary.ContainsKey(_eventName))
            integerDictionary[_eventName] += _action;
        else
            integerDictionary[_eventName] = _action;
    }
    public static void UnsubscribeInteger(string _eventName, Action<int> _action) {
        if (integerDictionary.ContainsKey(_eventName))
            integerDictionary[_eventName] -= _action;
    }
    public static void Trigger(string _eventName, int value, bool logDebug = true) {
        if (integerDictionary.ContainsKey(_eventName)) {
            integerDictionary[_eventName]?.Invoke(value);
            if (logDebug) Debug.Log("Event Triggered: " + _eventName + " With value: " + value);
        }
    }
    #endregion

    #region Float
    public static void SubscribeFloat(string _eventName, Action<float> _action) {
        if (floatDictionary.ContainsKey(_eventName))
            floatDictionary[_eventName] += _action;
        else
            floatDictionary[_eventName] = _action;
    }
    public static void UnsubscribeFloat(string _eventName, Action<float> _action) {
        if (floatDictionary.ContainsKey(_eventName))
            floatDictionary[_eventName] -= _action;
    }
    public static void Trigger(string _eventName, float value, bool logDebug = true) {
        if (floatDictionary.ContainsKey(_eventName)) {
            floatDictionary[_eventName]?.Invoke(value);
            if (logDebug) Debug.Log("Event Triggered: " + _eventName + " With value: " + value);
        }
    }
    #endregion
}