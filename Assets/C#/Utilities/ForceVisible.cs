using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MonsterStat))]
public class StatOnInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject);
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(HeroStat))]
public class PlayerStatOnInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif