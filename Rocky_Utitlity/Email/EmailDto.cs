using System.Collections.Generic;

namespace Rocky_Utility.Email
{
    public class EmailDto
    {
        public string Addresses { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Cc { get; set; }
        public IEnumerable<string> Attachments { get; set; }
    }
}
