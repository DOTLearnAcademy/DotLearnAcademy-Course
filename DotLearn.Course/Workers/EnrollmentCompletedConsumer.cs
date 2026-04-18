using Amazon.SQS;
using Amazon.SQS.Model;
using DotLearn.Course.Models.DTOs;
using DotLearn.Course.Repositories;
using System.Text.Json;

namespace DotLearn.Course.Workers;

public class EnrollmentCompletedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAmazonSQS _sqsClient;
    private readonly IConfiguration _config;
    private readonly ILogger<EnrollmentCompletedConsumer> _logger;

    public EnrollmentCompletedConsumer(
        IServiceScopeFactory scopeFactory,
        IAmazonSQS sqsClient,
        IConfiguration config,
        ILogger<EnrollmentCompletedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _sqsClient = sqsClient;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var queueUrl = _config["SQS:EnrollmentCompletedQueue"];
        if (string.IsNullOrWhiteSpace(queueUrl))
        {
            _logger.LogWarning("SQS:EnrollmentCompletedQueue is missing.");
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var response = await _sqsClient.ReceiveMessageAsync(
                    new ReceiveMessageRequest
                    {
                        QueueUrl = queueUrl,
                        MaxNumberOfMessages = 10,
                        WaitTimeSeconds = 20
                    },
                    ct);

                if (response?.Messages == null || response.Messages.Count == 0)
                {
                    await Task.Delay(2000, ct);
                    continue;
                }

                foreach (var message in response.Messages)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(message.Body))
                        {
                            await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, ct);
                            continue;
                        }

                        var evt = JsonSerializer.Deserialize<EnrollmentCompletedEventDto>(
                            message.Body,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (evt == null)
                        {
                            _logger.LogWarning(
                                "Invalid EnrollmentCompleted message: {Body}",
                                message.Body);
                            continue;
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<ICourseRepository>();
                        await repo.IncrementEnrollmentCountAsync(evt.CourseId);

                        await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, ct);
                        _logger.LogInformation(
                            "Updated EnrollmentCount for CourseId {CourseId}",
                            evt.CourseId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Failed processing EnrollmentCompleted message {Id}",
                            message.MessageId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SQS polling error in Course service");
                await Task.Delay(5000, ct);
            }
        }
    }
}
