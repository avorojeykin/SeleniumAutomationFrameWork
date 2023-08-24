using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimProcessor.Models
{
    public class DialogPopup
    {
        public string Id { get; set; }
        public string TitleId { get; set; }
        public string ErrorText { get; set; }
        public string ConfirmationText { get; set; }
        public string ButtonOkId { get; set; }
        public string ButtonYesId { get; set; }
        public string ButtonNoId { get; set; }
        public string ButtonCancelId { get; set; }
    }
}
