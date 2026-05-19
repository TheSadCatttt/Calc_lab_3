using System;
using System.Text;

namespace Calculator
{
    public enum TCtrlState { Start, Editing, OpSet, Result }
    public enum CalculatorMode { Real, Fraction, Complex }

    /// <summary>
    /// Управление универсальным калькулятором
    /// </summary>
    public class TCtrl
    {
        // Константы команд
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
        public const int CMD_IMAGINARY = 100;
        public const int CMD_FRACTION = 101; // Кнопка для ввода дроби (a/b)

        // Поля класса
        private TModeEditor editor;
        private object processor; // Будет хранить TProc<TPNumber>, TProc<TFrac> или TProc<TComplex>
        private object memory;    // Будет хранить TMemory<TPNumber>, TMemory<TFrac> или TMemory<TComplex>
        private TCtrlState state;
        private int currentBase;
        private int currentPrecision;
        private CalculatorMode currentMode;

        // Для истории
        private TFunc lastFunction;
        private bool lastWasFunction;
        private readonly StringBuilder historyBuilder = new StringBuilder();
        private string expressionInProgress = "";
        private string historyLeft = "";
        private string historyOp = "";

        // Свойства
        public TCtrlState State => state;
        public string Display => editor.GetString();
        public bool MemoryOn => GetMemoryState() == TMemoryState.On;
        public int CurrentBase => currentBase;
        public int CurrentPrecision => currentPrecision;
        public CalculatorMode GetMode() => currentMode;
        public string ExpressionInProgress => expressionInProgress;
        public string History => historyBuilder.ToString();

        // Конструктор
        public TCtrl(int numberBase = 10, int precision = 6, CalculatorMode mode = CalculatorMode.Real)
        {
            currentBase = numberBase;
            currentPrecision = precision;
            currentMode = mode;

            editor = new TModeEditor(numberBase, precision, ModeToEditorMode(mode));
            InitializeProcessorAndMemory();
            state = TCtrlState.Start;
            lastWasFunction = false;
        }

