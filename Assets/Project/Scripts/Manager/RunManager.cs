using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Actors.Player;

namespace Manager
{
    public enum RunState { Spawning, Playing, Ended }
    public enum RunResult { Escaped, Died }

    public class RunManager : MonoBehaviour
    {
        private static RunManager instance;
        public static RunManager Instance { get { return instance; } }

        [SerializeField] private MapManager mapManager;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private float endDelay = 2f;
        [SerializeField] private string hubSceneName = "HubScene";

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

            // -> 로비에서 가져온 아이템 전달
            // GameDataManager.Instance.ApplyLoadoutTo(currentPlayer.GetPlayerInventory());
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

        private void HandleRunEnded(RunResult result)
        {
            if (State == RunState.Ended)
            {
                return;
            }
            State = RunState.Ended;

            if (result == RunResult.Escaped)
            {   
                // 탈출 성공 시 아이템을 전부 창고로 전달
                GameDataManager.Instance.StoreRunResult(currentPlayer.GetPlayerInventory());
            }
            else
            {
                // 실패 시 모든 아이템 분실(임시 Inventory 초기화)
                GameDataManager.Instance.ClearLoadout();
            }

            GameDataManager.Instance.AdvanceDay();
            Invoke(nameof(GoToHubScene), endDelay);
        }

        private void GoToHubScene()
        {
            SceneManager.LoadScene(hubSceneName);
        }
    }
}