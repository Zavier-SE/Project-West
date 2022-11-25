using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public LayerMask targetLayer;
    public LayerMask obstructsLayer;

    private Mesh mesh;
    private Vector3 origin;
    private float startingDir;
    private float viewRadius;
    [Range(1, 360)] private float viewAngle;
    public int rayCount = 50;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
        viewAngle = 120f;
        viewRadius = 70;
    }

    private void LateUpdate()
    {
        DrawFOV();
    }

    private void DrawFOV()
    {
        float currentAngle = startingDir;
        float angleIncrease = viewAngle / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for(int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit = Physics2D.Raycast(origin, Utils.GetVectorFromAngle(currentAngle), viewRadius, obstructsLayer);
            Vector3 pos = transform.position;
            pos.z = -1;
            transform.position = pos;
            if (raycastHit.collider == null)
            {
                vertex = origin + Utils.GetVectorFromAngle(currentAngle) * viewRadius;
            }
            else
            {
                vertex = raycastHit.point;
            }
            vertices[vertexIndex] = vertex;

            if(i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;

            currentAngle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void SetOrigin(Vector3 origin)
    {
        origin.y -= 0.5f;
        this.origin = origin;
    }

    public void SetAimDir(Vector3 faceDirection)
    {
        startingDir = Utils.GetAngleFromVectorFloat(faceDirection) + viewAngle / 2f;
    }

    public void SetViewAngle(float angle)
    {
        this.viewAngle = angle;
    }

    public void SetViewRadius(float radius)
    {
        this.viewRadius = radius;
    }
}
