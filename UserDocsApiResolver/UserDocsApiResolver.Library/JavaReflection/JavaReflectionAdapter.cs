using System;
using System.Diagnostics;
using System.Threading;

namespace UserDocsApiResolver.Library.JavaReflection
{
    internal class JavaReflectionAdapter : IJavaReflectionAdapter
    {
        public JavaReflectionAdapter(String classPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(ProcessFileName, String.Format(ProcessArgsTemplate, classPath));
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            _javaReflectionApp = Process.Start(startInfo);
            _javaReflectionApp.StandardInput.AutoFlush = true;
            _javaReflectionApp.BeginErrorReadLine();
            _javaReflectionApp.BeginOutputReadLine();
            _javaReflectionApp.ErrorDataReceived += ErrorDataHandler;
            _javaReflectionApp.OutputDataReceived += OutputDataHandler;
        }

        public String[] ProcessRequest(String request)
        {
            if(_javaReflectionApp.HasExited)
                throw new InvalidOperationException("Already exited");
            _javaReflectionApp.StandardInput.WriteLine(request);
            _javaReflectionApp.StandardInput.Flush();
            lock (_syncRoot)
            {
                if (_availableOutputData == null && _availableErrorData == null)
                    Monitor.Wait(_syncRoot, ResponseWaitTime);
                if (_availableErrorData != null)
                {
                    _availableOutputData = null;
                    _availableErrorData = null;
                    throw new InvalidOperationException(String.Format("Error occur : {0}", _availableErrorData));
                }
                if (_availableOutputData != null)
                {
                    String[] result = String.IsNullOrEmpty(_availableOutputData)
                                          ? null
                                          : _availableOutputData.Split(ResultDelimiter);
                    _availableOutputData = null;
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
            if (!_javaReflectionApp.HasExited)
            {
                _javaReflectionApp.StandardInput.WriteLine(ExitCommand);
                _javaReflectionApp.WaitForExit(ResponseWaitTime);
            }
        }

        private void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                CloseImpl();
                _javaReflectionApp.Dispose();
            }
            else
            {
                if(!_javaReflectionApp.HasExited) _javaReflectionApp.Kill();
            }
        }

        private void OutputDataHandler(Object sender, DataReceivedEventArgs e)
        {
            lock (_syncRoot)
            {
                _availableOutputData = e.Data;
                Monitor.Pulse(_syncRoot);
            }
        }

        private void ErrorDataHandler(Object sender, DataReceivedEventArgs e)
        {
            lock (_syncRoot)
            {
                _availableErrorData = e.Data;
                Monitor.Pulse(_syncRoot);
            }
        }

        private readonly Process _javaReflectionApp;
        private readonly Object _syncRoot = new Object();
        private String _availableOutputData;
        private String _availableErrorData;

        private const String ProcessFileName = "java.exe";
        private const String ProcessArgsTemplate = "-cp {0} javareflectionapp.JavaReflectionApp";
        private const Char ResultDelimiter = ';';
        // in seconds
        private const Int32 ResponseWaitTime = 10*1000;
        private const String ExitCommand = "exit";
    }
}
