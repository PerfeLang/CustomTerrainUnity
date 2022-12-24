using UnityEngine;

public class InteractTerrain : MonoBehaviour
{
    public TerrainGenerator TerrainGenerator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            ClickToModify(-1);
        if (Input.GetKeyDown(KeyCode.Mouse1))
            ClickToModify(1);
    }

    private void ClickToModify(int height)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Vector3 position = TerrainGenerator.WorldToVoxelPosition(raycastHit.point);
            TerrainGenerator.ModifyVoxel(position, height);
        }
    }
}