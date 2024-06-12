using UnityEngine;

public class FieldOfViewDisplay : MonoBehaviour
{
    public FieldOfView fov;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    void Start()
    {
        fov = GetComponentInParent<FieldOfView>();  // This finds the FieldOfView component in the parent GameObject
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(fov.viewAngle * meshResolution);
        float stepAngleSize = fov.viewAngle / stepCount;
        ViewCastInfo oldViewCast = new ViewCastInfo();
        int vertexCount = 0;
        Vector3[] vertices = new Vector3[stepCount + 1];
        int[] triangles = new int[stepCount * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = fov.transform.eulerAngles.y - fov.viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        vertices[vertexCount + 1] = edge.pointA;
                        vertices[vertexCount + 2] = edge.pointB;
                        vertices[vertexCount + 3] = newViewCast.point;
                        triangles[vertexCount * 3] = 0;
                        triangles[vertexCount * 3 + 1] = vertexCount + 1;
                        triangles[vertexCount * 3 + 2] = vertexCount + 2;
                        vertexCount += 3;
                    }
                }

                vertices[vertexCount + 1] = newViewCast.point;
                triangles[vertexCount * 3] = 0;
                triangles[vertexCount * 3 + 1] = vertexCount + 1;
                triangles[vertexCount * 3 + 2] = vertexCount + 2;
                vertexCount++;
            }
            oldViewCast = newViewCast;
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, fov.viewRadius, fov.obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * fov.viewRadius, fov.viewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
