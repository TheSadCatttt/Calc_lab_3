using System;

namespace Calculator
{
    public class TComplex : TANumber
    {
        private TPNumber real;
        private TPNumber imaginary;
        private int numberBase;
        private int precision;

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

        public TComplex() : this(0, 0, 10, 6) { }

        private void ParseFromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new FormatException("Пустая строка");

            string s = str.Trim();

            // Убираем лишние плюсы в начале
            if (s.StartsWith("+"))
                s = s.Substring(1);

            // Если нет i - это действительное число
            if (!s.Contains("i"))
            {
                real = new TPNumber(s, numberBase, precision);
                imaginary = new TPNumber(0, numberBase, precision);
                return;
            }

            // Если строка просто "i"
            if (s == "i")
            {
                real = new TPNumber(0, numberBase, precision);
                imaginary = new TPNumber(1, numberBase, precision);
                return;
            }

            // Если строка "-i"
            if (s == "-i")
            {
                real = new TPNumber(0, numberBase, precision);
                imaginary = new TPNumber(-1, numberBase, precision);
                return;
            }

            // Разбираем комплексное число
            double realValue = 0;
            double imagValue = 0;

            // Ищем позицию i
            int iPos = s.IndexOf('i');
            string beforeI = s.Substring(0, iPos);

            // Если перед i ничего нет
            if (string.IsNullOrEmpty(beforeI))
            {
                real = new TPNumber(0, numberBase, precision);
                imaginary = new TPNumber(1, numberBase, precision);
                return;
            }

            // Парсим часть перед i
            // Нужно найти последний знак + или - который не является частью числа
            int lastOp = -1;
            for (int j = beforeI.Length - 1; j >= 0; j--)
            {
                if (beforeI[j] == '+' || beforeI[j] == '-')
                {
                    if (j == 0 || (beforeI[j - 1] != 'e' && beforeI[j - 1] != 'E'))
                    {
                        lastOp = j;
                        break;
                    }
                }
            }

            if (lastOp == -1)
            {
                // Вся строка перед i - это мнимая часть
                string imagStr = beforeI;
                if (imagStr == "+" || imagStr == "")
                    imagValue = 1;
                else if (imagStr == "-")
                    imagValue = -1;
                else
                    imagValue = double.Parse(imagStr, System.Globalization.CultureInfo.InvariantCulture);

                real = new TPNumber(0, numberBase, precision);
                imaginary = new TPNumber(imagValue, numberBase, precision);
            }
            else
            {
                // Есть и действительная, и мнимая часть
                string realStr = beforeI.Substring(0, lastOp);
                string imagStr = beforeI.Substring(lastOp + 1);

                if (string.IsNullOrEmpty(realStr))
                    realValue = 0;
                else
                    realValue = double.Parse(realStr, System.Globalization.CultureInfo.InvariantCulture);

                if (string.IsNullOrEmpty(imagStr) || imagStr == "+")
                    imagValue = 1;
                else if (imagStr == "-")
                    imagValue = -1;
                else
                    imagValue = double.Parse(imagStr, System.Globalization.CultureInfo.InvariantCulture);

                // Учитываем знак
                if (beforeI[lastOp] == '-')
                    imagValue = -imagValue;

                real = new TPNumber(realValue, numberBase, precision);
                imaginary = new TPNumber(imagValue, numberBase, precision);
            }
        }

        public TPNumber GetReal() => real;
        public TPNumber GetImaginary() => imaginary;

        public override TANumber Add(TANumber other)
        {
            TComplex c = other as TComplex ?? throw new InvalidOperationException("Несовместимые типы");
            return new TComplex(real.Add(c.real) as TPNumber ?? new TPNumber(0, numberBase, precision),
                               imaginary.Add(c.imaginary) as TPNumber ?? new TPNumber(0, numberBase, precision));
        }

        public override TANumber Subtract(TANumber other)
        {
            TComplex c = other as TComplex ?? throw new InvalidOperationException("Несовместимые типы");
            return new TComplex(real.Subtract(c.real) as TPNumber ?? new TPNumber(0, numberBase, precision),
                               imaginary.Subtract(c.imaginary) as TPNumber ?? new TPNumber(0, numberBase, precision));
        }

        public override TANumber Multiply(TANumber other)
        {
            TComplex c = other as TComplex ?? throw new InvalidOperationException("Несовместимые типы");

            // (a+bi)*(c+di) = (ac - bd) + (ad + bc)i
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
            return new TComplex(real.Negate() as TPNumber ?? new TPNumber(0, numberBase, precision),
                               imaginary.Negate() as TPNumber ?? new TPNumber(0, numberBase, precision));
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
            {
                string temp = imaginary.ToString();
                if (temp == "1") return "i";
                if (temp == "-1") return "-i";
                return temp + "i";
            }

            string sign = imaginary.GetNumber() >= 0 ? "+" : "-";
            string coefficient = imaginary.GetNumber() >= 0
                ? imaginary.ToString()
                : imaginary.Negate().ToString();

            if (coefficient == "1")
                return $"{real}{sign}i";

            return $"{real}{sign}{coefficient}i";
        }
    }
}