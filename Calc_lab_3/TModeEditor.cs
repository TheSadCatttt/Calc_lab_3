using System;

namespace Calculator
{
    public class TModeEditor
    {
        public enum NumberMode { Real, Fraction, Complex }

        private NumberMode currentMode;
        private int numberBase;
        private int precision;

        // Для действительных чисел
        private string realStr;

        // Для дробей
        private string fractionStr;

        // Для комплексных чисел - отдельные поля!
        private string complexReal;      // Действительная часть
        private string complexImag;      // Мнимая часть
        private bool editingReal;        // true - редактируем действительную часть, false - мнимую
        public bool IsEditingReal => (currentMode == NumberMode.Complex) && editingReal;

        public TModeEditor(int baseNum = 10, int prec = 6, NumberMode mode = NumberMode.Real)
        {
            numberBase = baseNum;
            precision = prec;
            currentMode = mode;
            Reset();
        }

        public void SetMode(NumberMode mode)
        {
            if (currentMode == mode) return;

            SaveCurrentValue();
            currentMode = mode;
            LoadValueForCurrentMode();
        }

        public NumberMode GetMode() => currentMode;

        public void SetBase(int newBase)
        {
            if (newBase == numberBase) return;
            numberBase = newBase;
            Reset();
        }

        public void SetPrecision(int newPrec)
        {
            precision = newPrec;
        }

        public void Reset()
        {
            realStr = "0";
            fractionStr = "0";
            complexReal = "0";
            complexImag = "0";
            editingReal = true;
        }

        private void SaveCurrentValue()
        {
            string current = GetString();
            switch (currentMode)
            {
                case NumberMode.Real:
                    realStr = current;
                    break;
                case NumberMode.Fraction:
                    fractionStr = current;
                    break;
                case NumberMode.Complex:
                    // Сохраняем уже разделенные поля не нужно, они и так хранятся
                    break;
            }
        }

        private void LoadValueForCurrentMode()
        {
            switch (currentMode)
            {
                case NumberMode.Real:
                    if (!IsValidRealString(realStr))
                        realStr = "0";
                    break;
                case NumberMode.Fraction:
                    if (!IsValidFractionString(fractionStr))
                        fractionStr = "0";
                    break;
                case NumberMode.Complex:
                    // Поля уже инициализированы
                    break;
            }
        }

        private bool IsValidRealString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char ch in s)
            {
                if (ch == '-' || ch == ',') continue;
                try
                {
                    int digit = TPNumber.CharToDigit(ch);
                    if (digit >= numberBase) return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsValidFractionString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            string[] parts = s.Split('/');
            if (parts.Length > 2) return false;
            return IsValidRealString(parts[0]) && (parts.Length == 1 || IsValidRealString(parts[1]));
        }

        public string GetString()
        {
            return currentMode switch
            {
                NumberMode.Real => realStr,
                NumberMode.Fraction => fractionStr,
                NumberMode.Complex => FormatComplex(),
                _ => "0"
            };
        }

        private string FormatComplex()
        {
            if (complexImag == "0")
                return complexReal;

            if (complexReal == "0")
            {
                if (complexImag == "1") return "i";
                if (complexImag == "-1") return "-i";
                return complexImag + "i";
            }

            string sign = complexImag.StartsWith("-") ? "" : "+";
            string imagPart = complexImag;
            if (imagPart == "1") imagPart = "";
            if (imagPart == "-1") imagPart = "-";

            return $"{complexReal}{sign}{imagPart}i";
        }

        public void SetString(string value)
        {
            switch (currentMode)
            {
                case NumberMode.Real:
                    realStr = string.IsNullOrEmpty(value) ? "0" : value;
                    break;
                case NumberMode.Fraction:
                    fractionStr = string.IsNullOrEmpty(value) ? "0" : value;
                    break;
                case NumberMode.Complex:
                    ParseComplexString(value);
                    break;
            }
        }

        private void ParseComplexString(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str == "0")
            {
                complexReal = "0";
                complexImag = "0";
                editingReal = true;
                return;
            }

            // Простой парсинг строки вида "a+bi" или "a-bi"
            int iPos = str.IndexOf('i');
            if (iPos == -1)
            {
                complexReal = str;
                complexImag = "0";
                editingReal = true;
                return;
            }

            string beforeI = str.Substring(0, iPos);
            if (string.IsNullOrEmpty(beforeI))
            {
                complexReal = "0";
                complexImag = "1";
                editingReal = false;
                return;
            }

            // Ищем последний + или -
            int lastOp = -1;
            for (int j = beforeI.Length - 1; j >= 0; j--)
            {
                if (beforeI[j] == '+' || beforeI[j] == '-')
                {
                    lastOp = j;
                    break;
                }
            }

            if (lastOp == -1)
            {
                complexReal = "0";
                complexImag = beforeI;
                editingReal = false;
            }
            else
            {
                complexReal = beforeI.Substring(0, lastOp);
                string imagPart = beforeI.Substring(lastOp + 1);
                if (string.IsNullOrEmpty(imagPart)) imagPart = "1";
                if (beforeI[lastOp] == '-') imagPart = "-" + imagPart;
                complexImag = imagPart;
                editingReal = false;
            }

            if (complexReal == "") complexReal = "0";
            if (complexImag == "") complexImag = "0";
        }

