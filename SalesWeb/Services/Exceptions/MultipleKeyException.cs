namespace SalesWeb.Services.Exceptions
{
    public class MultipleKeyException : ApplicationException
    {
        public MultipleKeyException(string? message) : base(message)
        {
        }
    }
}
