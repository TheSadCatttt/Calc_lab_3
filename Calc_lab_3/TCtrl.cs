using System;

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

        public TCtrl(int numberBase = 10, int precision = 10)
        {
            currentBase = numberBase;
            currentPrecision = precision;
            editor = new TEditor(numberBase);
            var zero = new TPNumber(0, numberBase, precision);
            processor = new TProc<TPNumber>(zero, zero);
            memory = new TMemory<TPNumber>(zero);
            state = TCtrlState.Start;
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
            if (state == TCtrlState.Result || state == TCtrlState.OpSet)
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

            switch (cmd)
            {
                case CMD_ADD: op = TOprtn.Add; break;
                case CMD_SUB: op = TOprtn.Sub; break;
                case CMD_MUL: op = TOprtn.Mul; break;
                case CMD_DIV: op = TOprtn.Dvd; break;
                default: return Display;
            }

            if (state == TCtrlState.OpSet)
            {
                processor.SetRightOperand(current);
                processor.RunOperation();
            }
            else
            {
                processor.SetLeftOperand(current);
            }

            processor.SetOperation(op);
            state = TCtrlState.OpSet;
            editor.Clear();

            TPNumber result = processor.GetLeftOperand();
            return result.ToString();
        }

        public string ExecuteFunction(int cmd)
        {
            // Кладём текущее число в левый операнд и применяем функцию к нему
            TPNumber current = ReadCurrentNumber();
            processor.SetLeftOperand(current);

            TFunc func = cmd == CMD_SQR ? TFunc.Sqr : TFunc.Rev;
            // RunFunction теперь работает с lopRes и возвращает результат
            TPNumber result = processor.RunFunction(func);

            state = TCtrlState.Result;
            WriteToEditor(result);
            return Display;
        }

        public string ExecuteEqual()
        {
            if (state != TCtrlState.Result)
            {
                processor.SetRightOperand(ReadCurrentNumber());
            }

            TPNumber result = processor.RunOperation();
            state = TCtrlState.Result;
            WriteToEditor(result);
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
                    WriteToEditor(memory.Take());
                    state = TCtrlState.Result;
                    break;
                case CMD_MP:
                    memory.Add(current);
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
            return Display;
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
            }
            catch
            {
                // Если не удалось распарсить, игнорируем
            }
            return Display;
        }

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
            editor.Clear();
            string text = value.ToString();
            foreach (char ch in text)
            {
                if (ch == '-')
                    editor.ToggleSign();
                else if (ch == ',')
                    editor.AddSeparator();
                else if (ch >= '0' && ch <= '9')
                    editor.AddDigit(ch - '0');
                else if (ch >= 'A' && ch <= 'F')
                    editor.AddDigit(ch - 'A' + 10);
            }
        }
    }
}