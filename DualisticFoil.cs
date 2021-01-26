using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class DualisticFoil : MonoBehaviour
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

    void Awake()
    {
        camera.backgroundColor = new Color(0, 0, 0, 0);
        camera.clearFlags = CameraClearFlags.Color;
    }

    public void Render(string path)
    {
        int width = flipbookRes.x / flipbookLayout.x;
        int height = flipbookRes.y / flipbookLayout.y;
        int frameCount = flipbookLayout.x * flipbookLayout.y;
        Texture2D flipBook = new Texture2D(flipbookRes.x, flipbookRes.y, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 32, RenderTextureFormat.ARGB32);
        camera.targetTexture = rt;
        camera.backgroundColor = new Color(0, 0, 0, 0);
        camera.clearFlags = CameraClearFlags.Color;
        for (int frame = 0; frame < frameCount; ++frame)
        {
            float t = Mathf.Lerp(0f, duration, (float)frame / frameCount);
            var particles = target?.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particles)
            {
                ps.Simulate(t);
            }

            camera.Render();
            RenderTexture.active = rt;
            int x = frame % flipbookLayout.x * (int)rect.width;
            int y = (flipbookLayout.y - frame / flipbookLayout.x - 1) * (int)rect.height;
            flipBook.ReadPixels(rect, x, y);
            flipBook.Apply();
        }

        camera.targetTexture = null;
        byte[] png = flipBook.EncodeToPNG();
        File.WriteAllBytes(path, png);
        AssetDatabase.Refresh();
        RenderTexture.ReleaseTemporary(rt);
    }
}