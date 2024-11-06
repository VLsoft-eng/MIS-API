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
    private IConnection _connection;
    private IModel _channel;

    public EmailSender()
    {
        ConfigureEmailSender();
    }

    private void ConfigureEmailSender()
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "user", Password = "password"};
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var notification = JsonSerializer.Deserialize<Notification>(message);
            await SendEmail(notification);
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };
        
        _channel.QueueDeclare(queue: "processing_message",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        _channel.BasicConsume(queue: "processing_message", 
            autoAck: false, 
            consumer: consumer);
    }

    private async Task SendEmail(Notification notification)
    {
        using var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_senderEmailAddress);
        mailMessage.To.Add(notification.email);
        mailMessage.Subject = "Пропущенный осмотр";
        mailMessage.Body = notification.message;

        using var smtpClient = new SmtpClient("maildev", 1025)
        {
            EnableSsl = false
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}