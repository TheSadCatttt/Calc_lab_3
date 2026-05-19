using System;
using System.Drawing;
using System.Windows.Forms;

namespace Calculator
{
    public partial class MainForm : Form
    {
        private TCtrl controller;

        public MainForm()
        {
            InitializeComponent();

            // Инициализация массивов
            if (digitButtons == null)
            {
                digitButtons = new Button[] {
                    btnDigit0, btnDigit1, btnDigit2, btnDigit3, btnDigit4, btnDigit5,
                    btnDigit6, btnDigit7, btnDigit8, btnDigit9,
                    btnDigitA, btnDigitB, btnDigitC, btnDigitD, btnDigitE, btnDigitF
                };
            }
            if (opButtons == null)
            {
                opButtons = new Button[] { btnAdd, btnSub, btnMul, btnDiv, btnSqr, btnRev };
            }
            if (memButtons == null)
            {
                memButtons = new Button[] { btnMC, btnMR, btnMS, btnMP };
            }

            // Настройка динамической кнопки в последней ячейке
            SetupDynamicButton();

            // Создание контроллера (по умолчанию p-ичные числа, основание 10, точность 6)
            controller = new TCtrl(10, 6, CalculatorMode.Real);

            SubscribeEvents();
            UpdateUI();
            UpdateDigitButtons();
        }

        private void SetupDynamicButton()
        {
            // Очищаем ячейку (5,5)
            var existingControl = buttonPanel.GetControlFromPosition(5, 5);
            if (existingControl != null)
                buttonPanel.Controls.Remove(existingControl);

            // Добавляем кнопку i по умолчанию
            buttonPanel.Controls.Add(btnImaginary, 5, 5);
        }

        private void SwapDynamicButton(CalculatorMode mode)
        {
            var currentControl = buttonPanel.GetControlFromPosition(5, 5);
            if (currentControl != null)
                buttonPanel.Controls.Remove(currentControl);

            if (mode == CalculatorMode.Fraction)
            {
                buttonPanel.Controls.Add(btnFraction, 5, 5);
                btnFraction.Enabled = true;
                btnImaginary.Enabled = false;
            }
            else if (mode == CalculatorMode.Complex)
            {
                buttonPanel.Controls.Add(btnImaginary, 5, 5);
                btnImaginary.Enabled = true;
                btnFraction.Enabled = false;
            }
            else
            {
                // Для Real режима показываем i, но отключаем её
                buttonPanel.Controls.Add(btnImaginary, 5, 5);
                btnImaginary.Enabled = false;
                btnFraction.Enabled = false;
            }
        }

        private void SubscribeEvents()
        {
            // Изменение основания
            numBase.ValueChanged += (s, e) =>
            {
                controller.SetBase((int)numBase.Value);
                UpdateUI();
                UpdateDigitButtons();
            };

            // Меню: копирование/вставка
            copyItem.Click += (s, e) => CopyToClipboard();
            pasteItem.Click += (s, e) => PasteFromClipboard();
            aboutItem.Click += (s, e) => ShowAbout();

            // Меню: точность
            precision0.Click += (s, e) => SetPrecision(0);
            precision2.Click += (s, e) => SetPrecision(2);
            precision4.Click += (s, e) => SetPrecision(4);
            precision6.Click += (s, e) => SetPrecision(6);
            precision8.Click += (s, e) => SetPrecision(8);

            // Переключение режимов
            rbReal.CheckedChanged += (s, e) =>
            {
                if (rbReal.Checked) SwitchMode(CalculatorMode.Real);
            };
            rbFraction.CheckedChanged += (s, e) =>
            {
                if (rbFraction.Checked) SwitchMode(CalculatorMode.Fraction);
            };
            rbComplex.CheckedChanged += (s, e) =>
            {
                if (rbComplex.Checked) SwitchMode(CalculatorMode.Complex);
            };

            // Цифровые кнопки
            for (int i = 0; i < digitButtons.Length; i++)
                digitButtons[i].Click += DigitClick;

            // Кнопки операций
            for (int i = 0; i < opButtons.Length; i++)
                opButtons[i].Click += OpClick;

            // Кнопки памяти
            for (int i = 0; i < memButtons.Length; i++)
                memButtons[i].Click += MemClick;

            // Специальные кнопки
            btnSeparator.Click += SpecialClick;
            btnSign.Click += SpecialClick;
            btnBackspace.Click += SpecialClick;
            btnClear.Click += SpecialClick;
            btnReset.Click += (s, e) => ExecuteCommand(TCtrl.CMD_RESET);
            btnEqual.Click += (s, e) => ExecuteCommand(TCtrl.CMD_EQUAL);
            btnImaginary.Click += (s, e) => ExecuteCommand(TCtrl.CMD_IMAGINARY);
            btnFraction.Click += (s, e) => ExecuteCommand(TCtrl.CMD_FRACTION); // Новая кнопка дроби

            // Клавиатура
            this.KeyPress += OnKeyPress;

            AddTooltips();
        }

