using System;

namespace Calculator
{
    /// <summary>
    /// Редактор чисел с поддержкой разных режимов (действительные, дроби, комплексные)
    /// Соответствует AEditor и его наследникам из методички
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

        // Для комплексных чисел: отслеживаем, редактируем ли сейчас мнимую часть
        private bool editingImaginary;
        private bool hasImaginarySeparator;

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

            // Сохраняем текущее значение в соответствующую переменную перед сменой режима
            SaveCurrentValue();
            currentMode = mode;
            LoadValueForCurrentMode();
        }

        public NumberMode GetMode() => currentMode;

        public void SetBase(int newBase)
        {
            if (newBase == numberBase) return;
            numberBase = newBase;

            // При смене основания нужно пересчитать все представления
            // Проще всего сбросить всё в 0
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
            editingImaginary = false;
            hasImaginarySeparator = false;
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
                    // Проверяем, что строка валидна для p-ичного числа
                    if (!IsValidRealString(realStr))
                        realStr = "0";
                    break;
                case NumberMode.Fraction:
                    if (!IsValidFractionString(fractionStr))
                        fractionStr = "0";
                    break;
                case NumberMode.Complex:
                    if (!IsValidComplexString(complexStr))
                        complexStr = "0";
                    editingImaginary = false;
                    hasImaginarySeparator = complexStr.Contains("i");
                    break;
            }
        }

        private bool IsValidRealString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            // Допустимые символы: цифры 0-9,A-F, запятая, знак минус
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
            // Дробь вида "число/число" или просто число
            string[] parts = s.Split('/');
            if (parts.Length > 2) return false;
            return IsValidRealString(parts[0]) && (parts.Length == 1 || IsValidRealString(parts[1]));
        }

        private bool IsValidComplexString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            // Комплексное число: "a+bi" или "a" или "bi"
            // Допускаем только простой формат
            int iPos = s.IndexOf('i');
            if (iPos == -1)
                return IsValidRealString(s);

            string withoutI = s.Replace("i", "");
            if (string.IsNullOrEmpty(withoutI) || withoutI == "+" || withoutI == "-")
                return true;

            return IsValidRealString(withoutI);
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
                    complexStr = string.IsNullOrEmpty(value) ? "0" : value;
                    hasImaginarySeparator = complexStr.Contains("i");
                    editingImaginary = hasImaginarySeparator && !complexStr.EndsWith("i");
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
            // Специальная обработка для кнопки дроби
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
            if (command == 100) // Специальный код для кнопки i
            {
                if (!complexStr.Contains("i"))
                {
                    complexStr += "i";
                    hasImaginarySeparator = true;
                    editingImaginary = true;
                }
                return complexStr;
            }

            // Обычное редактирование
            complexStr = ApplyBasicEdit(complexStr, command, IsValidRealDigit);

            // Обновляем флаги
            hasImaginarySeparator = complexStr.Contains("i");
            editingImaginary = hasImaginarySeparator && !complexStr.EndsWith("i");

            return complexStr;
        }

        private string ApplyBasicEdit(string current, int command, Func<char, bool> isValidDigit)
        {
            if (command >= 0 && command <= 15)
            {
                // Добавление цифры
                char ch = TPNumber.DigitToChar(command);
                if (current == "0")
                    current = ch.ToString();
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