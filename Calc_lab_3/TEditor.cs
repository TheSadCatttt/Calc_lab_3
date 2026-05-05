using System;

namespace Calculator
{
    public class TEditor // Класс для редактирования текстового представления числа в заданной системе счисления
    {
        private string str; // Текстовое представление числа, которое редактируется
        private int currentBase; // Текущее основание системы счисления (2..16)

        private const string SEPARATOR = ","; // Разделитель для дробной части
        private const string ZERO_STR = "0"; // Строковое представление нуля

        public TEditor(int numberBase = 10) // Конструктор, который инициализирует редактор с заданным основанием системы счисления и начальным значением "0"
        {
            currentBase = numberBase;
            str = ZERO_STR;
        }

        public string GetString() => str; // Геттер для получения текущего текстового представления числа
        public void SetString(string value) // Сеттер для установки нового текстового представления числа (проверяет его на валидность и заменяет на "0", если оно пустое или null)
        {
            str = string.IsNullOrEmpty(value) ? ZERO_STR : value;
        }
        public int GetBase() => currentBase; // Геттер для получения текущего основания системы счисления

        public bool IsZero() // Метод для проверки, является ли текущее число нулём (учитывает знак и возможное наличие разделителя)
        {
            string val = str.StartsWith("-") ? str[1..] : str;
            return val == ZERO_STR || val == "0";
        }

        public void SetBase(int newBase) // Метод для изменения основания системы счисления (проверяет, что новое основание находится в допустимом диапазоне)
        {
            if (newBase < 2 || newBase > 16)
                throw new ArgumentOutOfRangeException(nameof(newBase), "Основание должно быть 2..16");
            currentBase = newBase;
        }

        public string ToggleSign() // Метод для переключения знака числа (добавляет или удаляет минус в начале строки, а также корректирует строку, если она становится пустой или содержит только минус)
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

        public string AddDigit(int digit) // Метод для добавления цифры в конец строки (проверяет, что цифра допустима для текущей системы счисления, и корректирует строку, если она была "0" или "-0")
        {
            if (digit < 0 || digit >= currentBase)
                throw new ArgumentOutOfRangeException(nameof(digit));

            char ch = TPNumber.DigitToChar(digit);

            // Если текущее значение "0" и не дробное - заменяем
            if (str == "0" || str == "-0")
            {
                str = (str.StartsWith("-") ? "-" : "") + ch; // Сохраняем знак, если он был
            }
            else
            {
                str = str + ch;
            }

            return str;
        }

        public string AddZero() // Метод для добавления нуля в конец строки
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

        public string Backspace() // Метод для удаления последнего символа из строки (если строка становится пустой или содержит только минус, заменяет её на "0")
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

        public string Clear() // Метод для очистки строки (устанавливает её в "0")
        {
            str = ZERO_STR;
            return str;
        }

        public string Edit(int command) // Метод для редактирования строки в зависимости от команды
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

        private bool IsValidNumber(string s) // Метод для проверки, является ли строка допустимым числом в текущей системе счисления (учитывает знак, разделитель и допустимые символы)
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