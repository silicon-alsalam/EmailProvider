using Azure.Messaging.ServiceBus;
using EmailProvider.Models;

namespace EmailProvider.Services;

public interface IEmailSevice
{
    bool SendEmail(EmailRequest emailRequest);
    EmailRequest UnPackEmailRequest(ServiceBusReceivedMessage message);
}