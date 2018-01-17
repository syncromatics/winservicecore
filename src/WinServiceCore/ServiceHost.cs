using System;
using System.Diagnostics;
using System.Linq;
using DasMulli.Win32.ServiceUtils;
using System.ServiceProcess;

namespace WinServiceCore
{
    public static class ServiceHost
    {
        public static void Run(IService service)
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Contains("install"))
            {
                Install(service);
            }
            else if (args.Contains("start"))
            {
                Start(service);
            }
            else if (args.Contains("stop"))
            {
                Stop(service);
            }
            else if (args.Contains("uninstall"))
            {
                Uninstall(service);
            }
            else if (args.Contains("run-as-service"))
            {
                RunAsService(service);
            }
            else
            {
                RunConsole(service);
            }
        }

        private static void Install(IService service)
        {
            var failureAction = new ScAction
            {
                Type = ScActionType.ScActionRestart,
                Delay = TimeSpan.FromMinutes(1)
            };

            var serviceDefinition = new ServiceDefinitionBuilder(service.ServiceName)
                .WithDisplayName(service.DisplayName)
                .WithDescription(service.Description)
                .WithBinaryPath(Process.GetCurrentProcess().MainModule.FileName + " run-as-service")
                .WithAutoStart(true)
                .WithFailureActions(new ServiceFailureActions(TimeSpan.FromMinutes(1), "", "", new[] { failureAction, failureAction, failureAction }))
                .WithCredentials(Win32ServiceCredentials.LocalSystem)
                .Build();

            new Win32ServiceManager().CreateOrUpdateService(serviceDefinition);

            Console.WriteLine($"service {service.ServiceName} installed successfully.");
        }

        private static void Uninstall(IService service)
        {
            new Win32ServiceManager().DeleteService(service.ServiceName);
        }

        private static void Start(IService service)
        {
            using (var sc = new ServiceController(service.ServiceName))
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                Console.WriteLine($"service {service.ServiceName} started successfully.");
            }
        }

        private static void Stop(IService service)
        {
            using (var sc = new ServiceController(service.ServiceName))
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                Console.WriteLine($"service {service.ServiceName} stopped successfully.");
            }
        }

        private static void RunAsService(IService service)
        {
            var winService = new Service(service);
            var host = new Win32ServiceHost(winService);
            host.Run();
        }

        private static void RunConsole(IService service)
        {
            service.Start();
            Console.WriteLine("Running interactively, press enter to stop");
            Console.ReadLine();
            service.Stop();
        }
    }
}
