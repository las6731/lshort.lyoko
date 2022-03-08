using System.Text.Json;
using System.Text.Json.Serialization;

namespace LShort.Lyoko.Messaging.Abstractions;

[Serializable]
public class EventMessage
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("correlationId")]
    public Guid CorrelationId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonPropertyName("payload")]
    public string Payload { get; init; }

    [JsonPropertyName("aggregateVersion")]
    public int AggregateVersion { get; init; }

    [JsonPropertyName("sequenceNumber")]
    public int SequenceNumber { get; init; }

    [JsonPropertyName("schemaVersion")]
    public Version SchemaVersion { get; init; }

    public T GetPayload<T>()
    {
        return JsonSerializer.Deserialize<T>(Payload) ?? throw new InvalidOperationException($"Event payload does not match {typeof(T)}");
    }
}