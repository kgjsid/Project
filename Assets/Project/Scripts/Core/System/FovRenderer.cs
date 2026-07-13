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

        List<Vector3> viewPoints = new List<Vector3>();

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
                // angle -> transform y축 기준. viewAngle을 절반값 60이라면 (-30 ~ 30) 범위로 하여 하나씩 각도 계산
                float angle = transform.eulerAngles.y - checker.ViewAngle * 0.5f + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);
                viewPoints.Add(newViewCast.hitPoint);
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

        /// <summary>
        /// 각도를 벡터 방향으로 변환헤는 메소드
        /// </summary>
        /// <param name="angleInDegrees"></param>
        private Vector3 DirFromAngle(float angleInDegrees)
        {
            // float => ex 45도
            // 반지름 1인 원에서 빗변의 길이가 1이므로 
            // sin -> y좌표, cos -> x 좌표
            // 앞쪽이 z좌표이므로 변환
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, checker.ViewDistance, checker.ObstacleMask))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * checker.ViewDistance, checker.ViewDistance, globalAngle);
            }
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
