using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScheduleManager))]
public class ScheduleEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var generator = (ScheduleManager)target;
        if (GUILayout.Button("Collect Schedule")) {
            generator.CollectScheduleData();
        }
    }
}
