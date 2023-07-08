using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Configuration;
using System.Windows.Forms;

public class Program
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern short GetAsyncKeyState(int vKey);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;
    private const int VK_ESCAPE = 0x1B;

    private static readonly Random random = new Random();

    public static void Main()
    {
        bool escapePressed = false;

        // Start a new thread to listen for the Escape key
        var escapeThread = new Thread(() =>
        {
            while (!escapePressed)
            {
                if (GetAsyncKeyState(VK_ESCAPE) != 0)
                {
                    escapePressed = true;
                    break;
                }
            }
        });
        escapeThread.Start();

        // Retrieve random wait time parameters from config file
        int minWaitTime = int.Parse(ConfigurationManager.AppSettings["MinWaitTime"]);
        int maxWaitTime = int.Parse(ConfigurationManager.AppSettings["MaxWaitTime"]);

        int beepFreq = int.Parse(ConfigurationManager.AppSettings["BeepFrequency"]);
        int beepDuration = int.Parse(ConfigurationManager.AppSettings["BeepDuration"]);

        while (!escapePressed)
        {
            // Generate a random wait time between minWaitTime and maxWaitTime
            int waitTime = random.Next(minWaitTime, maxWaitTime);
            Thread.Sleep(waitTime);

            // Get the current mouse position
            var currentPosition = System.Windows.Forms.Cursor.Position;

            // Simulate a left mouse button down event
            mouse_event(MOUSEEVENTF_LEFTDOWN, currentPosition.X, currentPosition.Y, 0, 0);
            Thread.Sleep(50);
            // Simulate a left mouse button up event
            mouse_event(MOUSEEVENTF_LEFTUP, currentPosition.X, currentPosition.Y, 0, 0);

            Console.Beep(beepFreq, beepDuration);
        }
        // Join the escape thread to wait for it to complete
        escapeThread.Join();
    }
}
