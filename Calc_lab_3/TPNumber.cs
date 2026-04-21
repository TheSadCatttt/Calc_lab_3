using System;
using System.Text;

namespace Calculator
{
    public class TPNumber
    {
        private double n;
        private int b;
        private int c;

        public TPNumber(double a = 0, int b = 10, int c = 10)
        {
            this.b = CheckBase(b);
            this.c = CheckPrecision(c);
            this.n = a;
        }

        public TPNumber(string a, int b = 10, int c = 10)
        {
            this.b = CheckBase(b);
            this.c = CheckPrecision(c);
            this.n = Parse(a, this.b);
        }

        public double GetNumber() => n;
        public int GetBase() => b;
        public int GetPrecision() => c;

        public void SetBase(int newB)
        {
            this.b = CheckBase(newB);
        }

        public void SetPrecision(int newC)
        {
            this.c = CheckPrecision(newC);
        }

        public bool IsZero() => Math.Abs(n) < 1e-12;

        public TPNumber Copy() => new TPNumber(n, b, c);

        public TPNumber Add(TPNumber other)
        {
            EnsureCompatible(other);
            return new TPNumber(n + other.n, b, c);
        }

        public TPNumber Subtract(TPNumber other)
        {
            EnsureCompatible(other);
            return new TPNumber(n - other.n, b, c);
        }

        public TPNumber Multiply(TPNumber other)
        {
            EnsureCompatible(other);
            return new TPNumber(n * other.n, b, c);
        }

        public TPNumber Divide(TPNumber other)
        {
            EnsureCompatible(other);
            if (other.IsZero())
                throw new DivideByZeroException("Деление на ноль");
            return new TPNumber(n / other.n, b, c);
        }

        public TPNumber Square() => new TPNumber(n * n, b, c);

        public TPNumber Reciprocal()
        {
            if (IsZero())
                throw new DivideByZeroException("Обращение нуля");
            return new TPNumber(1.0 / n, b, c);
        }

        public TPNumber Negate() => new TPNumber(-n, b, c);

        public override string ToString()
        {
            if (double.IsNaN(n) || double.IsInfinity(n))
                return "0";

            string sign = n < 0 ? "-" : "";
            double abs = Math.Abs(n);
            long intPart = (long)Math.Floor(abs);
            double fracPart = abs - intPart;

            string intStr = IntToString(intPart, b);
            string fracStr = FracToString(fracPart, b, c);

            // Запятая выводится только если есть дробная часть
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