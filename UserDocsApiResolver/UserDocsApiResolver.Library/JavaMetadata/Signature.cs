using System;
using System.Collections.Generic;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.JavaMetadata
{
    internal class Signature
    {
        public String Name { get; set; }
        public List<String> Parameters { get; set; }

        public static Signature Parse(String signature)
        {
            Logger.DebugFormat("Parse({0}) enter", signature);
            Int32 startIndex = signature.IndexOf(MemberPartDef.ParameterBlockStart);
            Int32 endIndex = signature.LastIndexOf(MemberPartDef.ParameterBlockEnd);
            if (startIndex == -1 || endIndex == -1)
            {
                Logger.Warn("Parameter block absent");
                Logger.DebugFormat("Parse({0}) exit", signature);
                return null;
            }
            String parameterBlock = signature.Substring(startIndex + 1, endIndex - startIndex - 1);
            String nameBlock = signature.Substring(0, startIndex);
            Int32 nameIndex = nameBlock.LastIndexOf(MemberPartDef.ClassNameDelimiter);
            if (nameIndex == -1)
            {
                Logger.Warn("Incorrect name");
                Logger.DebugFormat("Parse({0}) exit", signature);
                return null;
            }
            String name = nameBlock.Substring(nameIndex + 1);
            String[] parameters = String.IsNullOrEmpty(parameterBlock)
                                      ? new string[0]
                                      : parameterBlock.Split(MemberPartDef.ParametersDelimiterChar);
            Logger.DebugFormat("Parse({0}) exit", signature);
            return new Signature {Name = name, Parameters = new List<String>(parameters)};
        }

        private Signature()
        {
        }

        private static readonly ILog Logger = LogManager.GetLogger("Signature");
    }
}
