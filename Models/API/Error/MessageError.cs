namespace MoneyManagerApi.Models.API.Error
{
    public class MessageError : Base.Error
    {
        public MessageError(string title, string message) : base(message, title)
        {
        }
    }
}
