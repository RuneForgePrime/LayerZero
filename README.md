# SygilParser

**SygilParser** is a lightweight, guard-backed type parser for .NET that unifies the handling of primitives, enums, datetime formats, and structured data.  
Designed for systems where input cannot be trusted and parsing must be intentional.

Part of the `LayerZero.Tools` suite — focused on foundational, intentional .NET tooling.

---

## 🔍 Features

- `.Parse<T>()` extension method for unified type parsing
- Supports:
  - `string`, `int`, `long`, `decimal`, `bool`
  - `DateTime`, `DateOnly`, `TimeOnly`
  - `enum` types (case-insensitive)
  - Complex objects via `JsonSerializer.Deserialize<T>()`
- Optional `strict` mode — throws on failure
- Optional `format` and `CultureInfo` for date/time types
- Integrates with `RuneGuard` extensions for input validation
- Minimal dependencies and zero allocations on valid fast-paths

---

## 🧪 Example

```csharp
using LayerZero.Tools.Sigil;

"42".Parse<int>();                             // 42
"2024-06-01".Parse<DateTime>();                // DateTime object
"ACTIVE".Parse<Status>();                      // Enum parsed case-insensitive
"{"id":1}".Parse<MyObject>();                // JSON to object
"bad".Parse<int>(strict: false);               // returns default(int)
"invalid".Parse<DayOfWeek>(strict: true);      // throws
"15/03/2024".Parse<DateOnly>(format: "dd/MM/yyyy");
```

---

## 📦 Installation

This is a standalone library, compatible with `.NET 6` and `.NET 8`.

Planned NuGet package:
```bash
dotnet add package LayerZero.Tools.Sigil
```

Until then:
- Clone or reference the `SygilParser.cs` file directly
- Use within internal toolsets, APIs, or console automation

---

## ⚡ Performance

Benchmark summary from `BenchmarkDotNet`:

| Type             | Mean      | Allocations |
|------------------|-----------|-------------|
| `int.Parse`      | ~9 ns     | 0 B         |
| `DateTime.Parse` | ~65 ns    | 0 B         |
| `enum.Parse`     | ~30 ns    | 0 B         |
| `Json → object`  | ~250 ns   | ~96 B       |
| Invalid fallback | 10–30x slower (try/catch triggered) |

Use `strict: false` to avoid exceptions during batch parse.

---

## ⚔️ Why SygilParser Exists

In .NET, parsing is fragmented:
- `int.Parse`, `bool.TryParse`, `Enum.Parse`, `DateTime.ParseExact`, and `JsonSerializer.Deserialize`

Each uses different semantics, error handling, and cultural assumptions.

**SygilParser** brings type parsing under a single, guarded construct —  
for developers who want clarity, predictability, and fallback options by design.

---

## 🧾 License

MIT

---

## 🔨 Author

Crafted by **[RuneForgePrime]**  
Part of the `LayerZero.Tools` initiative.

---

> “The shape of a system begins with the clarity of a single stroke.”