namespace Calculator
{
    public enum TMemoryState { Off, On } // Перечисление для состояния памяти (выключена или включена)

    public class TMemory<T> where T : class // Класс для управления памятью калькулятора
    {
        private T fNumber; // Поле для хранения числа в памяти
        private TMemoryState fState; // Поле для хранения состояния памяти

        public TMemory(T defaultValue) // Конструктор, который инициализирует память с заданным значением по умолчанию и устанавливает состояние в Off
        {
            fNumber = defaultValue;
            fState = TMemoryState.Off;
        }

        public TMemoryState GetState() => fState; // Метод для получения текущего состояния памяти
        public string GetStateString() => fState == TMemoryState.On ? "M" : ""; // Метод для получения строкового представления состояния памяти (возвращает "M", если память включена, и пустую строку, если выключена)

        public void Store(T e) // Метод для сохранения числа в памяти и включения состояния памяти
        {
            fNumber = e;
            fState = TMemoryState.On;
        }

        public T Take() // Метод для получения числа из памяти
        {
            return fNumber;
        }

        public void Add(T e) // Метод для прибавления числа к числу в памяти и включения состояния памяти
        {
            dynamic dynNumber = fNumber;
            dynamic dynE = e;
            fNumber = (T)(dynNumber.Add(dynE));
            fState = TMemoryState.On;
        }

        public void Clear(T zeroValue) // Метод для очистки памяти (устанавливает число в памяти в значение нуля и выключает состояние памяти)
        {
            fNumber = zeroValue;
            fState = TMemoryState.Off;
        }
    }
}