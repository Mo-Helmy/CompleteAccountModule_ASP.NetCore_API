using CompleteAccountModule.Application.Dtos.MailDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleteAccountModule.Application.Dtos.SMSDtos
{
    public class SMSDto
    {
        public string PhoneNumber { get; set; }
        public string Body { get; set; }
    }
}
