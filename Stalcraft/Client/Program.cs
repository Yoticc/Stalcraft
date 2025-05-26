using System.Diagnostics;
using System.Runtime.CompilerServices;

#if RELEASE
if (System.Diagnostics.Debugger.IsAttached)
    return;
#endif


var currentProcess = Process.GetCurrentProcess();
Process.GetProcessesByName(currentProcess.ProcessName).Where(process => process.Id != currentProcess.Id).ToList().ForEach(process => process.Kill());

Drivers.EnsureIsInstalled();

RuntimeHelpers.RunClassConstructor(typeof(ConfigurationFile).TypeHandle);

ConsoleApplication.Run<MainWindow>();

RuntimeHelpers.RunClassConstructor(typeof(StalcraftWindowCapture).TypeHandle);

Thread.Sleep(int.MaxValue);