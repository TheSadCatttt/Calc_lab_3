namespace Calculator
{
    public enum TMemoryState { Off, On } // Состояние памяти: выключена (Off) или включена (On)

    public class TMemory<T> where T : TANumber, new() // Класс памяти для хранения числа типа T, который должен быть наследником TANumber
    {
        private T fNumber;
        private TMemoryState fState;

        public TMemory(T defaultValue) // Конструктор, который принимает значение по умолчанию для инициализации памяти 
        {
            fNumber = defaultValue ?? new T();
            fState = TMemoryState.Off;
        }

        public TMemoryState GetState() => fState; // Метод для получения текущего состояния памяти
        public string GetStateString() => fState == TMemoryState.On ? "M" : ""; // Метод для получения строкового представления состояния памяти (например, "M" для включенной памяти)

        public void Store(T e) // Метод для сохранения значения в памяти
        {
            fNumber = e;
            fState = TMemoryState.On;
        }

        public T Take() // Метод для получения значения из памяти. Если память выключена, возвращает текущее значение (которое может быть нулем или другим значением по умолчанию).
        {
            return fNumber;
        }

        public void Add(T e) // Метод для добавления значения к текущему значению в памяти. Если память выключена, он просто сохраняет новое значение.
        {
            TANumber result = fNumber.Add(e);
            fNumber = result as T ?? new T();
            fState = TMemoryState.On;
        }

        public void Clear(T zeroValue) // Метод для очистки памяти, который устанавливает значение в ноль (или другое значение по умолчанию) и выключает память.
        {
            fNumber = zeroValue ?? new T();
            fState = TMemoryState.Off;
        }
    }
}