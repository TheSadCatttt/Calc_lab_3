using System;
using System.Windows.Forms;

namespace Calculator
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            btnOK.Click += (s, e) => Close();
        }
    }
}