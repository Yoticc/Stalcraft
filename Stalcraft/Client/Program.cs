#if RELEASE
if (System.Diagnostics.Debugger.IsAttached)
    return;
#endif

Installer.EnsureInstalled();

new ClientProgram().Main();