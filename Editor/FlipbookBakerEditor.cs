using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlipbookBaker))]
public class FlipbookBakerEditor : Editor
{
    private new FlipbookBaker target => base.target as FlipbookBaker;
    private static string m_savePath;

    private static string savePath
    {
        get
        {
            if (string.IsNullOrEmpty(m_savePath))
            {
                m_savePath = Path.Combine(Application.dataPath, "Flipbook.png");
            }
            return m_savePath;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
                return;
            m_savePath = value;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Render"))
        {
            string dir = Path.GetDirectoryName(savePath);
            string name = Path.GetFileName(savePath);
            string path = EditorUtility.SaveFilePanel(
                "Save As", 
                dir, 
                name, 
                ".png");
            if (string.IsNullOrEmpty(path))
                return;
            savePath = path;
            target.Render(path);
        }
    }
}