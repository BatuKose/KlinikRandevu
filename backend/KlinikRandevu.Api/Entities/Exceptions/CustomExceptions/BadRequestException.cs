namespace Entities.Exeptions.CustomExceptions
{
    public class BadRequestException:Exception
    {
        public BadRequestException(string message):base(message) { }
    }
}
