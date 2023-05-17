using System;
using System.Collections.Generic;
using UserDocsApiResolver.Library.ApiMemberLinkResolver;
using UserDocsApiResolver.Library.JavaMetadata;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberBuilder
{
    internal class JavaSpecificCollectionBuilder
    {
        public JavaSpecificCollectionBuilder(String baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public List<ApiMember> Build(JavaMetadataStorage storage, List<ApiMember> netApiMembers)
        {
            Logger.Debug("Build() enter");
            List<ApiMember> javaSpecificMembers = new List<ApiMember>();
            foreach (JavaClass javaClass in storage.GetAllClasses())
            {
                javaSpecificMembers.AddRange(Build(javaClass, netApiMembers));
            }
            Logger.DebugFormat("Build {0} members", javaSpecificMembers.Count);
            Logger.Debug("Build() exit");
            return javaSpecificMembers;
        }

        private IEnumerable<ApiMember> Build(JavaClass javaClass, List<ApiMember> netApiMembers)
        {
            List<ApiMember> javaSpecificMembers = new List<ApiMember>();
            // ApiMember 4 javaClass
            ApiMember classMember = CreateClassMember(javaClass);
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(classMember));
            javaSpecificMembers.Add(classMember);
            // ApiMembers 4 javaClass's Fields
            foreach (JavaField field in javaClass.FieldList)
            {
                ApiMember fieldMember = CreateFieldMember(javaClass, field);
                Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(fieldMember));
                javaSpecificMembers.Add(fieldMember);
            }
            // ApiMembers 4 javaClass's Constructors
            foreach (JavaConstructor ctor in javaClass.ConstructorList)
            {
                ApiMember ctorMember = CreateCtorMember(javaClass, ctor);
                Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(ctorMember));
                javaSpecificMembers.Add(ctorMember);
            }
            // ctors summary
            // ApiMembers 4 javaClass's Methods
            foreach (JavaMethod method in javaClass.MethodList)
            {
                ApiMember methodMember = CreateMethodMember(javaClass, method, netApiMembers);
                Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(methodMember));
                javaSpecificMembers.Add(methodMember);
            }
            // methods summary
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
                           Url = String.Format(JavaLinkDef.HLinkTemplate, _baseAddress, javaClass.ShortName.ToLower())
                       };
        }

        private ApiMember CreateFieldMember(JavaClass javaClass, JavaField field)
        {
            return new ApiMember
                       {
                           BaseType = null,
                           FriendlyName = javaClass.ShortName + MemberPartDef.ClassNameDelimiter + field.Name,
                           Key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + MemberPartDef.ClassNameDelimiter + field.Name,
                           Parameters = new List<ApiMemberParameter>(),
                           Type = ApiMemberType.Field,
                           Url = String.Format(JavaLinkDef.HLinkTemplate, _baseAddress, javaClass.ShortName)
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
                           FriendlyName = javaClass.ShortName + MemberPartDef.ClassNameDelimiter + CommonDefs.Ctor,
                           Key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + MemberPartDef.ClassNameDelimiter + CommonDefs.Ctor,
                           Parameters = apiMemberParameters,
                           Type = ApiMemberType.Method,
                           Url = String.Format(JavaLinkDef.MemberHLinkTemplate, _baseAddress, javaClass.ShortName, signature)
                       };
        }

        private ApiMember CreateMethodMember(JavaClass javaClass, JavaMethod method, List<ApiMember> netApiMembers)
        {
            if (IsProperty(method))
            {
                // e.g., member getText() similar to property, but it is method
                String member = javaClass.ShortName + MemberPartDef.ClassNameDelimiter + GetPropertyName(method);
                if (ApiMemberUtils.CheckExistenceByName(member, netApiMembers))
                    return CreatePropertyMember(javaClass, method);
            }
            String paramList = MemberPartDef.BuildParamList(method.Parameters, true);
            String signature = method.Name + paramList;
            String url = String.Format(JavaLinkDef.MemberHLinkTemplate, _baseAddress, javaClass.ShortName, signature);
            List<ApiMemberParameter> apiMemberParameters = method.Parameters.ConvertAll(param => new ApiMemberParameter(param));
            return new ApiMember
                       {
                           BaseType = null,
                           FriendlyName = javaClass.ShortName + MemberPartDef.ClassNameDelimiter + method.Name,
                           Key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + MemberPartDef.ClassNameDelimiter + method.Name,
                           Parameters = apiMemberParameters,
                           Type = ApiMemberType.Method,
                           Url = url
                       };
        }

        private ApiMember CreatePropertyMember(JavaClass javaClass, JavaMethod method)
        {
            String propertyName = GetPropertyName(method);
            String key = CommonDefs.JavaAsposePrefix + javaClass.ShortName + "." + method.Name;
            List<ApiMemberParameter> apiMemberParameters = method.Parameters.ConvertAll(param => new ApiMemberParameter(param));
            List<String> parameters = GetPropertyIndexParams(method);
            String paramList = MemberPartDef.BuildParamList(parameters, false);
            String signature = propertyName + paramList;
            String url = String.Format(JavaLinkDef.MemberHLinkTemplate, _baseAddress, javaClass.ShortName, signature);
            return new ApiMember
            {
                BaseType = null,
                FriendlyName = javaClass.ShortName + MemberPartDef.ClassNameDelimiter + propertyName,
                Key = key,
                Parameters = apiMemberParameters,
                Type = ApiMemberType.Method,
                Url = url
            };
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

        private readonly String _baseAddress;

        private static readonly ILog Logger = LogManager.GetLogger("JavaSpecificCollectionBuilder");

        private const String GetPrefix = "get";
        private const String SetPrefix = "set";
        private const String GetEnumPrefix = "getBy";
        private const String SetEnumPrefix = "setBy";
        private const String IndexerName = "Item";
        private const String IndexerEnumPrefix = "Item";
    }
}
