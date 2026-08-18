namespace Core.System.Pooling
{
    /// <summary>
    /// 풀에서 관리되는 오브젝트가 들고 있을 인터페이스
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// 풀에서 꺼내질 때
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// 풀로 반환될 때
        /// </summary>
        void OnDespawn();
    }
}