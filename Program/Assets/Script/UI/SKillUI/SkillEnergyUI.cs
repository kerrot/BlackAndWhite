using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class SkillEnergyUI : MonoBehaviour
{
    [SerializeField] public float MaxScale;
    [SerializeField] public float MinScale;
    [SerializeField] float percent;
    [SerializeField] GameObject Full;
    public float Percent
    {
        get { return percent; }
        set 
        { 
            percent = value;
            float s = MinScale + percent * (MaxScale - MinScale);
            transform.localScale = new Vector3(s, s, s);

            Full?.SetActive(percent >= 1f);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SkillEnergyUI))]
public class SkillEnergyUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SkillEnergyUI ui = target as SkillEnergyUI;
        
        EditorGUILayout.LabelField("Percent");
        ui.Percent = EditorGUILayout.Slider(ui.Percent, 0f, 1f);
    }
}
#endif
