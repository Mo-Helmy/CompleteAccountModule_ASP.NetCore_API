using CompleteAccountModule.Application.Dtos.SMSDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace CompleteAccountModule.Application.Services.Contract
{
    public interface ISMSService
    {
        MessageResource Send(SMSDto sms);
    }
}
