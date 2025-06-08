using BenchmarkDotNet.Attributes;
using LayerZero.Tools.CoreClasses;
using System.Globalization;

namespace LayerZero.Benchmark.Tools.CoreClasses
{
    public class SygilParserBenchmark
    {
        private const string IntInput = "42";
        private const string EnumInput = "Active";
        private const string DateInput = "2024-06-01";
        private const string JsonInput = """{"Id":123,"Name":"Bench"}""";

        private const string BadInt = "not-an-int";
        private const string BadDate = "2024/99/99";

        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        public enum Status { Inactive, Active, Suspended }

        public class Sample(int Id, string Name);

        [Benchmark]
        public int? Parse_Int_Valid()
            => IntInput.Parse<int>();

        [Benchmark]
        public DateTime? Parse_Date_Valid()
            => DateInput.Parse<DateTime>(culture: _culture);

        [Benchmark]
        public Status? Parse_Enum_Valid()
            => EnumInput.Parse<Status>();

        [Benchmark]
        public Sample? Parse_Object_Valid()
            => JsonInput.Parse<Sample>();

        [Benchmark]
        public int? Parse_Int_Invalid_NonStrict()
            => BadInt.Parse<int>(strict: false);

        [Benchmark]
        public DateTime? Parse_Date_Invalid_NonStrict()
            => BadDate.Parse<DateTime>(strict: false);
    }
}
