using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoginTrigger))]
public class LoginTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LoginTrigger loginTrigger = (LoginTrigger)target;
        if (GUILayout.Button("Login"))
        {
            loginTrigger.LoginUser();
        }
    }
}