using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using System.Globalization;

namespace LicenseSoftware
{
    internal sealed class ReportLogForm : DockContent
    {
        private System.Windows.Forms.RichTextBox richTextBox1;

        public ReportLogForm()
        {
            InitializeComponent();
        }

        public void Log(string text)
        {
            richTextBox1.AppendText((richTextBox1.Lines.Length == 0 ? "" : Environment.NewLine) + 
                (richTextBox1.Lines.Length + 1).ToString(CultureInfo.InvariantCulture) + ". " + text);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportLogForm));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.White;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.ForeColor = System.Drawing.Color.Black;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(572, 71);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // ReportLogForm
            // 
            this.ClientSize = new System.Drawing.Size(572, 71);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.richTextBox1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportLogForm";
            this.TabText = "Лог отчетов";
            this.Text = "Лог отчетов";
            this.ResumeLayout(false);

        }
    }
}
