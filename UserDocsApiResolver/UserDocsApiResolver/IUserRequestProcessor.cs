namespace UserDocsApiResolver
{
    public interface IUserRequestProcessor
    {
        void Process(UserActionKeys key);
    }
}
