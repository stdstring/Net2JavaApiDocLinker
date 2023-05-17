using System;
using System.Collections.Generic;

namespace UserDocsApiLinker
{
    /// <summary>
    /// Describes an API members with parameter collection.
    /// </summary>
    public class ApiMember : ICloneable
    {
        public ApiMember()
        {
            Parameters = new List<ApiMemberParameter>();
        }
        public string Key { get; set; }
        public string FriendlyName { get; set; }
        public string Url { get; set; }
        public string BaseType { get; set; }
        public ApiMemberType Type { get; set; }
        public List<ApiMemberParameter> Parameters { get; set; }
        
        public object Clone()
        {
            ApiMember retMember = new ApiMember();
            retMember.Key = Key;
            retMember.FriendlyName = FriendlyName;
            retMember.Type = Type;
            retMember.Url = Url;
            retMember.BaseType = BaseType;
            foreach(ApiMemberParameter amp in Parameters)
            {
                ApiMemberParameter retParam = (ApiMemberParameter)amp.Clone();
                retMember.Parameters.Add(retParam);
            }
            return retMember;
        }
    }
}
