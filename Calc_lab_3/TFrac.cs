using System;
using System.Numerics; // Для BigInteger (избегаем переполнения)

namespace Calculator
{
    /// <summary>
    /// Класс обыкновенных дробей с поддержкой p-ичной системы счисления
    /// Соответствует TFrac из методички
    /// </summary>
    public class TFrac : TANumber
    {
        private BigInteger numerator;   // Числитель
        private BigInteger denominator; // Знаменатель (всегда > 0)
        private int numberBase;         // Основание системы счисления для отображения
        private int precision;          // Точность (не используется напрямую, но нужно для интерфейса)

        public TFrac() : this(0, 1, 10, 6) { }

        public TFrac(BigInteger num = default, BigInteger den = default, int baseNum = 10, int prec = 6)
        {
            if (den == 0)
                throw new DivideByZeroException("Знаменатель не может быть нулём");

            // Устанавливаем значения по умолчанию
            BigInteger actualNum = num == default ? 0 : num;
            BigInteger actualDen = den == default ? 1 : den;

            numberBase = baseNum;
            precision = prec;
            numerator = actualNum;
            denominator = actualDen > 0 ? actualDen : -actualDen;

            // Если числитель отрицательный, знак переносим в числитель
            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            Reduce();
        }

        public TFrac(string str, int baseNum = 10, int prec = 6) : this(0, 1, baseNum, prec)
        {
            ParseFromString(str);
        }

        private void ParseFromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new FormatException("Пустая строка");

            string s = str.Trim();
            bool negative = s.StartsWith("-");
            if (negative) s = s[1..];

            string[] parts = s.Split('/');

            if (parts.Length == 1)
            {
                // Целое число в p-ичной системе
                numerator = ParsePNumber(parts[0]);
                denominator = 1;
            }
            else if (parts.Length == 2)
            {
                // Дробь вида "числитель/знаменатель"
                numerator = ParsePNumber(parts[0]);
                denominator = ParsePNumber(parts[1]);
                if (denominator == 0)
                    throw new DivideByZeroException("Знаменатель не может быть нулём");
            }
            else
            {
                throw new FormatException("Некорректный формат дроби");
            }

            if (negative) numerator = -numerator;
            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            Reduce();
        }

        private BigInteger ParsePNumber(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;

            BigInteger result = 0;
            foreach (char ch in s)
            {
                int digit = TPNumber.CharToDigit(ch);
                if (digit >= numberBase)
                    throw new FormatException($"Цифра '{ch}' недопустима для основания {numberBase}");
                result = result * numberBase + digit;
            }
            return result;
        }

        private string BigIntegerToPString(BigInteger value)
        {
            if (value == 0) return "0";

            bool negative = value < 0;
            BigInteger abs = negative ? -value : value;

            string result = "";
            while (abs > 0)
            {
                int digit = (int)(abs % numberBase);
                result = TPNumber.DigitToChar(digit) + result;
                abs /= numberBase;
            }

            return negative ? "-" + result : result;
        }

        private void Reduce()
        {
            if (numerator == 0)
            {
                denominator = 1;
                return;
            }

            BigInteger gcd = GCD(BigInteger.Abs(numerator), denominator);
            numerator /= gcd;
            denominator /= gcd;
        }

        private static BigInteger GCD(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public override TANumber Add(TANumber other)
        {
            TFrac f = other as TFrac ?? throw new InvalidOperationException("Несовместимые типы");
            BigInteger newNum = numerator * f.denominator + f.numerator * denominator;
            BigInteger newDen = denominator * f.denominator;
            return new TFrac(newNum, newDen, numberBase, precision);
        }

        public override TANumber Subtract(TANumber other)
        {
            TFrac f = other as TFrac ?? throw new InvalidOperationException("Несовместимые типы");
            BigInteger newNum = numerator * f.denominator - f.numerator * denominator;
            BigInteger newDen = denominator * f.denominator;
            return new TFrac(newNum, newDen, numberBase, precision);
        }

        public override TANumber Multiply(TANumber other)
        {
            TFrac f = other as TFrac ?? throw new InvalidOperationException("Несовместимые типы");
            return new TFrac(numerator * f.numerator, denominator * f.denominator, numberBase, precision);
        }

        public override TANumber Divide(TANumber other)
        {
            TFrac f = other as TFrac ?? throw new InvalidOperationException("Несовместимые типы");
            if (f.numerator == 0)
                throw new DivideByZeroException("Деление на ноль");
            return new TFrac(numerator * f.denominator, denominator * f.numerator, numberBase, precision);
        }

        public override TANumber Square()
        {
            return new TFrac(numerator * numerator, denominator * denominator, numberBase, precision);
        }

        public override TANumber Reciprocal()
        {
            if (numerator == 0)
                throw new DivideByZeroException("Обращение нуля");
            return new TFrac(denominator, numerator, numberBase, precision);
        }

        public override TANumber Negate()
        {
            return new TFrac(-numerator, denominator, numberBase, precision);
        }

        public override bool IsZero() => numerator == 0;

        public override bool Equals(TANumber other)
        {
            TFrac f = other as TFrac;
            if (f == null) return false;
            return numerator == f.numerator && denominator == f.denominator;
        }

        public override TANumber Copy()
        {
            return new TFrac(numerator, denominator, numberBase, precision);
        }

        public override string ToString()
        {
            if (numerator == 0) return "0";

            string numStr = BigIntegerToPString(numerator);
            string denStr = BigIntegerToPString(denominator);

            if (denominator == 1)
                return numStr;

            return $"{numStr}/{denStr}";
        }

        public override int GetBase() => numberBase;
        public override int GetPrecision() => precision;

        // Для совместимости с TProc
        public override double GetNumber() => (double)numerator / (double)denominator;
    }
}