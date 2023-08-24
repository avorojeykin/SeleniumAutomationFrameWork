using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class ProcessResponseEmail
    {
        public string Subject { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string Trailer { get; set; }
    }
}
