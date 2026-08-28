using UnityEngine;

namespace Core.System
{
    public class BeamRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer beamLinePrefab;
        [SerializeField] private float visibleTime = 0.06f;

        private const int MAX_BEAMS = 8;

        private LineRenderer[] lines;
        private float timer;
        private bool visible;

        private void Awake()
        {
            if (beamLinePrefab == null) return;

            lines = new LineRenderer[MAX_BEAMS];
            for(int i = 0; i < MAX_BEAMS; i++)
            {
                lines[i] = Instantiate(beamLinePrefab, transform);
                lines[i].enabled = true;
            }
        }

        public void ShowBeams(Vector3 start, Vector3[] ends, int count, float width)
        {
            if (lines == null) return;

            for(int i = 0; i < lines.Length; i++)
            {
                if(i < count)
                {
                    lines[i].SetPosition(0, start);
                    lines[i].SetPosition(1, ends[i]);
                    lines[i].startWidth = width;
                    lines[i].endWidth = width;
                    lines[i].enabled = true;
                }
                else
                {
                    lines[i].enabled = false;
                }
            }

            visible = true;
            timer = visibleTime;
        }

        private void Update()
        {
            if (!visible) return;

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                for (int i = 0; i < lines.Length; i++)
                    lines[i].enabled = false;
                visible = false;
            }
        }
    }
}