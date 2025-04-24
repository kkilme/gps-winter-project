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

    /// <summary>
    /// Renderer의 MaterialPropertyBlock을 사용하여 alpha값을 변경
    /// </summary>
    private static void SetRendererAlpha(Renderer renderer, Color baseColor, float alpha)
    {
        if (renderer == null || renderer.sharedMaterial == null) return;

        renderer.GetPropertyBlock(_block);

        Color color = baseColor;
        color.a = alpha;
        _block.SetColor("_Color", color);
        renderer.SetPropertyBlock(_block);
    }

    /// <summary>
    /// Renderer의 MaterialPropertyBlock을 사용하여 Fadein/Out 효과
    /// </summary>
    public static IEnumerator FadeRenderer(Renderer renderer, float duration = 1f, bool fadeIn = false)
    {
        Color baseColor = renderer.sharedMaterial.HasProperty("_Color") ? renderer.sharedMaterial.GetColor("_Color") : Color.white;

        float elapsed = 0f;
        float from = fadeIn ? 0f : 1f;
        float to = fadeIn ? 1f : 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(from, to, t);

            SetRendererAlpha(renderer, baseColor, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        SetRendererAlpha(renderer, baseColor, to);
    }

    /// <summary>
    /// Renderer의 MaterialPropertyBlock을 사용하여 Fadein/Out 효과
    /// </summary>
    public static IEnumerator FadeRenderers(Renderer[] renderers, float duration = 1f, bool fadeIn = false)
    {
        // 렌더러별 baseColor 저장
        List<Color> baseColors = renderers.Select(r => r.sharedMaterial.HasProperty("_Color") ? r.sharedMaterial.GetColor("_Color") : Color.white).ToList();

        float elapsed = 0f;
        float from = fadeIn ? 0f : 1f;
        float to = fadeIn ? 1f : 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(from, to, t);

            for (int i = 0; i < renderers.Length; i++)
            {
                SetRendererAlpha(renderers[i], baseColors[i], alpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 보정
        for (int i = 0; i < renderers.Length; i++)
        {
            SetRendererAlpha(renderers[i], baseColors[i], to);
        }
    }

}