        public string Edit(int command)
        {
            switch (currentMode)
            {
                case NumberMode.Real:
                    return EditReal(command);
                case NumberMode.Fraction:
                    return EditFraction(command);
                case NumberMode.Complex:
                    return EditComplex(command);
                default:
                    return GetString();
            }
        }

        private string EditReal(int command)
        {
            realStr = ApplyBasicEdit(realStr, command, IsValidRealDigit);
            return realStr;
        }

        private string EditFraction(int command)
        {
            if (command == TCtrl.CMD_FRACTION)
            {
                if (!fractionStr.Contains("/"))
                    fractionStr += "/";
                return fractionStr;
            }
            fractionStr = ApplyBasicEdit(fractionStr, command, IsValidRealDigit);
            return fractionStr;
        }

        private string EditComplex(int command)
        {
            // Кнопка i переключает между полями
            if (command == TCtrl.CMD_IMAGINARY)
            {
                editingReal = !editingReal;
                return GetString();
            }

            // Редактируем текущее активное поле
            if (editingReal)
            {
                complexReal = ApplyBasicEdit(complexReal, command, IsValidRealDigit);
            }
            else
            {
                complexImag = ApplyBasicEdit(complexImag, command, IsValidRealDigit);
                // Обработка знака для мнимой части
                if (complexImag == "-0") complexImag = "0";
                if (complexImag == "") complexImag = "0";
            }

            return GetString();
        }

        private string ApplyBasicEdit(string current, int command, Func<char, bool> isValidDigit)
        {
            if (command >= 0 && command <= 15)
            {
                char ch = TPNumber.DigitToChar(command);
                if (current == "0" || current == "-0")
                    current = (current.StartsWith("-") ? "-" : "") + ch;
                else
                    current += ch;
            }
            else if (command == TCtrl.CMD_SEPARATOR)
            {
                if (!current.Contains(","))
                    current += ",";
            }
            else if (command == TCtrl.CMD_BACKSPACE)
            {
                if (current.Length > 1)
                    current = current[..^1];
                else
                    current = "0";

                if (current == "-" || current == "")
                    current = "0";
                if (current == "-0")
                    current = "0";
            }
            else if (command == TCtrl.CMD_CLEAR)
            {
                current = "0";
            }
            else if (command == TCtrl.CMD_SIGN)
            {
                if (current.StartsWith("-"))
                    current = current[1..];
                else if (current != "0")
                    current = "-" + current;

                if (current == "-0") current = "0";
                if (string.IsNullOrEmpty(current)) current = "0";
            }

            return current;
        }

        private bool IsValidRealDigit(char ch)
        {
            try
            {
                int digit = TPNumber.CharToDigit(ch);
                return digit < numberBase;
            }
            catch
            {
                return false;
            }
        }

        public bool IsZero()
        {
            return GetString() == "0";
        }

        public void Clear()
        {
            SetString("0");
        }
    }
}