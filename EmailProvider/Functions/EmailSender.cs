using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Models;
using EmailProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Functions;

public class EmailSender(ILogger<EmailSender> logger, IEmailSevice emailSevice)
{
    private readonly ILogger<EmailSender> _logger = logger;
    private readonly IEmailSevice _emailSevice = emailSevice;


    [Function(nameof(EmailSender))]
    public async Task Run(
        [ServiceBusTrigger("email_request", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            var emailRequest = _emailSevice. UnPackEmailRequest(message);
            if (emailRequest != null && !string.IsNullOrEmpty(emailRequest.To))
            {
                if (_emailSevice. SendEmail(emailRequest))
                {
                    await messageActions.CompleteMessageAsync(message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.Run ::{ex.Message} ");
        }



        //_logger.LogInformation("Message ID: {id}", message.MessageId);
        //_logger.LogInformation("Message Body: {body}", message.Body);
        //_logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        //// Complete the message
        //await messageActions.CompleteMessageAsync(message);
    }


}
