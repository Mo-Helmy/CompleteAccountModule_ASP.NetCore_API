using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleteAccountModule.Application.Helpers
{
    public class TwilioSettings
    {
        public const string SectionKey = "TwilioSettings";
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string FromPhoneNumber { get; set; }
    }
}
