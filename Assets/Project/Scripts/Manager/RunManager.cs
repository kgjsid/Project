using UnityEngine;
using UnityEngine.SceneManagement;

using Actors.Player;

namespace Manager
{
    public enum RunState { Spawning, Playing, Ended }

    public class RunManager : MonoBehaviour
    {
        private static RunManager instance;
        public static RunManager Instance { get { return instance; } }

        [SerializeField] private SpawnManager spawnManager;

        private RunState curRunState;
        public RunState State { get { return curRunState; } private set { curRunState = value; } }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            State = RunState.Spawning;


            PlayerController player = spawnManager.SpawnAll();

            if (player == null)
            {
                Debug.LogError("SpawnManager: Player SpawnPoint가 씬에 없습니다.");
                State = RunState.Ended;
                return;
            }

            player.OnRunEnded += HandleRunEnded;

            State = RunState.Playing;
        }

        private void HandleRunEnded()
        {
            if (State == RunState.Ended)
            {
                return;
            }
            State = RunState.Ended;

            Invoke(nameof(ResetScene), 2f);
        }

        private void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}