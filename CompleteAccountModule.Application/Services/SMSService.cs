using CompleteAccountModule.Application.Dtos.SMSDtos;
using CompleteAccountModule.Application.Helpers;
using CompleteAccountModule.Application.Services.Contract;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CompleteAccountModule.Application.Services
{
    public class SMSService : ISMSService
    {
        private readonly TwilioSettings _options;

        public SMSService(IOptions<TwilioSettings> options)
        {
            this._options = options.Value;
        }

        public MessageResource Send(SMSDto sms)
        {
            TwilioClient.Init(_options.AccountSID, _options.AuthToken);
            var result = MessageResource.Create(
                from: new Twilio.Types.PhoneNumber(_options.FromPhoneNumber),
                to: sms.PhoneNumber,
                body: sms.Body
                );

            return result;
        }
    }
}
