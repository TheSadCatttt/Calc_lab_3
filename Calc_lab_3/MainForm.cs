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
            controller = new TCtrl(10, 6);
            SubscribeEvents();
            UpdateUI();
        }

        private void SubscribeEvents()
        {
            numBase.ValueChanged += (s, e) =>
            {
                controller.SetBase((int)numBase.Value);
                UpdateUI();
                UpdateDigitButtons();
            };

            copyItem.Click += (s, e) => CopyToClipboard();
            pasteItem.Click += (s, e) => PasteFromClipboard();
            aboutItem.Click += (s, e) => ShowAbout();

            precision0.Click += (s, e) => SetPrecision(0);
            precision2.Click += (s, e) => SetPrecision(2);
            precision4.Click += (s, e) => SetPrecision(4);
            precision6.Click += (s, e) => SetPrecision(6);
            precision8.Click += (s, e) => SetPrecision(8);

            // Цифровые кнопки
            for (int i = 0; i < digitButtons.Length; i++)
                digitButtons[i].Click += DigitClick;

            // Операции
            for (int i = 0; i < opButtons.Length; i++)
                opButtons[i].Click += OpClick;

            // Память
            for (int i = 0; i < memButtons.Length; i++)
                memButtons[i].Click += MemClick;

            // Специальные
            btnSeparator.Click += SpecialClick;
            btnSign.Click += SpecialClick;
            btnBackspace.Click += SpecialClick;
            btnClear.Click += SpecialClick;
            btnReset.Click += (s, e) => ExecuteCommand(TCtrl.CMD_RESET);
            btnEqual.Click += (s, e) => ExecuteCommand(TCtrl.CMD_EQUAL);

            this.KeyPress += OnKeyPress;

            AddTooltips();
        }

        private void AddTooltips()
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(btnSeparator, "Разделитель целой и дробной части");
            tt.SetToolTip(btnSign, "Сменить знак числа");
            tt.SetToolTip(btnBackspace, "Удалить последний символ");
            tt.SetToolTip(btnClear, "Очистить ввод");
            tt.SetToolTip(btnReset, "Полный сброс");
            tt.SetToolTip(btnEqual, "Вычислить");
            tt.SetToolTip(memButtons[0], "Очистить память");
            tt.SetToolTip(memButtons[1], "Восстановить из памяти");
            tt.SetToolTip(memButtons[2], "Сохранить в память");
            tt.SetToolTip(memButtons[3], "Добавить к памяти");
            tt.SetToolTip(numBase, "Основание системы счисления (2-16)");
            tt.SetToolTip(txtDisplay, "Строка ввода/вывода");
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
                    case >= 0 and <= 15:
                    case TCtrl.CMD_SEPARATOR:
                    case TCtrl.CMD_BACKSPACE:
                    case TCtrl.CMD_CLEAR:
                    case TCtrl.CMD_SIGN:
                        controller.ExecuteEditorCommand(cmd);
                        break;
                    case TCtrl.CMD_ADD:
                    case TCtrl.CMD_SUB:
                    case TCtrl.CMD_MUL:
                    case TCtrl.CMD_DIV:
                        controller.ExecuteOperation(cmd);
                        break;
                    case TCtrl.CMD_SQR:
                    case TCtrl.CMD_REV:
                        controller.ExecuteFunction(cmd);
                        break;
                    case TCtrl.CMD_EQUAL:
                        controller.ExecuteEqual();
                        break;
                    case TCtrl.CMD_MC:
                    case TCtrl.CMD_MS:
                    case TCtrl.CMD_MR:
                    case TCtrl.CMD_MP:
                        controller.ExecuteMemory(cmd);
                        break;
                    case TCtrl.CMD_RESET:
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

        private void SetPrecision(int precision)
        {
            controller = new TCtrl(controller.CurrentBase, precision);
            UpdateUI();
            UpdateDigitButtons();

            foreach (ToolStripMenuItem item in precisionMenuItem.DropDownItems)
                item.Checked = (int)item.Tag == precision;
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
            UpdateDigitButtons();
        }

        private void UpdateDigitButtons()
        {
            int curBase = controller.CurrentBase;
            for (int i = 0; i < 16; i++)
            {
                if (digitButtons[i] != null)
                    digitButtons[i].Enabled = i < curBase;
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                int digit = e.KeyChar - '0';
                if (digit < controller.CurrentBase)
                    ExecuteCommand(digit);
                e.Handled = true;
            }
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
            else if (e.KeyChar == '.' || e.KeyChar == ',') { ExecuteCommand(TCtrl.CMD_SEPARATOR); e.Handled = true; }
            else if (e.KeyChar == '+') { ExecuteCommand(TCtrl.CMD_ADD); e.Handled = true; }
            else if (e.KeyChar == '-') { ExecuteCommand(TCtrl.CMD_SUB); e.Handled = true; }
            else if (e.KeyChar == '*') { ExecuteCommand(TCtrl.CMD_MUL); e.Handled = true; }
            else if (e.KeyChar == '/') { ExecuteCommand(TCtrl.CMD_DIV); e.Handled = true; }
            else if (e.KeyChar == '=' || e.KeyChar == (char)Keys.Enter) { ExecuteCommand(TCtrl.CMD_EQUAL); e.Handled = true; }
            else if (e.KeyChar == '\b') { ExecuteCommand(TCtrl.CMD_BACKSPACE); e.Handled = true; }
        }

        private string GetUserMessage(Exception ex)
        {
            if (ex is DivideByZeroException) return "Деление на ноль недопустимо.";
            if (ex is FormatException) return "Некорректный формат числа для текущего основания.";
            if (ex is ArgumentOutOfRangeException) return "Основание должно быть в диапазоне 2..16.";
            if (ex.InnerException != null) return GetUserMessage(ex.InnerException);
            return "Произошла ошибка. Проверьте введённые данные.";
        }
    }
}