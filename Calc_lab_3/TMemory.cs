namespace Calculator
{
    public enum TMemoryState { Off, On }

    public class TMemory<T> where T : TANumber, new()
    {
        private T fNumber;
        private TMemoryState fState;

        public TMemory(T defaultValue)
        {
            fNumber = defaultValue ?? new T();
            fState = TMemoryState.Off;
        }

        public TMemoryState GetState() => fState;
        public string GetStateString() => fState == TMemoryState.On ? "M" : "";

        public void Store(T e)
        {
            fNumber = e;
            fState = TMemoryState.On;
        }

        public T Take()
        {
            return fNumber;
        }

        public void Add(T e)
        {
            TANumber result = fNumber.Add(e);
            fNumber = result as T ?? new T();
            fState = TMemoryState.On;
        }

        public void Clear(T zeroValue)
        {
            fNumber = zeroValue ?? new T();
            fState = TMemoryState.Off;
        }
    }
}