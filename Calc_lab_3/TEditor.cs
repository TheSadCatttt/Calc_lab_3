using System;

namespace Calculator
{
    public class TEditor
    {
        private string str;
        private int currentBase;

        private const string SEPARATOR = ",";
        private const string ZERO_STR = "0";

        public TEditor(int numberBase = 10)
        {
            currentBase = numberBase;
            str = ZERO_STR;
        }

        public string GetString() => str;
        public void SetString(string value)
        {
            str = string.IsNullOrEmpty(value) ? ZERO_STR : value;
        }
        public int GetBase() => currentBase;

        public bool IsZero()
        {
            string val = str.StartsWith("-") ? str[1..] : str;
            return val == ZERO_STR || val == "0";
        }

        public void SetBase(int newBase)
        {
            if (newBase < 2 || newBase > 16)
                throw new ArgumentOutOfRangeException(nameof(newBase), "Основание должно быть 2..16");
            currentBase = newBase;
        }

        public string ToggleSign()
        {
            if (str.StartsWith("-"))
                str = str[1..];
            else
                str = "-" + str;

            if (str == "-" || str == "")
                str = ZERO_STR;
            if (str == "-0")
                str = "0";

            return str;
        }

        public string AddDigit(int digit)
        {
            if (digit < 0 || digit >= currentBase)
                throw new ArgumentOutOfRangeException(nameof(digit));

            char ch = TPNumber.DigitToChar(digit);

            // Если текущее значение "0" и не дробное - заменяем
            if (str == "0" || str == "-0")
            {
                str = (str.StartsWith("-") ? "-" : "") + ch;
            }
            else
            {
                str = str + ch;
            }

            return str;
        }

        public string AddZero()
        {
            return AddDigit(0);
        }

        // Надо добавлять запятую ТОЛЬКО после вызова или в дробном ответе!!!!!!!!!!
        public string AddSeparator()
        {
            // Добавляем запятую, только если её ещё нет
            if (!str.Contains(SEPARATOR))
            {
                str += SEPARATOR;
            }
            return str;
        }

        public string Backspace()
        {
            if (str.Length <= 1)
            {
                str = ZERO_STR;
                return str;
            }

            str = str[..^1];

            if (str == "" || str == "-")
                str = ZERO_STR;
            if (str == "-0")
                str = "0";

            return str;
        }

        public string Clear()
        {
            str = ZERO_STR;
            return str;
        }

        public string Edit(int command)
        {
            return command switch
            {
                0 => AddZero(),
                >= 1 and <= 15 => AddDigit(command),
                16 => AddSeparator(),
                17 => Backspace(),
                18 => Clear(),
                20 => ToggleSign(),
                _ => str
            };
        }

        private bool IsValidNumber(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;

            string checkStr = s.StartsWith("-") ? s[1..] : s;

            foreach (char ch in checkStr)
            {
                if (ch == ',') continue;
                try
                {
                    int digit = TPNumber.CharToDigit(ch);
                    if (digit >= currentBase) return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}