# repro-dotnet-runtime-async-aot-json

Minimal standalone NativeAOT repro for the ILC crash seen in:
https://github.com/npgsql/npgsql/actions/runs/24466578450/job/71495217567?pr=6488

> Requires the .NET 11 preview SDK (`11.0.100-preview.3.26207.106` or later). See `global.json`.

## Repro

```bash
cd ReproAsyncAotJson
dotnet publish -c Release
```

The `runtime-async` feature is enabled by default via the `RuntimeAsync` MSBuild property in the `.csproj`:

```xml
<!-- Set to false to verify the issue only occurs with runtime-async enabled -->
<RuntimeAsync Condition="'$(RuntimeAsync)' == ''">true</RuntimeAsync>
```

To confirm the crash is **only** triggered by `runtime-async`, pass `RuntimeAsync=false`:

```bash
dotnet publish -c Release -p:RuntimeAsync=false   # succeeds
dotnet publish -c Release -p:RuntimeAsync=true    # fails (default)
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

