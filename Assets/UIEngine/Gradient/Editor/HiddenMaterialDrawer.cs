using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[UsedImplicitly]
public class HiddenMaterialDrawer : MaterialPropertyDrawer
{
    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
    }
    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return -EditorGUIUtility.standardVerticalSpacing;
    }
}
