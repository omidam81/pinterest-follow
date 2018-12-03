using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace PinterestFollow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadExit += Application_ThreadExit;
            Application.ThreadException +=Application_ThreadException;

            
            Database = new Classes.Models.Database();
            Application.Run(new frmMain());

        }

        static void Application_ThreadExit(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Database.Save();
        }

        
        public static Classes.Models.Database Database { get; set; }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var strLogFilePath = Path.Combine(new FileInfo(Application.ExecutablePath).DirectoryName, "Error.log");
            if (!File.Exists(strLogFilePath))
            {
                StreamWriter str1 = new StreamWriter(File.Create(strLogFilePath));
                str1.Write(e.Exception.ToString() + Environment.NewLine);
                str1.Close();
            }

            StreamWriter str = new StreamWriter(strLogFilePath, true);
            str.Write(e.Exception.ToString() + Environment.NewLine);
            str.Close();
        }

    }
}
