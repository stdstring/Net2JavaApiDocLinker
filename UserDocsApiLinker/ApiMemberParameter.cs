using System;

namespace UserDocsApiLinker
{
    /// <summary>
    /// Class describes parameter of ApiMember.
    /// </summary>
    public class ApiMemberParameter : ICloneable
    {
        public ApiMemberParameter(string key)
        {
            Key = key;
        }
        public string Key { get; set; }

        public object Clone()
        {
            ApiMemberParameter retParam = new ApiMemberParameter(this.Key);
            return retParam;
        }
    }
}
