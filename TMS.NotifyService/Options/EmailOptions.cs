namespace TMS.NotifyService.Options
{
    public class EmailOptions
    {
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = "";
        public string SmtpPassword { get; set; } = "";
        public string FromEmail { get; set; } = "";
    }
}
