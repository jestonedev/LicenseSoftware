namespace LicenseSoftware.Viewport.SearchForms
{
    partial class SearchSoftwareForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchSoftwareForm));
            this.comboBoxSoftwareType = new System.Windows.Forms.ComboBox();
            this.checkBoxSoftwareTypeEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxSoftwareNameEnable = new System.Windows.Forms.CheckBox();
            this.textBoxSoftwareName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxSoftwareMaker = new System.Windows.Forms.ComboBox();
            this.checkBoxSoftwareMakerEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxSoftwareType
            // 
            this.comboBoxSoftwareType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSoftwareType.Enabled = false;
            this.comboBoxSoftwareType.FormattingEnabled = true;
            this.comboBoxSoftwareType.Location = new System.Drawing.Point(42, 67);
            this.comboBoxSoftwareType.Name = "comboBoxSoftwareType";
            this.comboBoxSoftwareType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxSoftwareType.TabIndex = 3;
            // 
            // checkBoxSoftwareTypeEnable
            // 
            this.checkBoxSoftwareTypeEnable.AutoSize = true;
            this.checkBoxSoftwareTypeEnable.Location = new System.Drawing.Point(17, 71);
            this.checkBoxSoftwareTypeEnable.Name = "checkBoxSoftwareTypeEnable";
            this.checkBoxSoftwareTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSoftwareTypeEnable.TabIndex = 2;
            this.checkBoxSoftwareTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxSoftwareTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxSoftwareTypeEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "Вид ПО";
            // 
            // vButtonCancel
            // 
            this.vButtonCancel.AllowAnimations = true;
            this.vButtonCancel.BackColor = System.Drawing.Color.Transparent;
            this.vButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButtonCancel.Location = new System.Drawing.Point(257, 145);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 7;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(119, 145);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 6;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // checkBoxSoftwareNameEnable
            // 
            this.checkBoxSoftwareNameEnable.AutoSize = true;
            this.checkBoxSoftwareNameEnable.Location = new System.Drawing.Point(18, 28);
            this.checkBoxSoftwareNameEnable.Name = "checkBoxSoftwareNameEnable";
            this.checkBoxSoftwareNameEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSoftwareNameEnable.TabIndex = 0;
            this.checkBoxSoftwareNameEnable.UseVisualStyleBackColor = true;
            this.checkBoxSoftwareNameEnable.CheckedChanged += new System.EventHandler(this.checkBoxSoftwareNameEnable_CheckedChanged);
            // 
            // textBoxSoftwareName
            // 
            this.textBoxSoftwareName.Enabled = false;
            this.textBoxSoftwareName.Location = new System.Drawing.Point(43, 25);
            this.textBoxSoftwareName.MaxLength = 255;
            this.textBoxSoftwareName.Name = "textBoxSoftwareName";
            this.textBoxSoftwareName.Size = new System.Drawing.Size(437, 21);
            this.textBoxSoftwareName.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 8);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 15);
            this.label11.TabIndex = 32;
            this.label11.Text = "Наименование ПО";
            // 
            // comboBoxSoftwareMaker
            // 
            this.comboBoxSoftwareMaker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSoftwareMaker.Enabled = false;
            this.comboBoxSoftwareMaker.FormattingEnabled = true;
            this.comboBoxSoftwareMaker.Location = new System.Drawing.Point(42, 110);
            this.comboBoxSoftwareMaker.Name = "comboBoxSoftwareMaker";
            this.comboBoxSoftwareMaker.Size = new System.Drawing.Size(437, 23);
            this.comboBoxSoftwareMaker.TabIndex = 5;
            // 
            // checkBoxSoftwareMakerEnable
            // 
            this.checkBoxSoftwareMakerEnable.AutoSize = true;
            this.checkBoxSoftwareMakerEnable.Location = new System.Drawing.Point(17, 114);
            this.checkBoxSoftwareMakerEnable.Name = "checkBoxSoftwareMakerEnable";
            this.checkBoxSoftwareMakerEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSoftwareMakerEnable.TabIndex = 4;
            this.checkBoxSoftwareMakerEnable.UseVisualStyleBackColor = true;
            this.checkBoxSoftwareMakerEnable.CheckedChanged += new System.EventHandler(this.checkBoxSoftwareMakerEnable_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 93);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(103, 15);
            this.label12.TabIndex = 36;
            this.label12.Text = "Разработчик ПО";
            // 
            // SearchSoftwareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(493, 189);
            this.Controls.Add(this.comboBoxSoftwareMaker);
            this.Controls.Add(this.checkBoxSoftwareMakerEnable);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.checkBoxSoftwareNameEnable);
            this.Controls.Add(this.textBoxSoftwareName);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboBoxSoftwareType);
            this.Controls.Add(this.checkBoxSoftwareTypeEnable);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.vButtonCancel);
            this.Controls.Add(this.vButtonSearch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchSoftwareForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация программного обеспечения";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButtonSearch;
        private VIBlend.WinForms.Controls.vButton vButtonCancel;
        private System.Windows.Forms.ComboBox comboBoxSoftwareType;
        private System.Windows.Forms.CheckBox checkBoxSoftwareTypeEnable;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxSoftwareNameEnable;
        private System.Windows.Forms.TextBox textBoxSoftwareName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBoxSoftwareMaker;
        private System.Windows.Forms.CheckBox checkBoxSoftwareMakerEnable;
        private System.Windows.Forms.Label label12;

    }
}