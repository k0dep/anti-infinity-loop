using System.Linq;

namespace AntiDeathLoop
{
    public static class NativeMessageBox
    {
        
#if UNITY_EDITOR_WINDOWS

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern System.IntPtr GetActiveWindow();

        public static System.IntPtr GetWindowHandle()
        {
            return GetActiveWindow();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern int MessageBox(System.IntPtr hwnd, String lpText, String lpCaption, uint uType);

#endif
        public static int Show(string title, string message, params string[] buttons)
        {
#if UNITY_EDITOR_LINUX
            var buttonsContent = string.Join(",",
                buttons.Where(b => !string.IsNullOrWhiteSpace(b))
                    .Select((button, index) => $"{button}:{GetExitCodeForButton(index)}"));
            
            var parameters = $"-title '{title}' ";
            if (!string.IsNullOrEmpty(buttonsContent))
            {
                parameters += $"-buttons {buttonsContent} ";
            }
            parameters += message;
            
            var process = System.Diagnostics.Process.Start("xmessage", parameters);
            process.WaitForExit();
            
            if (process.ExitCode == 1)
            {
                return -1;
            }
            
            return process.ExitCode == 0 ? 0 : process.ExitCode - 1;
            
#elif UNITY_EDITOR_WINDOWS

#endif
        }

        private static int GetExitCodeForButton(int index)
        {
            return index == 0 ? 0 : index + 1;
        }
    }
}