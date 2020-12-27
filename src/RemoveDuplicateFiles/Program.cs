using RemoveDuplicateFiles.ViewModel;
using System;
using System.Windows.Forms;

namespace RemoveDuplicateFiles
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IDuplicateFilesViewModel duplicateFileSearcher = new DuplicateFilesViewModel();
            Application.Run(new FormMain(duplicateFileSearcher));
        }
    }
}
