using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerInput))]
public class InputInspector : Editor 
{
    PlayerInput input
    {
        get
        {
            return (PlayerInput)target;
        }
    }

    string controls;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(controls, MessageType.Info);

        input.controlType = (PlayerInput.ControlType)EditorGUILayout.EnumPopup("Control Type", input.controlType);
        if (input.controlType == PlayerInput.ControlType.Swipes)
        {
            input.JumpOnTap = EditorGUILayout.Toggle("Jump On Tap", input.JumpOnTap);

            if(!input.JumpOnTap)
                controls = "Current Controls is:" + "\n" + "Swipe for jump and Swipe for swap.";
            else
                controls = "Current Controls is:" + "\n" + "Tap for jump and Swipe for swap.";
        }
        else
        {
            input.Invert = EditorGUILayout.Toggle("Invert", input.Invert);

            if (!input.Invert)
                controls = "Current Controls is:" + "\n" + "Left tap for swap and Right tap for jump.";
            else
                controls = "Current Controls is:" + "\n" + "Left tap for jump and Right tap for swap.";
        }

        if (GUI.changed)
            EditorUtility.SetDirty(input);
    }
	
}
