using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quest
{
    public Data.QuestData QuestData { get; }

    public Quest(Data.QuestData data)
    {
        QuestData = data;
    }
}