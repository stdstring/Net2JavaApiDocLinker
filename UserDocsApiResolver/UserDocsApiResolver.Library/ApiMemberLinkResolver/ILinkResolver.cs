using System;
using System.Collections.Generic;
using System.Reflection;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal interface ILinkResolver
    {
        ApiMember FillType(ApiMember source, Type typeInfo);
        ApiMember FillEvent(ApiMember source, EventInfo eventInfo);
        ApiMember FillField(ApiMember source, FieldInfo fieldInfo);
        ApiMember FillProperty(ApiMember source, List<PropertyInfo> propertyList);
        ApiMember FillConstructor(ApiMember source, List<ConstructorInfo> constructorList);
        ApiMember FillMethod(ApiMember source, List<MethodInfo> methodList);
        ApiMember FillMethodSummary(ApiMember source);
        ApiMember FillConstructorSummary(ApiMember source);
    }
}
