using System;
using System.Windows.Forms;

namespace ATM
{
    internal static class Program
    {
        public static string CardNumber { get; set; }

        public static int WithdrawalAmount { get; set; }

        public static int CurrentBalance { get; set; }

        public static string TransactionType { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}