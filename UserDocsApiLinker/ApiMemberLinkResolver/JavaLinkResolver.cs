using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UserDocsApiLinker.JavaMetadata;
using UserDocsApiLinker.Utils;

namespace UserDocsApiLinker.ApiMemberLinkResolver
{
    internal class JavaLinkResolver : ILinkResolver
    {
        public JavaLinkResolver(String baseAddress, JavaMetadataStorage javaMetadataStorage)
        {
            address = baseAddress;
            this.javaMetadataStorage = javaMetadataStorage;
            typeTransformer = new TypeTransformer();
            methodNameTransformer = new MethodNameTransformer();
            parameterListTransformer = new ParameterListTransformer(javaMetadataStorage);
        }

        public ApiMember FillType(ApiMember source, Type typeInfo)
        {
            source.Url = String.Format(JavaLinkDef.HLinkTemplate, address, typeInfo.Name.ToLower());
            Type baseType = typeInfo.BaseType;
            if (baseType != null)
                source.BaseType = baseType.FullName;
            return source;
        }

        public ApiMember FillEvent(ApiMember source, EventInfo eventInfo)
        {
            // TODO (std_string) : what do in this situation ???
            source.Url = "unknown";
            return source;
        }

        public ApiMember FillField(ApiMember source, FieldInfo fieldInfo)
        {
            Type containedType = fieldInfo.ReflectedType;
            if(containedType.IsEnum)
            {
                source.Url = String.Format(JavaLinkDef.HLinkTemplate, address, containedType.Name);
            }
            else
            {
                source.Url = "unknown";
                if (containedType.BaseType != null)
                    source.BaseType = containedType.BaseType.FullName;
            }
            return source;
        }

        public ApiMember FillProperty(ApiMember source, List<PropertyInfo> propertyList)
        {
            Int32 index = propertyList.FindIndex(prop => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(prop), source, -1));
            if (index == -1)
                return source;
            PropertyInfo property = propertyList[index];
            Type containedType = property.ReflectedType;
            String propertySignature = methodNameTransformer.Transform2HLink(property) + BuildParamList(parameterTransformer => parameterTransformer.Transform(property), false);
            return FillMember(source, containedType, propertySignature);
        }

        public ApiMember FillConstructor(ApiMember source, List<ConstructorInfo> constructorList)
        {
            Int32 index = constructorList.FindIndex(ctor => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(ctor), source, -1));
            if (index == -1)
                return source;
            ConstructorInfo constructor = constructorList[index];
            Type containedType = constructor.ReflectedType;
            String constructorName = methodNameTransformer.Transform(constructor);
            String constructorSignature = constructorName + BuildParamList(parameterTransformer => parameterTransformer.Transform(constructor), true);
            return FillMember(source, containedType, constructorSignature);
        }

        public ApiMember FillMethod(ApiMember source, List<MethodInfo> methodList)
        {
            Int32 index = methodList.FindIndex(meth => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(meth), source, -1));
            if (index == -1)
                return source;
            MethodInfo method = methodList[index];
            Type containedType = method.ReflectedType;
            String methodName = methodNameTransformer.Transform(method);
            String methodSignature = methodName + BuildParamList(parameterTransformer => parameterTransformer.Transform(method), true);
            return FillMember(source, containedType, methodSignature);
        }

        public ApiMember FillMethodSummary(ApiMember source)
        {
            String sourceType = ApiMemberKeyManager.GetContainedType(source);
            String methodName = ApiMemberKeyManager.GetMemberName(source);
            String javaType = typeTransformer.TransformAsposeType(sourceType);
            String javaMethodName = methodNameTransformer.TransformMethod(methodName);
            IList<JavaMethod> methods = javaMetadataStorage.FindMethods(javaType, javaMethodName);
            JavaMethod method = methods.Count > 0 ? methods[0] : null;
            source.Url = method != null ? String.Format(JavaLinkDef.MemberHLinkTemplate, address, MemberPartDef.GetLastNamePart(javaType), MemberPartDef.GetMethodSignature(method.Source)) : "unknown";
            return source;
        }

        public ApiMember FillConstructorSummary(ApiMember source)
        {
            String sourceType = ApiMemberKeyManager.GetContainedType(source);
            String javaType = typeTransformer.TransformAsposeType(sourceType);
            IList<JavaConstructor> ctors = javaMetadataStorage.FindConstructors(javaType);
            JavaConstructor ctor = ctors.Count > 0 ? ctors[0] : null;
            source.Url = ctor != null ? String.Format(JavaLinkDef.MemberHLinkTemplate, address, MemberPartDef.GetLastNamePart(javaType), MemberPartDef.GetMethodSignature(ctor.Source)) : "unknown";
            return source;
        }

        private ApiMember FillMember(ApiMember source, Type containedType, String signature)
        {
            source.Url = String.Format(JavaLinkDef.MemberHLinkTemplate, address, containedType.Name, signature);
            // TODO (std_string) : think about BaseType
            if (containedType.BaseType != null)
                source.BaseType = containedType.BaseType.FullName;
            return source;
        }

        private String BuildParamList(Func<ParameterListTransformer, String[]> parametersTransformer, Boolean allowEmptyList)
        {
            String[] javaParameters = parametersTransformer(parameterListTransformer);
            if (javaParameters == null)
                return String.Empty;
            if (javaParameters.Length == 0 && !allowEmptyList)
                return String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (Int32 index = 0; index < javaParameters.Length; ++index)
            {
                if (index > 0) sb.Append(ParameterDelimiter);
                sb.Append(javaParameters[index]);
            }
            sb.Append(")");
            return sb.ToString();
        }

        private const String ParameterDelimiter = ", ";

        private readonly String address;
        private readonly JavaMetadataStorage javaMetadataStorage;
        private readonly TypeTransformer typeTransformer;
        private readonly MethodNameTransformer methodNameTransformer;
        private readonly ParameterListTransformer parameterListTransformer;
    }
}
