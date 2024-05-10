using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Services;

public class EmailSevice(EmailClient emailclient, ILogger<EmailSevice> logger) : IEmailSevice
{
    private readonly EmailClient _emailClient = emailclient;
    private readonly ILogger<EmailSevice> _logger = logger;

    public EmailRequest UnPackEmailRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var emailRequest = JsonConvert.DeserializeObject<EmailRequest>(message.Body.ToString());
            if (emailRequest != null)
                return emailRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.UnPackEmailRequest () ::{ex.Message} ");
        }
        return null!;
    }

    public bool SendEmail(EmailRequest emailRequest)
    {
        try
        {
            var result = _emailClient.Send(
                WaitUntil.Completed,
                senderAddress: Environment.GetEnvironmentVariable("SenderAddress"),
                recipientAddress: emailRequest.To,
                subject: emailRequest.Subject,
                htmlContent: emailRequest.HtmlBody,
                plainTextContent: emailRequest.PlainText);

            if (result.HasCompleted)
                return true;

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.SendEmailAsync() ::{ex.Message} ");
        }
        return false;
    }
}
