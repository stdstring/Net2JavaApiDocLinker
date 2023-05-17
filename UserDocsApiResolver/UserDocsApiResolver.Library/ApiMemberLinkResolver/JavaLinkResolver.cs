using System;
using System.Collections.Generic;
using System.Reflection;
using UserDocsApiResolver.Library.JavaMetadata;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal class JavaLinkResolver : ILinkResolver
    {
        public JavaLinkResolver(String baseAddress, JavaMetadataStorage javaMetadataStorage)
        {
            _baseAddress = baseAddress;
            _javaMetadataStorage = javaMetadataStorage;
            _typeTransformer = new TypeTransformer();
            _methodNameTransformer = new MethodNameTransformer();
            _parameterListTransformer = new ParameterListTransformer(javaMetadataStorage);
        }

        public ApiMember FillType(ApiMember source, Type typeInfo)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            source.Url = String.Format(JavaLinkDef.HLinkTemplate, _baseAddress, typeInfo.Name.ToLower());
            Type baseType = typeInfo.BaseType;
            if (baseType != null)
                source.BaseType = baseType.FullName;
            return source;
        }

        public ApiMember FillEvent(ApiMember source, EventInfo eventInfo)
        {
            // TODO (std_string) : what do in this situation ???
            Logger.WarnFormat("Process {0} Failed", ApiMemberUtils.GetString(source));
            source.Url = "";
            return source;
        }

        public ApiMember FillField(ApiMember source, FieldInfo fieldInfo)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            Type containedType = fieldInfo.ReflectedType;
            if (containedType.IsEnum)
                source.Url = String.Format(JavaLinkDef.HLinkTemplate, _baseAddress, containedType.Name);
            else
            {
                // TODO (std_string) : what do in this situation ???
                Logger.WarnFormat("Process {0} Failed", ApiMemberUtils.GetString(source));
                source.Url = "";
            }
            return source;
        }

        public ApiMember FillProperty(ApiMember source, List<PropertyInfo> propertyList)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            Int32 index = propertyList.FindIndex(prop => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(prop), source, -1));
            if (index == -1)
            {
                Logger.WarnFormat("Process {0} Failed", ApiMemberUtils.GetString(source));
                return source;
            }
            PropertyInfo property = propertyList[index];
            Type containedType = property.ReflectedType;
            String paramList = MemberPartDef.BuildParamList(_parameterListTransformer.Transform(property), false);
            String propertySignature = _methodNameTransformer.Transform2HLink(property) + paramList;
            return FillMember(source, containedType, propertySignature);
        }

        public ApiMember FillConstructor(ApiMember source, List<ConstructorInfo> constructorList)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            Int32 index = constructorList.FindIndex(ctor => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(ctor), source, -1));
            if (index == -1)
            {
                Logger.WarnFormat("Process {0} Failed", ApiMemberUtils.GetString(source));
                return source;
            }
            ConstructorInfo constructor = constructorList[index];
            Type containedType = constructor.ReflectedType;
            String constructorName = _methodNameTransformer.Transform(constructor);
            String paramList = MemberPartDef.BuildParamList(_parameterListTransformer.Transform(constructor), true);
            String constructorSignature = constructorName + paramList;
            return FillMember(source, containedType, constructorSignature);
        }

        public ApiMember FillMethod(ApiMember source, List<MethodInfo> methodList)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            Int32 index = methodList.FindIndex(meth => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(meth), source, -1));
            if (index == -1)
            {
                Logger.WarnFormat("Process {0} Failed", ApiMemberUtils.GetString(source));
                return source;
            }
            MethodInfo method = methodList[index];
            Type containedType = method.ReflectedType;
            String methodName = _methodNameTransformer.Transform(method);
            String paramList = MemberPartDef.BuildParamList(_parameterListTransformer.Transform(method), true);
            String methodSignature = methodName + paramList;
            return FillMember(source, containedType, methodSignature);
        }

        public ApiMember FillMethodSummary(ApiMember source)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            String sourceType = ApiMemberUtils.GetContainedType(source);
            String methodName = ApiMemberUtils.GetMemberName(source);
            String javaType = _typeTransformer.TransformAsposeType(sourceType);
            String javaMethodName = _methodNameTransformer.TransformMethod(methodName);
            IList<JavaMethod> methods = _javaMetadataStorage.FindMethods(javaType, javaMethodName);
            JavaMethod method = methods.Count > 0 ? methods[0] : null;
            if (method != null)
                source.Url = String.Format(JavaLinkDef.MemberHLinkTemplate, _baseAddress, MemberPartDef.GetLastNamePart(javaType), MemberPartDef.GetMethodSignature(method.Source));
            else
            {
                Logger.WarnFormat("Process {0} Failed", ApiMemberUtils.GetString(source));
                source.Url = "";
            }
            return source;
        }

        public ApiMember FillConstructorSummary(ApiMember source)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            String sourceType = ApiMemberUtils.GetContainedType(source);
            String javaType = _typeTransformer.TransformAsposeType(sourceType);
            IList<JavaConstructor> ctors = _javaMetadataStorage.FindConstructors(javaType);
            JavaConstructor ctor = ctors.Count > 0 ? ctors[0] : null;
            if (ctor != null)
                source.Url = String.Format(JavaLinkDef.MemberHLinkTemplate, _baseAddress, MemberPartDef.GetLastNamePart(javaType), MemberPartDef.GetMethodSignature(ctor.Source));
            else
            {
                Logger.WarnFormat("Process {0} Failed", ApiMemberUtils.GetString(source));
                source.Url = "";
            }
            return source;
        }

        private ApiMember FillMember(ApiMember source, Type containedType, String signature)
        {
            source.Url = String.Format(JavaLinkDef.MemberHLinkTemplate, _baseAddress, containedType.Name, signature);
            // TODO (std_string) : think about BaseType
            if (containedType.BaseType != null)
                source.BaseType = containedType.BaseType.FullName;
            return source;
        }

        private readonly String _baseAddress;
        private readonly JavaMetadataStorage _javaMetadataStorage;
        private readonly TypeTransformer _typeTransformer;
        private readonly MethodNameTransformer _methodNameTransformer;
        private readonly ParameterListTransformer _parameterListTransformer;

        private static readonly ILog Logger = LogManager.GetLogger("JavaLinkResolver");
    }
}
