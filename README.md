# repro-dotnet-runtime-async-aot-json

Minimal standalone NativeAOT repro for the ILC crash seen in:
https://github.com/npgsql/npgsql/actions/runs/24466578450/job/71495217567?pr=6488

> `RuntimeAsync=on` requires an ILC/toolchain version that supports `--runtime-async`.

## Repro

```bash
cd /home/runner/work/repro-dotnet-runtime-async-aot-json/repro-dotnet-runtime-async-aot-json/ReproAsyncAotJson
dotnet publish -c Release -p:RuntimeAsync=on
```

This project passes `--runtime-async=on` to ILC when `RuntimeAsync` is set:

```xml
<IlcArg Include="--runtime-async=$(RuntimeAsync)" />
```

The failing pattern is the generic async overload:

```csharp
await JsonSerializer.DeserializeAsync(stream, typeInfoOfT, cancellationToken)
```

## Workaround

Replace the generic `DeserializeAsync<T>` calls with the non-generic overload using `JsonTypeInfo`:

```csharp
(T?)await JsonSerializer.DeserializeAsync(stream, (JsonTypeInfo)typeInfoOfT, cancellationToken)
```
