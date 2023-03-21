namespace MoneyManagerApi.Models.API.Error
{
    public class NoExistsError : Base.Error
    {
        public NoExistsError() : base("Item doesn't exists")
        {
        }
    }
}