        private void AddTooltips()
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(btnSeparator, "Разделитель целой и дробной части");
            tt.SetToolTip(btnSign, "Сменить знак числа");
            tt.SetToolTip(btnBackspace, "Удалить последний символ");
            tt.SetToolTip(btnClear, "Очистить ввод (CE)");
            tt.SetToolTip(btnReset, "Полный сброс (C)");
            tt.SetToolTip(btnEqual, "Вычислить");
            tt.SetToolTip(btnImaginary, "Мнимая единица (только для комплексных чисел)");
            tt.SetToolTip(btnFraction, "Разделитель числителя и знаменателя (только для дробей)");
            tt.SetToolTip(btnMC, "Очистить память");
            tt.SetToolTip(btnMR, "Восстановить из памяти");
            tt.SetToolTip(btnMS, "Сохранить в память");
            tt.SetToolTip(btnMP, "Добавить к памяти");
            tt.SetToolTip(numBase, "Основание системы счисления (2-16)");
            tt.SetToolTip(txtDisplay, "Строка ввода/вывода");
            tt.SetToolTip(rbReal, "Режим p-ичных чисел");
            tt.SetToolTip(rbFraction, "Режим обыкновенных дробей");
            tt.SetToolTip(rbComplex, "Режим комплексных чисел");
        }

        private void SwitchMode(CalculatorMode mode)
        {
            controller.SetMode(mode);
            SwapDynamicButton(mode);
            UpdateUI();
            UpdateDigitButtons();
        }

        private void SetPrecision(int precision)
        {
            controller.SetPrecision(precision);
            UpdateUI();
            UpdateDigitButtons();

            // Обновляем галочки в меню
            foreach (ToolStripMenuItem item in precisionMenuItem.DropDownItems)
                item.Checked = ((int)item.Tag == precision);
        }

