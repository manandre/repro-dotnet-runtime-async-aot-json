# repro-dotnet-runtime-async-aot-json

Minimal standalone NativeAOT repro for the ILC crash seen in:
https://github.com/npgsql/npgsql/actions/runs/24466578450/job/71495217567?pr=6488

> Requires the .NET 11 SDK.

## Repro

```bash
cd ReproAsyncAotJson
dotnet publish -c Release
```

The `runtime-async` feature is enabled via the first-class MSBuild property in the `.csproj`:

```xml
<RuntimeAsync>true</RuntimeAsync>
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

