namespace Calculator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // Отображение
        private System.Windows.Forms.TextBox txtDisplay;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.Label lblBase;
        private System.Windows.Forms.NumericUpDown numBase;

        // Меню
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem copyItem;
        private System.Windows.Forms.ToolStripMenuItem pasteItem;
        private System.Windows.Forms.ToolStripMenuItem settingsMenu;
        private System.Windows.Forms.ToolStripMenuItem precisionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem precision0;
        private System.Windows.Forms.ToolStripMenuItem precision2;
        private System.Windows.Forms.ToolStripMenuItem precision4;
        private System.Windows.Forms.ToolStripMenuItem precision6;
        private System.Windows.Forms.ToolStripMenuItem precision8;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutItem;

        // Панель кнопок
        private System.Windows.Forms.TableLayoutPanel buttonPanel;

        // Цифровые кнопки
        private System.Windows.Forms.Button[] digitButtons;
        private System.Windows.Forms.Button btnDigit0;
        private System.Windows.Forms.Button btnDigit1;
        private System.Windows.Forms.Button btnDigit2;
        private System.Windows.Forms.Button btnDigit3;
        private System.Windows.Forms.Button btnDigit4;
        private System.Windows.Forms.Button btnDigit5;
        private System.Windows.Forms.Button btnDigit6;
        private System.Windows.Forms.Button btnDigit7;
        private System.Windows.Forms.Button btnDigit8;
        private System.Windows.Forms.Button btnDigit9;
        private System.Windows.Forms.Button btnDigitA;
        private System.Windows.Forms.Button btnDigitB;
        private System.Windows.Forms.Button btnDigitC;
        private System.Windows.Forms.Button btnDigitD;
        private System.Windows.Forms.Button btnDigitE;
        private System.Windows.Forms.Button btnDigitF;

        // Кнопки операций
        private System.Windows.Forms.Button[] opButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.Button btnMul;
        private System.Windows.Forms.Button btnDiv;
        private System.Windows.Forms.Button btnSqr;
        private System.Windows.Forms.Button btnRev;

        // Кнопки памяти
        private System.Windows.Forms.Button[] memButtons;
        private System.Windows.Forms.Button btnMC;
        private System.Windows.Forms.Button btnMR;
        private System.Windows.Forms.Button btnMS;
        private System.Windows.Forms.Button btnMP;

        // Специальные кнопки
        private System.Windows.Forms.Button btnSeparator;
        private System.Windows.Forms.Button btnSign;
        private System.Windows.Forms.Button btnBackspace;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnEqual;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtDisplay = new System.Windows.Forms.TextBox();
            this.lblMemory = new System.Windows.Forms.Label();
            this.lblBase = new System.Windows.Forms.Label();
            this.numBase = new System.Windows.Forms.NumericUpDown();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.copyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.precisionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.precision0 = new System.Windows.Forms.ToolStripMenuItem();
            this.precision2 = new System.Windows.Forms.ToolStripMenuItem();
            this.precision4 = new System.Windows.Forms.ToolStripMenuItem();
            this.precision6 = new System.Windows.Forms.ToolStripMenuItem();
            this.precision8 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutItem = new System.Windows.Forms.ToolStripMenuItem();

            this.buttonPanel = new System.Windows.Forms.TableLayoutPanel();

            // Цифры
            this.btnDigit0 = new System.Windows.Forms.Button();
            this.btnDigit1 = new System.Windows.Forms.Button();
            this.btnDigit2 = new System.Windows.Forms.Button();
            this.btnDigit3 = new System.Windows.Forms.Button();
            this.btnDigit4 = new System.Windows.Forms.Button();
            this.btnDigit5 = new System.Windows.Forms.Button();
            this.btnDigit6 = new System.Windows.Forms.Button();
            this.btnDigit7 = new System.Windows.Forms.Button();
            this.btnDigit8 = new System.Windows.Forms.Button();
            this.btnDigit9 = new System.Windows.Forms.Button();
            this.btnDigitA = new System.Windows.Forms.Button();
            this.btnDigitB = new System.Windows.Forms.Button();
            this.btnDigitC = new System.Windows.Forms.Button();
            this.btnDigitD = new System.Windows.Forms.Button();
            this.btnDigitE = new System.Windows.Forms.Button();
            this.btnDigitF = new System.Windows.Forms.Button();

            // Операции
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.btnMul = new System.Windows.Forms.Button();
            this.btnDiv = new System.Windows.Forms.Button();
            this.btnSqr = new System.Windows.Forms.Button();
            this.btnRev = new System.Windows.Forms.Button();

            // Память
            this.btnMC = new System.Windows.Forms.Button();
            this.btnMR = new System.Windows.Forms.Button();
            this.btnMS = new System.Windows.Forms.Button();
            this.btnMP = new System.Windows.Forms.Button();

            // Специальные
            this.btnSeparator = new System.Windows.Forms.Button();
            this.btnSign = new System.Windows.Forms.Button();
            this.btnBackspace = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnEqual = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.numBase)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();

            // ── txtDisplay ──────────────────────────────────────────────────
            this.txtDisplay.Font = new System.Drawing.Font("Consolas", 18F);
            this.txtDisplay.Location = new System.Drawing.Point(12, 35);
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.ReadOnly = true;
            this.txtDisplay.Size = new System.Drawing.Size(390, 35);
            this.txtDisplay.TabIndex = 0;
            this.txtDisplay.Text = "0";
            this.txtDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDisplay.BackColor = System.Drawing.Color.White;

            // ── lblMemory ───────────────────────────────────────────────────
            this.lblMemory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMemory.Location = new System.Drawing.Point(12, 10);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(40, 25);
            this.lblMemory.Text = "";
            this.lblMemory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── lblBase ─────────────────────────────────────────────────────
            this.lblBase.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBase.Location = new System.Drawing.Point(295, 10);
            this.lblBase.Name = "lblBase";
            this.lblBase.Size = new System.Drawing.Size(45, 25);
            this.lblBase.Text = "Осн:";
            this.lblBase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ── numBase ─────────────────────────────────────────────────────
            this.numBase.Location = new System.Drawing.Point(342, 12);
            this.numBase.Minimum = 2;
            this.numBase.Maximum = 16;
            this.numBase.Name = "numBase";
            this.numBase.Size = new System.Drawing.Size(60, 22);
            this.numBase.TabIndex = 1;
            this.numBase.Value = 10;

            // ── mainMenu ────────────────────────────────────────────────────
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.editMenu, this.settingsMenu, this.helpMenu });
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(420, 24);
            this.mainMenu.TabIndex = 2;

            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.copyItem, this.pasteItem });
            this.editMenu.Name = "editMenu";
            this.editMenu.Text = "Правка";

            this.copyItem.Name = "copyItem";
            this.copyItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;
            this.copyItem.Text = "Копировать";

            this.pasteItem.Name = "pasteItem";
            this.pasteItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V;
            this.pasteItem.Text = "Вставить";

            this.settingsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.precisionMenuItem });
            this.settingsMenu.Name = "settingsMenu";
            this.settingsMenu.Text = "Настройка";

            this.precisionMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.precision0, this.precision2, this.precision4, this.precision6, this.precision8 });
            this.precisionMenuItem.Name = "precisionMenuItem";
            this.precisionMenuItem.Text = "Точность";

            this.precision0.Name = "precision0"; this.precision0.Text = "0"; this.precision0.Tag = 0;
            this.precision2.Name = "precision2"; this.precision2.Text = "2"; this.precision2.Tag = 2;
            this.precision4.Name = "precision4"; this.precision4.Text = "4"; this.precision4.Tag = 4;
            this.precision6.Name = "precision6"; this.precision6.Text = "6"; this.precision6.Tag = 6; this.precision6.Checked = true;
            this.precision8.Name = "precision8"; this.precision8.Text = "8"; this.precision8.Tag = 8;

            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.aboutItem });
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Text = "Справка";

            this.aboutItem.Name = "aboutItem";
            this.aboutItem.Text = "О программе";

            // ── Вспомогательный метод настройки кнопки ──────────────────────
            System.Drawing.Font fontNormal = new System.Drawing.Font("Segoe UI", 10F);
            System.Drawing.Font fontSmall = new System.Drawing.Font("Segoe UI", 9F);
            System.Drawing.Font fontLarge = new System.Drawing.Font("Segoe UI", 12F);
            System.Drawing.Font fontBoldLg = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            System.Drawing.Color opColor = System.Drawing.Color.FromArgb(240, 240, 240);
            System.Drawing.Color memColor = System.Drawing.Color.FromArgb(220, 220, 220);

            // ── Цифры ────────────────────────────────────────────────────────
            string[] digitLabels = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            System.Windows.Forms.Button[] allDigits = {
                btnDigit0, btnDigit1, btnDigit2, btnDigit3, btnDigit4, btnDigit5,
                btnDigit6, btnDigit7, btnDigit8, btnDigit9,
                btnDigitA, btnDigitB, btnDigitC, btnDigitD, btnDigitE, btnDigitF
            };
            for (int i = 0; i < 16; i++)
            {
                allDigits[i].Dock = System.Windows.Forms.DockStyle.Fill;
                allDigits[i].Font = fontNormal;
                allDigits[i].Margin = new System.Windows.Forms.Padding(2);
                allDigits[i].Name = "btnDigit" + digitLabels[i];
                allDigits[i].Tag = i;
                allDigits[i].Text = digitLabels[i];
            }
            this.digitButtons = allDigits;

            // ── Операции ─────────────────────────────────────────────────────
            string[] opLabels = { "+", "-", "×", "÷", "x²", "1/x" };
            int[] opTags = { TCtrl.CMD_ADD, TCtrl.CMD_SUB, TCtrl.CMD_MUL, TCtrl.CMD_DIV, TCtrl.CMD_SQR, TCtrl.CMD_REV };
            System.Windows.Forms.Button[] allOps = { btnAdd, btnSub, btnMul, btnDiv, btnSqr, btnRev };
            for (int i = 0; i < 6; i++)
            {
                allOps[i].Dock = System.Windows.Forms.DockStyle.Fill;
                allOps[i].Font = fontNormal;
                allOps[i].Margin = new System.Windows.Forms.Padding(2);
                allOps[i].BackColor = opColor;
                allOps[i].Tag = opTags[i];
                allOps[i].Text = opLabels[i];
            }
            this.opButtons = allOps;

            // ── Память ───────────────────────────────────────────────────────
            string[] memLabels = { "MC", "MR", "MS", "M+" };
            int[] memTags = { TCtrl.CMD_MC, TCtrl.CMD_MR, TCtrl.CMD_MS, TCtrl.CMD_MP };
            System.Windows.Forms.Button[] allMem = { btnMC, btnMR, btnMS, btnMP };
            for (int i = 0; i < 4; i++)
            {
                allMem[i].Dock = System.Windows.Forms.DockStyle.Fill;
                allMem[i].Font = fontSmall;
                allMem[i].Margin = new System.Windows.Forms.Padding(2);
                allMem[i].BackColor = memColor;
                allMem[i].Tag = memTags[i];
                allMem[i].Text = memLabels[i];
            }
            this.memButtons = allMem;

            // ── Специальные ──────────────────────────────────────────────────
            this.btnSeparator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSeparator.Font = fontLarge;
            this.btnSeparator.Margin = new System.Windows.Forms.Padding(2);
            this.btnSeparator.Name = "btnSeparator";
            this.btnSeparator.Tag = TCtrl.CMD_SEPARATOR;
            this.btnSeparator.Text = ",";

            this.btnSign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSign.Font = fontSmall;
            this.btnSign.Margin = new System.Windows.Forms.Padding(2);
            this.btnSign.Name = "btnSign";
            this.btnSign.Tag = TCtrl.CMD_SIGN;
            this.btnSign.Text = "+/-";

            this.btnBackspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBackspace.Font = fontNormal;
            this.btnBackspace.Margin = new System.Windows.Forms.Padding(2);
            this.btnBackspace.Name = "btnBackspace";
            this.btnBackspace.Tag = TCtrl.CMD_BACKSPACE;
            this.btnBackspace.Text = "←";

            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.Font = fontNormal;
            this.btnClear.Margin = new System.Windows.Forms.Padding(2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Tag = TCtrl.CMD_CLEAR;
            this.btnClear.Text = "CE";

            this.btnReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReset.Font = fontNormal;
            this.btnReset.Margin = new System.Windows.Forms.Padding(2);
            this.btnReset.Name = "btnReset";
            this.btnReset.Tag = TCtrl.CMD_RESET;
            this.btnReset.Text = "C";

            this.btnEqual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEqual.Font = fontBoldLg;
            this.btnEqual.Margin = new System.Windows.Forms.Padding(2);
            this.btnEqual.Name = "btnEqual";
            this.btnEqual.Tag = TCtrl.CMD_EQUAL;
            this.btnEqual.Text = "=";
            this.btnEqual.BackColor = System.Drawing.Color.FromArgb(62, 163, 255);
            this.btnEqual.ForeColor = System.Drawing.Color.White;

            // ── TableLayoutPanel ─────────────────────────────────────────────
            this.buttonPanel.Location = new System.Drawing.Point(12, 80);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(396, 380);
            this.buttonPanel.ColumnCount = 6;
            this.buttonPanel.RowCount = 6;

            for (int i = 0; i < 6; i++)
                this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f / 6));
            for (int i = 0; i < 6; i++)
                this.buttonPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f / 6));

            // Строка 0: MC, MR, MS, M+, BS, CE
            this.buttonPanel.Controls.Add(this.btnMC, 0, 0);
            this.buttonPanel.Controls.Add(this.btnMR, 1, 0);
            this.buttonPanel.Controls.Add(this.btnMS, 2, 0);
            this.buttonPanel.Controls.Add(this.btnMP, 3, 0);
            this.buttonPanel.Controls.Add(this.btnBackspace, 4, 0);
            this.buttonPanel.Controls.Add(this.btnClear, 5, 0);

            // Строка 1: 7 8 9  ÷  x²
            this.buttonPanel.Controls.Add(this.btnDigit7, 0, 1);
            this.buttonPanel.Controls.Add(this.btnDigit8, 1, 1);
            this.buttonPanel.Controls.Add(this.btnDigit9, 2, 1);
            this.buttonPanel.Controls.Add(this.btnDiv, 3, 1);
            this.buttonPanel.Controls.Add(this.btnSqr, 4, 1);

            // Строка 2: 4 5 6  ×  1/x
            this.buttonPanel.Controls.Add(this.btnDigit4, 0, 2);
            this.buttonPanel.Controls.Add(this.btnDigit5, 1, 2);
            this.buttonPanel.Controls.Add(this.btnDigit6, 2, 2);
            this.buttonPanel.Controls.Add(this.btnMul, 3, 2);
            this.buttonPanel.Controls.Add(this.btnRev, 4, 2);

            // Строка 3: 1 2 3  -  C
            this.buttonPanel.Controls.Add(this.btnDigit1, 0, 3);
            this.buttonPanel.Controls.Add(this.btnDigit2, 1, 3);
            this.buttonPanel.Controls.Add(this.btnDigit3, 2, 3);
            this.buttonPanel.Controls.Add(this.btnSub, 3, 3);
            this.buttonPanel.Controls.Add(this.btnReset, 4, 3);

            // Строка 4: 0 A B  +  = (span 2)
            this.buttonPanel.Controls.Add(this.btnDigit0, 0, 4);
            this.buttonPanel.Controls.Add(this.btnDigitA, 1, 4);
            this.buttonPanel.Controls.Add(this.btnDigitB, 2, 4);
            this.buttonPanel.Controls.Add(this.btnAdd, 3, 4);
            this.buttonPanel.Controls.Add(this.btnEqual, 4, 4);
            this.buttonPanel.SetColumnSpan(this.btnEqual, 2);

            // Строка 5: C D E F  ,  +/-
            this.buttonPanel.Controls.Add(this.btnDigitC, 0, 5);
            this.buttonPanel.Controls.Add(this.btnDigitD, 1, 5);
            this.buttonPanel.Controls.Add(this.btnDigitE, 2, 5);
            this.buttonPanel.Controls.Add(this.btnDigitF, 3, 5);
            this.buttonPanel.Controls.Add(this.btnSeparator, 4, 5);
            this.buttonPanel.Controls.Add(this.btnSign, 5, 5);

            // ── MainForm ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 480);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "Калькулятор p-ичных чисел";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.numBase);
            this.Controls.Add(this.lblBase);
            this.Controls.Add(this.lblMemory);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.mainMenu);

            ((System.ComponentModel.ISupportInitialize)(this.numBase)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}