        private void DigitClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int digit = (int)btn.Tag;
            ExecuteCommand(digit);
        }

        private void OpClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int cmd = (int)btn.Tag;
            ExecuteCommand(cmd);
        }

        private void MemClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int cmd = (int)btn.Tag;
            ExecuteCommand(cmd);
        }

        private void SpecialClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int cmd = (int)btn.Tag;
            ExecuteCommand(cmd);
        }

        private void ExecuteCommand(int cmd)
        {
            try
            {
                switch (cmd)
                {
                    case >= 0 and <= 15:           // Цифры 0-F
                    case TCtrl.CMD_SEPARATOR:       // Запятая
                    case TCtrl.CMD_BACKSPACE:       // Backspace
                    case TCtrl.CMD_CLEAR:           // CE
                    case TCtrl.CMD_SIGN:            // +/-
                    case TCtrl.CMD_IMAGINARY:       // i
                    case TCtrl.CMD_FRACTION:        // a/b (для дробей)
                        controller.ExecuteEditorCommand(cmd);
                        break;

                    case TCtrl.CMD_ADD:             // +
                    case TCtrl.CMD_SUB:             // -
                    case TCtrl.CMD_MUL:             // ×
                    case TCtrl.CMD_DIV:             // ÷
                        controller.ExecuteOperation(cmd);
                        break;

                    case TCtrl.CMD_SQR:             // x²
                    case TCtrl.CMD_REV:             // 1/x
                        controller.ExecuteFunction(cmd);
                        break;

                    case TCtrl.CMD_EQUAL:           // =
                        controller.ExecuteEqual();
                        break;

                    case TCtrl.CMD_MC:              // Очистить память
                    case TCtrl.CMD_MS:              // Сохранить в память
                    case TCtrl.CMD_MR:              // Восстановить из памяти
                    case TCtrl.CMD_MP:              // Добавить к памяти
                        controller.ExecuteMemory(cmd);
                        break;

                    case TCtrl.CMD_RESET:           // Полный сброс
                        controller.ExecuteReset();
                        break;
                }
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetUserMessage(ex), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateUI();
            }
        }

        private void CopyToClipboard()
        {
            string value = controller.CopyToClipboard();
            Clipboard.SetText(value);
        }

        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsText())
            {
                try
                {
                    controller.PasteFromClipboard(Clipboard.GetText());
                    UpdateUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(GetUserMessage(ex), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ShowAbout()
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }

        private void UpdateUI()
        {
            txtDisplay.Text = controller.Display;
            lblMemory.Text = controller.MemoryOn ? "M" : "";

            // Для комплексных чисел показываем подсказку, какая часть редактируется
            if (controller.GetMode() == CalculatorMode.Complex)
            {
                // Можно сделать разный цвет или тултип
                // Пока просто меняем подпись на кнопке i
                btnImaginary.Text = controller.IsEditingReal ? "Re" : "Im";
            }
            else
            {
                btnImaginary.Text = "i";
            }

            // Обновляем состояние радио-кнопок
            CalculatorMode currentMode = controller.GetMode();
            rbReal.Checked = (currentMode == CalculatorMode.Real);
            rbFraction.Checked = (currentMode == CalculatorMode.Fraction);
            rbComplex.Checked = (currentMode == CalculatorMode.Complex);

            UpdateDigitButtons();
        }

        private void UpdateDigitButtons()
        {
            int curBase = controller.CurrentBase;
            for (int i = 0; i < digitButtons.Length; i++)
            {
                if (digitButtons[i] != null)
                    digitButtons[i].Enabled = (i < curBase);
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            // Цифры 0-9
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                int digit = e.KeyChar - '0';
                if (digit < controller.CurrentBase)
                    ExecuteCommand(digit);
                e.Handled = true;
            }
            // Буквы A-F
            else if (e.KeyChar >= 'A' && e.KeyChar <= 'F')
            {
                int digit = e.KeyChar - 'A' + 10;
                if (digit < controller.CurrentBase)
                    ExecuteCommand(digit);
                e.Handled = true;
            }
            else if (e.KeyChar >= 'a' && e.KeyChar <= 'f')
            {
                int digit = e.KeyChar - 'a' + 10;
                if (digit < controller.CurrentBase)
                    ExecuteCommand(digit);
                e.Handled = true;
            }
            // Разделитель
            else if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                ExecuteCommand(TCtrl.CMD_SEPARATOR);
                e.Handled = true;
            }
            // Для дробей: клавиша / добавляет разделитель дроби
            else if (e.KeyChar == '/')
            {
                if (controller.GetMode() == CalculatorMode.Fraction)
                    ExecuteCommand(TCtrl.CMD_FRACTION);
                else
                    ExecuteCommand(TCtrl.CMD_DIV);
                e.Handled = true;
            }
            // Операции
            else if (e.KeyChar == '+')
            {
                ExecuteCommand(TCtrl.CMD_ADD);
                e.Handled = true;
            }
            else if (e.KeyChar == '-')
            {
                ExecuteCommand(TCtrl.CMD_SUB);
                e.Handled = true;
            }
            else if (e.KeyChar == '*')
            {
                ExecuteCommand(TCtrl.CMD_MUL);
                e.Handled = true;
            }
            // Равно и Enter
            else if (e.KeyChar == '=' || e.KeyChar == (char)Keys.Enter)
            {
                ExecuteCommand(TCtrl.CMD_EQUAL);
                e.Handled = true;
            }
            // Backspace
            else if (e.KeyChar == '\b')
            {
                ExecuteCommand(TCtrl.CMD_BACKSPACE);
                e.Handled = true;
            }
            // Буква i (для комплексных чисел)
            else if (e.KeyChar == 'i' || e.KeyChar == 'I')
            {
                if (controller.GetMode() == CalculatorMode.Complex)
                    ExecuteCommand(TCtrl.CMD_IMAGINARY);
                e.Handled = true;
            }
        }

        private string GetUserMessage(Exception ex)
        {
            if (ex is DivideByZeroException)
                return "Деление на ноль недопустимо.";
            if (ex is FormatException)
                return "Некорректный формат числа для текущего режима и основания.";
            if (ex is ArgumentOutOfRangeException)
                return "Основание должно быть в диапазоне 2..16.";
            if (ex.InnerException != null)
                return GetUserMessage(ex.InnerException);
            return $"Произошла ошибка: {ex.Message}";
        }
    }
}