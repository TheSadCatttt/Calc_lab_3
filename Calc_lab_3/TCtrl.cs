using System;
using System.Text;

namespace Calculator
{
    public enum TCtrlState { Start, Editing, OpSet, Result }

    public class TCtrl
    {
        public const int CMD_DIGIT_0 = 0;
        public const int CMD_DIGIT_1 = 1;
        public const int CMD_DIGIT_2 = 2;
        public const int CMD_DIGIT_3 = 3;
        public const int CMD_DIGIT_4 = 4;
        public const int CMD_DIGIT_5 = 5;
        public const int CMD_DIGIT_6 = 6;
        public const int CMD_DIGIT_7 = 7;
        public const int CMD_DIGIT_8 = 8;
        public const int CMD_DIGIT_9 = 9;
        public const int CMD_DIGIT_A = 10;
        public const int CMD_DIGIT_B = 11;
        public const int CMD_DIGIT_C = 12;
        public const int CMD_DIGIT_D = 13;
        public const int CMD_DIGIT_E = 14;
        public const int CMD_DIGIT_F = 15;
        public const int CMD_SEPARATOR = 16;
        public const int CMD_BACKSPACE = 17;
        public const int CMD_CLEAR = 18;
        public const int CMD_SIGN = 20;
        public const int CMD_ADD = 21;
        public const int CMD_SUB = 22;
        public const int CMD_MUL = 23;
        public const int CMD_DIV = 24;
        public const int CMD_EQUAL = 25;
        public const int CMD_SQR = 26;
        public const int CMD_REV = 27;
        public const int CMD_MC = 28;
        public const int CMD_MS = 29;
        public const int CMD_MR = 30;
        public const int CMD_MP = 31;
        public const int CMD_COPY = 32;
        public const int CMD_PASTE = 33;
        public const int CMD_RESET = 34;

        private TEditor editor;
        private TProc<TPNumber> processor;
        private TMemory<TPNumber> memory;
        private TCtrlState state;
        private int currentBase;
        private int currentPrecision;

        // Для хранения последней выполненной функции (для повторения через Enter)
        private TFunc lastFunction;
        private bool lastWasFunction;

        // ── История вычислений ───────────────────────────────────────────────
        // Хранит строки вида "3 + sqr(2) = 7"
        private readonly StringBuilder historyBuilder = new StringBuilder();

        // Промежуточное выражение, которое отображается ДО нажатия =
        // Например: "3 +" или "3 + sqr(2)"
        private string expressionInProgress = "";

        // Левый операнд в виде строки для истории
        private string historyLeft = "";

        // Символ операции для истории
        private string historyOp = "";

        public string ExpressionInProgress => expressionInProgress;
        public string History => historyBuilder.ToString();

        public TCtrl(int numberBase = 10, int precision = 10)
        {
            currentBase = numberBase;
            currentPrecision = precision;
            editor = new TEditor(numberBase);
            var zero = new TPNumber(0, numberBase, precision);
            processor = new TProc<TPNumber>(zero, zero);
            memory = new TMemory<TPNumber>(zero);
            state = TCtrlState.Start;
            lastWasFunction = false;
        }

        public TCtrlState State => state;
        public string Display => editor.GetString();
        public bool MemoryOn => memory.GetState() == TMemoryState.On;
        public int CurrentBase => currentBase;

        public void SetBase(int newBase)
        {
            if (newBase == currentBase) return;

            currentBase = newBase;
            editor.SetBase(newBase);

            try
            {
                TPNumber current = new TPNumber(editor.GetString(), currentBase, currentPrecision);
                editor.SetString(current.ToString());
            }
            catch
            {
                editor.Clear();
            }
        }

        public string ExecuteEditorCommand(int cmd)
        {
            // Если мы в состоянии Result и пользователь начал ввод цифры или разделителя - начинаем новый ввод
            if (state == TCtrlState.Result && (cmd >= 0 && cmd <= 16 || cmd == CMD_SIGN))
            {
                editor.Clear();
                state = TCtrlState.Editing;
                lastWasFunction = false;
                expressionInProgress = "";
                historyLeft = "";
                historyOp = "";
            }
            else if (state == TCtrlState.OpSet && (cmd >= 0 && cmd <= 16))
            {
                editor.Clear();
                state = TCtrlState.Editing;
            }

            string result = editor.Edit(cmd);

            if (state == TCtrlState.Start)
                state = TCtrlState.Editing;

            return result;
        }

