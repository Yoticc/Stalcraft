static class DebugTools
{
    public static void Debug(object obj) => Debug(obj.ToString());
    public static void Debug(string? message) => System.Diagnostics.Debug.WriteLine($"[Debug] {message}");
}