using System;

namespace UserDocsApiResolver.Library.JavaReflection
{
    internal class JavaReflectionRequest
    {
        public JavaReflectionRequest(String command, String arg)
        {
            Command = command;
            Arg = arg;
        }

        public String Command { get; private set; }
        public String Arg { get; private set; }

        public String GetRequestString()
        {
            return String.Format(RequestTemplate, Command, Arg);
        }

        public override string ToString()
        {
            return GetRequestString();
        }

        private const String RequestTemplate = "{0}:{1}";
    }

    internal static class JavaReflectionCommands
    {
        public const String GetClassesFromJar = "Classes";
        public const String GetMembersForClass = "Members";
    }
}
