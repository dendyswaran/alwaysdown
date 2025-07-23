using System;
using System.IO;
using System.Windows.Forms;

namespace WinForm
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles and text rendering improvements
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Set high DPI support
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            
            // Initialize application configuration
            ApplicationConfiguration.Initialize();
            
            try
            {
                // Run the main form
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                // Write full exception details to log file for debugging
                try
                {
                    var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fatal_error.log");
                    File.WriteAllText(logPath, $"Fatal Error at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n{ex.ToString()}");
                }
                catch
                {
                    // Ignore logging errors
                }
                
                MessageBox.Show(
                    $"An unexpected error occurred:\n\n{ex.Message}\n\nFull details saved to fatal_error.log\n\nThe application will now close.",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}