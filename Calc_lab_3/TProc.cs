using System;

namespace Calculator
{
    public enum TOprtn { None, Add, Sub, Mul, Dvd }
    public enum TFunc { Rev, Sqr }

    public class TProc<T> where T : class
    {
        private T lopRes;
        private T rop;
        private TOprtn operation;

        public TProc(T leftDefault, T rightDefault)
        {
            lopRes = leftDefault;
            rop = rightDefault;
            operation = TOprtn.None;
        }

        public void Reset(T leftDefault, T rightDefault)
        {
            lopRes = leftDefault;
            rop = rightDefault;
            operation = TOprtn.None;
        }

        public void ClearOperation() => operation = TOprtn.None;
        public TOprtn GetOperation() => operation;
        public void SetOperation(TOprtn oprtn) => operation = oprtn;

        public T GetLeftOperand() => lopRes;
        public void SetLeftOperand(T operand) => lopRes = operand;

        public T GetRightOperand() => rop;
        public void SetRightOperand(T operand) => rop = operand;

        public T RunOperation()
        {
            dynamic left = lopRes;
            dynamic right = rop;

            try
            {
                lopRes = operation switch
                {
                    TOprtn.Add => (T)(left.Add(right)),
                    TOprtn.Sub => (T)(left.Subtract(right)),
                    TOprtn.Mul => (T)(left.Multiply(right)),
                    TOprtn.Dvd => (T)(left.Divide(right)),
                    _ => lopRes
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка выполнения операции", ex);
            }
            return lopRes;
        }

        // Функция применяется к левому операнду (lopRes), результат сохраняется туда же
        public T RunFunction(TFunc func)
        {
            dynamic val = lopRes;

            try
            {
                lopRes = func switch
                {
                    TFunc.Rev => (T)(val.Reciprocal()),
                    TFunc.Sqr => (T)(val.Square()),
                    _ => lopRes
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка выполнения функции", ex);
            }
            return lopRes;
        }
    }
}