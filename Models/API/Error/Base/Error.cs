namespace MoneyManagerApi.Models.API.Error.Base
{
    public abstract class Error
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public Error(string description, string title = "Error")
        {
            Title = title;
            Description = description;
        }
    }
}
