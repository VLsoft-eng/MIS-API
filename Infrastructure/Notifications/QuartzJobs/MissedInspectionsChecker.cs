using System.Text.Json;
using Application.Abstractions.Repository;
using Quartz;
using RabbitMQ.Client;

namespace Infrastructure.Notifications.QuartzJobs;

public class MissedInspectionsChecker(IInspectionRepository inspectionRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await CheckMissedInspections();
    }

    private async Task CheckMissedInspections()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        Console.WriteLine("0");

        channel.QueueDeclare(queue: "processing_message",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var inspections = await inspectionRepository.GetMissedInspections();
        foreach (var inspection  in inspections)
        {
            var email = inspection.doctor.email;
            var patientName = inspection.patient.name;
            var doctorName = inspection.doctor.name;
            var inspectionDate = inspection.date;

            var message =
                $"Здравствуйте, {doctorName}!\nСообщаем, что пациент {patientName} пропустил осмотр, запланированный на {inspectionDate}.";

            var notification = new Notification(email, message, inspection.id);
            
            var body = JsonSerializer.SerializeToUtf8Bytes(notification);
            channel.BasicPublish(exchange: string.Empty,
                routingKey: "processing_message",
                basicProperties: null,
                body: body);
            await inspectionRepository.UpdateIsNotified(inspection.id, true);
        }
    }
}