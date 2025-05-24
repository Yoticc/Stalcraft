using System.Security.Principal;

class Drivers
{
    public static void EnsureIsInstalled()
    {
        if (!HasAdminPrivileges)
            User32.MessageBox("error", "no admin privileges");

        if (!IsInterceptionInstalled())
            User32.MessageBox("error", "no driver installed");
    }

    static bool IsInterceptionInstalled() => IsDriverInstalled("keyboard") && IsDriverInstalled("mouse");

    static bool HasAdminPrivileges => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    const string DriversDirectory = @"C:\Windows\System32\drivers\";
    static bool IsDriverInstalled(string driverName) => File.Exists($"{DriversDirectory}{driverName}.sys");
}