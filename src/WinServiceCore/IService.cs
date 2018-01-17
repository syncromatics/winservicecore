namespace WinServiceCore
{
    public interface IService
    {
        string ServiceName { get; }
        string DisplayName { get; }
        string Description { get; }

        void Start();
        void Stop();
    }
}