        public string ExecuteOperation(int cmd)
        {
            TPNumber current = ReadCurrentNumber();
            TOprtn op;
            string opSymbol;

            switch (cmd)
            {
                case CMD_ADD: op = TOprtn.Add; opSymbol = "+"; break;
                case CMD_SUB: op = TOprtn.Sub; opSymbol = "-"; break;
                case CMD_MUL: op = TOprtn.Mul; opSymbol = "×"; break;
                case CMD_DIV: op = TOprtn.Dvd; opSymbol = "÷"; break;
                default: return Display;
            }

            lastWasFunction = false;

            // Если в состоянии Editing и есть ожидающая операция - сначала вычисляем
            if (state == TCtrlState.Editing && processor.GetOperation() != TOprtn.None)
            {
                processor.SetRightOperand(current);
                TPNumber result = processor.RunOperation();

                processor.SetLeftOperand(result);
                processor.SetOperation(op);
                processor.ClearRightOperand();

                WriteToEditor(result);
                state = TCtrlState.OpSet;

                // Обновляем историю выражения
                historyLeft = result.ToString();
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
                return Display;
            }

            // Если мы в состоянии Result, используем текущее число как левый операнд
            if (state == TCtrlState.Result)
            {
                processor.SetLeftOperand(current);
                processor.SetOperation(op);
                state = TCtrlState.OpSet;

                historyLeft = current.ToString();
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
                editor.Clear();
                return Display;
            }

            if (state == TCtrlState.OpSet)
            {
                // Смена операции без ввода правого — просто обновляем операцию
                processor.SetOperation(op);
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
            }
            else
            {
                // Start или Editing без ожидающей операции
                processor.SetLeftOperand(current);
                processor.SetOperation(op);

                historyLeft = current.ToString();
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
            }

            state = TCtrlState.OpSet;
            editor.Clear();

            return processor.GetLeftOperand().ToString();
        }

        public string ExecuteEqual()
        {
            // Если была выполнена функция и теперь нажали Enter - повторяем функцию
            if (state == TCtrlState.Result && lastWasFunction)
            {
                try
                {
                    TPNumber current = ReadCurrentNumber();
                    processor.SetLeftOperand(current);
                    TPNumber result = processor.RunFunction(lastFunction);
                    string funcName = lastFunction == TFunc.Sqr ? "sqr" : "1/";
                    AppendHistory($"{funcName}({current}) = {result}");
                    expressionInProgress = "";
                    WriteToEditor(result);
                    processor.SetLeftOperand(result);
                    return Display;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Нельзя повторить функцию", ex);
                }
            }

            // Если в состоянии Result - повторяем последнюю операцию
            if (state == TCtrlState.Result)
            {
                try
                {
                    string leftBefore = processor.GetLeftOperand().ToString();
                    string rightRepeat = processor.GetLastRightOperand()?.ToString() ?? "";
                    string opRepeat = OpSymbol(processor.GetLastOperation());

                    TPNumber result = processor.RepeatLastOperation();
                    AppendHistory($"{leftBefore} {opRepeat} {rightRepeat} = {result}");
                    expressionInProgress = "";
                    WriteToEditor(result);
                    processor.SetLeftOperand(result);
                    lastWasFunction = false;
                    return Display;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Нельзя повторить операцию", ex);
                }
            }

            // Обычное выполнение операции
            TPNumber currentVal = ReadCurrentNumber();
            processor.SetRightOperand(currentVal);

            // Формируем строку для истории
            string rightStr = currentVal.ToString();
            string fullExpr = $"{historyLeft} {historyOp} {rightStr}";

            TPNumber operationResult = processor.RunOperation();

            AppendHistory($"{fullExpr} = {operationResult}");
            expressionInProgress = "";
            historyLeft = "";
            historyOp = "";

            WriteToEditor(operationResult);
            processor.SetLeftOperand(operationResult);
            processor.ClearOperation();
            lastWasFunction = false;

            state = TCtrlState.Result;
            return Display;
        }

