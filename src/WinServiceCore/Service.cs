using DasMulli.Win32.ServiceUtils;

namespace WinServiceCore
{
    internal class Service : IWin32Service
    {
        public string ServiceName => _service.ServiceName;

        private readonly IService _service;

        public Service(IService service)
        {
            _service = service;
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            _service.Start();
        }

        public void Stop()
        {
            _service.Stop();
        }
    }
}
