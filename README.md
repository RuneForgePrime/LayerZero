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

> “The shape of a system begins with the clarity of a single stroke.”

## 🧰 SafeLinQ

**SafeLinQ** provides defensive, expressive alternatives to common LINQ operations in .NET, guarding against nulls, empty sources, and missing elements.

Part of the `LayerZero.Tools` suite — built for resilience and clarity in everyday collection usage.

---


### 📦 Installation

Planned NuGet package:

```bash
dotnet add package LayerZero.Tools
```
---


### 🔍 Features

- `ElementAtSafe()` with index bounds validation
- `ToDictionarySafe()` with optional overwrite
- `DistinctSafe()` with optional custom comparer
- `FallbackIfEmpty()` for graceful default population

Built to prevent:

- `ArgumentNullException`
- `IndexOutOfRangeException`
- Silent LINQ misbehavior (e.g., `.First()` on empty sources)

---

### 🧪 Example

```csharp
using LayerZero.Tools.Linq;

var list = new[] { "alpha", "beta", "gamma" };

list.ElementAtSafe(1);                     // "beta"
list.ElementAtSafe(10);                    // null (safe fallback)

list.ToDictionarySafe(x => x.Length, false);
// { 5: "alpha", 4: "beta" } — keeps first occurrence

list.DistinctSafe();                       // distinct items
list.FallbackIfEmpty(() => "default");    // yields "default" if empty
```

---


### ⚔️ Why SafeLinQ Exists

In standard LINQ, silent failures and non-obvious exceptions are common:

```csharp
source.First();        // throws on empty
source.ElementAt(99);  // throws on out-of-range
source.ToDictionary(); // throws on duplicate keys
```

**SafeLinQ** addresses these issues by:

- Adding precondition checks
- Allowing graceful degradation
- Preserving caller intent

---
## ⚙️ GeasMaster

**GeasMaster** is a synchronous execution harness for asynchronous .NET tasks. Useful when async code must run in blocking contexts — safely, consistently, and without `ConfigureAwait` chaos.

Part of the `LayerZero.Tools` suite — focused on deterministic control and guard-backed execution.

---

### 🔍 Features

- `RunSync<T>()` to execute async functions synchronously and retrieve result
- Overloads for both `Func<Task<T>>` and `Func<CancellationToken, Task<T>>`
- Includes void-returning task support
- Based on `TaskFactory` to avoid deadlocks in UI or legacy contexts

---

### 🧪 Example

```csharp
using LayerZero.Tools.Runtime;

var result = GeasMaster.RunSync(() => MyAsyncMethod());

GeasMaster.RunSync(async ct =>
{
    await DoSomethingAsync(ct);
});
```

---

### ⚔️ Why GeasMaster Exists

Async/await is ideal — but legacy codebases, ASP.NET constructors, or sync APIs occasionally require a deterministic, blocking call.

**GeasMaster** gives you this capability safely:

- No deadlocks from `Result` or `.Wait()`
- Controlled sync execution via `TaskFactory`
- Minimal ceremony, full control

---

> “A task without form is a thread unbound.”


---


## 🌐 LeyLineCaller

**LeyLineCaller** is a minimalistic HTTP client wrapper with optional authentication headers, built-in sync/async handling, and typed parsing — ideal for internal APIs or automation tools.

---

### 🔍 Features

- Built-in support for `GET` and `POST` requests
- Sync and async execution support (via `GeasMaster`)
- Optional authentication via `SigilBinder`
- Smart response parsing with support for:
  - JSON (via `SygilParser`)
  - Binary streams (as `EchoShard`)
  - Text/plain and fallback formats

---

### 🧪 Example

```csharp
var caller = new LeyLineCaller(myTokenBinder);

var result = caller.Get<MyDto>("https://api.myapp.com/data");

await caller.PostAsync<MyResponse>("/submit", new { id = 42 });
```


### ⚔️ Why LeyLineCaller Exists

- HttpClient is powerful but verbose
- Most HTTP wrappers are overkill or opinionated
- Sync scenarios are needed in CLI tools or scripting environments

**LeyLineCaller** balances minimalism and power:

- Transparent async/sync bridging
- Typed deserialization with safety
- Built for internal systems where speed matters

---

### 💾 License

MIT

---

### 🔨 Author

Crafted by **[RuneForgePrime]**  
Part of the `LayerZero.Tools` initiative.

---

> “The line is open. Let the ley energies flow.”

