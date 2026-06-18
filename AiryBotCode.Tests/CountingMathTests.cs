using AiryBotCode.Application.Features.Counting;
using Xunit;

namespace AiryBotCode.Tests
{
    public class CountingMathTests
    {
        private static double Eval(string s)
        {
            var r = CountingMath.Evaluate(s);
            Assert.True(r.Ok, $"expected '{s}' to evaluate, got error: {r.Error}");
            return r.Value;
        }

        [Theory]
        [InlineData("12", 12)]
        [InlineData("6*2", 12)]
        [InlineData("2+3", 5)]
        [InlineData("10-4", 6)]
        [InlineData("10/4", 2.5)]
        [InlineData("10%3", 1)]
        [InlineData("2^10", 1024)]
        [InlineData("(2+3)*4", 20)]
        [InlineData("-5+8", 3)]
        [InlineData("2 + 3 * 4", 14)]      // precedence
        [InlineData("sqrt(144)", 12)]
        [InlineData("abs(-7)", 7)]
        [InlineData("floor(3.9)", 3)]
        [InlineData("ceil(3.1)", 4)]
        [InlineData("round(3.5)", 4)]
        [InlineData("log(1000)", 3)]
        [InlineData("5!", 120)]
        [InlineData("3! + 2", 8)]
        public void Evaluates_expressions(string expr, double expected)
        {
            Assert.Equal(expected, Eval(expr), 6);
        }

        [Fact]
        public void Power_is_right_associative()
        {
            Assert.Equal(512, Eval("2^3^2"), 6);   // 2^(3^2) = 2^9
        }

        [Fact]
        public void Unary_minus_binds_below_power()
        {
            Assert.Equal(-9, Eval("-3^2"), 6);      // -(3^2)
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("2+")]
        [InlineData("(2+3")]
        [InlineData("2+3)")]
        [InlineData("hack()")]
        [InlineData("2 3")]
        [InlineData("/0 bad")]
        public void Rejects_garbage(string expr)
        {
            Assert.False(CountingMath.Evaluate(expr).Ok);
        }

        [Fact]
        public void Division_by_zero_is_not_finite()
        {
            Assert.False(CountingMath.Evaluate("1/0").Ok);
        }

        [Fact]
        public void Factorial_rejects_non_integer_and_huge()
        {
            Assert.False(CountingMath.Evaluate("2.5!").Ok);
            Assert.False(CountingMath.Evaluate("200!").Ok);
        }

        [Fact]
        public void Overlong_input_rejected()
        {
            Assert.False(CountingMath.Evaluate(new string('1', CountingMath.MaxLength + 1)).Ok);
        }
    }
}
