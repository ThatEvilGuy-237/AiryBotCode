using System.Globalization;

namespace AiryBotCode.Application.Features.Counting
{
    /// <summary>
    /// A small, dependency-free arithmetic evaluator for counting answers — so a
    /// player can post "6*2" or "sqrt(144)" instead of "12". It's a shunting-yard
    /// parser (no eval, no code execution), so it's safe by construction.
    ///
    /// Supports: + - * / % ^, unary minus, parentheses, decimals, postfix factorial
    /// (!), and the functions sqrt abs floor ceil round log(=log10) ln. Pure and
    /// fully unit-tested.
    /// </summary>
    public static class CountingMath
    {
        public const int MaxLength = 256;
        private const double FactorialMax = 170; // 171! overflows double

        public readonly record struct Result(bool Ok, double Value, string? Error)
        {
            public static Result Fail(string e) => new(false, 0, e);
            public static Result From(double v) => new(true, v, null);
        }

        public static Result Evaluate(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Result.Fail("empty");
            if (input.Length > MaxLength) return Result.Fail("too long");
            try
            {
                var tokens = Tokenize(input);
                if (tokens.Count == 0) return Result.Fail("no tokens");
                var rpn = ToRpn(tokens);
                var value = EvalRpn(rpn);
                if (double.IsNaN(value) || double.IsInfinity(value)) return Result.Fail("not finite");
                return Result.From(value);
            }
            catch (Exception ex) { return Result.Fail(ex.Message); }
        }

        private enum Kind { Number, Func, Op, LParen, RParen, Bang }
        private readonly record struct Tok(Kind Kind, double Num, string Text);

        private static readonly HashSet<string> Functions = new(StringComparer.OrdinalIgnoreCase)
        { "sqrt", "abs", "floor", "ceil", "round", "log", "ln" };

        private static List<Tok> Tokenize(string s)
        {
            var tokens = new List<Tok>();
            int i = 0;
            while (i < s.Length)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (char.IsDigit(c) || c == '.')
                {
                    int start = i;
                    while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
                    var text = s[start..i];
                    if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var n))
                        throw new FormatException($"bad number '{text}'");
                    tokens.Add(new Tok(Kind.Number, n, text));
                    continue;
                }

                if (char.IsLetter(c))
                {
                    int start = i;
                    while (i < s.Length && char.IsLetter(s[i])) i++;
                    var name = s[start..i];
                    if (!Functions.Contains(name)) throw new FormatException($"unknown function '{name}'");
                    tokens.Add(new Tok(Kind.Func, 0, name.ToLowerInvariant()));
                    continue;
                }

                switch (c)
                {
                    case '(': tokens.Add(new Tok(Kind.LParen, 0, "(")); break;
                    case ')': tokens.Add(new Tok(Kind.RParen, 0, ")")); break;
                    case '!': tokens.Add(new Tok(Kind.Bang, 0, "!")); break;
                    case '+': case '-': case '*': case '/': case '%': case '^':
                        // Unary minus/plus: at the start, or after another operator or '('.
                        var prev = tokens.Count > 0 ? tokens[^1] : default;
                        bool unaryPos = tokens.Count == 0 || prev.Kind == Kind.Op || prev.Kind == Kind.LParen;
                        if (c == '-' && unaryPos) tokens.Add(new Tok(Kind.Op, 0, "u-"));
                        else if (c == '+' && unaryPos) { /* unary plus: no-op */ }
                        else tokens.Add(new Tok(Kind.Op, 0, c.ToString()));
                        break;
                    default: throw new FormatException($"unexpected '{c}'");
                }
                i++;
            }
            return tokens;
        }

        // Operator precedence; higher binds tighter. '^' is right-associative,
        // unary minus sits below '^' (so -3^2 = -(3^2)).
        private static int Prec(string op) => op switch
        {
            "+" or "-" => 1,
            "*" or "/" or "%" => 2,
            "u-" => 3,
            "^" => 4,
            _ => 0,
        };
        private static bool RightAssoc(string op) => op is "^" or "u-";

        private static List<Tok> ToRpn(List<Tok> tokens)
        {
            var output = new List<Tok>();
            var stack = new Stack<Tok>();

            foreach (var t in tokens)
            {
                switch (t.Kind)
                {
                    case Kind.Number:
                        output.Add(t);
                        break;
                    case Kind.Func:
                        stack.Push(t);
                        break;
                    case Kind.Bang: // postfix: operand is already emitted
                        output.Add(t);
                        break;
                    case Kind.Op:
                        while (stack.Count > 0 && stack.Peek().Kind == Kind.Op &&
                               (Prec(stack.Peek().Text) > Prec(t.Text) ||
                                (Prec(stack.Peek().Text) == Prec(t.Text) && !RightAssoc(t.Text))))
                            output.Add(stack.Pop());
                        stack.Push(t);
                        break;
                    case Kind.LParen:
                        stack.Push(t);
                        break;
                    case Kind.RParen:
                        while (stack.Count > 0 && stack.Peek().Kind != Kind.LParen)
                            output.Add(stack.Pop());
                        if (stack.Count == 0) throw new FormatException("mismatched )");
                        stack.Pop(); // discard '('
                        if (stack.Count > 0 && stack.Peek().Kind == Kind.Func)
                            output.Add(stack.Pop());
                        break;
                }
            }
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top.Kind is Kind.LParen or Kind.RParen) throw new FormatException("mismatched (");
                output.Add(top);
            }
            return output;
        }

        private static double EvalRpn(List<Tok> rpn)
        {
            var stack = new Stack<double>();
            foreach (var t in rpn)
            {
                switch (t.Kind)
                {
                    case Kind.Number:
                        stack.Push(t.Num);
                        break;
                    case Kind.Bang:
                        stack.Push(Factorial(Pop(stack)));
                        break;
                    case Kind.Func:
                        stack.Push(ApplyFunc(t.Text, Pop(stack)));
                        break;
                    case Kind.Op:
                        if (t.Text == "u-") { stack.Push(-Pop(stack)); break; }
                        double b = Pop(stack), a = Pop(stack);
                        stack.Push(t.Text switch
                        {
                            "+" => a + b,
                            "-" => a - b,
                            "*" => a * b,
                            "/" => a / b,
                            "%" => a % b,
                            "^" => Math.Pow(a, b),
                            _ => throw new FormatException($"bad op {t.Text}"),
                        });
                        break;
                }
            }
            if (stack.Count != 1) throw new FormatException("malformed expression");
            return stack.Pop();
        }

        private static double Pop(Stack<double> s) =>
            s.Count > 0 ? s.Pop() : throw new FormatException("missing operand");

        private static double ApplyFunc(string f, double x) => f switch
        {
            "sqrt" => Math.Sqrt(x),
            "abs" => Math.Abs(x),
            "floor" => Math.Floor(x),
            "ceil" => Math.Ceiling(x),
            "round" => Math.Round(x),
            "log" => Math.Log10(x),
            "ln" => Math.Log(x),
            _ => throw new FormatException($"bad function {f}"),
        };

        private static double Factorial(double x)
        {
            if (x < 0 || x > FactorialMax || Math.Abs(x - Math.Round(x)) > 1e-9)
                throw new FormatException("factorial needs a non-negative integer ≤ 170");
            double result = 1;
            for (int k = 2; k <= (int)Math.Round(x); k++) result *= k;
            return result;
        }
    }
}
