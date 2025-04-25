using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public static class RenderUtility
{
    private static MaterialPropertyBlock _block;

    /// <summary>
    /// Sprite로 Mesh 생성
    /// </summary>
    public static Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new()
        {
            vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i),
            uv = sprite.uv,
            triangles = Array.ConvertAll(sprite.triangles, i => (int)i)
        };

        return mesh;
    }

    /// <summary>
    /// Renderer Material의 Color를 변경하여 간접적인 밝기 조절 효과 (brightness = 0: 검정색, 1: 원래 색상)
    /// </summary>
    public static void SetRendererBrightness(Renderer renderer, float brightness = 1f)
    {
        if (renderer == null || renderer.sharedMaterial == null)
            return;

        if (_block == null)
            _block = new MaterialPropertyBlock();

        renderer.GetPropertyBlock(_block);

        var baseColor = renderer.sharedMaterial.HasProperty("_Color")
            ? renderer.sharedMaterial.GetColor("_Color")
            : Color.white;

        Color targetColor = baseColor * brightness;
        targetColor.a = 1f; // RenderingMode가 Cutout인 Material 대응

        _block.SetColor("_Color", targetColor);
        renderer.SetPropertyBlock(_block);
    }

    /// <summary>
    /// Renderer Material의 Color를 변경하여 간접적인 밝기 조절 효과 (brightness = 0: 검정색, 1: 원래 색상)
    /// </summary>
    public static void SetRenderersBrightness(Renderer[] renderers, float brightness = 1f)
    {
        foreach (var renderer in renderers)
        {
            SetRendererBrightness(renderer, brightness);
        }
    }
}
