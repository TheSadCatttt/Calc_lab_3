using System;

namespace Calculator
{
    public enum TOprtn { None, Add, Sub, Mul, Dvd }
    public enum TFunc { Rev, Sqr }

    /// <summary>
    /// Процессор для выполнения арифметических операций над числами типа T
    /// Соответствует TProc из методички
    /// </summary>
    public class TProc<T> where T : TANumber, new()
    {
        private T lopRes;           // Левый операнд и результат (Lop_Res)
        private T rop;              // Правый операнд (Rop)
        private TOprtn operation;   // Текущая установленная операция
        private TOprtn lastOperation;     // Для повторения последней операции
        private T lastRightOperand;       // Для повторения последней операции

        /// <summary>Конструктор процессора</summary>
        public TProc(T leftDefault, T rightDefault)
        {
            lopRes = leftDefault ?? new T();
            rop = rightDefault ?? new T();
            operation = TOprtn.None;
            lastOperation = TOprtn.None;
            lastRightOperand = null;
        }

        /// <summary>Сброс процессора в начальное состояние</summary>
        public void Reset(T leftDefault, T rightDefault)
        {
            lopRes = leftDefault ?? new T();
            rop = rightDefault ?? new T();
            operation = TOprtn.None;
            lastOperation = TOprtn.None;
            lastRightOperand = null;
        }

        /// <summary>Сброс операции</summary>
        public void ClearOperation()
        {
            operation = TOprtn.None;
        }

        /// <summary>Установка операции</summary>
        public void SetOperation(TOprtn oprtn)
        {
            operation = oprtn;
            if (oprtn != TOprtn.None)
                lastOperation = oprtn;
        }

        /// <summary>Получить текущую операцию</summary>
        public TOprtn GetOperation() => operation;

        /// <summary>Получить последнюю выполненную операцию</summary>
        public TOprtn GetLastOperation() => lastOperation;

        /// <summary>Получить последний правый операнд</summary>
        public TANumber GetLastRightOperand() => lastRightOperand;

        /// <summary>Чтение левого операнда</summary>
        public TANumber GetLeftOperand() => lopRes;

        /// <summary>Запись левого операнда</summary>
        public void SetLeftOperand(TANumber operand)
        {
            if (operand is T typedOperand)
                lopRes = typedOperand;
            else
                throw new InvalidOperationException("Несовместимый тип операнда");
        }

        /// <summary>Чтение правого операнда</summary>
        public TANumber GetRightOperand() => rop;

        /// <summary>Запись правого операнда</summary>
        public void SetRightOperand(TANumber operand)
        {
            if (operand is T typedOperand)
            {
                rop = typedOperand;
                lastRightOperand = typedOperand.Copy() as T;
            }
            else
                throw new InvalidOperationException("Несовместимый тип операнда");
        }

        /// <summary>Очистка правого операнда (установка в 0)</summary>
        public void ClearRightOperand()
        {
            rop = new T();
        }

        /// <summary>Повторение последней операции с текущим левым операндом</summary>
        public TANumber RepeatLastOperation()
        {
            if (lastOperation == TOprtn.None || lastRightOperand == null)
                throw new InvalidOperationException("Нет операции для повторения");

            TANumber result = lastOperation switch
            {
                TOprtn.Add => lopRes.Add(lastRightOperand),
                TOprtn.Sub => lopRes.Subtract(lastRightOperand),
                TOprtn.Mul => lopRes.Multiply(lastRightOperand),
                TOprtn.Dvd => lopRes.Divide(lastRightOperand),
                _ => lopRes.Copy()
            };

            lopRes = result as T ?? new T();
            return lopRes;
        }

        /// <summary>Выполнить установленную операцию</summary>
        public TANumber RunOperation()
        {
            if (lopRes == null || rop == null)
                throw new InvalidOperationException("Операнды не установлены");

            TANumber result = operation switch
            {
                TOprtn.Add => lopRes.Add(rop),
                TOprtn.Sub => lopRes.Subtract(rop),
                TOprtn.Mul => lopRes.Multiply(rop),
                TOprtn.Dvd => lopRes.Divide(rop),
                _ => lopRes.Copy()
            };

            if (operation != TOprtn.None)
            {
                lastOperation = operation;
                lastRightOperand = rop.Copy() as T;
            }

            lopRes = result as T ?? new T();
            return lopRes;
        }

        /// <summary>Выполнить функцию над левым операндом</summary>
        public TANumber RunFunction(TFunc func)
        {
            TANumber result = func switch
            {
                TFunc.Rev => lopRes.Reciprocal(),
                TFunc.Sqr => lopRes.Square(),
                _ => lopRes.Copy()
            };

            lopRes = result as T ?? new T();
            return lopRes;
        }

        /// <summary>Выполнить функцию над заданным операндом</summary>
        public TANumber RunFunctionOnOperand(TFunc func, TANumber operand)
        {
            return func switch
            {
                TFunc.Rev => operand.Reciprocal(),
                TFunc.Sqr => operand.Square(),
                _ => operand.Copy()
            };
        }
    }
}