using System;
using System.Collections.Generic;
using System.Reflection;

namespace UserDocsApiLinker.ApiMemberLinkResolver
{
    internal class NetLinkResolver : ILinkResolver
    {
        public NetLinkResolver(String baseAddress)
        {
            address = baseAddress;
        }

        public ApiMember FillType(ApiMember source, Type typeInfo)
        {
            return Fill(source, source.Key, typeInfo.BaseType);
        }

        public ApiMember FillEvent(ApiMember source, EventInfo eventInfo)
        {
            return Fill(source, source.Key, eventInfo.ReflectedType.BaseType);
        }

        public ApiMember FillField(ApiMember source, FieldInfo fieldInfo)
        {
            Type reflectedType = fieldInfo.ReflectedType;
            return Fill(source, reflectedType.FullName, reflectedType.BaseType);
        }

        public ApiMember FillProperty(ApiMember source, List<PropertyInfo> propertyList)
        {
            Int32 index = propertyList.FindIndex(prop => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(prop), source, -1));
            if (index == -1)
                return source;
            PropertyInfo property = propertyList[index];
            return Fill(source, property.ReflectedType, source.Key, NetLinkDef.OverloadPropertyHLinkTemplate, propertyList.Count == 1 ? 0 : index + 1);
        }

        public ApiMember FillConstructor(ApiMember source, List<ConstructorInfo> constructorList)
        {
            Int32 index = constructorList.FindIndex(ctor => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(ctor), source, -1));
            if (index == -1)
                return source;
            ConstructorInfo constructor = constructorList[index];
            String key = constructor.ReflectedType.FullName;
            return Fill(source, constructor.ReflectedType, key, NetLinkDef.OverloadCtorHLinkTemplate, constructorList.Count == 1 ? 0 : index + 1);
        }

        public ApiMember FillMethod(ApiMember source, List<MethodInfo> methodList)
        {
            Int32 index = methodList.FindIndex(meth => ApiMemberUtils.CompareMembers(ApiMemberFactory.Create(meth), source, -1));
            if (index == -1)
                return source;
            MethodInfo method = methodList[index];
            return Fill(source, method.ReflectedType, source.Key, NetLinkDef.OverloadMethodHLinkTemplate, methodList.Count == 1 ? 0 : index + 1);
        }

        public ApiMember FillMethodSummary(ApiMember source)
        {
          source.Url = String.Format(NetLinkDef.MethodSummaryHLinkTemplate, address, source.Key.ToLower());
          return source;
        }

        public ApiMember FillConstructorSummary(ApiMember source)
        {
          String key = ApiMemberKeyManager.GetContainedType(source);
          source.Url = String.Format(NetLinkDef.CtorSummaryHLinkTemplate, address, key);
          return source;
        }

        private ApiMember Fill(ApiMember source, String key, Type baseType)
        {
            source.Url = String.Format(NetLinkDef.HLinkTemplate, address, key.ToLower());
            if (baseType != null)
                source.BaseType = baseType.FullName;
            return source;
        }

        private ApiMember Fill(ApiMember source, Type containedType, String key, String overloadTemplate, Int32 overloadIndex)
        {
            source.Url = overloadIndex == 0
                             ? String.Format(NetLinkDef.HLinkTemplate, address, key.ToLower())
                             : String.Format(overloadTemplate, address, key.ToLower(), overloadIndex);
            // TODO (std_string) : think about BaseType
            if (containedType.BaseType != null)
                source.BaseType = containedType.BaseType.FullName;
            return source;
        }

        private readonly String address;
    }
}