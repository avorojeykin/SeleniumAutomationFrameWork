using System;
using System.Collections.Generic;
using System.Text;

namespace ModuleTester.Models
{
    public class EmailSettings
    {
        public bool SendNotification { get; set; }
        public string SmtpHost { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
    }
}
