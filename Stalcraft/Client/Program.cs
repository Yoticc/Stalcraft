#if RELEASE
if (System.Diagnostics.Debugger.IsAttached)
    return;
#endif

DriverInstaller.EnsureInstalled();

ConsoleApplication.Run<MainWindow>();