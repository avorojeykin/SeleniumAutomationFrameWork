using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class LoginSettings
    {
        public string HomePageButtonId { get; set; }
        public string EmailBoxId { get; set; }
        public string EmailSubmitButtonId { get; set; }
        public string PasswordBoxId { get; set; }
        public string PasswordSubmitButtonId { get; set; }
        public string StaySignInButtonId { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
        public string LoginButtonCssSelector { get; set; }
        public string UserGmailEmail { get; set; }
    }
}
