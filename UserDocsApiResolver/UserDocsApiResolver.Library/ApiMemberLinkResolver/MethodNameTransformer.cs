using System;
using System.Collections.Generic;
using System.Reflection;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal class MethodNameTransformer
    {
        public MethodNameTransformer()
        {
            _typeTransformer = new TypeTransformer();
        }

        public String Transform(MethodInfo method)
        {
            Logger.DebugFormat("TransformMethod {0}", MemberInfoPresentation.GetString(method));
            return TransformMethod(method.Name);
        }

        public String TransformMethod(String methodName)
        {
            Logger.DebugFormat("TransformMethod {0}", methodName);
            // Some methods can have another name
            if (_methodNameLinks.ContainsKey(methodName))
                return _methodNameLinks[methodName];
            // in .NET UpperCamelCase, in Java lowerCamelCase
            return LowerFirstLetter(methodName);
        }

        public String Transform(ConstructorInfo constructor)
        {
            Logger.DebugFormat("TransformConstructor {0}", MemberInfoPresentation.GetString(constructor));
            // TODO (std_string) : think, because this is not right for primitive, arrays and generics
            Type containdedType = constructor.ReflectedType;
            String javaType = _typeTransformer.Transform(containdedType);
            if (String.IsNullOrEmpty(javaType))
                return null;
            Int32 shortTypeNameIndex = javaType.LastIndexOf(MemberPartDef.ClassNameDelimiter);
            return javaType.Substring(shortTypeNameIndex + 1);
        }

        public String Transform2Java(PropertyInfo property)
        {
            Logger.DebugFormat("Transform2Java {0}", MemberInfoPresentation.GetString(property));
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
            Logger.DebugFormat("Transform2HLink {0}", MemberInfoPresentation.GetString(property));
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

        private readonly TypeTransformer _typeTransformer;
        private readonly IDictionary<String, String> _methodNameLinks = new Dictionary<String, String>
                                                                            {
                                                                                {"Clone", "deepClone"}
                                                                            };

        private static readonly ILog Logger = LogManager.GetLogger("MethodNameTransformer");

        private const String PropertyGetTemplate = "get{0}";
        private const String IndexerName = "get";
        private const String IndexerTemplateForEnum = "getBy{0}";

        private const String IndexerHLink = "Item";
        private const String IndexerHLinkTemplateForEnum = "ItemBy{0}";
    }
}
