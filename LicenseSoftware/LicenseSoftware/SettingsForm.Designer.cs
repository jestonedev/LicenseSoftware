namespace LicenseSoftware
{
    partial class SettingsForm
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.vButton2 = new VIBlend.WinForms.Controls.vButton();
            this.vButton1 = new VIBlend.WinForms.Controls.vButton();
            this.numericUpDownMaxDBConnectionCount = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxLDAPUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLDAPPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxActivityManagerPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxActivityManagerOutputCodepage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxActivityManagerConfigsPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownDataModelsCallbackUpdateTimeout = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownCalcDataModelsUpdateTimeout = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxUseLDAP = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxDBConnectionCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataModelsCallbackUpdateTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCalcDataModelsUpdateTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // vButton2
            // 
            this.vButton2.AllowAnimations = true;
            this.vButton2.BackColor = System.Drawing.Color.Transparent;
            this.vButton2.Location = new System.Drawing.Point(103, 406);
            this.vButton2.Name = "vButton2";
            this.vButton2.RoundedCornersMask = ((byte)(15));
            this.vButton2.Size = new System.Drawing.Size(117, 35);
            this.vButton2.TabIndex = 10;
            this.vButton2.Text = "Сохранить";
            this.vButton2.UseVisualStyleBackColor = false;
            this.vButton2.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButton2.Click += new System.EventHandler(this.vButton2_Click);
            // 
            // vButton1
            // 
            this.vButton1.AllowAnimations = true;
            this.vButton1.BackColor = System.Drawing.Color.Transparent;
            this.vButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.vButton1.Location = new System.Drawing.Point(239, 406);
            this.vButton1.Name = "vButton1";
            this.vButton1.RoundedCornersMask = ((byte)(15));
            this.vButton1.Size = new System.Drawing.Size(117, 35);
            this.vButton1.TabIndex = 11;
            this.vButton1.Text = "Отменить";
            this.vButton1.UseVisualStyleBackColor = false;
            this.vButton1.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // numericUpDownMaxDBConnectionCount
            // 
            this.numericUpDownMaxDBConnectionCount.Location = new System.Drawing.Point(12, 272);
            this.numericUpDownMaxDBConnectionCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxDBConnectionCount.Name = "numericUpDownMaxDBConnectionCount";
            this.numericUpDownMaxDBConnectionCount.Size = new System.Drawing.Size(432, 21);
            this.numericUpDownMaxDBConnectionCount.TabIndex = 6;
            this.numericUpDownMaxDBConnectionCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 255);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(338, 15);
            this.label9.TabIndex = 49;
            this.label9.Text = "Количество одновременных подключений к базе данных";
            // 
            // textBoxConnectionString
            // 
            this.textBoxConnectionString.Location = new System.Drawing.Point(12, 26);
            this.textBoxConnectionString.MaxLength = 16;
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            this.textBoxConnectionString.Size = new System.Drawing.Size(431, 21);
            this.textBoxConnectionString.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(121, 15);
            this.label10.TabIndex = 51;
            this.label10.Text = "Строка соединения";
            // 
            // textBoxLDAPUserName
            // 
            this.textBoxLDAPUserName.Location = new System.Drawing.Point(12, 67);
            this.textBoxLDAPUserName.MaxLength = 16;
            this.textBoxLDAPUserName.Name = "textBoxLDAPUserName";
            this.textBoxLDAPUserName.Size = new System.Drawing.Size(431, 21);
            this.textBoxLDAPUserName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 15);
            this.label1.TabIndex = 53;
            this.label1.Text = "Пользователь LDAP";
            // 
            // textBoxLDAPPassword
            // 
            this.textBoxLDAPPassword.Location = new System.Drawing.Point(12, 108);
            this.textBoxLDAPPassword.MaxLength = 16;
            this.textBoxLDAPPassword.Name = "textBoxLDAPPassword";
            this.textBoxLDAPPassword.PasswordChar = '•';
            this.textBoxLDAPPassword.Size = new System.Drawing.Size(431, 21);
            this.textBoxLDAPPassword.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 15);
            this.label2.TabIndex = 55;
            this.label2.Text = "Пароль LDAP";
            // 
            // textBoxActivityManagerPath
            // 
            this.textBoxActivityManagerPath.Location = new System.Drawing.Point(12, 149);
            this.textBoxActivityManagerPath.MaxLength = 16;
            this.textBoxActivityManagerPath.Name = "textBoxActivityManagerPath";
            this.textBoxActivityManagerPath.Size = new System.Drawing.Size(431, 21);
            this.textBoxActivityManagerPath.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 15);
            this.label3.TabIndex = 57;
            this.label3.Text = "Путь до менеджера отчетов";
            // 
            // textBoxActivityManagerOutputCodepage
            // 
            this.textBoxActivityManagerOutputCodepage.Location = new System.Drawing.Point(12, 190);
            this.textBoxActivityManagerOutputCodepage.MaxLength = 16;
            this.textBoxActivityManagerOutputCodepage.Name = "textBoxActivityManagerOutputCodepage";
            this.textBoxActivityManagerOutputCodepage.Size = new System.Drawing.Size(431, 21);
            this.textBoxActivityManagerOutputCodepage.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 173);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(236, 15);
            this.label4.TabIndex = 59;
            this.label4.Text = "Кодовая страница менеджера отчетов";
            // 
            // textBoxActivityManagerConfigsPath
            // 
            this.textBoxActivityManagerConfigsPath.Location = new System.Drawing.Point(12, 231);
            this.textBoxActivityManagerConfigsPath.MaxLength = 16;
            this.textBoxActivityManagerConfigsPath.Name = "textBoxActivityManagerConfigsPath";
            this.textBoxActivityManagerConfigsPath.Size = new System.Drawing.Size(431, 21);
            this.textBoxActivityManagerConfigsPath.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 214);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(267, 15);
            this.label5.TabIndex = 61;
            this.label5.Text = "Путь до конфигурационных файлов отчетов";
            // 
            // numericUpDownDataModelsCallbackUpdateTimeout
            // 
            this.numericUpDownDataModelsCallbackUpdateTimeout.Location = new System.Drawing.Point(11, 313);
            this.numericUpDownDataModelsCallbackUpdateTimeout.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownDataModelsCallbackUpdateTimeout.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownDataModelsCallbackUpdateTimeout.Name = "numericUpDownDataModelsCallbackUpdateTimeout";
            this.numericUpDownDataModelsCallbackUpdateTimeout.Size = new System.Drawing.Size(432, 21);
            this.numericUpDownDataModelsCallbackUpdateTimeout.TabIndex = 7;
            this.numericUpDownDataModelsCallbackUpdateTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 296);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(297, 15);
            this.label6.TabIndex = 64;
            this.label6.Text = "Периодичность обновления моделей данных (мс)";
            // 
            // numericUpDownCalcDataModelsUpdateTimeout
            // 
            this.numericUpDownCalcDataModelsUpdateTimeout.Location = new System.Drawing.Point(11, 354);
            this.numericUpDownCalcDataModelsUpdateTimeout.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numericUpDownCalcDataModelsUpdateTimeout.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownCalcDataModelsUpdateTimeout.Name = "numericUpDownCalcDataModelsUpdateTimeout";
            this.numericUpDownCalcDataModelsUpdateTimeout.Size = new System.Drawing.Size(432, 21);
            this.numericUpDownCalcDataModelsUpdateTimeout.TabIndex = 8;
            this.numericUpDownCalcDataModelsUpdateTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 337);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(334, 15);
            this.label7.TabIndex = 66;
            this.label7.Text = "Периодичность обновления вычисляемых моделей (мс)";
            // 
            // checkBoxUseLDAP
            // 
            this.checkBoxUseLDAP.AutoSize = true;
            this.checkBoxUseLDAP.Location = new System.Drawing.Point(15, 378);
            this.checkBoxUseLDAP.Name = "checkBoxUseLDAP";
            this.checkBoxUseLDAP.Size = new System.Drawing.Size(144, 19);
            this.checkBoxUseLDAP.TabIndex = 9;
            this.checkBoxUseLDAP.Text = "Использовать LDAP";
            this.checkBoxUseLDAP.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(456, 450);
            this.Controls.Add(this.checkBoxUseLDAP);
            this.Controls.Add(this.numericUpDownCalcDataModelsUpdateTimeout);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericUpDownDataModelsCallbackUpdateTimeout);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxActivityManagerConfigsPath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxActivityManagerOutputCodepage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxActivityManagerPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxLDAPPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxLDAPUserName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxConnectionString);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.numericUpDownMaxDBConnectionCount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.vButton2);
            this.Controls.Add(this.vButton1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxDBConnectionCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataModelsCallbackUpdateTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCalcDataModelsUpdateTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VIBlend.WinForms.Controls.vButton vButton2;
        private VIBlend.WinForms.Controls.vButton vButton1;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxDBConnectionCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxLDAPUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLDAPPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxActivityManagerPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxActivityManagerOutputCodepage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxActivityManagerConfigsPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownDataModelsCallbackUpdateTimeout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDownCalcDataModelsUpdateTimeout;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxUseLDAP;
    }
}