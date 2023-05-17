using System;
using System.Reflection;
using UserDocsApiResolver.Library.Utils;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal static class ApiMemberFactory
    {
        public static ApiMember Create(PropertyInfo propertyInfo)
        {
            return Create(propertyInfo.ReflectedType, propertyInfo, ApiMemberType.Property, propertyInfo.GetIndexParameters());
        }

        public static ApiMember Create(ConstructorInfo constructorInfo)
        {
            return Create(constructorInfo.ReflectedType, constructorInfo, ApiMemberType.Method, constructorInfo.GetParameters());
        }

        public static ApiMember Create(MethodInfo methodInfo)
        {
            return Create(methodInfo.ReflectedType, methodInfo, ApiMemberType.Method, methodInfo.GetParameters());
        }

        private static ApiMember Create(Type containedType, MemberInfo memberInfo, ApiMemberType memberType, ParameterInfo[] parameters)
        {
            ApiMember member = new ApiMember
                                   {
                                       Key = ApiMemberUtils.GetKey(memberInfo),
                                       FriendlyName = string.Empty,
                                       Type = memberType
                                   };
            Array.ForEach(parameters, parameter => member.Parameters.Add(new ApiMemberParameter(parameter.ParameterType.FullName)));
            if (containedType.BaseType != null)
                member.BaseType = containedType.BaseType.FullName;
            return member;
        }
    }
}
