using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RegisterTrigger))]
public class RegisterTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RegisterTrigger registerTrigger = (RegisterTrigger)target;
        if (GUILayout.Button("Register"))
        {
            registerTrigger.RegisterUser();
        }
    }
}