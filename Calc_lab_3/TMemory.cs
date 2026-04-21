namespace Calculator
{
    public enum TMemoryState { Off, On }

    public class TMemory<T> where T : class
    {
        private T fNumber;
        private TMemoryState fState;

        public TMemory(T defaultValue)
        {
            fNumber = defaultValue;
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
            dynamic dynNumber = fNumber;
            dynamic dynE = e;
            fNumber = (T)(dynNumber.Add(dynE));
            fState = TMemoryState.On;
        }

        public void Clear(T zeroValue)
        {
            fNumber = zeroValue;
            fState = TMemoryState.Off;
        }
    }
}