using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 확장 메소드를 지원하는 Class
public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action<PointerEventData> action,
        Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static void ClearEvent(this GameObject go)
    {
        UI_Base.ClearEvent(go);
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null & go.activeSelf;
    }

    public static string QuestRewardToString(this Data.QuestReward[] rewards)
    {
        string toString = "";

        foreach (var reward in rewards)
        {
            toString += $"{Managers.DataMng.ItemDataDict[reward.RewardDataId].Name} x{reward.Quantity} ";
        }

        return toString;
    }
}
