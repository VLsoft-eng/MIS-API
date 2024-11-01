using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Application.Abstractions.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Notifications;

public class EmailSender
{
    private readonly string _senderEmailAddress = "info@mis.ru";

    public EmailSender()
    {
        ConfigureEmailSender();
    }

    private void ConfigureEmailSender()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var notification = JsonSerializer.Deserialize<Notification>(message);
            await SendEmail(notification);
        };
        
        channel.BasicConsume(queue: "processing_message", 
            autoAck: true, 
            consumer: consumer);
    }

    private async Task SendEmail(Notification notification)
    {
        using var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_senderEmailAddress);
        mailMessage.To.Add(notification.email);
        mailMessage.Subject = "Пропущенный осмотр";
        mailMessage.Body = notification.message;

        using var smtpClient = new SmtpClient("localhost", 1025)
        {
            EnableSsl = false
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}