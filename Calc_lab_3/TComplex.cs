using System;

namespace Calculator
{
    /// <summary>
    /// Класс комплексных чисел с поддержкой p-ичной системы счисления
    /// </summary>
    /// 
    public class TComplex : TANumber
    {
        private TPNumber real;      // Действительная часть
        private TPNumber imaginary; // Мнимая часть
        private int numberBase;
        private int precision;

        public TComplex() : this(0, 0, 10, 6) { }
        public TComplex(double re = 0, double im = 0, int baseNum = 10, int prec = 6)
        {
            numberBase = baseNum;
            precision = prec;
            real = new TPNumber(re, baseNum, prec);
            imaginary = new TPNumber(im, baseNum, prec);
        }

        public TComplex(TPNumber re, TPNumber im)
        {
            numberBase = re.GetBase();
            precision = re.GetPrecision();
            real = re.Copy() as TPNumber ?? re;
            imaginary = im.Copy() as TPNumber ?? im;
        }

        public TComplex(string str, int baseNum = 10, int prec = 6) : this(0, 0, baseNum, prec)
        {
            ParseFromString(str);
        }

        private void ParseFromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new FormatException("Пустая строка");

            string s = str.Trim();

            int iPos = s.IndexOf('i');
            if (iPos == -1)
            {
                real = new TPNumber(s, numberBase, precision);
                imaginary = new TPNumber(0, numberBase, precision);
                return;
            }

            string imPart = s.Substring(0, iPos);
            string rePart = "";

            int plusPos = s.LastIndexOf('+', iPos);
            int minusPos = s.LastIndexOf('-', iPos);

            if (minusPos > 0 && (plusPos < 0 || minusPos > plusPos))
            {
                rePart = s.Substring(0, minusPos);
                imPart = s.Substring(minusPos, iPos - minusPos);
            }
            else if (plusPos >= 0)
            {
                rePart = s.Substring(0, plusPos);
                imPart = s.Substring(plusPos, iPos - plusPos);
            }
            else
            {
                rePart = "";
            }

            real = string.IsNullOrEmpty(rePart)
                ? new TPNumber(0, numberBase, precision)
                : new TPNumber(rePart, numberBase, precision);

            if (string.IsNullOrEmpty(imPart) || imPart == "+" || imPart == "-")
                imPart += "1";

            imaginary = new TPNumber(imPart, numberBase, precision);
        }

        public TPNumber GetReal() => real;
        public TPNumber GetImaginary() => imaginary;

        public override TANumber Add(TANumber other)
        {
            TComplex c = other as TComplex ?? throw new InvalidOperationException("Несовместимые типы");

            TPNumber newReal = real.Add(c.real) as TPNumber;
            TPNumber newImag = imaginary.Add(c.imaginary) as TPNumber;

            return new TComplex(newReal ?? new TPNumber(0, numberBase, precision),
                               newImag ?? new TPNumber(0, numberBase, precision));
        }

        public override TANumber Subtract(TANumber other)
        {
            TComplex c = other as TComplex ?? throw new InvalidOperationException("Несовместимые типы");

            TPNumber newReal = real.Subtract(c.real) as TPNumber;
            TPNumber newImag = imaginary.Subtract(c.imaginary) as TPNumber;

            return new TComplex(newReal ?? new TPNumber(0, numberBase, precision),
                               newImag ?? new TPNumber(0, numberBase, precision));
        }

        public override TANumber Multiply(TANumber other)
        {
            TComplex c = other as TComplex ?? throw new InvalidOperationException("Несовместимые типы");

            TPNumber ac = real.Multiply(c.real) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber bd = imaginary.Multiply(c.imaginary) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber ad = real.Multiply(c.imaginary) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber bc = imaginary.Multiply(c.real) as TPNumber ?? new TPNumber(0, numberBase, precision);

            TPNumber newReal = ac.Subtract(bd) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber newImag = ad.Add(bc) as TPNumber ?? new TPNumber(0, numberBase, precision);

            return new TComplex(newReal, newImag);
        }

        public override TANumber Divide(TANumber other)
        {
            TComplex c = other as TComplex ?? throw new InvalidOperationException("Несовместимые типы");
            if (c.real.IsZero() && c.imaginary.IsZero())
                throw new DivideByZeroException("Деление на ноль");

            TPNumber ac = real.Multiply(c.real) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber bd = imaginary.Multiply(c.imaginary) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber bc = imaginary.Multiply(c.real) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber ad = real.Multiply(c.imaginary) as TPNumber ?? new TPNumber(0, numberBase, precision);

            TPNumber c2 = c.real.Square() as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber d2 = c.imaginary.Square() as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber denominator = c2.Add(d2) as TPNumber ?? c2;

            if (denominator.IsZero())
                throw new DivideByZeroException("Деление на ноль");

            TPNumber numeratorReal = ac.Add(bd) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber numeratorImag = bc.Subtract(ad) as TPNumber ?? new TPNumber(0, numberBase, precision);

            TPNumber newReal = numeratorReal.Divide(denominator) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber newImag = numeratorImag.Divide(denominator) as TPNumber ?? new TPNumber(0, numberBase, precision);

            return new TComplex(newReal, newImag);
        }

        public override TANumber Square()
        {
            TPNumber a2 = real.Square() as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber b2 = imaginary.Square() as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber two = new TPNumber(2, numberBase, precision);
            TPNumber twoab = two.Multiply(real).Multiply(imaginary) as TPNumber ?? new TPNumber(0, numberBase, precision);

            TPNumber newReal = a2.Subtract(b2) as TPNumber ?? new TPNumber(0, numberBase, precision);

            return new TComplex(newReal, twoab);
        }

        public override TANumber Reciprocal()
        {
            if (real.IsZero() && imaginary.IsZero())
                throw new DivideByZeroException("Обращение нуля");

            TPNumber a2 = real.Square() as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber b2 = imaginary.Square() as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber denominator = a2.Add(b2) as TPNumber ?? a2;

            TPNumber newReal = real.Divide(denominator) as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber newImag = imaginary.Negate().Divide(denominator) as TPNumber ?? new TPNumber(0, numberBase, precision);

            return new TComplex(newReal, newImag);
        }

        public override TANumber Negate()
        {
            TPNumber newReal = real.Negate() as TPNumber ?? new TPNumber(0, numberBase, precision);
            TPNumber newImag = imaginary.Negate() as TPNumber ?? new TPNumber(0, numberBase, precision);
            return new TComplex(newReal, newImag);
        }

        public override bool IsZero() => real.IsZero() && imaginary.IsZero();

        public override bool Equals(TANumber other)
        {
            TComplex c = other as TComplex;
            if (c == null) return false;
            return Math.Abs(real.GetNumber() - c.real.GetNumber()) < 1e-12 &&
                   Math.Abs(imaginary.GetNumber() - c.imaginary.GetNumber()) < 1e-12;
        }

        public override TANumber Copy()
        {
            return new TComplex(real.Copy() as TPNumber ?? real, imaginary.Copy() as TPNumber ?? imaginary);
        }

        public override int GetBase() => numberBase;
        public override int GetPrecision() => precision;

        public override double GetNumber() => real.GetNumber();

        public override string ToString()
        {
            if (imaginary.IsZero())
                return real.ToString();

            if (real.IsZero())
                return $"{imaginary}i";

            string sign = imaginary.GetNumber() >= 0 ? "+" : "";
            return $"{real}{sign}{imaginary}i";
        }
    }
}