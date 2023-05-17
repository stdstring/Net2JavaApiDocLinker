using System;
using System.Collections.Generic;
using log4net;

namespace UserDocsApiResolver.Library.JavaMetadata
{
    internal class JavaMetadataSearch
    {
        public IList<JavaConstructor> Search(IEnumerable<JavaConstructor> source, IList<ISearchCondition> paramConditions)
        {
            Logger.Debug("Search() enter");
            IList<JavaConstructor> dest = new List<JavaConstructor>();
            foreach (JavaConstructor ctor in source)
            {
                if (CompareParameters(ctor.Parameters, paramConditions))
                    dest.Add(ctor);
            }
            Logger.Debug("Search() exit");
            return dest;
        }

        public IList<JavaMethod> Search(IEnumerable<JavaMethod> source, String name, IList<ISearchCondition> paramConditions)
        {
            Logger.Debug("Search() enter");
            IList<JavaMethod> dest = new List<JavaMethod>();
            foreach (JavaMethod method in source)
            {
                if(method.Name.Equals(name) && CompareParameters(method.Parameters, paramConditions))
                    dest.Add(method);
            }
            Logger.Debug("Search() exit");
            return dest;
        }

        private Boolean CompareParameters(IList<String> parameters, IList<ISearchCondition> paramConditions)
        {
            if (parameters.Count != paramConditions.Count)
                return false;
            for (Int32 index = 0; index < paramConditions.Count; ++index)
            {
                if (!paramConditions[index].IsMatch(parameters[index]))
                    return false;
            }
            return true;
        }

        private static readonly ILog Logger = LogManager.GetLogger("JavaMetadataSearch");
    }
}
