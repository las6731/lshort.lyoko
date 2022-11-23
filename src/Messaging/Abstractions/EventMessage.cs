using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LShort.Lyoko.Messaging.Abstractions;

[Serializable]
[DataContract]
public class EventMessage
{
    [JsonPropertyName("id")]
    [DataMember(Name = "id")]
    public Guid Id { get; init; }

    [JsonPropertyName("correlationId")]
    [DataMember(Name = "correlationId")]
    public Guid CorrelationId { get; init; }

    [JsonPropertyName("aggregateId")]
    [DataMember(Name = "aggregateId")]
    public Guid AggregateId { get; init; }

    [JsonPropertyName("name")]
    [DataMember(Name = "name")]
    public string Name { get; init; }

    [JsonPropertyName("timestamp")]
    [DataMember(Name = "timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonPropertyName("payload")]
    [DataMember(Name = "payload")]
    public string Payload { get; init; }

    [JsonPropertyName("aggregateVersion")]
    [DataMember(Name = "aggregateVersion")]
    public int AggregateVersion { get; init; }

    [JsonPropertyName("sequenceNumber")]
    [DataMember(Name = "sequenceNumber")]
    public int SequenceNumber { get; init; }

    [JsonPropertyName("schemaVersion")]
    [DataMember(Name = "schemaVersion")]
    public Version SchemaVersion { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventMessage"/> class.
    /// </summary>
    public EventMessage()
    {
        this.Id = Guid.Empty;
        this.CorrelationId = Guid.Empty;
        this.AggregateId = Guid.Empty;
        this.Name = string.Empty;
        this.Payload = string.Empty;
        this.Timestamp = DateTime.MinValue;
        this.SchemaVersion = new Version("1.0");
    }

    public T GetPayload<T>()
    {
        var json = JsonSerializer.Serialize(this.Payload);
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException($"Event payload does not match {typeof(T)}");
    }
}