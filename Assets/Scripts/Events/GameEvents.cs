using System;
using System.Collections.Specialized;
using UnityEngine;

public static class GameEvents
{
    // HP/TP 변동 이벤트
    public static event Action<int> OnHPChange;
    public static event Action<int> OnTPChange;
    public static event Action<string> OnStanding;
    public static event Action<string> OnDisplay;
    public static event Action<string> OnHide;
    public static event Action<string> onCameraMove;
    public static event Action<string> onText;
    public static event Action<string> onName;
    public static event Action<bool> onNameSpace;

    // HP 변동 이벤트 발생
    public static void TriggerHPChange(int amount)
    {
        Debug.Log($"HP Change Event: {amount}");
        OnHPChange?.Invoke(amount);
    }

    // TP 변동 이벤트 발생
    public static void TriggerTPChange(int amount)
    {
        Debug.Log($"TP Change Event: {amount}");
        OnTPChange?.Invoke(amount);
    }

    public static void TriggerStanding(string bg)
    {
        OnStanding?.Invoke(bg);
    }

    public static void TriggerDisplay(string amount)
    {
        OnDisplay?.Invoke(amount);
    }

    public static void TriggerHide(string amount)
    {
        OnHide?.Invoke(amount);
    }
    
    public static void TriggerCameraMove(string amount)
    {
        onCameraMove?.Invoke(amount);
    }

    public static void TriggerText(string amount)
    {
        onText?.Invoke(amount);
    }

    public static void TriggerName(string amount)
    {
        onName?.Invoke(amount);
    }
    
    public static void TriggerNameSpace(bool amount)
    {
        onNameSpace?.Invoke(amount);
    }
}