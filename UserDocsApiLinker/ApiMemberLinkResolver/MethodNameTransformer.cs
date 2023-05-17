using System;
using System.Collections.Generic;
using System.Reflection;
using UserDocsApiLinker.Utils;

namespace UserDocsApiLinker.ApiMemberLinkResolver
{
    internal class MethodNameTransformer
    {
        public MethodNameTransformer()
        {
            typeTransformer = new TypeTransformer();
        }

        public String Transform(MethodInfo method)
        {
            return TransformMethod(method.Name);
        }

        public String TransformMethod(String methodName)
        {
            // Some methods can have another name
            if (methodNameLinks.ContainsKey(methodName))
                return methodNameLinks[methodName];
            // in .NET UpperCamelCase, in Java lowerCamelCase
            return LowerFirstLetter(methodName);
        }

        public String Transform(ConstructorInfo constructor)
        {
            // TODO (std_string) : think, because this is not right for primitive, arrays and generics
            Type containdedType = constructor.ReflectedType;
            String javaType = typeTransformer.Transform(containdedType);
            if (String.IsNullOrEmpty(javaType))
                return null;
            Int32 shortTypeNameIndex = javaType.LastIndexOf(MemberPartDef.ClassNameDelimiter);
            return javaType.Substring(shortTypeNameIndex + 1);
        }

        public String Transform2Java(PropertyInfo property)
        {
            if(ApiMemberUtils.IsContainSpecialPrefix(property.Name))
            {
                String propertyName = property.Name;
                return LowerFirstLetter(propertyName);
            }
            if(property.GetIndexParameters().Length == 0)
                return String.Format(PropertyGetTemplate, property.Name);
            // TODO (std_string) : show case when we have not getter
            if (property.PropertyType.IsEnum)
                return String.Format(IndexerTemplateForEnum, property.PropertyType.Name);
            return IndexerName;
        }

        public String Transform2HLink(PropertyInfo property)
        {
            if (property.GetIndexParameters().Length == 0)
                return property.Name;
            if (property.PropertyType.IsEnum)
                return String.Format(IndexerHLinkTemplateForEnum, property.PropertyType.Name);
            return IndexerHLink;
        }

        private String LowerFirstLetter(String name)
        {
            return String.IsNullOrEmpty(name) ? null : Char.ToLower(name[0]) + name.Substring(1);
        }

        private readonly TypeTransformer typeTransformer;
        private readonly IDictionary<String, String> methodNameLinks = new Dictionary<String, String> {{"Clone", "deepClone"}};

        private const String PropertyGetTemplate = "get{0}";
        private const String IndexerName = "get";
        private const String IndexerTemplateForEnum = "getBy{0}";

        private const String IndexerHLink = "Item";
        private const String IndexerHLinkTemplateForEnum = "ItemBy{0}";
    }
}
