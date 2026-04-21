namespace Calculator
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblAuthors;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.Label lblTask;
        private System.Windows.Forms.Button btnOK;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblAuthors = new Label();
            lblGroup = new Label();
            lblTask = new Label();
            btnOK = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(12, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(360, 30);
            lblTitle.TabIndex = 10;
            lblTitle.Text = "Калькулятор p-ичных чисел";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAuthors
            // 
            lblAuthors.Font = new Font("Segoe UI", 10F);
            lblAuthors.Location = new Point(12, 50);
            lblAuthors.Name = "lblAuthors";
            lblAuthors.Size = new Size(360, 25);
            lblAuthors.TabIndex = 8;
            lblAuthors.Text = "Программу разработали: Андреева Д., Шурыгина А.";
            lblAuthors.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblGroup
            // 
            lblGroup.Font = new Font("Segoe UI", 10F);
            lblGroup.Location = new Point(12, 99);
            lblGroup.Name = "lblGroup";
            lblGroup.Size = new Size(360, 25);
            lblGroup.TabIndex = 7;
            lblGroup.Text = "Группа: ПМИ-32";
            lblGroup.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTask
            // 
            lblTask.Font = new Font("Segoe UI", 9F);
            lblTask.Location = new Point(12, 150);
            lblTask.Name = "lblTask";
            lblTask.Size = new Size(360, 40);
            lblTask.TabIndex = 6;
            lblTask.Text = "Лабораторная работа №3\nКалькулятор p-ичных чисел";
            lblTask.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(150, 210);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 30);
            btnOK.TabIndex = 5;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // AboutForm
            // 
            ClientSize = new Size(384, 260);
            Controls.Add(btnOK);
            Controls.Add(lblTask);
            Controls.Add(lblGroup);
            Controls.Add(lblAuthors);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "О программе";
            ResumeLayout(false);
        }
    }
}