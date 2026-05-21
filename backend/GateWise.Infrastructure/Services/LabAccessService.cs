using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GateWise.Core.DTOs;
using GateWise.Core.Enums;
using GateWise.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;

public class LabAccessService : ILabAccessService
{
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly string _privateKey;
    private readonly string _espPublicKey;
    private readonly IHubContext<AccessConfirmationHub> _hub;

    public LabAccessService(
        IAccessLogRepository accessLogRepository,
        IUserRepository userRepository,
        IMqttClient mqttClient,
        MqttClientOptions mqttOptions,
        IConfiguration config, 
        IHubContext<AccessConfirmationHub> hub)
    {
        _accessLogRepository = accessLogRepository;
        _userRepository = userRepository;
        _mqttClient = mqttClient;
        _mqttOptions = mqttOptions;
        _hub = hub;


        var privateKeyPath = config["CryptoKeys:PrivateKeyPath"];
        var espPublicKeyPath = config["CryptoKeys:EspPublicKeyPath"];

        _privateKey = File.ReadAllText(privateKeyPath);
        _espPublicKey = File.ReadAllText(espPublicKeyPath);

        Console.WriteLine($"[Startup] Private key loaded.");
        Console.WriteLine($"[Startup] ESP public key path: {espPublicKeyPath}");
    }

    public async Task<string> RequestLabAccessAsync(string userId, int labId, AccessLogCreateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var devicePublicKey = user?.DevicePublicKeyPem;

        if (string.IsNullOrWhiteSpace(devicePublicKey))
            throw new InvalidOperationException("Device public key not registered.");

        var message = $"open:{dto.Timestamp}";
        var isValid = VerifyWithPublicKey(message, dto.Signature, devicePublicKey);
        if (!isValid)
            throw new SecurityException("Invalid signature from client device.");

        if (!_mqttClient.IsConnected)
        {
            var tcpOptions = _mqttOptions.ChannelOptions as MQTTnet.Client.MqttClientTcpOptions;
            var host = tcpOptions?.Server ?? "undefined";
            var port = tcpOptions?.Port ?? 1883;

            Console.WriteLine("[MQTT] Attempting to connect to the broker...");
            Console.WriteLine($"[MQTT] Host: {host}");
            Console.WriteLine($"[MQTT] Port: {port}");

            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions);
                Console.WriteLine("[MQTT] Successfully connected to broker.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MQTT] Failed to connect to broker: {ex.Message}");
                throw;
            }
        }

        var commandId = Guid.NewGuid().ToString();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var messageToSign = $"open:{timestamp}";
        var signature = SignWithPrivateKey(messageToSign, _privateKey);

        var mqttPayload = new
        {
            command = "open",
            commandId,
            timestamp,
            signature
        };

        var payloadJson = JsonSerializer.Serialize(mqttPayload);
        var log = new AccessLog
        {
            CommandId = commandId,
            UserId = userId,
            LabId = labId,
            RawRequestJson = payloadJson,
            Status = AccessStatus.PENDING_CONFIRMATION
        };

        await _accessLogRepository.CreateAsync(log);
        await _accessLogRepository.SaveChangesAsync();

        var messageMQQT = new MqttApplicationMessageBuilder()
            .WithTopic("command/open-lock")
            .WithPayload(payloadJson)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _mqttClient.PublishAsync(messageMQQT);
        return payloadJson;
    }

    public async Task<bool> ConfirmAccessAsync(AccessLogConfirmDto dto)
    {
        var message = $"confirmed:{dto.CommandId}:{dto.Timestamp}";
        var isValid = VerifyWithPublicKey(message, dto.Signature, _espPublicKey);
        if (!isValid)
        {
            Console.WriteLine("[Security] Invalid signature in confirmation sent by ESP.");
            return false;
        }

        var log = await _accessLogRepository.GetByCommandIdAsync(dto.CommandId);
        if (log == null) return false;

        try
        {
            log.Status = AccessStatus.GRANTED;
            log.ConfirmedAt = DateTime.UtcNow;
            log.RawConfirmationJson = JsonSerializer.Serialize(dto);

            await _accessLogRepository.UpdateAsync(log);
            await _accessLogRepository.SaveChangesAsync();

            Console.WriteLine($"[Access] ESP reported lock status 'OPENED' for user {log.UserId} (CommandId: {log.CommandId})");

            await _hub.Clients.All.SendAsync("access_result", new
            {
                commandId = log.CommandId,
                status = "opened"
            });

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Access] Failed to process lock status for CommandId={dto.CommandId}: {ex.Message}");

            log.Status = AccessStatus.NO_CONFIRMATION;
            log.ConfirmedAt = DateTime.UtcNow;
            log.RawConfirmationJson = JsonSerializer.Serialize(dto);

            await _accessLogRepository.UpdateAsync(log);
            await _accessLogRepository.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("access_result", new
            {
                commandId = dto.CommandId,
                status = "failed"
            });

            return false;
        }
    }

    private string SignWithPrivateKey(string message, string privateKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem.ToCharArray());
        var data = Encoding.UTF8.GetBytes(message);
        var signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signature);
    }

    private bool VerifyWithPublicKey(string message, string signatureBase64, string publicKeyPem)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());
            var data = Encoding.UTF8.GetBytes(message);
            var signature = Convert.FromBase64String(signatureBase64);
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }
}
