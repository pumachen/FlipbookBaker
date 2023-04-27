using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class FlipbookBaker : MonoBehaviour
{
    private Camera m_camera = null;
    private new Camera camera
    {
        get
        {
            if (m_camera == null)
            {
                m_camera = GetComponent<Camera>();
            }
            return m_camera;
        }
    }
    public GameObject target;
    public Vector2Int flipbookRes = new Vector2Int(256, 256);
    public Vector2Int flipbookLayout = new Vector2Int(4, 4);
    public float duration = 1f;

    public void Render(string path)
    {
        int width = flipbookRes.x / flipbookLayout.x;
        int height = flipbookRes.y / flipbookLayout.y;
        int frameCount = flipbookLayout.x * flipbookLayout.y;
        Texture2D blackFB = new Texture2D(flipbookRes.x, flipbookRes.y, TextureFormat.ARGB32, false, false);
        Texture2D whiteFB = new Texture2D(flipbookRes.x, flipbookRes.y, TextureFormat.ARGB32, false, false);
        Rect rect = new Rect(0, 0, width, height);
        RenderTexture blackRT = RenderTexture.GetTemporary(width, height, 32, GraphicsFormat.R8G8B8A8_SRGB);
        RenderTexture whiteRT = RenderTexture.GetTemporary(width, height, 32, GraphicsFormat.R8G8B8A8_SRGB);
        RenderTexture flipbookRT = RenderTexture.GetTemporary(flipbookRes.x, flipbookRes.y, 32, GraphicsFormat.R8G8B8A8_SRGB);
        
        camera.clearFlags = CameraClearFlags.Color;
        for (int frame = 0; frame < frameCount; ++frame)
        {
            int x = frame % flipbookLayout.x * (int)rect.width;
            int y = (flipbookLayout.y - frame / flipbookLayout.x - 1) * (int)rect.height;
            
            float t = Mathf.Lerp(0f, duration, (float)frame / frameCount);
            float deltaT = duration / frameCount;
            
            var particles = target?.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particles)
            {
                ps.Simulate(deltaT, false, frame == 0);
            }

            camera.targetTexture = whiteRT;
            camera.backgroundColor = new Color(1, 1, 1, 1);
            camera.Render();
            Canvas canvas;

            RenderTexture.active = whiteRT;
            whiteFB.ReadPixels(rect, x, y);
            whiteFB.Apply();
            
            camera.targetTexture = blackRT;
            camera.backgroundColor = new Color(0, 0, 0, 1);
            camera.Render();
            RenderTexture.active = blackRT;
            blackFB.ReadPixels(rect, x, y);
            blackFB.Apply();
        }

        Material mat = new Material(Shader.Find("Hidden/FlipbookBaker"));
        mat.SetTexture("_FlipbookBlack", blackFB);
        mat.SetTexture("_FlipbookWhite", whiteFB);
        
        Graphics.Blit(null, flipbookRT, mat);
        blackFB.ReadPixels(new Rect(0, 0, flipbookRes.x, flipbookRes.y), 0, 0);
        blackFB.Apply();
        
        Object.DestroyImmediate(mat);

        camera.targetTexture = null;
        byte[] png = blackFB.EncodeToPNG();
        File.WriteAllBytes(path, png);
        AssetDatabase.Refresh();
        RenderTexture.ReleaseTemporary(blackRT);
        RenderTexture.ReleaseTemporary(whiteRT);
        RenderTexture.ReleaseTemporary(flipbookRT);
    }
}