        public string ExecuteFunction(int cmd)
        {
            TPNumber current = ReadCurrentNumber();
            TFunc func = cmd == CMD_SQR ? TFunc.Sqr : TFunc.Rev;
            string funcName = func == TFunc.Sqr ? "sqr" : "1/";

            TPNumber result;

            // ── ИСПРАВЛЕНИЕ БАГ #3 ──────────────────────────────────────────
            // Если есть ожидающая операция (состояние OpSet ИЛИ Editing с операцией)
            // функция применяется к ПРАВОМУ операнду, левый и операция не трогаются.
            if (state == TCtrlState.OpSet ||
                (state == TCtrlState.Editing && processor.GetOperation() != TOprtn.None))
            {
                // Применяем функцию к текущему числу (правому операнду)
                result = processor.RunFunctionOnOperand(func, current);
                // Сохраняем результат как правый операнд
                processor.SetRightOperand(result);
                // Показываем результат на дисплее, не трогая левый операнд и операцию
                WriteToEditor(result);
                // Обновляем выражение в строке истории
                expressionInProgress = $"{historyLeft} {historyOp} {funcName}({current})";
                state = TCtrlState.OpSet;
            }
            else if (state == TCtrlState.Result)
            {
                // Применяем функцию к текущему результату
                processor.SetLeftOperand(current);
                result = processor.RunFunction(func);
                AppendHistory($"{funcName}({current}) = {result}");
                expressionInProgress = "";
                WriteToEditor(result);
                processor.SetLeftOperand(result);
                processor.ClearOperation();
                state = TCtrlState.Result;
            }
            else
            {
                // Start или Editing без ожидающей операции
                processor.SetLeftOperand(current);
                result = processor.RunFunction(func);
                AppendHistory($"{funcName}({current}) = {result}");
                expressionInProgress = "";
                WriteToEditor(result);
                processor.SetLeftOperand(result);
                processor.ClearOperation();
                state = TCtrlState.Result;
            }

            lastWasFunction = true;
            lastFunction = func;

            return Display;
        }

        public string ExecuteMemory(int cmd)
        {
            TPNumber current = ReadCurrentNumber();
            var zero = new TPNumber(0, currentBase, currentPrecision);

            switch (cmd)
            {
                case CMD_MS:
                    memory.Store(current);
                    break;

                case CMD_MR:
                    TPNumber memValue = memory.Take() as TPNumber;
                    TPNumber converted = memValue != null
                        ? new TPNumber(memValue.GetNumber(), currentBase, currentPrecision)
                        : zero;

                    WriteToEditor(converted);

                    // ── ИСПРАВЛЕНИЕ БАГ #1 ──────────────────────────────────────────
                    // Если есть ожидающая операция, число из памяти становится
                    // правым операндом — остаёмся в OpSet, чтобы = выполнило операцию.
                    // Если операции нет — это просто загрузка значения (Result).
                    if (processor.GetOperation() != TOprtn.None)
                    {
                        processor.SetRightOperand(converted);
                        expressionInProgress = $"{historyLeft} {historyOp} {converted}";
                        state = TCtrlState.OpSet;
                    }
                    else
                    {
                        processor.SetLeftOperand(converted);
                        processor.ClearOperation();
                        expressionInProgress = "";
                        state = TCtrlState.Result;
                    }
                    lastWasFunction = false;
                    break;

                case CMD_MP:
                    TPNumber memVal = memory.Take() as TPNumber;
                    if (memVal != null)
                    {
                        TPNumber memConverted = new TPNumber(memVal.GetNumber(), currentBase, currentPrecision);
                        TPNumber sum = memConverted.Add(current);
                        memory.Store(sum);
                    }
                    else
                    {
                        memory.Add(current);
                    }
                    break;

                case CMD_MC:
                    memory.Clear(zero);
                    break;
            }
            return Display;
        }

        public string ExecuteReset()
        {
            editor.Clear();
            var zero = new TPNumber(0, currentBase, currentPrecision);
            processor.Reset(zero, zero);
            state = TCtrlState.Start;
            lastWasFunction = false;
            expressionInProgress = "";
            historyLeft = "";
            historyOp = "";
            return Display;
        }

        public void ClearHistory()
        {
            historyBuilder.Clear();
        }

        public string CopyToClipboard() => Display;

        public string PasteFromClipboard(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Display;

            try
            {
                TPNumber parsed = new TPNumber(text.Trim(), currentBase, currentPrecision);
                WriteToEditor(parsed);
                state = TCtrlState.Result;
                processor.SetLeftOperand(parsed);
                processor.ClearOperation();
                lastWasFunction = false;
                expressionInProgress = "";
            }
            catch
            {
                // Если не удалось распарсить, игнорируем
            }
            return Display;
        }

        // ── Вспомогательные методы ───────────────────────────────────────────

        private TPNumber ReadCurrentNumber()
        {
            try
            {
                return new TPNumber(editor.GetString(), currentBase, currentPrecision);
            }
            catch
            {
                return new TPNumber(0, currentBase, currentPrecision);
            }
        }

        private void WriteToEditor(TPNumber value)
        {
            editor.SetString(value.ToString());
        }

        private void AppendHistory(string line)
        {
            if (historyBuilder.Length > 0)
                historyBuilder.AppendLine();
            historyBuilder.Append(line);
        }

        private static string OpSymbol(TOprtn op) => op switch
        {
            TOprtn.Add => "+",
            TOprtn.Sub => "-",
            TOprtn.Mul => "×",
            TOprtn.Dvd => "÷",
            _ => ""
        };
    }
}