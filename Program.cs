using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BuildBOFs
{
    class Program
    {
        private static List<string> CFiles = new List<string>();
        private static List<string> MakeFIles = new List<string>();
        private static List<string> msvcFiles = new List<string>();
        private static List<string> MiscFiles = new List<string>();
        private static int timeout = 3000;

        public static void Main(string[] args)
        {
            Logo();
           string rootdir = args[0];
           Console.WriteLine("[*] Searching root dir: " + rootdir);
           Console.WriteLine("[*] Finding all the files to build");
           DirSearch(rootdir);//get files of type we want
           Console.WriteLine("[*] Total files Found: "+(CFiles.Count+ MakeFIles.Count+ msvcFiles.Count+ MiscFiles.Count));

            Console.WriteLine("[*] Building .c files");
            foreach (string file in CFiles)
            {
                BuildCL(file);
                BuildBashC(file);
            }
            Console.WriteLine("[*] Building MakeFile files");
            foreach (string file in MakeFIles)
            {
                BuildMakeLinux(file);
            }
            Console.WriteLine("[*] Building .msvc files");
            foreach (string file in msvcFiles)
            {
                Buildnmake(file);
            }
            Console.WriteLine("[*] Building misc (.bat) files");
            foreach (string file in MiscFiles)
            {
                BuildBatFile(file);
            }
        }

        private static void Logo()
        {
            Console.WriteLine(@"
 .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------. 
| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
| |   ______     | || |     ____     | || |  _________   | || |              | || |   ______     | || | _____  _____ | || |     _____    | || |   _____      | || |  ________    | || |  _________   | || |  _______     | |
| |  |_   _ \    | || |   .'    `.   | || | |_   ___  |  | || |              | || |  |_   _ \    | || ||_   _||_   _|| || |    |_   _|   | || |  |_   _|     | || | |_   ___ `.  | || | |_   ___  |  | || | |_   __ \    | |
| |    | |_) |   | || |  /  .--.  \  | || |   | |_  \_|  | || |    ______    | || |    | |_) |   | || |  | |    | |  | || |      | |     | || |    | |       | || |   | |   `. \ | || |   | |_  \_|  | || |   | |__) |   | |
| |    |  __'.   | || |  | |    | |  | || |   |  _|      | || |   |______|   | || |    |  __'.   | || |  | '    ' |  | || |      | |     | || |    | |   _   | || |   | |    | | | || |   |  _|  _   | || |   |  __ /    | |
| |   _| |__) |  | || |  \  `--'  /  | || |  _| |_       | || |              | || |   _| |__) |  | || |   \ `--' /   | || |     _| |_    | || |   _| |__/ |  | || |  _| |___.' / | || |  _| |___/ |  | || |  _| |  \ \_  | |
| |  |_______/   | || |   `.____.'   | || | |_____|      | || |              | || |  |_______/   | || |    `.__.'    | || |    |_____|   | || |  |________|  | || | |________.'  | || | |_________|  | || | |____| |___| | |
| |              | || |              | || |              | || |              | || |              | || |              | || |              | || |              | || |              | || |              | || |              | |
| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'
");
        }

        private static void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    try
                    {
                        foreach (string f in Directory.GetFiles(d))
                        {
                            if (Path.GetExtension(f).ToLower().Equals(".c") == true)
                            {
                                Console.WriteLine(" [+] Found '.c' file: " + f);
                                CFiles.Add(f);
                            }
                            else if (Path.GetExtension(f).ToLower().Equals(".msvc") == true)
                            {
                                Console.WriteLine(" [+] Found '.msvc' file: " + f);

                                msvcFiles.Add(f);
                            }
                            else if (Path.GetFileName(f).ToLower().Contains("makefile"))
                            {
                                Console.WriteLine(" [+] Found 'MakeFile' file: " + f);

                                MakeFIles.Add(f);
                            }
                            else if (Path.GetFileName(f).ToLower().Contains(".bat"))
                            {
                                Console.WriteLine(" [+] Found misc file: " + f);

                                MiscFiles.Add(f);
                            }
                        }
                        DirSearch(d);
                    }
                    catch (System.Exception excpt)
                    {
                        Console.WriteLine("[!] " + excpt.Message);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private static void outdata(object sender, DataReceivedEventArgs e)
        {
            var data = e.Data;
            System.Console.WriteLine(data);
        }

        public static void BuildBatFile(string BatFile)
        {
            string filename = Path.GetFileName(BatFile).Split('.')[0];
            string workingdir = Path.GetDirectoryName(BatFile);

            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = workingdir, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine("start "+BatFile);
                process.WaitForExit(timeout);
            }
        }

        public static void BuildCL(string CFile)
        {
            string filename = Path.GetFileName(CFile).Split('.')[0];
            string workingdir = Path.GetDirectoryName(CFile);
            //string startVScl = "\"" + @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\bin\vcvars32.bat" + "\"";
            //string startVScl = "C:\\Program Files(x86)\\Microsoft Visual Studio\\2019\\Community\\VC\\Auxiliary\\Build\\vcvars64.bat";

            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd",Arguments="",WorkingDirectory= workingdir, UseShellExecute = false ,RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine(@"cl /GS- /nologo /Od /Oi /c " + filename + ".c /F" + filename + ".o");
                process.WaitForExit(timeout);
            }
        }

        public static void Buildnmake(string MsvcFile)
        {
            string filename = Path.GetFileName(MsvcFile).Split('.')[0];
            string workingdir = Path.GetDirectoryName(MsvcFile);
            //string startVScl = "\"" + @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\bin\vcvars32.bat" + "\"";

            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = workingdir, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine(@"nmake -f "+ MsvcFile + " build");
                process.WaitForExit(timeout);
            }
        }

        public static void BuildMakeLinux(string MakeFile)
        {
            string filename = Path.GetFileName(MakeFile).Split('.')[0];
            string workingdir = Path.GetDirectoryName(MakeFile);
            string linuxfilePath = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename + ".c";
            string outputPathLinux = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename + ".o";
            workingdir = Path.GetDirectoryName(MakeFile).Replace('\\', '/').Replace(@"C:/", @"/mnt/c/");
            
            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = Path.GetDirectoryName((MakeFile)), UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine("bash -c \"cd "+workingdir+" && make\"");
                process.WaitForExit(timeout);
            }
        }

        public static void BuildBashC(string CFile)
        {
            string filename = Path.GetFileName(CFile).Split('.')[0];
            string workingdir = Path.GetDirectoryName(CFile);
            string linuxfilePath = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename+ ".c";
            string outputPathLinux = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename + ".o";

            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = Path.GetDirectoryName((CFile)), UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine("bash -c \"x86_64-w64-mingw32-gcc -c " + linuxfilePath + " -o " + outputPathLinux + "\"");
                process.WaitForExit(timeout);
            }
        }
        
    }
}
