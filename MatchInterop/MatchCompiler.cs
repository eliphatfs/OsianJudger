using System;
using System.Diagnostics;
using System.Text;

namespace OsianJudger.MatchInterop {
    public class MatchCompiler {
        public class CompileResult {
            public string CompiledExecutablePath;
            public string CompilerHints;
            public bool IsCompileSuccessful;
        }
        public static string COMPILER_COMMAND = @"D:\Program Files (x86)\CodeBlocks\MinGW\bin\g++.exe";
        public const string COMPILER_FLAGS = "-O2";
        /// <summary>
        /// Compiles a file.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns>Compiling Result.</returns>
        public static CompileResult Compile(string sourcePath) {
            try {
                var target = Guid.NewGuid().ToString() + ".exe";
                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = COMPILER_COMMAND;
                si.Arguments = COMPILER_FLAGS + " " + sourcePath + " -o " + target;
                si.CreateNoWindow = true;
                si.UseShellExecute = false;
                si.RedirectStandardError = true;
                si.RedirectStandardInput = false;
                si.RedirectStandardOutput = false;

                var proc = Process.Start(si);
                var errorBuffer = new StringBuilder();
                proc.ErrorDataReceived += (sender, x) => errorBuffer.Append(x.Data);
                proc.BeginErrorReadLine();
                if (!proc.WaitForExit(20000)) {
                    proc.Kill();
                    return new CompileResult() {
                        IsCompileSuccessful = false,
                        CompiledExecutablePath = null,
                        CompilerHints = "The Compiling Process Reached Time Limit of 20 Seconds."
                    };
                }
                if (proc.ExitCode == 0)
                    return new CompileResult() {
                        IsCompileSuccessful = true,
                        CompiledExecutablePath = target,
                        CompilerHints = errorBuffer.ToString()
                    };
                else
                    return new CompileResult() {
                        IsCompileSuccessful = false,
                        CompiledExecutablePath = null,
                        CompilerHints = errorBuffer.ToString()
                    };
            } catch (Exception e) {
                return new CompileResult() {
                    IsCompileSuccessful = false,
                    CompiledExecutablePath = null,
                    CompilerHints = e.Message
                };
            }
        }
    }
}
