using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GoDaddyMigratorService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].CompareTo("-remove") == 0) //remove service
            {
                System.Console.WriteLine("Remove as a service...");
                String argument = "-u " + Process.GetCurrentProcess().MainModule.ModuleName;
                String launchCmd = RuntimeEnvironment.GetRuntimeDirectory() + "InstallUtil.exe";
                launchProcess(launchCmd, argument);

                return;
            }
            else if (args.Length > 0 && args[0].CompareTo("-install") == 0) //install as a service
            {
                System.Console.WriteLine("Installing as a service...");

                StringBuilder argument = new StringBuilder();
                int i = 1;
                while (i < args.Length)
                {
                    if (args[i].ToLower().CompareTo("-user") == 0)
                    {
                        argument.Append(" /user=");
                        argument.Append(args[i + 1]);
                        i += 2;
                    }
                    else if (args[i].ToLower().CompareTo("-password") == 0)
                    {
                        argument.Append(" /password=");
                        argument.Append(args[i + 1]);
                        i += 2;
                    }
                    else
                    {
                        i++;
                    }
                }

                argument.Append(" ");
                argument.Append(Process.GetCurrentProcess().MainModule.ModuleName);

                String launchCmd = RuntimeEnvironment.GetRuntimeDirectory() + "InstallUtil.exe";
                launchProcess(launchCmd, argument.ToString());
                return;
            }
            if (args.Length == 1 && args[0].CompareTo("-help") == 0) //help
            {
                System.Console.WriteLine("Version " + Process.GetCurrentProcess().MainModule.FileVersionInfo.ToString());
                return;
            }
            if (args.Length == 1 && args[0].CompareTo("-run") == 0) // run on commandline
            {
                System.Console.WriteLine("Running in foreground...");
                Service1 a = new Service1();
                Console.CancelKeyPress += delegate
                {
                    System.Console.WriteLine("Exiting...");
                    a.Stop();
                };

                a.DatabaseQueueChecker();                
                return;
            }
                        
            ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            ServicesToRun = new ServiceBase[] { new Service1() };

            ServiceBase.Run(ServicesToRun);
        }

        static void launchProcess(String binary, String argument)
        {
            System.Diagnostics.ProcessStartInfo psInfo =
                new System.Diagnostics.ProcessStartInfo(binary, argument);

            System.Console.WriteLine();
            System.Console.WriteLine(psInfo.FileName + " " + psInfo.Arguments);
            System.Console.WriteLine();

            psInfo.RedirectStandardOutput = true;
            psInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psInfo.UseShellExecute = false;
            System.Diagnostics.Process ps;
            ps = System.Diagnostics.Process.Start(psInfo);
            System.IO.StreamReader msgOut = ps.StandardOutput;
            ps.WaitForExit(5000); //wait up to 5 seconds 
            if (ps.HasExited)
            {
                System.Console.WriteLine(msgOut.ReadToEnd()); //write the output
            }
            return;
        }
    }
}
