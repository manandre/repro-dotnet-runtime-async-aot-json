using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

var options = new JsonSerializerOptions
{
    TypeInfoResolver = ReproJsonContext.Default
};

using var stream = new MemoryStream("""{"Value":"hello"}"""u8.ToArray());
var converter = new ReproConverter<Payload, Payload>(options);
var payload = await converter.ReadAsync(async: true, stream, CancellationToken.None);

Console.WriteLine(payload?.Value);

sealed class ReproConverter<T, TBase> where T : TBase?
{
    readonly JsonTypeInfo _jsonTypeInfo;

    public ReproConverter(JsonSerializerOptions serializerOptions)
    {
        _jsonTypeInfo = typeof(TBase) != typeof(object) && typeof(T) != typeof(TBase)
            ? (JsonTypeInfo<TBase>)serializerOptions.GetTypeInfo(typeof(TBase))
            : (JsonTypeInfo<T>)serializerOptions.GetTypeInfo(typeof(T));
    }

    public async ValueTask<T?> ReadAsync(bool async, Stream stream, CancellationToken cancellationToken)
    {
        return _jsonTypeInfo switch
        {
            JsonTypeInfo<T> typeInfoOfT => async
                ? await JsonSerializer.DeserializeAsync(stream, typeInfoOfT, cancellationToken).ConfigureAwait(false)
                : JsonSerializer.Deserialize(stream, typeInfoOfT),

            _ => (T?)(async
                ? await JsonSerializer.DeserializeAsync(stream, (JsonTypeInfo<TBase?>)_jsonTypeInfo, cancellationToken).ConfigureAwait(false)
                : JsonSerializer.Deserialize(stream, (JsonTypeInfo<TBase?>)_jsonTypeInfo))
        };
    }
}

sealed class Payload
{
    public string? Value { get; set; }
}

[JsonSerializable(typeof(Payload))]
partial class ReproJsonContext : JsonSerializerContext;
