using FluentAssertions;
using LayerZero.Tools.CoreClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LayerZero.Test.Tools.CoreClasses
{
    public class SygilParserTests
    {
        private enum Status
        {
            Inactive,
            Active,
            Suspended
        }

        private class Sample { public int Id { get; init; } }

        // === VALID INPUTS ===

        [Fact]
        public void Parse_String_ReturnsSame()
        {
            "hello".Parse<string>().Should().Be("hello");
        }

        [Fact]
        public void Parse_Int_ReturnsParsedInt()
        {
            "42".Parse<int>().Should().Be(42);
        }

        [Fact]
        public void Parse_Decimal_ReturnsParsedDecimal()
        {
            "12.34".Parse<decimal>().Should().Be(12.34m);
        }

        [Fact]
        public void Parse_Bool_ReturnsParsedBool()
        {
            "true".Parse<bool>().Should().BeTrue();
        }

        [Fact]
        public void Parse_Enum_ReturnsCorrectValue()
        {
            "Active".Parse<Status>().Should().Be(Status.Active);
        }

        [Fact]
        public void Parse_DateTime_WithoutFormat_Works()
        {
            "2024-01-01".Parse<DateTime>().Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public void Parse_DateTime_WithFormat_Works()
        {
            "01-02-2024".Parse<DateTime>(format: "dd-MM-yyyy")
                .Should().Be(new DateTime(2024, 2, 1));
        }

        [Fact]
        public void Parse_DateOnly_WithFormat_Works()
        {
            "15/03/2024".Parse<DateOnly>(format: "dd/MM/yyyy")
                .Should().Be(new DateOnly(2024, 3, 15));
        }

        [Fact]
        public void Parse_TimeOnly_WithFormat_Works()
        {
            "18:45".Parse<TimeOnly>(format: "HH:mm")
                .Should().Be(new TimeOnly(18, 45));
        }

        [Fact]
        public void Parse_ComplexJson_ReturnsDeserializedObject()
        {
            var json = "{\"Id\":123}";
            var result = json.Parse<Sample>();
            result.Should().NotBeNull();
            result!.Id.Should().Be(123);
        }

        // === INVALID INPUTS — EXCEPTION CASES ===

        [Fact]
        public void Parse_InvalidInt_Throws_WhenStrict()
        {
            Action act = () => "notanint".Parse<int>(strict: true, format: "123");
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void Parse_InvalidDate_WithStrict_Throws()
        {
            Action act = () => "31-31-9999".Parse<DateTime>(strict: true, format: "yyyy/mm/dd");
            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void Parse_InvalidEnum_WithStrict_Throws()
        {
            Action act = () => "nope".Parse<Status>(strict: true);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Parse_MalformedJson_WithStrict_Throws()
        {
            Action act = () => "{\"".Parse<Sample>(strict: true, format: "123");
            act.Should().Throw<JsonException>();
        }

        // === INVALID INPUTS — NON-STRICT FALLBACK ===

        [Fact]
        public void Parse_InvalidInt_ReturnsDefault_WhenNotStrict()
        {
            "notanint".Parse<int>(strict: false).Should().Be(default);
        }

        [Fact]
        public void Parse_InvalidEnum_ReturnsDefault_WhenNotStrict()
        {
            "unknown".Parse<Status>(strict: false).Should().Be(default);
        }

        [Fact]
        public void Parse_InvalidDate_ReturnsDefault_WhenNotStrict()
        {
            "notadate".Parse<DateTime>(strict: false).Should().Be(default);
        }

        [Fact]
        public void Parse_MalformedJson_ReturnsDefault_WhenNotStrict()
        {
            var result = "oops".Parse<Sample>(strict: false);
            result.Should().BeNull();
        }
    }
}
