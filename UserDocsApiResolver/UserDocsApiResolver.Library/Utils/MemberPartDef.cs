using System;
using System.Collections.Generic;
using System.Text;

namespace UserDocsApiResolver.Library.Utils
{
    internal static class MemberPartDef
    {
        public static String GetLastNamePart(String name)
        {
            if (String.IsNullOrEmpty(name))
                return name;
            Int32 index = name.LastIndexOf(ClassNameDelimiter);
            return index == -1 ? name : name.Substring(index + 1);
        }

        public static String GetMethodSignature(String source)
        {
            // from xxx.yyy.ClassName.MethodName(blablabla1,blablabla2) obtain MethodName(blablabla1,blablabla2)
            Int32 parameterBlockIndex = source.IndexOf(ParameterBlockStart);
            if (parameterBlockIndex == -1)
                return String.Empty;
            Int32 methodNameIndex = source.LastIndexOf(ClassNameDelimiter, parameterBlockIndex);
            return methodNameIndex != -1 ? source.Substring(methodNameIndex + 1) : source;
        }

        public static String BuildParamList(IList<String> parameters, Boolean allowEmptyList)
        {
            if (parameters == null)
                return String.Empty;
            if (parameters.Count == 0 && !allowEmptyList)
                return String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (Int32 index = 0; index < parameters.Count; ++index)
            {
                if (index > 0) sb.Append(ParametersDelimiter);
                sb.Append(parameters[index]);
            }
            sb.Append(")");
            return sb.ToString();
        }

        public const String ParameterBlockStart = "(";
        public const String ParameterBlockEnd = ")";
        public const String ParametersDelimiter = ",";
        public const String ClassNameDelimiter = ".";

        public const Char ParametersDelimiterChar = ',';
        public const Char ClassNameDelimiterChar = '.';
  }
}
