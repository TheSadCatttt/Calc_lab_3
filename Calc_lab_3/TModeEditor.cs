using System;

namespace Calculator
{
    /// <summary>
    /// Редактор чисел с поддержкой разных режимов (действительные, дроби, комплексные)
    /// </summary>
    public class TModeEditor
    {
        public enum NumberMode { Real, Fraction, Complex }

        private NumberMode currentMode;
        private int numberBase;
        private int precision;

        // Для каждого режима своё строковое представление
        private string realStr;
        private string fractionStr;
        private string complexStr;

        // Для комплексных чисел: храним отдельно действительную и мнимую части
        private string complexRealPart;
        private string complexImagPart;
        private bool isEditingImaginary;

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
            complexStr = "0";
            complexRealPart = "0";
            complexImagPart = "";
            isEditingImaginary = false;
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
                    complexStr = current;
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
                    ParseComplexString(complexStr);
                    break;
            }
        }

        private void ParseComplexString(string str)
        {
            if (string.IsNullOrEmpty(str) || str == "0")
            {
                complexRealPart = "0";
                complexImagPart = "";
                isEditingImaginary = false;
                complexStr = "0";
                return;
            }

            // Парсим строку вида "a+bi" или "a-bi" или "a" или "bi"
            int iPos = str.IndexOf('i');
            if (iPos == -1)
            {
                // Только действительная часть
                complexRealPart = str;
                complexImagPart = "";
                isEditingImaginary = false;
            }
            else
            {
                // Есть мнимая часть
                string imagPart = str.Substring(0, iPos);
                string realPart = "";

                int plusPos = str.LastIndexOf('+', iPos);
                int minusPos = str.LastIndexOf('-', iPos);

                if (minusPos > 0 && (plusPos < 0 || minusPos > plusPos))
                {
                    realPart = str.Substring(0, minusPos);
                    imagPart = str.Substring(minusPos, iPos - minusPos);
                }
                else if (plusPos >= 0)
                {
                    realPart = str.Substring(0, plusPos);
                    imagPart = str.Substring(plusPos, iPos - plusPos);
                }
                else
                {
                    realPart = "";
                    imagPart = imagPart;
                }

                complexRealPart = string.IsNullOrEmpty(realPart) ? "0" : realPart;

                if (string.IsNullOrEmpty(imagPart) || imagPart == "+" || imagPart == "-")
                    imagPart += "1";
                complexImagPart = imagPart;
                isEditingImaginary = true;
            }
            UpdateComplexString();
        }

        private void UpdateComplexString()
        {
            if (string.IsNullOrEmpty(complexImagPart) || complexImagPart == "0")
            {
                complexStr = complexRealPart;
            }
            else if (complexRealPart == "0")
            {
                complexStr = complexImagPart + "i";
            }
            else
            {
                string sign = complexImagPart.StartsWith("-") ? "" : "+";
                complexStr = $"{complexRealPart}{sign}{complexImagPart}i";
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
                NumberMode.Complex => complexStr,
                _ => "0"
            };
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
                    ParseComplexString(string.IsNullOrEmpty(value) ? "0" : value);
                    break;
            }
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
            // Специальная обработка для кнопки i
            if (command == TCtrl.CMD_IMAGINARY)
            {
                if (string.IsNullOrEmpty(complexImagPart))
                {
                    // Начинаем ввод мнимой части
                    complexImagPart = "1";
                    isEditingImaginary = true;
                }
                else if (complexImagPart == "1" && isEditingImaginary)
                {
                    // Уже есть мнимая часть, ничего не делаем
                }
                UpdateComplexString();
                return complexStr;
            }

            // Редактирование в зависимости от того, какую часть редактируем
            if (isEditingImaginary)
            {
                string newImagPart = ApplyBasicEdit(complexImagPart, command, IsValidRealDigit);
                if (newImagPart != "0")
                {
                    complexImagPart = newImagPart;
                }
                else
                {
                    complexImagPart = "";
                    isEditingImaginary = false;
                }
            }
            else
            {
                string newRealPart = ApplyBasicEdit(complexRealPart, command, IsValidRealDigit);
                if (newRealPart != "0" || string.IsNullOrEmpty(complexImagPart))
                {
                    complexRealPart = newRealPart;
                }
            }

            UpdateComplexString();
            return complexStr;
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