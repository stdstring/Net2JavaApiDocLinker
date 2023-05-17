namespace UserDocsApiLinker
{
    class AppArguments
    {
        [Argument(ArgumentType.Required, LongName = "assemblyfile", ShortName = "af", HelpText = "Assembly file.")]
        public string AssemblyFile;

        [Argument(ArgumentType.Required, LongName = "sourcefile", ShortName = "sf", HelpText = "Source file.")]
        public string SourceFile;
        
        [Argument(ArgumentType.Required, LongName = "destfile", ShortName = "df", HelpText = "Dest file.")]
        public string DestFile;

        [Argument(ArgumentType.Required, ShortName = "xml", LongName = "xmlfile", HelpText = "file name of the IntelliSense XML xml file")]
        public string XmlFile;

        [Argument(ArgumentType.AtMostOnce, ShortName = "pt", LongName = "platformtype", HelpText = "platform type (net, otherwise - java)", DefaultValue = "net")]
        public string PlatformType;

        [Argument(ArgumentType.Required, ShortName = "url", HelpText = "url for creating hyperlinks, can be absolute or relative.")]
        public string Url;

        [Argument(ArgumentType.AtMostOnce, ShortName = "logf", LongName = "logfile", HelpText = "File name of the log.", DefaultValue = "UserDocsApiLinkerLogFile.txt")]
        public string LogFileName;

        [Argument(ArgumentType.AtMostOnce, ShortName = "logl", LongName = "loglevel", HelpText = "Level of logging.", DefaultValue = 0)]
        public int LogLevel;
    }
}
