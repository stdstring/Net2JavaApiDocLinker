using System;

namespace UserDocsApiResolver.Library
{
    public class ApiMemberParameter
    {
        public ApiMemberParameter(String key)
        {
            Key = key;
        }
        public String Key { get; set; }

        public ApiMemberParameter Clone()
        {
            return new ApiMemberParameter(Key);
        }
    }
}
