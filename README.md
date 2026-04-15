# repro-dotnet-runtime-async-aot-json

Minimal standalone NativeAOT repro for the ILC crash seen in:
https://github.com/npgsql/npgsql/actions/runs/24466578450/job/71495217567?pr=6488

> Requires the .NET 11 preview SDK (`11.0.100-preview.3.26207.106` or later). See `global.json`.

## Repro

```bash
cd ReproAsyncAotJson
dotnet publish -c Release
```

The `runtime-async` feature is enabled via preview feature flags in the `.csproj`:

```xml
<EnablePreviewFeatures>true</EnablePreviewFeatures>
<Features>$(Features);runtime-async=on</Features>
```

The failing pattern is the generic async overload called from a generic method:

```csharp
await JsonSerializer.DeserializeAsync(stream, typeInfoOfT, cancellationToken)
```

## Workaround

Replace the generic `DeserializeAsync<T>` calls with the non-generic overload using `JsonTypeInfo`:

```csharp
(T?)await JsonSerializer.DeserializeAsync(stream, (JsonTypeInfo)typeInfoOfT, cancellationToken)
```

