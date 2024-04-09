namespace Factories.WebApi.BLL.Database
{
    public class DbConnectException : Exception
    {
        public DbConnectException()
        {
        }

        public DbConnectException(string message)
            : base(message)
        {
        }

        public DbConnectException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
