using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureDemo))]
public class PerlinEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate!"))
        {
            ((TextureDemo)target).ApplyTexture();
        }
    }
}
