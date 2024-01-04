namespace Rocky.Utils.Email
{
    public class EmailSettings
    {
        public string DefaultSender { get; set; }
        public SmtpInfo Smtp { get; set; }
        public bool SendEmail { get; set; }
        public string EmailFolder { get; set; }
        public string EmailAdmin { get; set; }
    }

    public class SmtpInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseTls { get; set; }
    }
}
