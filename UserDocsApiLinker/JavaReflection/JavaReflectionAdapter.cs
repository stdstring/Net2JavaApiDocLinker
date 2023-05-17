using System;
using System.Diagnostics;
using System.Threading;

namespace UserDocsApiLinker.JavaReflection
{
    internal class JavaReflectionAdapter : IJavaReflectionAdapter
    {
        public JavaReflectionAdapter(String classPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(ProcessFileName, ProcessArgs);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            javaReflectionApp = Process.Start(startInfo);
            javaReflectionApp.StandardInput.AutoFlush = true;
            javaReflectionApp.BeginErrorReadLine();
            javaReflectionApp.BeginOutputReadLine();
            javaReflectionApp.ErrorDataReceived += ErrorDataHandler;
            javaReflectionApp.OutputDataReceived += OutputDataHandler;
        }

        public String[] ProcessRequest(String request)
        {
            if(javaReflectionApp.HasExited)
                throw new InvalidOperationException("Already exited");
            javaReflectionApp.StandardInput.WriteLine(request);
            javaReflectionApp.StandardInput.Flush();
            lock (syncRoot)
            {
                if (availableOutputData == null && availableErrorData == null)
                    Monitor.Wait(syncRoot, ResponseWaitTime);
                if (availableErrorData != null)
                {
                    availableOutputData = null;
                    availableErrorData = null;
                    throw new InvalidOperationException(String.Format("Error occur : {0}", availableErrorData));
                }
                if (availableOutputData != null)
                {
                    String[] result = String.IsNullOrEmpty(availableOutputData)
                                          ? null
                                          : availableOutputData.Split(ResultDelimiter);
                    availableOutputData = null;
                    return result;
                }
                throw new InvalidOperationException("Timeout");
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaReflectionAdapter()
        {
            Dispose(false);
        }

        public void CloseImpl()
        {
            if (!javaReflectionApp.HasExited)
            {
                javaReflectionApp.StandardInput.WriteLine(ExitCommand);
                javaReflectionApp.WaitForExit(ResponseWaitTime);
            }
        }

        private void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                CloseImpl();
                javaReflectionApp.Dispose();
            }
            else
            {
                if(!javaReflectionApp.HasExited) javaReflectionApp.Kill();
            }
        }

        private void OutputDataHandler(Object sender, DataReceivedEventArgs e)
        {
            lock (syncRoot)
            {
                availableOutputData = e.Data;
                Monitor.Pulse(syncRoot);
            }
        }

        private void ErrorDataHandler(Object sender, DataReceivedEventArgs e)
        {
            lock (syncRoot)
            {
                availableErrorData = e.Data;
                Monitor.Pulse(syncRoot);
            }
        }

        private readonly Process javaReflectionApp;
        private readonly Object syncRoot = new Object();
        private String availableOutputData;
        private String availableErrorData;

        private const String ProcessFileName = "java.exe";
        private const String ProcessArgs = "-cp \"X:\\awuex\\lib\\Aspose.Words.jdk15.jar\";\"X:\\www.Aspose.Shared\\Docs\\bin\\JavaReflectionApp.jar\" javareflectionapp.JavaReflectionApp";
        private const Char ResultDelimiter = ';';
        // in seconds
        private const Int32 ResponseWaitTime = 10*1000;
        private const String ExitCommand = "exit";
    }
}
