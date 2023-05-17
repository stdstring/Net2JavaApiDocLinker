using System;
using System.Collections.Generic;
using UserDocsApiLinker.JavaMetadata;
using UserDocsApiLinker.Utils;

namespace UserDocsApiLinker.ApiMemberLinkResolver
{
    internal class JavaApiMemberBuilder
    {
        public JavaApiMemberBuilder(String baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public IList<ApiMember> Build(JavaClass javaClass, List<ApiMember> netApiMembers)
        {
            List<ApiMember> javaSpecificMembers = new List<ApiMember>();
            // ApiMember 4 javaClass
            Boolean commonClass = true;
            if (netApiMembers.Find(member => member.FriendlyName.Equals(javaClass.ShortName)) == null)
            {
                javaSpecificMembers.Add(CreateClassMember(javaClass));
                commonClass = false;
            }
            // ApiMembers 4 javaClass's Fields
            foreach (JavaField field in javaClass.FieldList)
            {
                ApiMember fieldMember = CreateFieldMember(javaClass, field);
                javaSpecificMembers.Add(fieldMember);
            }
            // ApiMembers 4 javaClass's Constructors
            foreach (JavaConstructor ctor in javaClass.ConstructorList)
            {
                ApiMember ctorMember = CreateCtorMember(javaClass, ctor);
                javaSpecificMembers.Add(ctorMember);
            }
            // ApiMembers 4 javaClass's Methods
            foreach (JavaMethod method in javaClass.MethodList)
            {
                String friendlyName = javaClass.ShortName + "." + method.Name;
                if (netApiMembers.Find(member => member.FriendlyName.Equals(friendlyName)) != null)
                        continue;
                ApiMember methodMember = CreateMethodMember(javaClass, method, commonClass);
                javaSpecificMembers.Add(methodMember);
            }
            return javaSpecificMembers;
        }

        private ApiMember CreateClassMember(JavaClass javaClass)
        {
            return new ApiMember
                       {
                           BaseType = null,
                           FriendlyName = javaClass.ShortName,
                           Key = CommonDefs.JavaAsposePrefix + javaClass.ShortName,
                           Parameters = new List<ApiMemberParameter>(),
                           Type = ApiMemberType.Type,
                           Url = String.Format(JavaLinkDef.HLinkTemplate, baseAddress, javaClass.ShortName.ToLower())
                       };
        }

        private ApiMember CreateFieldMember(JavaClass javaClass, JavaField field)
        {
            return new ApiMember
                       {
                           BaseType = null,
                           FriendlyName = javaClass.ShortName + "." + field.Name,
                           Key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + "." + field.Name,
                           Parameters = new List<ApiMemberParameter>(),
                           Type = ApiMemberType.Field,
                           Url = String.Format(JavaLinkDef.HLinkTemplate, baseAddress, javaClass.ShortName)
                       };
        }

        private ApiMember CreateCtorMember(JavaClass javaClass, JavaConstructor ctor)
        {
            List<String> parameters = ctor.Parameters.ConvertAll<String>(MemberPartDef.GetLastNamePart);
            List<ApiMemberParameter> apiMemberParameters = ctor.Parameters.ConvertAll(param => new ApiMemberParameter(param));
            String paramList = MemberPartDef.BuildParamList(parameters, false);
            String signature = javaClass.ShortName + paramList;
            return new ApiMember
                       {
                           BaseType = null,
                           FriendlyName = javaClass.ShortName + CtorName,
                           Key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + CtorName,
                           Parameters = apiMemberParameters,
                           Type = ApiMemberType.Method,
                           Url = String.Format(JavaLinkDef.MemberHLinkTemplate, baseAddress, javaClass.ShortName, signature)
                       };
        }

        private ApiMember CreateMethodMember(JavaClass javaClass, JavaMethod method, Boolean commonClass)
        {
            if (IsProperty(method))
                return CreatePropertyMember(javaClass, method, commonClass);
            String paramList = MemberPartDef.BuildParamList(method.Parameters, true);
            String signature = method.Name + paramList;
            String url = String.Format(JavaLinkDef.MemberHLinkTemplate, baseAddress, javaClass.ShortName, signature);
            List<ApiMemberParameter> apiMemberParameters = method.Parameters.ConvertAll(param => new ApiMemberParameter(param));
            return new ApiMember
                       {
                           BaseType = null,
                           FriendlyName = javaClass.ShortName + "." + method.Name,
                           Key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + "." + method.Name,
                           Parameters = apiMemberParameters,
                           Type = ApiMemberType.Method,
                           Url = url
                       };
        }

        private ApiMember CreatePropertyMember(JavaClass javaClass, JavaMethod method, Boolean commonClass)
        {
            String propertyName = GetPropertyName(method);
            String friendlyName = javaClass.ShortName + "." + propertyName;
            if(commonClass)
            {
                String key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + "." + method.Name;
                List<ApiMemberParameter> apiMemberParameters = method.Parameters.ConvertAll(param => new ApiMemberParameter(param));
                List<String> parameters = GetPropertyIndexParams(method);
                String paramList = MemberPartDef.BuildParamList(parameters, false);
                String signature = propertyName + paramList;
                String url = String.Format(JavaLinkDef.MemberHLinkTemplate, baseAddress, javaClass.ShortName, signature);
                return new ApiMember
                           {
                               BaseType = null,
                               FriendlyName = friendlyName,
                               Key = key,
                               Parameters = apiMemberParameters,
                               Type = ApiMemberType.Method,
                               Url = url
                           };
            }
            else
            {
                String key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + "." + propertyName;
                List<String> parameters = GetPropertyIndexParams(method);
                List<ApiMemberParameter> apiMemberParameters = parameters.ConvertAll(param => new ApiMemberParameter(param));
                String paramList = MemberPartDef.BuildParamList(parameters, true);
                String signature = method.Name + paramList;
                String url = String.Format(JavaLinkDef.MemberHLinkTemplate, baseAddress, javaClass.ShortName, signature);
                return new ApiMember
                {
                    BaseType = null,
                    FriendlyName = friendlyName,
                    Key = key,
                    Parameters = apiMemberParameters,
                    Type = ApiMemberType.Property,
                    Url = url
                };
            }
        }

        private Boolean IsProperty(JavaMethod method)
        {
            if (method.Name.Length < GetPrefix.Length)
                return false;
            if (method.Name.Equals(GetPrefix) || method.Name.Equals(SetPrefix))
                return true;
            if (method.Name.StartsWith(GetPrefix) && Char.IsUpper(method.Name[GetPrefix.Length]))
                return true;
            if (method.Name.StartsWith(SetPrefix) && Char.IsUpper(method.Name[SetPrefix.Length]))
                return true;
            return false;
        }

        private String GetPropertyName(JavaMethod method)
        {
            if (!IsProperty(method))
                return String.Empty;
            if (method.Name.Equals(GetPrefix) || method.Name.Equals(SetPrefix))
                return IndexerName;
            if (method.Name.StartsWith(GetEnumPrefix))
                return IndexerEnumPrefix + method.Name.Substring(GetEnumPrefix.Length);
            if (method.Name.StartsWith(SetEnumPrefix))
                return IndexerEnumPrefix + method.Name.Substring(SetEnumPrefix.Length);
            return method.Name.Substring(GetPrefix.Length);
        }

        private List<String> GetPropertyIndexParams(JavaMethod method)
        {
            if (!IsProperty(method))
                return new List<String>();
            if (method.Name.StartsWith(SetPrefix))
                return new List<String>(method.Parameters.GetRange(1, method.Parameters.Count - 1));
            return new List<String>(method.Parameters);
        }

        private readonly String baseAddress;

        private const String GetPrefix = "get";
        private const String SetPrefix = "set";
        private const String GetEnumPrefix = "getBy";
        private const String SetEnumPrefix = "setBy";
        private const String IndexerName = "Item";
        private const String IndexerEnumPrefix = "Item";
        private const String CtorName = ".#ctor";
    }
}
