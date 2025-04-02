using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quest
{
    public QuestData QuestData { get; }

    public Quest(QuestData data)
    {
        QuestData = data;
    }
}