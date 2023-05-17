namespace UserDocsApiResolver.Library.Utils
{
    public delegate TResult Func<TResult>();
    public delegate TResult Func<TArg, TResult>(TArg arg);

    public delegate void Action<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);
}
