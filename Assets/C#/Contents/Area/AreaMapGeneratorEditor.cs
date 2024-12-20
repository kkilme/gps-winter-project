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
        DrawGenerateEventTilesButton();

        EditorGUILayout.LabelField("Info Text");
        DrawGridPositionTextButton();
        DrawTileTypeTextButton();
        DrawPathToBossButton();
        DrawClearDebugObjects();
    }

    private void DrawGenerateSubtileButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Subtile"))
        {
            if(_generator.Init()) _generator.GenerateSubtiles();
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
                if (_generator.Init())
                {
                    _generator.GenerateSubtiles();
                    _generator.GenerateMainTile();
                }
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
                if (_generator.Init())
                {
                    _generator.GenerateSubtiles();
                    _generator.GenerateMainTile();
                    _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                    _playableFieldPos = playableFieldPos;
                    _unplayableFieldPos = unplayableFieldPos;
                }
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
                _generator.GenerateUnplayableFieldObstacles(_unplayableFieldPos);
            }
            else
            {
                if (_generator.Init())
                {
                    _generator.GenerateSubtiles();
                    _generator.GenerateMainTile();
                    _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                    _playableFieldPos = playableFieldPos;
                    _unplayableFieldPos = unplayableFieldPos;
                    _generator.GenerateUnplayableFieldObstacles(unplayableFieldPos);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGeneratePlayableFieldDecorationButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Playable Field Decoration"))
        {   
            if (_generator.CurrentGeneratePhase == AreaMapGenerator.MapGeneratePhase.UnplayableFieldObstacleGenerate)
            {
                _generator.GeneratePlayableFieldObstacles(_playableFieldPos);
            }
            else
            {
                if (_generator.Init())
                {
                    _generator.GenerateSubtiles();
                    _generator.GenerateMainTile();
                    _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                    _playableFieldPos = playableFieldPos;
                    _unplayableFieldPos = unplayableFieldPos;
                    _generator.GenerateUnplayableFieldObstacles(unplayableFieldPos);
                    _generator.GeneratePlayableFieldObstacles(playableFieldPos);
                }
            }

        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGenerateEventTilesButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Event Tiles"))
        {
            if (_generator.CurrentGeneratePhase == AreaMapGenerator.MapGeneratePhase.PlayableFieldObstacleGenerate)
            {
                _generator.GenerateEventTiles();
            }
            else
            {
                if (_generator.Init())
                {
                    _generator.GenerateSubtiles();
                    _generator.GenerateMainTile();
                    _generator.SetupPlayableField(out var playableFieldPos, out var unplayableFieldPos);
                    _playableFieldPos = playableFieldPos;
                    _unplayableFieldPos = unplayableFieldPos;
                    _generator.GenerateUnplayableFieldObstacles(unplayableFieldPos);
                    _generator.GeneratePlayableFieldObstacles(playableFieldPos);
                    _generator.GenerateEventTiles();
                }
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
    private void DrawPathToBossButton()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show path to boss"))
        {
            _generator.ShowPathToBoss();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawClearDebugObjects()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Debug Objects"))
        {
            AreaMapGenerator.ClearDebugObjects();
        }
        EditorGUILayout.EndHorizontal();
    }

}