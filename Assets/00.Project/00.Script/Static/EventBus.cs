using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// static 기반 매개변수 유/무 대응 EventBus
/// </summary>
public static class EventBus
{
    // 1. 매개변수가 없는 이벤트 관리 (Enum 또는 string 등의 키 기반)
    private static readonly Dictionary<object, Action> _noParamEvents = new Dictionary<object, Action>();

    // 2. 매개변수가 있는 이벤트 관리 (타입 기반)
    private static readonly Dictionary<Type, Delegate> _paramEvents = new Dictionary<Type, Delegate>();

    #region 매개변수가 없는 이벤트 (No Parameters)

    /// <summary>
    /// 매개변수 없는 이벤트 구독
    /// </summary>
    public static void Subscribe(object eventKey, Action listener)
    {
        if (_noParamEvents.ContainsKey(eventKey))
        {
            _noParamEvents[eventKey] += listener;
        }
        else
        {
            _noParamEvents[eventKey] = listener;
        }
    }

    /// <summary>
    /// 매개변수 없는 이벤트 구독 해제
    /// </summary>
    public static void Unsubscribe(object eventKey, Action listener)
    {
        if (_noParamEvents.TryGetValue(eventKey, out Action currentDel))
        {
            currentDel -= listener;
            if (currentDel == null)
                _noParamEvents.Remove(eventKey);
            else
                _noParamEvents[eventKey] = currentDel;
        }
    }

    /// <summary>
    /// 매개변수 없는 이벤트 발행
    /// </summary>
    public static void Publish(object eventKey)
    {
        if (_noParamEvents.TryGetValue(eventKey, out Action action))
        {
            action?.Invoke();
        }
    }

    #endregion

    #region 매개변수가 있는 이벤트 (With Parameters)

    /// <summary>
    /// 매개변수(데이터 구조체/클래스)가 있는 이벤트 구독
    /// </summary>
    public static void Subscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (_paramEvents.TryGetValue(eventType, out Delegate currentDel))
        {
            _paramEvents[eventType] = Delegate.Combine(currentDel, listener);
        }
        else
        {
            _paramEvents[eventType] = listener;
        }
    }

    /// <summary>
    /// 매개변수가 있는 이벤트 구독 해제
    /// </summary>
    public static void Unsubscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (_paramEvents.TryGetValue(eventType, out Delegate currentDel))
        {
            Delegate updatedDel = Delegate.Remove(currentDel, listener);
            if (updatedDel == null)
                _paramEvents.Remove(eventType);
            else
                _paramEvents[eventType] = updatedDel;
        }
    }

    /// <summary>
    /// 매개변수가 있는 이벤트 발행
    /// </summary>
    public static void Publish<T>(T eventData)
    {
        Type eventType = typeof(T);

        if (_paramEvents.TryGetValue(eventType, out Delegate del))
        {
            (del as Action<T>)?.Invoke(eventData);
        }
    }

    #endregion

    /// <summary>
    /// 씬 전환 시 static 참조 누수 방지를 위한 전체 초기화
    /// </summary>
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    //public static void Clear()
    //{
    //    _noParamEvents.Clear();
    //    _paramEvents.Clear();
    //}
}