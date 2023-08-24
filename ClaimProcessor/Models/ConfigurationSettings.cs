using Common.Models;

namespace ClaimProcessor.Models
{
    public class ConfigurationSettings
    {
        public string HomepageLoadClass { get; set; }       
        public bool PopupClick { get; set; }
        public DialogPopup DialogWindow { get; set; }
    }
}