        private void InitializeProcessorAndMemory()
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    var zeroReal = new TPNumber(0, currentBase, currentPrecision);
                    processor = new TProc<TPNumber>(zeroReal, zeroReal);
                    memory = new TMemory<TPNumber>(zeroReal);
                    break;
                case CalculatorMode.Fraction:
                    var zeroFrac = new TFrac(0, 1, currentBase, currentPrecision);
                    processor = new TProc<TFrac>(zeroFrac, zeroFrac);
                    memory = new TMemory<TFrac>(zeroFrac);
                    break;
                case CalculatorMode.Complex:
                    var zeroComplex = new TComplex(0, 0, currentBase, currentPrecision);
                    processor = new TProc<TComplex>(zeroComplex, zeroComplex);
                    memory = new TMemory<TComplex>(zeroComplex);
                    break;
            }
        }

        private TModeEditor.NumberMode ModeToEditorMode(CalculatorMode mode) => mode switch
        {
            CalculatorMode.Real => TModeEditor.NumberMode.Real,
            CalculatorMode.Fraction => TModeEditor.NumberMode.Fraction,
            CalculatorMode.Complex => TModeEditor.NumberMode.Complex,
            _ => TModeEditor.NumberMode.Real
        };

        private TANumber CreateNumber(string value)
        {
            return currentMode switch
            {
                CalculatorMode.Real => new TPNumber(value, currentBase, currentPrecision),
                CalculatorMode.Fraction => new TFrac(value, currentBase, currentPrecision),
                CalculatorMode.Complex => new TComplex(value, currentBase, currentPrecision),
                _ => new TPNumber(value, currentBase, currentPrecision)
            };
        }

        private TANumber ReadCurrentNumber()
        {
            try
            {
                return CreateNumber(editor.GetString());
            }
            catch
            {
                return CreateNumber("0");
            }
        }

        private void WriteToEditor(TANumber value)
        {
            editor.SetString(value.ToString());
        }

        // Вспомогательные методы для работы с processor и memory
        private void SetLeftOperand(TANumber value)
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TProc<TPNumber>)processor).SetLeftOperand(value as TPNumber);
                    break;
                case CalculatorMode.Fraction:
                    ((TProc<TFrac>)processor).SetLeftOperand(value as TFrac);
                    break;
                case CalculatorMode.Complex:
                    ((TProc<TComplex>)processor).SetLeftOperand(value as TComplex);
                    break;
            }
        }

        private void SetRightOperand(TANumber value)
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TProc<TPNumber>)processor).SetRightOperand(value as TPNumber);
                    break;
                case CalculatorMode.Fraction:
                    ((TProc<TFrac>)processor).SetRightOperand(value as TFrac);
                    break;
                case CalculatorMode.Complex:
                    ((TProc<TComplex>)processor).SetRightOperand(value as TComplex);
                    break;
            }
        }

        private TANumber GetLeftOperand()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).GetLeftOperand() as TANumber,
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).GetLeftOperand() as TANumber,
                CalculatorMode.Complex => ((TProc<TComplex>)processor).GetLeftOperand() as TANumber,
                _ => null
            };
        }

        private TANumber RunOperation()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).RunOperation() as TANumber,
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).RunOperation() as TANumber,
                CalculatorMode.Complex => ((TProc<TComplex>)processor).RunOperation() as TANumber,
                _ => null
            };
        }

        private TANumber RunFunction(TFunc func)
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).RunFunction(func) as TANumber,
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).RunFunction(func) as TANumber,
                CalculatorMode.Complex => ((TProc<TComplex>)processor).RunFunction(func) as TANumber,
                _ => null
            };
        }

        private TANumber RunFunctionOnOperand(TFunc func, TANumber operand)
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).RunFunctionOnOperand(func, operand as TPNumber) as TANumber,
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).RunFunctionOnOperand(func, operand as TFrac) as TANumber,
                CalculatorMode.Complex => ((TProc<TComplex>)processor).RunFunctionOnOperand(func, operand as TComplex) as TANumber,
                _ => null
            };
        }

        private void ResetProcessor(TANumber left, TANumber right)
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TProc<TPNumber>)processor).Reset(left as TPNumber, right as TPNumber);
                    break;
                case CalculatorMode.Fraction:
                    ((TProc<TFrac>)processor).Reset(left as TFrac, right as TFrac);
                    break;
                case CalculatorMode.Complex:
                    ((TProc<TComplex>)processor).Reset(left as TComplex, right as TComplex);
                    break;
            }
        }

        private TMemoryState GetMemoryState()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TMemory<TPNumber>)memory).GetState(),
                CalculatorMode.Fraction => ((TMemory<TFrac>)memory).GetState(),
                CalculatorMode.Complex => ((TMemory<TComplex>)memory).GetState(),
                _ => TMemoryState.Off
            };
        }

        private void MemoryStore(TANumber value)
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TMemory<TPNumber>)memory).Store(value as TPNumber);
                    break;
                case CalculatorMode.Fraction:
                    ((TMemory<TFrac>)memory).Store(value as TFrac);
                    break;
                case CalculatorMode.Complex:
                    ((TMemory<TComplex>)memory).Store(value as TComplex);
                    break;
            }
        }

        private TANumber MemoryTake()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TMemory<TPNumber>)memory).Take() as TANumber,
                CalculatorMode.Fraction => ((TMemory<TFrac>)memory).Take() as TANumber,
                CalculatorMode.Complex => ((TMemory<TComplex>)memory).Take() as TANumber,
                _ => null
            };
        }

        private void MemoryAdd(TANumber value)
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TMemory<TPNumber>)memory).Add(value as TPNumber);
                    break;
                case CalculatorMode.Fraction:
                    ((TMemory<TFrac>)memory).Add(value as TFrac);
                    break;
                case CalculatorMode.Complex:
                    ((TMemory<TComplex>)memory).Add(value as TComplex);
                    break;
            }
        }

        private void MemoryClear(TANumber zero)
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TMemory<TPNumber>)memory).Clear(zero as TPNumber);
                    break;
                case CalculatorMode.Fraction:
                    ((TMemory<TFrac>)memory).Clear(zero as TFrac);
                    break;
                case CalculatorMode.Complex:
                    ((TMemory<TComplex>)memory).Clear(zero as TComplex);
                    break;
            }
        }

        private void ClearOperation()
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TProc<TPNumber>)processor).ClearOperation();
                    break;
                case CalculatorMode.Fraction:
                    ((TProc<TFrac>)processor).ClearOperation();
                    break;
                case CalculatorMode.Complex:
                    ((TProc<TComplex>)processor).ClearOperation();
                    break;
            }
        }

        private void SetOperation(TOprtn op)
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TProc<TPNumber>)processor).SetOperation(op);
                    break;
                case CalculatorMode.Fraction:
                    ((TProc<TFrac>)processor).SetOperation(op);
                    break;
                case CalculatorMode.Complex:
                    ((TProc<TComplex>)processor).SetOperation(op);
                    break;
            }
        }

        private TOprtn GetOperation()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).GetOperation(),
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).GetOperation(),
                CalculatorMode.Complex => ((TProc<TComplex>)processor).GetOperation(),
                _ => TOprtn.None
            };
        }

        private TANumber GetLastRightOperand()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).GetLastRightOperand() as TANumber,
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).GetLastRightOperand() as TANumber,
                CalculatorMode.Complex => ((TProc<TComplex>)processor).GetLastRightOperand() as TANumber,
                _ => null
            };
        }

        private TOprtn GetLastOperation()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).GetLastOperation(),
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).GetLastOperation(),
                CalculatorMode.Complex => ((TProc<TComplex>)processor).GetLastOperation(),
                _ => TOprtn.None
            };
        }

        private void ClearRightOperand()
        {
            switch (currentMode)
            {
                case CalculatorMode.Real:
                    ((TProc<TPNumber>)processor).ClearRightOperand();
                    break;
                case CalculatorMode.Fraction:
                    ((TProc<TFrac>)processor).ClearRightOperand();
                    break;
                case CalculatorMode.Complex:
                    ((TProc<TComplex>)processor).ClearRightOperand();
                    break;
            }
        }

        private TANumber RepeatLastOperation()
        {
            return currentMode switch
            {
                CalculatorMode.Real => ((TProc<TPNumber>)processor).RepeatLastOperation() as TANumber,
                CalculatorMode.Fraction => ((TProc<TFrac>)processor).RepeatLastOperation() as TANumber,
                CalculatorMode.Complex => ((TProc<TComplex>)processor).RepeatLastOperation() as TANumber,
                _ => null
            };
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

        // Публичные методы
        public void SetMode(CalculatorMode newMode)
        {
            if (currentMode == newMode) return;

            currentMode = newMode;
            editor.SetMode(ModeToEditorMode(newMode));
            InitializeProcessorAndMemory();
            ExecuteReset();
        }

        public void SetBase(int newBase)
        {
            if (newBase == currentBase) return;
            currentBase = newBase;
            editor.SetBase(newBase);

            try
            {
                TANumber current = ReadCurrentNumber();
                WriteToEditor(current);
            }
            catch
            {
                editor.Clear();
            }
        }

        public void SetPrecision(int newPrecision)
        {
            if (newPrecision == currentPrecision) return;
            currentPrecision = newPrecision;
            editor.SetPrecision(newPrecision);

            try
            {
                TANumber current = ReadCurrentNumber();
                WriteToEditor(current);
            }
            catch
            {
                editor.Clear();
            }
        }

        public string ExecuteEditorCommand(int cmd)
        {
            // В режиме Fraction кнопка CMD_FRACTION
            if (currentMode == CalculatorMode.Fraction && cmd == CMD_FRACTION)
            {
                editor.Edit(cmd);
                if (state == TCtrlState.Start)
                    state = TCtrlState.Editing;
                return editor.GetString();
            }

            // В режиме Complex кнопка CMD_IMAGINARY
            if (currentMode == CalculatorMode.Complex && cmd == CMD_IMAGINARY)
            {
                editor.Edit(cmd);
                if (state == TCtrlState.Start)
                    state = TCtrlState.Editing;
                return editor.GetString();
            }

            // Если в состоянии Result и начали ввод - начинаем новое число
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
            TANumber current = ReadCurrentNumber();
            TOprtn op;
            string opSymbol;

            switch (cmd)
            {
                case CMD_ADD: op = TOprtn.Add; opSymbol = "+"; break;
                case CMD_SUB: op = TOprtn.Sub; opSymbol = "-"; break;
                case CMD_MUL: op = TOprtn.Mul; opSymbol = "×"; break;
                case CMD_DIV: op = TOprtn.Dvd; opSymbol = "÷"; break;
                default: return editor.GetString();
            }

            lastWasFunction = false;

            if (state == TCtrlState.Editing && GetOperation() != TOprtn.None)
            {
                SetRightOperand(current);
                TANumber result = RunOperation();

                SetLeftOperand(result);
                SetOperation(op);
                ClearRightOperand();

                WriteToEditor(result);
                state = TCtrlState.OpSet;

                historyLeft = result.ToString();
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
                return editor.GetString();
            }

            if (state == TCtrlState.Result)
            {
                SetLeftOperand(current);
                SetOperation(op);
                state = TCtrlState.OpSet;

                historyLeft = current.ToString();
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
                editor.Clear();
                return editor.GetString();
            }

            if (state == TCtrlState.OpSet)
            {
                SetOperation(op);
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
            }
            else
            {
                SetLeftOperand(current);
                SetOperation(op);

                historyLeft = current.ToString();
                historyOp = opSymbol;
                expressionInProgress = $"{historyLeft} {historyOp}";
            }

            state = TCtrlState.OpSet;
            editor.Clear();

            return GetLeftOperand()?.ToString() ?? "0";
        }

        public string ExecuteEqual()
        {
            if (state == TCtrlState.Result && lastWasFunction)
            {
                try
                {
                    TANumber current = ReadCurrentNumber();
                    SetLeftOperand(current);
                    TANumber result = RunFunction(lastFunction);
                    string funcName = lastFunction == TFunc.Sqr ? "sqr" : "1/";
                    AppendHistory($"{funcName}({current}) = {result}");
                    expressionInProgress = "";
                    WriteToEditor(result);
                    SetLeftOperand(result);
                    state = TCtrlState.Result;
                    return editor.GetString();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Нельзя повторить функцию", ex);
                }
            }

            if (state == TCtrlState.Result)
            {
                try
                {
                    string leftBefore = GetLeftOperand()?.ToString() ?? "";
                    string rightRepeat = GetLastRightOperand()?.ToString() ?? "";
                    string opRepeat = OpSymbol(GetLastOperation());

                    TANumber result = RepeatLastOperation();
                    AppendHistory($"{leftBefore} {opRepeat} {rightRepeat} = {result}");
                    expressionInProgress = "";
                    WriteToEditor(result);
                    SetLeftOperand(result);
                    lastWasFunction = false;
                    state = TCtrlState.Result;
                    return editor.GetString();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Нельзя повторить операцию", ex);
                }
            }

            TANumber currentVal = ReadCurrentNumber();
            SetRightOperand(currentVal);

            string rightStr = currentVal.ToString();
            string fullExpr = $"{historyLeft} {historyOp} {rightStr}";

            TANumber operationResult = RunOperation();

            AppendHistory($"{fullExpr} = {operationResult}");
            expressionInProgress = "";
            historyLeft = "";
            historyOp = "";

            WriteToEditor(operationResult);
            SetLeftOperand(operationResult);
            ClearOperation();
            lastWasFunction = false;

            state = TCtrlState.Result;
            return editor.GetString();
        }

        public string ExecuteFunction(int cmd)
        {
            TANumber current = ReadCurrentNumber();
            TFunc func = cmd == CMD_SQR ? TFunc.Sqr : TFunc.Rev;
            string funcName = func == TFunc.Sqr ? "sqr" : "1/";

            TANumber result;

            if (state == TCtrlState.OpSet || (state == TCtrlState.Editing && GetOperation() != TOprtn.None))
            {
                result = RunFunctionOnOperand(func, current);
                SetRightOperand(result);
                WriteToEditor(result);
                expressionInProgress = $"{historyLeft} {historyOp} {funcName}({current})";
                state = TCtrlState.OpSet;
            }
            else if (state == TCtrlState.Result)
            {
                SetLeftOperand(current);
                result = RunFunction(func);
                AppendHistory($"{funcName}({current}) = {result}");
                expressionInProgress = "";
                WriteToEditor(result);
                SetLeftOperand(result);
                ClearOperation();
                state = TCtrlState.Result;
            }
            else
            {
                SetLeftOperand(current);
                result = RunFunction(func);
                AppendHistory($"{funcName}({current}) = {result}");
                expressionInProgress = "";
                WriteToEditor(result);
                SetLeftOperand(result);
                ClearOperation();
                state = TCtrlState.Result;
            }

            lastWasFunction = true;
            lastFunction = func;

            return editor.GetString();
        }

        public string ExecuteMemory(int cmd)
        {
            TANumber current = ReadCurrentNumber();
            var zero = CreateNumber("0");

            switch (cmd)
            {
                case CMD_MS:
                    MemoryStore(current);
                    break;

                case CMD_MR:
                    TANumber memValue = MemoryTake();
                    if (memValue != null)
                    {
                        WriteToEditor(memValue);

                        if (GetOperation() != TOprtn.None)
                        {
                            SetRightOperand(memValue);
                            expressionInProgress = $"{historyLeft} {historyOp} {memValue}";
                            state = TCtrlState.OpSet;
                        }
                        else
                        {
                            SetLeftOperand(memValue);
                            ClearOperation();
                            expressionInProgress = "";
                            state = TCtrlState.Result;
                        }
                    }
                    lastWasFunction = false;
                    break;

                case CMD_MP:
                    MemoryAdd(current);
                    break;

                case CMD_MC:
                    MemoryClear(zero);
                    break;
            }
            return editor.GetString();
        }

        public string ExecuteReset()
        {
            editor.Clear();
            var zero = CreateNumber("0");
            ResetProcessor(zero, zero);
            state = TCtrlState.Start;
            lastWasFunction = false;
            expressionInProgress = "";
            historyLeft = "";
            historyOp = "";
            return editor.GetString();
        }

        public void ClearHistory()
        {
            historyBuilder.Clear();
        }

        public string CopyToClipboard() => editor.GetString();

        public string PasteFromClipboard(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return editor.GetString();

            try
            {
                TANumber parsed = CreateNumber(text.Trim());
                WriteToEditor(parsed);
                state = TCtrlState.Result;
                SetLeftOperand(parsed);
                ClearOperation();
                lastWasFunction = false;
                expressionInProgress = "";
            }
            catch
            {
                // Если не удалось распарсить - игнорируем
            }
            return editor.GetString();
        }
    }
}