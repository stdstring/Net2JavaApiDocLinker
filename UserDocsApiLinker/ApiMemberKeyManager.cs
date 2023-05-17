using System;
using System.Reflection;
using UserDocsApiLinker.Utils;

namespace UserDocsApiLinker
{
    internal static class ApiMemberKeyManager
    {
        public static String GetKey(MemberInfo memberInfo)
        {
            if (memberInfo is Type)
                return ((Type) memberInfo).FullName;
            if (memberInfo is ConstructorInfo)
                return String.Format(CompKeyTemplate, memberInfo.ReflectedType.FullName, ConstructorName);
            return String.Format(CompKeyTemplate, memberInfo.ReflectedType.FullName, memberInfo.Name);
        }

        public static String GetContainedType(ApiMember member)
        {
            if (member.Type == ApiMemberType.Type || member.Type == ApiMemberType.Unknown)
                return member.Key;
            Int32 lastDotIndex = member.Key.LastIndexOf(MemberPartDef.ClassNameDelimiter);
            return lastDotIndex == -1 ? member.Key : member.Key.Substring(0, lastDotIndex);
        }

        public static String GetMemberName(ApiMember member)
        {
            if (member.Type == ApiMemberType.Type || member.Type == ApiMemberType.Unknown)
                return String.Empty;
            return MemberPartDef.GetLastNamePart(member.Key);
        }

        public const String ConstructorName = "#ctor";
        private const String CompKeyTemplate = "{0}.{1}";
    }
}
