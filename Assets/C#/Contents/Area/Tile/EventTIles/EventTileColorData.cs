using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "EventTile Color Data", menuName = "Scriptable Object/EventTileColorData")]
public class EventTileColorData : ScriptableObject
{
    [Header("Fill Colors")]
    public Color FillDefaultColor;
    public Color FillHighlightColor;

    [Header("Outline Colors")]
    public Color OutlineDefaultColor;
    public Color OutlineHighlightColor;
}
