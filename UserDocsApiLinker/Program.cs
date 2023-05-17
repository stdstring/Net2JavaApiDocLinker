using System;

namespace UserDocsApiLinker
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                AppArguments appArgs = new AppArguments();
                if (!Parser.ParseArgumentsWithUsage(args, appArgs))
                    return 101;

                int errorCode = Process(appArgs);

                Console.Write("Success");
                return errorCode;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return 101;
            }
        }
        private static int Process(AppArguments appArguments)
        {
            ApiMemberLinker apiMemberLinker = new ApiMemberLinker(
                (appArguments.PlatformType == "net") ? ApiMemberPlatform.Net : ApiMemberPlatform.Java,
                appArguments.Url,
                appArguments.AssemblyFile,
                appArguments.XmlFile,
                appArguments.LogFileName,
                appArguments.LogLevel);

            apiMemberLinker.ProcessDocument(appArguments.SourceFile, appArguments.DestFile);

            return 0;
        }
    }
}
