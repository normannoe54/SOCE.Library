using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SOCE.Portal
{
    class Program
    {
        static string basepath = "W:\\Documnts\\Software\\Portal\\InstallerFiles\\";
        static string executingpath = "C:\\ProgramData\\SOCE\\";

        static void Main(string[] args)
        {
            string filename = @"VersionChecker.xml";
            string basefile = Path.Combine(basepath, filename);

            //W drive location exists
            string excutingfile = Path.Combine(executingpath, filename);

            string directoryname = System.IO.Path.GetDirectoryName(executingpath);
            bool folderexists = Directory.Exists(directoryname);

            if (!folderexists)
            {
                //add directory
                System.IO.Directory.CreateDirectory(executingpath);

                //add files
                CopyFiles();
            }
            else
            {
                //look at version numbers
                bool executingexists = File.Exists(excutingfile);

                if (!executingexists)
                {
                    //copy them over.
                    CopyFiles();
                }
                else
                {
                    StreamReader basereader = new System.IO.StreamReader(basefile);
                    double baseversion = DetermineVersion(basereader);
                    basereader.Close();

                    StreamReader executingreader = new System.IO.StreamReader(excutingfile);
                    double executingversion = DetermineVersion(executingreader);
                    executingreader.Close();

                    if (baseversion != executingversion)
                    {
                        CopyFiles();
                    }
                }
            }
            

            string exepath = $"{executingpath}\\SOCEPortal\\SOCE.Library.UI.exe";
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = exepath;
            processInfo.ErrorDialog = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.WorkingDirectory = Path.GetDirectoryName(exepath);

            //int length = Process.GetProcessesByName("SOCE.Library.UI").Length;

            if (Process.GetProcessesByName("SOCE.Library.UI").Length == 0)
            {
                Process proc = Process.Start(processInfo);
            }
        }

        private static double DetermineVersion(StreamReader file)
        {
            double val = 0;
            string version = "0";

            //open the .addin file
            int counter = 0;
            string line;
            while ((line = file.ReadLine()) != null)
            {
                string toBeSearched = "<version>";
                if (line.Contains(toBeSearched))
                {
                    int ix = line.IndexOf(toBeSearched);

                    if (ix != -1)
                    {
                        string trimmed = line.Substring(ix + toBeSearched.Length);
                        int ix2 = trimmed.IndexOf("<");

                        version = trimmed.Substring(0, ix2);
                        break;
                    }
                }

                counter++;
            }

            bool works = Double.TryParse(version, out val);
            return val;
        }

        private static void CopyFiles()
        {
            //delete additional files:
            System.IO.DirectoryInfo di = new DirectoryInfo(executingpath);

            foreach (FileInfo file in di.GetFiles())
            {
                if (file.Name ==  @"VersionChecker.xml")
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {

                    }
                }
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            CloneDirectory(basepath, executingpath);
        }

        private static void CloneDirectory(string root, string dest)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                //Get the path of the new directory
                var newDirectory = Path.Combine(dest, Path.GetFileName(directory));
                //Create the directory if it doesn't already exist
                Directory.CreateDirectory(newDirectory);
                //Recursively clone the directory
                CloneDirectory(directory, newDirectory);
            }

            foreach (var file in Directory.GetFiles(root))
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
            }
        }
    }
}
