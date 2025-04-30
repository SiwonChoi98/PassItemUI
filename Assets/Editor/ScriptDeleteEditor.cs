using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ScriptDeleteEditor : EditorWindow
{
    [MenuItem("Tools/Delete Scripts")]
    public static void ShowWindow()
    {
        GetWindow<ScriptDeleteEditor>("Script Remover");
    }

    void OnGUI()
    {
        GUILayout.Label("Script Remover", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Select GameObjects in the hierarchy and click one of the buttons below.", MessageType.Info);
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Remove Missing Scripts Only"))
        {
            RemoveMissingScripts();
        }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Remove ALL Scripts"))
        {
            if (EditorUtility.DisplayDialog("Warning", 
                "This will remove ALL scripts from selected GameObjects. This action cannot be undone.\n\nAre you sure?", 
                "Yes, Remove All", "Cancel"))
            {
                RemoveAllScripts();
            }
        }
    }

    private void RemoveMissingScripts()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Please select at least one GameObject.", "OK");
            return;
        }

        int totalRemoved = 0;
        List<string> objectsWithRemovedScripts = new List<string>();

        foreach (GameObject obj in selectedObjects)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            
            if (removed > 0)
            {
                totalRemoved += removed;
                objectsWithRemovedScripts.Add(obj.name);
            }
        }

        if (totalRemoved > 0)
        {
            string message = $"Removed {totalRemoved} missing scripts from {objectsWithRemovedScripts.Count} GameObjects:\n";
            foreach (string objName in objectsWithRemovedScripts)
            {
                message += $"- {objName}\n";
            }
            EditorUtility.DisplayDialog("Success", message, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Result", "No missing scripts found in the selected GameObjects.", "OK");
        }
    }
    
    private void RemoveAllScripts()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Please select at least one GameObject.", "OK");
            return;
        }

        int totalRemoved = 0;
        Dictionary<string, int> objectsWithRemovedScripts = new Dictionary<string, int>();

        foreach (GameObject obj in selectedObjects)
        {
            // First remove missing scripts
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            
            // Then remove all remaining scripts
            Component[] components = obj.GetComponents<MonoBehaviour>();
            int removed = 0;
            
            foreach (Component component in components)
            {
                if (component != null)
                {
                    Undo.DestroyObjectImmediate(component);
                    removed++;
                }
            }
            
            if (removed > 0)
            {
                totalRemoved += removed;
                objectsWithRemovedScripts[obj.name] = removed;
            }
        }

        if (totalRemoved > 0)
        {
            string message = $"Removed {totalRemoved} scripts from {objectsWithRemovedScripts.Count} GameObjects:\n";
            foreach (KeyValuePair<string, int> pair in objectsWithRemovedScripts)
            {
                message += $"- {pair.Key}: {pair.Value} scripts\n";
            }
            EditorUtility.DisplayDialog("Success", message, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Result", "No scripts found in the selected GameObjects.", "OK");
        }
    }
}
