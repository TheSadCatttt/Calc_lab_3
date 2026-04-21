namespace Calculator
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVersion;
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblAuthors = new System.Windows.Forms.Label();
            this.lblGroup = new System.Windows.Forms.Label();
            this.lblTask = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 20);
            this.lblTitle.Size = new System.Drawing.Size(360, 30);
            this.lblTitle.Text = "Калькулятор p-ичных чисел";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblVersion.Location = new System.Drawing.Point(12, 55);
            this.lblVersion.Size = new System.Drawing.Size(360, 25);
            this.lblVersion.Text = "Версия: 1.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblAuthors.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAuthors.Location = new System.Drawing.Point(12, 85);
            this.lblAuthors.Size = new System.Drawing.Size(360, 25);
            this.lblAuthors.Text = "Разработчики: Весёлый Д.А., Ворончук И.А., Лыкова М.А.";
            this.lblAuthors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblGroup.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblGroup.Location = new System.Drawing.Point(12, 115);
            this.lblGroup.Size = new System.Drawing.Size(360, 25);
            this.lblGroup.Text = "Группа: ПМИ-32";
            this.lblGroup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.lblTask.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTask.Location = new System.Drawing.Point(12, 150);
            this.lblTask.Size = new System.Drawing.Size(360, 40);
            this.lblTask.Text = "Лабораторная работа №3\nКалькулятор p-ичных чисел";
            this.lblTask.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnOK.Location = new System.Drawing.Point(150, 210);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 30);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;

            this.ClientSize = new System.Drawing.Size(384, 260);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblTask);
            this.Controls.Add(this.lblGroup);
            this.Controls.Add(this.lblAuthors);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "О программе";
            this.ResumeLayout(false);
        }
    }
}