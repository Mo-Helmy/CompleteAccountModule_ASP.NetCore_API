using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleteAccountModule.Application.Helpers
{
    public class MailSettings
    {
        public const string SectionKey = "MailSettings";
        public string? Mail { get; set; }
        public string? Bcc { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Username { get; set; }
        public string? Host { get; set; }
        public string? PickupPath { get; set; }
        public bool UseSsl { get; set; }
        public bool TestMode { get; set; }
        public string TestEmail { get; set; }
        public int Port { get; set; }
    }
}
