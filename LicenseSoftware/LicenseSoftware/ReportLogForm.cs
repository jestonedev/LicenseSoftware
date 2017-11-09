using System;
using WeifenLuo.WinFormsUI.Docking;
using System.Globalization;

namespace LicenseSoftware
{
    internal sealed class ReportLogForm : DockContent
    {
        private System.Windows.Forms.RichTextBox _richTextBoxLog;

        public ReportLogForm()
        {
            InitializeComponent();
        }

        public void Log(string text)
        {
            _richTextBoxLog.AppendText((_richTextBoxLog.Lines.Length == 0 ? "" : Environment.NewLine) + 
                (_richTextBoxLog.Lines.Length + 1).ToString(CultureInfo.InvariantCulture) + ". " + text);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportLogForm));
            this._richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBoxLog
            // 
            this._richTextBoxLog.BackColor = System.Drawing.Color.White;
            this._richTextBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._richTextBoxLog.ForeColor = System.Drawing.Color.Black;
            this._richTextBoxLog.Location = new System.Drawing.Point(0, 0);
            this._richTextBoxLog.Name = "_richTextBoxLog";
            this._richTextBoxLog.ReadOnly = true;
            this._richTextBoxLog.Size = new System.Drawing.Size(572, 71);
            this._richTextBoxLog.TabIndex = 0;
            this._richTextBoxLog.Text = "";
            // 
            // ReportLogForm
            // 
            this.ClientSize = new System.Drawing.Size(572, 71);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this._richTextBoxLog);
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
