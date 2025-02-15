using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(CreatureStat))]
public class StatOnInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif