using UnityEditor;
using RPG.CameraUI;

// TODO consider changing to a property drawer
[CustomEditor(typeof(CameraRaycaster))]
[CanEditMultipleObjects]
public class CameraRaycasterEditor : Editor
{
    SerializedProperty layerPrioritiesProp;
    bool isLayerPrioritiesUnfolded = true; // store the UI state

    private void OnEnable ()
    {
        layerPrioritiesProp = serializedObject.FindProperty("layerPriorities");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Serialize cameraRaycaster instance

        isLayerPrioritiesUnfolded = EditorGUILayout.Foldout(isLayerPrioritiesUnfolded, "Layer Priorities");
        if (isLayerPrioritiesUnfolded)
        {
            EditorGUI.indentLevel++;
            {
                BindArraySize();
                BindArrayElements();
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties(); // De-serialize back to cameraRaycaster (and create undo point)
    }

    void BindArraySize()
    {
        int currentArraySize = layerPrioritiesProp.arraySize;
        int requiredArraySize = EditorGUILayout.IntField("Size", currentArraySize);
        if (requiredArraySize != currentArraySize)
        {
            layerPrioritiesProp.arraySize = requiredArraySize;
        }
    }

    void BindArrayElements()
    {
        int currentArraySize = layerPrioritiesProp.arraySize;
        for (int i = 0; i < currentArraySize; i++)
        {
            var prop = serializedObject.FindProperty(string.Format("layerPriorities.Array.data[{0}]", i));
            prop.intValue = EditorGUILayout.LayerField(string.Format("Layer {0}:", i), prop.intValue);
        }
    }
}



