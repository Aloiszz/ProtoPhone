using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Fov : MonoBehaviour
{
    private Mesh mesh;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float fov;
    [SerializeField] private int rayCount = 50;
    [SerializeField] private float viewDistance = 50;
    private Vector3 origin;
    private float startingAngle;
    
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = transform.position;
    }
    void LateUpdate()
    {
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;
        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit raycastHit;
            bool ray = Physics.Raycast(origin, GetVectorFromAngle(angle), out raycastHit, viewDistance, mask);
            
            if (raycastHit.collider == null)
            {
                vertex = origin + GetVectorFromAngle(angle) *viewDistance;
            }
            else
            {
                vertex = raycastHit.point;
            }
            vertices[vertexIndex] = vertex;
            
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
                angle -= angleIncrease;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        
    }


    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}


