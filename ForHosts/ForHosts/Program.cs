using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System.Reflection;

namespace ForHosts
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!IsRunningAdminstrator())
            {

                var mProcessStartInfo = new ProcessStartInfo();
                //using operating shell and setting the ProcessStartInfo.verb to "runas" will let it run as admin
                mProcessStartInfo.UseShellExecute = true;
                mProcessStartInfo.FileName = Application.ExecutablePath;
                mProcessStartInfo.Verb = "runas";
                //start the application as new process
                Process.Start(mProcessStartInfo);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }


        public static bool IsRunningAdminstrator()
        {
            //Get Current windows user
            var mWindowsIdentity = WindowsIdentity.GetCurrent();
            //get current windows user principal
            var mWindowsPrincipal = new WindowsPrincipal(mWindowsIdentity);
            //return true if user is in role "Administrator"
            return mWindowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}
