using System.Text;
using UnityEngine;

public class MeshLogger : MonoBehaviour
{
    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        StringBuilder message = new StringBuilder();

        foreach (var vertice in meshFilter.mesh.vertices)
            message.AppendLine(vertice.ToString());
        message.AppendLine();

        foreach (var triangle in meshFilter.mesh.triangles)
            message.AppendLine(triangle.ToString());
        message.AppendLine();

        foreach (var uv in meshFilter.mesh.uv)
            message.AppendLine(uv.ToString());

        Debug.Log(message);
    }
}