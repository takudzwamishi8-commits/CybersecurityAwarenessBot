using System;
using System.Windows.Forms;
using CybersecurityAwarenessBot.UI;

namespace CybersecurityAwarenessBot
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }
    }
}