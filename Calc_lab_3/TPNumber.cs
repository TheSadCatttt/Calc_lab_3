using System;
using System.Text;

namespace Calculator
{
    public class TPNumber : TANumber
    {
        private double n; // Внутреннее представление числа в десятичной форме
        private int b; // Основание системы счисления (2..16)
        private int c; // Точность (количество знаков после запятой)

        public TPNumber() : this(0, 10, 10) { }

        public TPNumber(double a = 0, int b = 10, int c = 10)
        {
            this.b = CheckBase(b);
            this.c = CheckPrecision(c);
            this.n = Math.Round(a, c, MidpointRounding.AwayFromZero);
        }

        public TPNumber(string a, int b = 10, int c = 10)
        {
            this.b = CheckBase(b);
            this.c = CheckPrecision(c);
            this.n = Parse(a, this.b);
            this.n = Math.Round(this.n, c, MidpointRounding.AwayFromZero);
        }

        // Переопределение методов TANumber
        public override TANumber Add(TANumber other)
        {
            TPNumber otherNum = other as TPNumber ?? throw new InvalidOperationException("Несовместимые типы");
            EnsureCompatible(otherNum);
            return new TPNumber(n + otherNum.n, b, c);
        }

        public override TANumber Subtract(TANumber other)
        {
            TPNumber otherNum = other as TPNumber ?? throw new InvalidOperationException("Несовместимые типы");
            EnsureCompatible(otherNum);
            return new TPNumber(n - otherNum.n, b, c);
        }

        public override TANumber Multiply(TANumber other)
        {
            TPNumber otherNum = other as TPNumber ?? throw new InvalidOperationException("Несовместимые типы");
            EnsureCompatible(otherNum);
            return new TPNumber(n * otherNum.n, b, c);
        }

        public override TANumber Divide(TANumber other)
        {
            TPNumber otherNum = other as TPNumber ?? throw new InvalidOperationException("Несовместимые типы");
            EnsureCompatible(otherNum);
            if (otherNum.IsZero())
                throw new DivideByZeroException("Деление на ноль");
            return new TPNumber(n / otherNum.n, b, c);
        }

        public override TANumber Square()
        {
            return new TPNumber(n * n, b, c);
        }

        public override TANumber Reciprocal()
        {
            if (IsZero())
                throw new DivideByZeroException("Обращение нуля");
            return new TPNumber(1.0 / n, b, c);
        }

        public override TANumber Negate()
        {
            return new TPNumber(-n, b, c);
        }

        public override bool IsZero() => Math.Abs(n) < 1e-12;

        public override bool Equals(TANumber other)
        {
            TPNumber otherNum = other as TPNumber;
            if (otherNum == null) return false;
            return Math.Abs(n - otherNum.n) < 1e-12 && b == otherNum.b && c == otherNum.c;
        }

        public override TANumber Copy() => new TPNumber(n, b, c);

        public override int GetBase() => b;
        public override int GetPrecision() => c;

        // Переопределяем виртуальный метод GetNumber из TANumber
        public override double GetNumber() => n;

        // Дополнительные методы для TPNumber
        public void SetBase(int newB)
        {
            this.b = CheckBase(newB);
        }

        public void SetPrecision(int newC)
        {
            this.c = CheckPrecision(newC);
        }

        public override string ToString()
        {
            if (double.IsNaN(n) || double.IsInfinity(n))
                return "0";

            string sign = n < 0 ? "-" : "";
            double abs = Math.Abs(n);

            // Округляем с учётом точности
            double rounded = Math.Round(abs, c, MidpointRounding.AwayFromZero);

            // Корректировка из-за погрешностей округления
            if (Math.Abs(rounded - 1.0) < 1e-12 && abs > 0.5)
            {
                rounded = 1.0;
            }

            long intPart = (long)Math.Floor(rounded);
            double fracPart = rounded - intPart;

            // Дополнительная проверка на погрешности
            if (fracPart > 0.9999999999)
            {
                intPart++;
                fracPart = 0;
            }

            string intStr = IntToString(intPart, b);

            if (intPart == 0 && string.IsNullOrEmpty(intStr))
                intStr = "0";

            string fracStr = "";
            if (c > 0 && fracPart > 1e-12)
            {
                fracStr = FracToString(fracPart, b, c);
                fracStr = fracStr.TrimEnd('0');
            }

            if (string.IsNullOrEmpty(fracStr))
                return sign + intStr;
            return sign + intStr + "," + fracStr;
        }

        public static double Parse(string text, int numberBase)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Входная строка пуста");

            CheckBase(numberBase);

            string src = text.Trim();
            double sign = 1;
            if (src.StartsWith("-"))
            {
                sign = -1;
                src = src[1..];
            }

            string[] parts = src.Split(',');
            if (parts.Length > 2)
                throw new FormatException("Некорректный формат числа");

            string intPart = parts[0];
            string fracPart = parts.Length == 2 ? parts[1] : "";

            double intValue = 0;
            foreach (char ch in intPart)
            {
                int digit = CharToDigit(ch);
                if (digit >= numberBase)
                    throw new FormatException($"Цифра '{ch}' недопустима для основания {numberBase}");
                intValue = intValue * numberBase + digit;
            }

            double fracValue = 0;
            double weight = 1.0 / numberBase;
            foreach (char ch in fracPart)
            {
                int digit = CharToDigit(ch);
                if (digit >= numberBase)
                    throw new FormatException($"Цифра '{ch}' недопустима для основания {numberBase}");
                fracValue += digit * weight;
                weight /= numberBase;
            }

            return sign * (intValue + fracValue);
        }

        private static int CheckBase(int numberBase)
        {
            if (numberBase < 2 || numberBase > 16)
                throw new ArgumentOutOfRangeException(nameof(numberBase), "Основание должно быть 2..16");
            return numberBase;
        }

        private static int CheckPrecision(int precision)
        {
            if (precision < 0)
                throw new ArgumentOutOfRangeException(nameof(precision), "Точность должна быть >= 0");
            return precision;
        }

        private static string IntToString(long value, int numberBase)
        {
            if (value == 0) return "0";

            var sb = new StringBuilder();
            long current = value;
            while (current > 0)
            {
                int digit = (int)(current % numberBase);
                sb.Insert(0, DigitToChar(digit));
                current /= numberBase;
            }
            return sb.ToString();
        }

        private static string FracToString(double frac, int numberBase, int precision)
        {
            if (precision == 0 || Math.Abs(frac) < 1e-12) return "";

            var sb = new StringBuilder();
            double current = frac;
            for (int i = 0; i < precision; i++)
            {
                current *= numberBase;
                int digit = (int)Math.Floor(current);
                sb.Append(DigitToChar(digit));
                current -= digit;
                if (current < 1e-12) break;
            }
            return sb.ToString();
        }

        public static int CharToDigit(char ch)
        {
            if (ch >= '0' && ch <= '9') return ch - '0';
            if (ch >= 'A' && ch <= 'F') return ch - 'A' + 10;
            if (ch >= 'a' && ch <= 'f') return ch - 'a' + 10;
            throw new FormatException($"Недопустимая цифра '{ch}'");
        }

        public static char DigitToChar(int digit)
        {
            if (digit < 10) return (char)('0' + digit);
            return (char)('A' + (digit - 10));
        }

        private void EnsureCompatible(TPNumber other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            if (b != other.b)
                throw new InvalidOperationException("Основания систем счисления не совпадают");
            if (c != other.c)
                throw new InvalidOperationException("Точности не совпадают");
        }
    }
}