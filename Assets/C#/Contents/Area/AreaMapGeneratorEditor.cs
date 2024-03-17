using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AreaMapGenerator))]
public class AreaMapGeneratorEditor : Editor
{
    private AreaMapGenerator _generator;

    private List<Vector2Int> _playableFieldPos;
    private List<Vector2Int> _unplayableFieldPos;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _generator = (AreaMapGenerator)target;

        EditorGUILayout.LabelField("Map Generate");
        DrawGenerateSubtileButton();
        DrawGenerateMaintileButton();
        DrawSetupPlayableFieldButton();
        DrawGenerateUnplayableFieldDecorationButton();
        DrawGeneratePlayableFieldDecorationButton();

        EditorGUILayout.LabelField("Info Text");
        DrawGridPositionTextButton();
        DrawTileTypeTextButton();
        DrawClearTextButton();
    }

    private void DrawGenerateSubtileButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Subtile"))
        {
            _generator.Init();
            _generator.GenerateSubtiles();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGenerateMaintileButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Maintile"))
        {
            if (_generator.CurrentGeneratePhase == AreaMapGenerator.MapGeneratePhase.SubtileGenerate)
            {
                _generator.GenerateMainTile();
            }
            else
            {
                _generator.Init();
                _generator.GenerateSubtiles();
                _generator.GenerateMainTile();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSetupPlayableFieldButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Setup Playable Field"))
        {
            if (_generator.CurrentGeneratePhase == AreaMapGenerator.MapGeneratePhase.Maintilegenerate)
            {
                _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                _playableFieldPos = playableFieldPos;
                _unplayableFieldPos = unplayableFieldPos;
            }
            else
            {
                _generator.Init();
                _generator.GenerateSubtiles();
                _generator.GenerateMainTile();
                _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                _playableFieldPos = playableFieldPos;
                _unplayableFieldPos = unplayableFieldPos;
            }

        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGenerateUnplayableFieldDecorationButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Unplayable Field Decoration"))
        {
            if (_generator.CurrentGeneratePhase == AreaMapGenerator.MapGeneratePhase.PlayableFieldSetup)
            {
                _generator.GenerateUnplayableFieldDecoration(_unplayableFieldPos);
            }
            else
            {
                _generator.Init();
                _generator.GenerateSubtiles();
                _generator.GenerateMainTile();
                _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                _playableFieldPos = playableFieldPos;
                _unplayableFieldPos = unplayableFieldPos;
                _generator.GenerateUnplayableFieldDecoration(unplayableFieldPos);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGeneratePlayableFieldDecorationButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Playable Field Decoration"))
        {   
            if (_generator.CurrentGeneratePhase == AreaMapGenerator.MapGeneratePhase.UnplayableFieldDecorationGenerate)
            {
                _generator.GeneratePlayableFieldDecoration(_playableFieldPos);
            }
            else
            {
                _generator.Init();
                _generator.GenerateSubtiles();
                _generator.GenerateMainTile();
                _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                _playableFieldPos = playableFieldPos;
                _unplayableFieldPos = unplayableFieldPos;
                _generator.GenerateUnplayableFieldDecoration(unplayableFieldPos);
                _generator.GeneratePlayableFieldDecoration(playableFieldPos);
            }

        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridPositionTextButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Grid Position"))
        {
            _generator.ShowGridPositionText();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTileTypeTextButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Tile Type"))
        {
            _generator.ShowTileTypeText();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawClearTextButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Text"))
        {
            _generator.ClearText();
        }
        EditorGUILayout.EndHorizontal();
    }
}