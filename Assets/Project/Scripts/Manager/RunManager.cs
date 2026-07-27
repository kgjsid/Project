using System;
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
        [SerializeField] private MapManager mapManager;

        private RunState curRunState;
        public RunState State { get { return curRunState; } private set { curRunState = value; } }

        private PlayerController currentPlayer;

        public event Action<PlayerController> OnPlayerSpawned;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            State = RunState.Spawning;

            mapManager.GenerateMap();
            var points = mapManager.CollectSpawnPoints();
            currentPlayer = spawnManager.SpawnAll(points);
            
            if (currentPlayer == null)
            {
                Debug.LogError("SpawnManager: Player SpawnPoint가 씬에 없습니다.");
                State = RunState.Ended;
                return;
            }

            OnPlayerSpawned?.Invoke(currentPlayer);

            currentPlayer.OnRunEnded += HandleRunEnded;
            
            State = RunState.Playing;
        }

        public void SubscribeToPlayerSpawnEvent(Action<PlayerController> callback)
        {
            if(currentPlayer != null)
            {
                callback(currentPlayer);
            }
            else
            {
                OnPlayerSpawned += callback;
            }
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