namespace Entities.Data_Transfer_Objects.Email
{
    public class UpdateEmailConfigDto
    {
        public int Id { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string? Password { get; set; }
    }
}
