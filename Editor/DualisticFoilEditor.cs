using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DualisticFoil))]
public class DualisticFoilEditor : Editor
{
    private new DualisticFoil target => base.target as DualisticFoil;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Render"))
        {
            string path = EditorUtility.SaveFilePanel(
                "Save As", 
                Application.dataPath, 
                "Flipbook.png", 
                ".png");
            target.Render(path);
        }
    }
}