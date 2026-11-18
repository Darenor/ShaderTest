
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LiquidSurfaceController : MonoBehaviour
{
    [Header("Bottle")]
    [SerializeField] private Transform bottle;

    [Header("Liquid")]
    [SerializeField] private Renderer liquidRenderer;

    [Header("Surface")]
    [SerializeField] private float waterLevel = 0.5f;
    [SerializeField] private float radius = 0.45f;
    [SerializeField] private int segments = 64;
    [SerializeField] private float maxMajorRadius = 2.0f;

    private Mesh surfaceMesh;
    private MeshFilter meshFilter;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        surfaceMesh = new Mesh();
        surfaceMesh.name = "Generated Liquid Surface";
        meshFilter.mesh = surfaceMesh;

        propertyBlock = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        if (bottle == null)
            return;

        UpdateSurface();
        UpdateLiquidShader();
    }

    private void UpdateSurface()
    {
        Vector3 axis = bottle.up.normalized;

        Vector3 horizontalNormal = Vector3.up;

        float verticalDot = Mathf.Abs(
            Vector3.Dot(axis, horizontalNormal)
        );

        float safeDot = Mathf.Max(verticalDot, 0.15f);

        float distance = (waterLevel - bottle.position.y) / axis.y;

        if (Mathf.Abs(axis.y) < 0.01f)
            distance = 0f;

        Vector3 center = bottle.position + axis * distance;
        center.y = waterLevel;

        Vector3 majorDirection =
            Vector3.ProjectOnPlane(axis, Vector3.up);

        if (majorDirection.sqrMagnitude < 0.001f)
            majorDirection = bottle.forward;

        majorDirection.Normalize();

        Vector3 minorDirection =
            Vector3.Cross(majorDirection, Vector3.up).normalized;

        float majorRadius = Mathf.Min(
            radius / safeDot,
            maxMajorRadius
        );

        float minorRadius = radius;

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = transform.InverseTransformPoint(center);

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 worldPoint =
                center
                + majorDirection * (Mathf.Cos(angle) * majorRadius)
                + minorDirection * (Mathf.Sin(angle) * minorRadius);

            vertices[i + 1] =
                transform.InverseTransformPoint(worldPoint);
        }

        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segments + 1;

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = next;
            triangles[i * 3 + 2] = current;
        }

        surfaceMesh.Clear();
        surfaceMesh.vertices = vertices;
        surfaceMesh.triangles = triangles;
        surfaceMesh.RecalculateNormals();
        surfaceMesh.RecalculateBounds();
    }

    private void UpdateLiquidShader()
    {
        if (liquidRenderer == null)
            return;

        liquidRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_FillHeight", waterLevel);
        liquidRenderer.SetPropertyBlock(propertyBlock);
    }
}
