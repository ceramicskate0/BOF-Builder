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
        private static int counter = 1;
        private static bool x64 = true;
        private static string rootdir = "";
        private static string LinuxBuild = "bash -c";
        private static string migngw = "x86_64-w64-mingw32-gcc";
        private static string syscallMasmArg = "-masm=intel";
        public static void Main(string[] args)
        {
            Logo();
            if (args.Length < 0)
            {
                Console.WriteLine("[!] Error. No input.");
                Environment.Exit(1);
            }
            ParseArgs(args);
            Console.WriteLine("[*] Searching root dir: " + rootdir);
            Console.WriteLine("[*] Finding all the files to build");
            DirSearch(rootdir);//get files of type we want
            Console.WriteLine("[*] Total files Found: " + (CFiles.Count + MakeFIles.Count + msvcFiles.Count + MiscFiles.Count));
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("[*] Building .c files");
            foreach (string file in CFiles)
            {
                Console.WriteLine("[*] Working file (" + counter + "\\" + (CFiles.Count + MakeFIles.Count + msvcFiles.Count + MiscFiles.Count) + ") "+file);
                Console.WriteLine("[*] Building .c files via windows CL.exe");
                BuildCL(file);
                Console.WriteLine("[*] Building .c files via mingw '" + LinuxBuild + "'");
                BuildBashC(file);
                counter++;
            }
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("[*] Building MakeFile files");
            foreach (string file in MakeFIles)
            {
                Console.WriteLine("[*] Working file (" + counter + "\\" + (CFiles.Count + MakeFIles.Count + msvcFiles.Count + MiscFiles.Count) + ") " + file);
                try
                {
                    BuildMakeLinux(file);
                }
                catch
                {
                    Console.WriteLine("[!] Error building " + file);
                }
                counter++;
            }
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("[*] Building .msvc files");
            foreach (string file in msvcFiles)
            {
                Console.WriteLine("[*] Working file (" + counter + "\\" + (CFiles.Count + MakeFIles.Count + msvcFiles.Count + MiscFiles.Count) + ") " + file);
                try
                {
                    Buildnmake(file);
                }
                catch
                {
                    Console.WriteLine("[!] Error building " + file); 
                }
                counter++;
            }
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("[*] Building misc (.bat) files");
            foreach (string file in MiscFiles)
            {
                Console.WriteLine("[*] Working file (" + counter + "\\" + (CFiles.Count + MakeFIles.Count + msvcFiles.Count + MiscFiles.Count) + ") " + file);
                try
                {
                    BuildBatFile(file);
                }
                catch
                {
                    Console.WriteLine("[!] Error building " + file);
                }
            counter++;
            }
        }

        private static void Usage()
        {
            Console.WriteLine(@"
Usage:

Example: BuildBOFs.exe -rootdir C:\Path\to\BOFs

Commands:
-rootdir
    The folder that contains the BOF files to build (REQUIRED)
-x86
    Tell to compile linux bins for x86 (i686-w64-mingw32-gcc)
-x64
    Tell to compile linux bins for x64 (86_64-w64-mingw32-gcc) (DEFAULT)
-timeout
    Sets the timeout for the process who is building bin (DEFAULT 3 seconds) (time in milliseconds)
-wsl
    Tell app to use wsl.exe instead of bash.exe to compile linux bins
                ");
        }

        private static void ParseArgs(string[] args)
        {
            if (args.Length <= 0)
            {
                Usage();
                Environment.Exit(0);
            }
            for (int x = 0; x < args.Length; ++x)
            {
                try
                {
                    switch (args[0].ToLower())
                    {
                        case "-x86":
                            x64 = false;
                            migngw = "i686-w64-mingw32-gcc";
                            break;
                        case "-x64":
                            x64 = true;
                            break;
                        case "-timeout":
                            timeout = Convert.ToInt32(args[x+1]);
                            break;
                        case "-wsl":
                            LinuxBuild = "wsl -e";
                            break;
                        case "-rootdir":
                            rootdir = args[x+1];
                    break;
                }
                }
                catch (Exception e)
                {
                    Usage();
                    Console.WriteLine("[Error] Invalid input " + e.Message.ToString());
                }
            }
            if (string.IsNullOrEmpty(rootdir)== true)
            {
                Console.WriteLine("[Error] MISSING REQUIRED rootdir arg/input!!!");

                Usage();
                Environment.Exit(0);
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
                process.StandardInput.WriteLine("start " + BatFile);
                process.WaitForExit(timeout);
                // process.StandardInput.WriteLine("exit");
            }
        }

        public static void BuildCL(string CFile)
        {
            string filename = Path.GetFileName(CFile);
            string ext = Path.GetExtension(CFile);
            filename = filename.Replace(ext, "");
            string workingdir = Path.GetDirectoryName(CFile);


            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = workingdir, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine(@"cl /GS- /nologo /Od /Oi /c " + filename + ext + " /F" + filename + ".o");
                process.WaitForExit(timeout);
                //process.StandardInput.WriteLine("exit");
            }
        }

        public static void Buildnmake(string MsvcFile)
        {
            string filename = Path.GetFileName(MsvcFile);
            string ext = Path.GetExtension(MsvcFile);
            filename = filename.Replace(ext, "");
            string workingdir = Path.GetDirectoryName(MsvcFile);

            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = workingdir, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine(@"nmake -f " + MsvcFile + " build");
                process.WaitForExit(timeout);
                //process.StandardInput.WriteLine("exit");
            }
        }

        public static void BuildMakeLinux(string MakeFile)
        {
            string filename = Path.GetFileName(MakeFile);
            string ext = Path.GetExtension(MakeFile);
            filename = filename.Replace(ext, "");
            string workingdir = Path.GetDirectoryName(MakeFile);
            string linuxfilePath = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename + ext;
            string outputPathLinux = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename + ".o";
            workingdir = Path.GetDirectoryName(MakeFile).Replace('\\', '/').Replace(@"C:/", @"/mnt/c/");

            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = Path.GetDirectoryName((MakeFile)), UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine(LinuxBuild + " \"cd " + workingdir + " &&  make\"");
                process.WaitForExit(timeout);
                //process.StandardInput.WriteLine("exit");
            }
        }

        public static void BuildBashC(string CFile)
        {
            string filename = Path.GetFileName(CFile);
            string ext = Path.GetExtension(CFile);
            filename = filename.Replace(ext, "");
            string workingdir = Path.GetDirectoryName(CFile);
            string linuxfilePath = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename + ext;
            string outputPathLinux = workingdir.Replace('\\', '/').Replace(@"C:/", @"/mnt/c/") + '/' + filename + ".o";
            string[] files = System.IO.Directory.GetFiles(workingdir, "*yscall*.h", System.IO.SearchOption.TopDirectoryOnly);
            if (files.Length <= 0)
            {
                syscallMasmArg = "";
            }
            using (var process = Process.Start(new ProcessStartInfo { FileName = @"cmd", Arguments = "", WorkingDirectory = Path.GetDirectoryName((CFile)), UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true }))
            {
                process.OutputDataReceived += new DataReceivedEventHandler(outdata);
                process.BeginOutputReadLine();
                process.StandardInput.WriteLine(LinuxBuild + " \""+ migngw + " -c " + linuxfilePath + " -o " + outputPathLinux + " "+ syscallMasmArg+"\"");
                process.WaitForExit(timeout);
                //process.StandardInput.WriteLine("exit");
            }
            syscallMasmArg = "-masm=intel";
        }

    }
}
