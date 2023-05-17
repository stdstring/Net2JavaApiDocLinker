using System.Collections.Generic;

namespace UserDocsApiResolver.Library
{
    public class ApiMember
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

        public ApiMember Clone()
        {
            ApiMember retMember = new ApiMember();
            retMember.Key = Key;
            retMember.FriendlyName = FriendlyName;
            retMember.Type = Type;
            retMember.Url = Url;
            retMember.BaseType = BaseType;
            foreach(ApiMemberParameter amp in Parameters)
            {
                ApiMemberParameter retParam = amp.Clone();
                retMember.Parameters.Add(retParam);
            }
            return retMember;
        }
    }
}
