using System;
using System.Collections.Generic;
using UserDocsApiLinker.Utils;

namespace UserDocsApiLinker.JavaMetadata
{
    internal class Signature
    {
        public String Name { get; set; }
        public List<String> Parameters { get; set; }

        public static Signature Parse(String signature)
        {
            Int32 startIndex = signature.IndexOf(MemberPartDef.ParameterBlockStart);
            Int32 endIndex = signature.LastIndexOf(MemberPartDef.ParameterBlockEnd);
            if (startIndex == -1 || endIndex == -1)
                return null;
            String parameterBlock = signature.Substring(startIndex + 1, endIndex - startIndex - 1);
            String nameBlock = signature.Substring(0, startIndex);
            Int32 nameIndex = nameBlock.LastIndexOf(MemberPartDef.ClassNameDelimiter);
            if (nameIndex == -1)
                return null;
            String name = nameBlock.Substring(nameIndex + 1);
            String[] parameters = String.IsNullOrEmpty(parameterBlock)
                                      ? new string[0]
                                      : parameterBlock.Split(MemberPartDef.ParametersDelimiter);
            return new Signature {Name = name, Parameters = new List<String>(parameters)};
        }

        private Signature()
        {
	}
    }
}
