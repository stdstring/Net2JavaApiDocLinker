using System;

namespace UserDocsApiLinker.JavaReflection
{
    internal interface IJavaReflectionAdapter : IDisposable
    {
        String[] ProcessRequest(String request);
        void Close();
    }
}
