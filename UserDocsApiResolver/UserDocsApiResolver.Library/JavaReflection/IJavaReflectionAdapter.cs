using System;

namespace UserDocsApiResolver.Library.JavaReflection
{
    internal interface IJavaReflectionAdapter : IDisposable
    {
        String[] ProcessRequest(String request);
        void Close();
    }
}
