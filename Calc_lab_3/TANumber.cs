using System;

namespace Calculator
{

    // Абстрактный базовый класс для всех типов чисел
    public abstract class TANumber
    {
        // Основные арифметические операции (должны возвращать конкретный тип)
        public abstract TANumber Add(TANumber other);
        public abstract TANumber Subtract(TANumber other);
        public abstract TANumber Multiply(TANumber other);
        public abstract TANumber Divide(TANumber other);

        // Унарные операции
        public abstract TANumber Square();
        public abstract TANumber Reciprocal();
        public abstract TANumber Negate();

        // Проверки
        public abstract bool IsZero();
        public abstract bool Equals(TANumber other);

        // Копирование и строковое представление
        public abstract TANumber Copy();
        public abstract override string ToString();

        // Свойства для совместимости с существующей архитектурой
        public abstract int GetBase();
        public abstract int GetPrecision();

        // Для поддержки dynamic в TProc
        public virtual double GetNumber() => 0;
    }
}