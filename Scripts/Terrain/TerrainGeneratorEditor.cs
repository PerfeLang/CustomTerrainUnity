using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

//https://forum.unity.com/threads/editor-scripting-how-to-add-objects-in-editor-on-click.42097/
[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseUp)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(worldRay, out RaycastHit hitInfo))
            {
                TerrainGenerator terrainGenerator = (TerrainGenerator)target;
                if (!terrainGenerator.Generated)
                    terrainGenerator.Generate();
                Vector3 position = terrainGenerator.WorldToVoxelPosition(hitInfo.point);
                terrainGenerator.ModifyVoxel(position, (Event.current.button * 2) - 3);
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
        Event.current.Use();
    }
}