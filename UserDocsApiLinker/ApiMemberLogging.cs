using System;
using System.Collections.Generic;
using System.IO;

namespace UserDocsApiLinker
{
    class ApiMemberLogging
    {
        public string LogFileName
        {
            get { return mLogFileName; }
            set { mLogFileName = value; }
        }

        public int LogLevel
        {
            get; set;
        }

        public void WriteMemberEntry(ApiMember apiMember)
        {
            switch (LogLevel)
            {
                case 0:
                    return;
                case 1:
                    WriteMemberEntryConsole(apiMember);
                    break;
                case 2:
                    WriteMemberEntryConsole(apiMember);
                    WriteMemberEntryLog(apiMember);
                    break;
            }
        }

        public void WriteMemberEntries(IList<ApiMember> apiMembers)
        {
            switch (LogLevel)
            {
                case 0:
                    return;
                case 1:
                    foreach (ApiMember apiMember in apiMembers)
                        WriteMemberEntryConsole(apiMember);
                    break;
                case 2:
                    foreach (ApiMember apiMember in apiMembers)
                    {
                        WriteMemberEntryConsole(apiMember);
                        WriteMemberEntryLog(apiMember);
                    }
                        
                    break;
            }
        }

        public void WriteToLog(string logText)
        {
            switch (LogLevel)
            {
                case 0:
                    return;
                case 1:
                    WriteLogText("Warning!", logText);
                    break;
                case 2:
                    WriteLogText("Warning!", logText);
                    WriteConsoleText("Warning!", logText);
                    break;
                case 3:
                    WriteLogText("Warning!", logText);
                    WriteConsoleText("Warning!", logText);
                    throw new Exception(logText);
                default:
                    return;
            }
        }

        private void WriteLogText(string logLevel, string logText)
        {
            using(StreamWriter sw = new StreamWriter(mLogFileName, true) )
            {
                sw.WriteLine("------------------------------------------");
                sw.WriteLine("{0}: {1} - {2}", DateTime.Now, logLevel, logText);
                sw.WriteLine(string.Empty);
            }
        }

        private static void WriteConsoleText(string logLevel, string logText)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("{0}: {1} - {2}", DateTime.Now, logLevel, logText);
            Console.WriteLine(string.Empty);
        }

        private static void WriteMemberEntryConsole(ApiMember apiMember)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Key: {0}", apiMember.Key);
            Console.WriteLine("FriendlyName: {0}", apiMember.FriendlyName);
            Console.WriteLine("BaseType: {0}", apiMember.BaseType);
            Console.WriteLine("Type: {0}", apiMember.Type);
            Console.WriteLine("URL: {0}", apiMember.Url);
            foreach(ApiMemberParameter amp in apiMember.Parameters)
            {
                Console.WriteLine("\tParameter: {0}", amp.Key);
            }
        }

        private void WriteMemberEntryLog(ApiMember apiMember)
        {
            using (StreamWriter sw = new StreamWriter(mLogFileName, true))
            {
                sw.WriteLine("------------------------------------------");
                sw.WriteLine("Key: {0}", apiMember.Key);
                sw.WriteLine("FriendlyName: {0}", apiMember.FriendlyName);
                sw.WriteLine("BaseType: {0}", apiMember.BaseType);
                sw.WriteLine("Type: {0}", apiMember.Type);
                sw.WriteLine("URL: {0}", apiMember.Url);
                foreach (ApiMemberParameter amp in apiMember.Parameters)
                {
                    sw.WriteLine("\tParameter: {0}", amp.Key);
                }
            }
        }

        private string mLogFileName;
    }
}
