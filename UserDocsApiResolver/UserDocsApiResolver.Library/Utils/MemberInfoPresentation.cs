using System;
using System.Collections.Generic;
using System.Reflection;

namespace UserDocsApiResolver.Library.Utils
{
    internal static class MemberInfoPresentation
    {
        public static String GetString(MethodInfo method)
        {
            IList<String> parameters = Array.ConvertAll(method.GetParameters(), parameter => parameter.ParameterType.FullName);
            String parameterList = MemberPartDef.BuildParamList(parameters, true);
            return method.ReflectedType.FullName + MemberPartDef.ClassNameDelimiter + method.Name + parameterList;
        }

        public static String GetString(ConstructorInfo ctor)
        {
            IList<String> parameters = Array.ConvertAll(ctor.GetParameters(), parameter => parameter.ParameterType.FullName);
            String parameterList = MemberPartDef.BuildParamList(parameters, true);
            return ctor.ReflectedType.FullName + MemberPartDef.ClassNameDelimiter + CommonDefs.Ctor + parameterList;
        }

        public static String GetString(PropertyInfo property)
        {
            IList<String> parameters = Array.ConvertAll(property.GetIndexParameters(), parameter => parameter.ParameterType.FullName);
            String parameterList = MemberPartDef.BuildParamList(parameters, false);
            return property.ReflectedType.FullName + MemberPartDef.ClassNameDelimiter + property.Name + parameterList;
        }
    }
}
