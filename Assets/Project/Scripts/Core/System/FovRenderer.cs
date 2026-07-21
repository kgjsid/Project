using System.Collections.Generic;
using UnityEngine;

namespace Core.System
{
    public class FovRenderer : MonoBehaviour
    {
        [SerializeField] private FovChecker checker;
        [SerializeField] float meshResolution = 0.3f;

        private Mesh fovMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private List<Vector3> viewPoints = new List<Vector3>();

        public FovChecker Chekcer
        {
            get
            {
                return checker;
            }
            set
            {
                checker = value;
            }
        }

        private const string FOVMESH_OBJECTNAME = "FOV Mesh";

        private void Awake()
        {   
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            fovMesh = new Mesh();
            fovMesh.name = FOVMESH_OBJECTNAME;
            meshFilter.mesh = fovMesh;

            Material fovMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

            fovMat.color = new Color(1f, 1f, 1f, 0.3f);
            fovMat.SetFloat("_Surface", 1);
            fovMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            fovMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            fovMat.SetInt("_ZWrite", 0);
            fovMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            meshRenderer.material = fovMat;

            meshRenderer.sortingLayerName = "Default";  // 실제 쓰시는 레이어 이름으로 교체
            meshRenderer.sortingOrder = 10;             // 캐릭터/바닥보다 위에 그려지도록 값 조정
        }

        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }

        private void LateUpdate()
        {
            MakeFOVMesh();
        }

        private void MakeFOVMesh()
        {
            if (Mathf.Approximately(meshResolution, 0f))
                return;

            int stepCount = Mathf.RoundToInt(checker.ViewAngle * meshResolution);
            float stepAngleSize = checker.ViewAngle / stepCount;
            viewPoints.Clear();

            for (int i = 0; i <= stepCount; i++)
            {
                // angle -> ViewAngle 기준. viewAngle을 절반값 60이라면 (-30 ~ 30) 범위로 하여 하나씩 각도 계산
                float angleOffset = -checker.ViewAngle * 0.5f + stepAngleSize * i;
                Vector2 dir = RotateVector(checker.FacingDirection, angleOffset);
                viewPoints.Add(ViewCast(dir));
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            fovMesh.Clear();
            fovMesh.vertices = vertices;
            fovMesh.triangles = triangles;
        }

        private Vector2 RotateVector(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        private Vector2 ViewCast(Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, checker.ViewDistance, checker.ObstacleMask);

            return hit.collider != null
                ? hit.point
                : (Vector2)transform.position + dir * checker.ViewDistance;
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 hitPoint;
            public float dist;
            public float angle;

            public ViewCastInfo(bool hit, Vector3 hitPoint, float dist, float angle)
            {
                this.hit = hit;
                this.hitPoint = hitPoint;
                this.dist = dist;
                this.angle = angle;
            }
        }
    }
}
