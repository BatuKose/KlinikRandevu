namespace Entities.Data_Transfer_Objects.Sms
{
    public class SmsConfigListeDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string TokenMaskeli { get; set; }
        public string FromNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
