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
        private TOprtn lastOperation; // Для повторения последней операции
        private T lastRightOperand;   // Для повторения последней операции

        public void ClearRightOperand()
        {
            if (lopRes != null)
            {
                dynamic left = lopRes;
                rop = (T)Activator.CreateInstance(typeof(T), 0, left.GetBase(), left.GetPrecision());
            }
            else
            {
                rop = null;
            }
        }

        public TOprtn GetOperation() => operation;
        public T RunFunctionOnOperand(TFunc func, T operand)
        {
            dynamic val = operand;

            try
            {
                return func switch
                {
                    TFunc.Rev => (T)(val.Reciprocal()),
                    TFunc.Sqr => (T)(val.Square()),
                    _ => operand
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка выполнения функции", ex);
            }
        }
        public TProc(T leftDefault, T rightDefault)
        {
            lopRes = leftDefault;
            rop = rightDefault;
            operation = TOprtn.None;
            lastOperation = TOprtn.None;
            lastRightOperand = null;
        }

        public void Reset(T leftDefault, T rightDefault)
        {
            lopRes = leftDefault;
            rop = rightDefault;
            operation = TOprtn.None;
            lastOperation = TOprtn.None;
            lastRightOperand = null;
        }

        public void ClearOperation() => operation = TOprtn.None;
        public void SetOperation(TOprtn oprtn)
        {
            operation = oprtn;
            // Запоминаем последнюю операцию только если это не None
            if (oprtn != TOprtn.None)
                lastOperation = oprtn;
        }

        public T GetLeftOperand() => lopRes;
        public void SetLeftOperand(T operand) => lopRes = operand;

        public T GetRightOperand() => rop;
        public void SetRightOperand(T operand)
        {
            rop = operand;
            // Запоминаем последний правый операнд для повторения
            lastRightOperand = operand;
        }

        // Повторение последней операции с текущим левым операндом
        public T RepeatLastOperation()
        {
            if (lastOperation == TOprtn.None || lastRightOperand == null)
                throw new InvalidOperationException("Нет операции для повторения");

            dynamic left = lopRes;
            dynamic right = lastRightOperand;

            try
            {
                switch (lastOperation)
                {
                    case TOprtn.Add:
                        lopRes = (T)(left.Add(right));
                        break;
                    case TOprtn.Sub:
                        lopRes = (T)(left.Subtract(right));
                        break;
                    case TOprtn.Mul:
                        lopRes = (T)(left.Multiply(right));
                        break;
                    case TOprtn.Dvd:
                        if (right.IsZero())
                            throw new DivideByZeroException("Деление на ноль");
                        lopRes = (T)(left.Divide(right));
                        break;
                    default:
                        return lopRes;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка выполнения операции", ex);
            }
            return lopRes;
        }

        public T RunOperation()
        {
            if (lopRes == null || rop == null)
                throw new InvalidOperationException("Операнды не установлены");

            dynamic left = lopRes;
            dynamic right = rop;

            try
            {
                Console.WriteLine($"RunOperation: left={left.GetNumber()}, right={right.GetNumber()}, op={operation}");

                switch (operation)
                {
                    case TOprtn.Add:
                        lopRes = (T)(left.Add(right));
                        break;
                    case TOprtn.Sub:
                        lopRes = (T)(left.Subtract(right));
                        break;
                    case TOprtn.Mul:
                        lopRes = (T)(left.Multiply(right));
                        break;
                    case TOprtn.Dvd:
                        if (right.IsZero())
                            throw new DivideByZeroException("Деление на ноль");
                        lopRes = (T)(left.Divide(right));
                        break;
                    default:
                        return lopRes;
                }

                // Запоминаем операцию и правый операнд для повторения
                if (operation != TOprtn.None)
                {
                    lastOperation = operation;
                    lastRightOperand = rop;
                }

                Console.WriteLine($"RunOperation result: {((TPNumber)(object)lopRes).GetNumber()}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка выполнения операции", ex);
            }
            return lopRes;
        }

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

        // Геттеры для последней операции (для отладки)
        public TOprtn GetLastOperation() => lastOperation;
        public T GetLastRightOperand() => lastRightOperand;
    }
}