using System;
using System.Collections.Generic;
using System.Reflection;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal class NetLinkResolver : ILinkResolver
    {
        public NetLinkResolver(String baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public ApiMember FillType(ApiMember source, Type typeInfo)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            return Fill(source, source.Key, typeInfo.BaseType);
        }

        public ApiMember FillEvent(ApiMember source, EventInfo eventInfo)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            return Fill(source, source.Key, eventInfo.ReflectedType.BaseType);
        }

        public ApiMember FillField(ApiMember source, FieldInfo fieldInfo)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            Type reflectedType = fieldInfo.ReflectedType;
            return Fill(source, reflectedType.FullName, reflectedType.BaseType);
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
            return Fill(source, property.ReflectedType, source.Key, NetLinkDef.OverloadPropertyHLinkTemplate, propertyList.Count == 1 ? 0 : index + 1);
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
            String key = constructor.ReflectedType.FullName;
            return Fill(source, constructor.ReflectedType, key, NetLinkDef.OverloadCtorHLinkTemplate, constructorList.Count == 1 ? 0 : index + 1);
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
            return Fill(source, method.ReflectedType, source.Key, NetLinkDef.OverloadMethodHLinkTemplate, methodList.Count == 1 ? 0 : index + 1);
        }

        public ApiMember FillMethodSummary(ApiMember source)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            source.Url = String.Format(NetLinkDef.MethodSummaryHLinkTemplate, _baseAddress, source.Key.ToLower());
            return source;
        }

        public ApiMember FillConstructorSummary(ApiMember source)
        {
            Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(source));
            String key = ApiMemberUtils.GetContainedType(source);
            source.Url = String.Format(NetLinkDef.CtorSummaryHLinkTemplate, _baseAddress, key);
            return source;
        }

        private ApiMember Fill(ApiMember source, String key, Type baseType)
        {
            source.Url = String.Format(NetLinkDef.HLinkTemplate, _baseAddress, key.ToLower());
            if (baseType != null)
                source.BaseType = baseType.FullName;
            return source;
        }

        private ApiMember Fill(ApiMember source, Type containedType, String key, String overloadTemplate, Int32 overloadIndex)
        {
            source.Url = overloadIndex == 0
                             ? String.Format(NetLinkDef.HLinkTemplate, _baseAddress, key.ToLower())
                             : String.Format(overloadTemplate, _baseAddress, key.ToLower(), overloadIndex);
            // TODO (std_string) : think about BaseType
            if (containedType.BaseType != null)
                source.BaseType = containedType.BaseType.FullName;
            return source;
        }

        private readonly String _baseAddress;

        private static readonly ILog Logger = LogManager.GetLogger("NetLinkResolver");
    }
}