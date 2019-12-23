﻿/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2009-2013 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using OpenHardwareMonitor.GUI;

namespace OpenHardwareMonitor
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var consoleWriter = new GUIConsoleWriter();

            string test = "Console Report by Nic Weiss";
            Console.WriteLine(test);

            MainForm tst = new MainForm();
            tst.print_cmd_report();
        }

        public class GUIConsoleWriter
        {
            private const int ATTACH_PARENT_PROCESS = -1;

            StreamWriter _stdOutWriter;

            public GUIConsoleWriter()
            {
                var stdout = Console.OpenStandardOutput();
                _stdOutWriter = new StreamWriter(stdout);
                _stdOutWriter.AutoFlush = true;

                AttachConsole(ATTACH_PARENT_PROCESS);
            }
            [DllImport("kernel32.dll")]
            private static extern bool AttachConsole(int dwProcessId);
        }

        private static bool IsFileAvailable(string fileName)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath) +
              Path.DirectorySeparatorChar;

            if (!File.Exists(path + fileName))
            {
                MessageBox.Show("The following file could not be found: " + fileName +
                  "\nPlease extract all files from the archive.", "Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private static bool AllRequiredFilesAvailable()
        {
            if (!IsFileAvailable("Aga.Controls.dll"))
                return false;
            if (!IsFileAvailable("OpenHardwareMonitorLib.dll"))
                return false;
            if (!IsFileAvailable("OxyPlot.dll"))
                return false;
            if (!IsFileAvailable("OxyPlot.WindowsForms.dll"))
                return false;

            return true;
        }

        private static void ReportException(Exception e)
        {
            CrashForm form = new CrashForm();
            form.Exception = e;
            form.ShowDialog();
        }

        public static void Application_ThreadException(object sender,
          ThreadExceptionEventArgs e)
        {
            try
            {
                ReportException(e.Exception);
            }
            catch
            {
            }
            finally
            {
                Application.Exit();
            }
        }

        public static void CurrentDomain_UnhandledException(object sender,
          UnhandledExceptionEventArgs args)
        {
            try
            {
                Exception e = args.ExceptionObject as Exception;
                if (e != null)
                    ReportException(e);
            }
            catch
            {
            }
            finally
            {
                Environment.Exit(0);
            }
        }
    }
}
