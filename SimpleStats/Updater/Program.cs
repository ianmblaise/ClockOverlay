using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Updater
{
    class Program
    {
        public const string ClientVersion = "1.0";

        static void Main(string[] args)
        {
            CheckForUpdates();
        }

        public static void CheckForUpdates()
        {
            System.Net.WebClient client = new System.Net.WebClient(); // <-- your web client object.

            string serverVersion = client.DownloadString(new Uri("http://yourwebserver/updates/currentversion.txt"));

            if (ClientVersion == serverVersion)
            {
                return; // versions match, so get the heck outta here.
            }

            bool updateApplied = false;

            while (!updateApplied)
            {
                client.DownloadFile(new Uri("http://yourwebserver/updates/UpdateMe.exe"), "UpdateMe.exe.new");

                // if the file doesn't exist then ask to try again.
                if (!File.Exists("UpdateMe.exe.new"))
                {
                    Console.Write("Update was not downloaded successfully, try again? (y/n) : ");
                    string answer = Console.ReadLine();

                    if (answer != "y")
                    {
                        Console.WriteLine("Failed to apply update, try again later.");
                        Console.WriteLine("Press any key to continue..");
                        Console.Read();

                        Environment.Exit(0);
                    }

                    continue;
                }

                Process[] yourProcesses = Process.GetProcessesByName("UpdateMe");
                if (yourProcesses.Length <= 0)
                {
                    // No open instances of the program that is being updated so go ahead and apply the update..    
                    updateApplied = ReplaceWithNewVersion();
                }

                // Since there are some instances of UpdateMe running already, probably best to terminate them to avoid IO errors.. 
                // So ask the user if it's okay to terminate them.. or just be a dick and do it without asking.
                Console.WriteLine("Need to close all running instances of UpdateMe. I'll be a dick and just close them, fuck what you heard.");

                foreach (Process process in yourProcesses)
                {
                    Console.WriteLine("Killing UpdateMe (id: {0}). He will be missed. :(", process.Id);
                    process.Kill();
                }
            }
        }

        public static bool ReplaceWithNewVersion()
        {
            if (File.Exists("UpdateMe.exe"))
            {
                try
                {
                    // Take the old version and rename it to UpdateMe.exe.old or something like that.
                    File.Move("UpdateMe.exe", "UpdateMe.exe.old");

                    // Take the new version which you downloaded and rename it to UpdateMe.exe
                    File.Move("UpdateMe.exe.new", "UpdateMe.exe");
                    return true;
                }
                catch (IOException exception)
                {
                    Console.WriteLine("Something went wrong... here is what I know..\n" + exception.Message);
                    return false;
                }
                finally
                {
                    if (File.Exists("UpdateMe.exe.old") && File.Exists("UpdateMe.exe"))
                    {
                        // do whatever you want with the old versions, probably just delete them though.
                        File.Delete("UpdateMe.exe.old");
                    }
                }
            }
            Console.WriteLine("File doesn't even exist, what am I updating? Redownload the program.");
            return false;
        }
    }
}
