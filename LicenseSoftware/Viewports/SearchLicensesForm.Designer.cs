namespace LicenseSoftware.SearchForms
{
    partial class SearchLicensesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchLicensesForm));
            this.comboBoxSoftwareType = new System.Windows.Forms.ComboBox();
            this.checkBoxSoftwareTypeEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.vButtonCancel = new VIBlend.WinForms.Controls.vButton();
            this.vButtonSearch = new VIBlend.WinForms.Controls.vButton();
            this.checkBoxSoftwareNameEnable = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxSoftwareMaker = new System.Windows.Forms.ComboBox();
            this.checkBoxSoftwareMakerEnable = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxDepartmentID = new System.Windows.Forms.ComboBox();
            this.comboBoxSupplierID = new System.Windows.Forms.ComboBox();
            this.comboBoxSoftwareName = new System.Windows.Forms.ComboBox();
            this.comboBoxLicType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxSupplierEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxLicTypeEnable = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxDepartmentEnable = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxDocNumberEnable = new System.Windows.Forms.CheckBox();
            this.textBoxDocNumber = new System.Windows.Forms.TextBox();
            this.checkBoxLicDocTypeEnable = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxLicDocType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dateTimePickerBuyLicenseDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxBuyLicenseDateEnable = new System.Windows.Forms.CheckBox();
            this.comboBoxOpBuyLicenseDate = new System.Windows.Forms.ComboBox();
            this.comboBoxOpExpireLicenseDate = new System.Windows.Forms.ComboBox();
            this.checkBoxExpireLicenseDateEnable = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePickerExpireLicenseDate = new System.Windows.Forms.DateTimePicker();
            this.comboBoxLicKey = new System.Windows.Forms.ComboBox();
            this.checkBoxLicKeyEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxSoftwareType
            // 
            this.comboBoxSoftwareType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSoftwareType.Enabled = false;
            this.comboBoxSoftwareType.FormattingEnabled = true;
            this.comboBoxSoftwareType.Location = new System.Drawing.Point(42, 68);
            this.comboBoxSoftwareType.Name = "comboBoxSoftwareType";
            this.comboBoxSoftwareType.Size = new System.Drawing.Size(437, 23);
            this.comboBoxSoftwareType.TabIndex = 3;
            // 
            // checkBoxSoftwareTypeEnable
            // 
            this.checkBoxSoftwareTypeEnable.AutoSize = true;
            this.checkBoxSoftwareTypeEnable.Location = new System.Drawing.Point(18, 73);
            this.checkBoxSoftwareTypeEnable.Name = "checkBoxSoftwareTypeEnable";
            this.checkBoxSoftwareTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSoftwareTypeEnable.TabIndex = 2;
            this.checkBoxSoftwareTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxSoftwareTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxSoftwareTypeEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 51);
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
            this.vButtonCancel.Location = new System.Drawing.Point(257, 487);
            this.vButtonCancel.Name = "vButtonCancel";
            this.vButtonCancel.RoundedCornersMask = ((byte)(15));
            this.vButtonCancel.Size = new System.Drawing.Size(117, 35);
            this.vButtonCancel.TabIndex = 25;
            this.vButtonCancel.Text = "Отмена";
            this.vButtonCancel.UseVisualStyleBackColor = false;
            this.vButtonCancel.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            // 
            // vButtonSearch
            // 
            this.vButtonSearch.AllowAnimations = true;
            this.vButtonSearch.BackColor = System.Drawing.Color.Transparent;
            this.vButtonSearch.Location = new System.Drawing.Point(119, 487);
            this.vButtonSearch.Name = "vButtonSearch";
            this.vButtonSearch.RoundedCornersMask = ((byte)(15));
            this.vButtonSearch.Size = new System.Drawing.Size(117, 35);
            this.vButtonSearch.TabIndex = 24;
            this.vButtonSearch.Text = "Поиск";
            this.vButtonSearch.UseVisualStyleBackColor = false;
            this.vButtonSearch.VIBlendTheme = VIBlend.Utilities.VIBLEND_THEME.OFFICEBLUE;
            this.vButtonSearch.Click += new System.EventHandler(this.vButtonSearch_Click);
            // 
            // checkBoxSoftwareNameEnable
            // 
            this.checkBoxSoftwareNameEnable.AutoSize = true;
            this.checkBoxSoftwareNameEnable.Location = new System.Drawing.Point(18, 29);
            this.checkBoxSoftwareNameEnable.Name = "checkBoxSoftwareNameEnable";
            this.checkBoxSoftwareNameEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSoftwareNameEnable.TabIndex = 0;
            this.checkBoxSoftwareNameEnable.UseVisualStyleBackColor = true;
            this.checkBoxSoftwareNameEnable.CheckedChanged += new System.EventHandler(this.checkBoxSoftwareNameEnable_CheckedChanged);
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
            this.comboBoxSoftwareMaker.Location = new System.Drawing.Point(42, 111);
            this.comboBoxSoftwareMaker.Name = "comboBoxSoftwareMaker";
            this.comboBoxSoftwareMaker.Size = new System.Drawing.Size(437, 23);
            this.comboBoxSoftwareMaker.TabIndex = 5;
            // 
            // checkBoxSoftwareMakerEnable
            // 
            this.checkBoxSoftwareMakerEnable.AutoSize = true;
            this.checkBoxSoftwareMakerEnable.Location = new System.Drawing.Point(18, 116);
            this.checkBoxSoftwareMakerEnable.Name = "checkBoxSoftwareMakerEnable";
            this.checkBoxSoftwareMakerEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSoftwareMakerEnable.TabIndex = 4;
            this.checkBoxSoftwareMakerEnable.UseVisualStyleBackColor = true;
            this.checkBoxSoftwareMakerEnable.CheckedChanged += new System.EventHandler(this.checkBoxSoftwareMakerEnable_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 94);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(103, 15);
            this.label12.TabIndex = 36;
            this.label12.Text = "Разработчик ПО";
            // 
            // comboBoxDepartmentID
            // 
            this.comboBoxDepartmentID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDepartmentID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDepartmentID.Enabled = false;
            this.comboBoxDepartmentID.FormattingEnabled = true;
            this.comboBoxDepartmentID.Location = new System.Drawing.Point(43, 242);
            this.comboBoxDepartmentID.Name = "comboBoxDepartmentID";
            this.comboBoxDepartmentID.Size = new System.Drawing.Size(436, 23);
            this.comboBoxDepartmentID.TabIndex = 11;
            // 
            // comboBoxSupplierID
            // 
            this.comboBoxSupplierID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSupplierID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSupplierID.Enabled = false;
            this.comboBoxSupplierID.FormattingEnabled = true;
            this.comboBoxSupplierID.Location = new System.Drawing.Point(42, 154);
            this.comboBoxSupplierID.Name = "comboBoxSupplierID";
            this.comboBoxSupplierID.Size = new System.Drawing.Size(437, 23);
            this.comboBoxSupplierID.TabIndex = 7;
            // 
            // comboBoxSoftwareName
            // 
            this.comboBoxSoftwareName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSoftwareName.Enabled = false;
            this.comboBoxSoftwareName.FormattingEnabled = true;
            this.comboBoxSoftwareName.Location = new System.Drawing.Point(43, 25);
            this.comboBoxSoftwareName.Name = "comboBoxSoftwareName";
            this.comboBoxSoftwareName.Size = new System.Drawing.Size(436, 23);
            this.comboBoxSoftwareName.TabIndex = 1;
            this.comboBoxSoftwareName.DropDownClosed += new System.EventHandler(this.comboBoxSoftwareName_DropDownClosed);
            this.comboBoxSoftwareName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxSoftwareName_KeyUp);
            this.comboBoxSoftwareName.Leave += new System.EventHandler(this.comboBoxSoftwareName_Leave);
            // 
            // comboBoxLicType
            // 
            this.comboBoxLicType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLicType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLicType.Enabled = false;
            this.comboBoxLicType.FormattingEnabled = true;
            this.comboBoxLicType.Location = new System.Drawing.Point(43, 198);
            this.comboBoxLicType.Name = "comboBoxLicType";
            this.comboBoxLicType.Size = new System.Drawing.Size(436, 23);
            this.comboBoxLicType.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 15);
            this.label1.TabIndex = 41;
            this.label1.Text = "Поставщик ПО";
            // 
            // checkBoxSupplierEnable
            // 
            this.checkBoxSupplierEnable.AutoSize = true;
            this.checkBoxSupplierEnable.Location = new System.Drawing.Point(18, 159);
            this.checkBoxSupplierEnable.Name = "checkBoxSupplierEnable";
            this.checkBoxSupplierEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSupplierEnable.TabIndex = 6;
            this.checkBoxSupplierEnable.UseVisualStyleBackColor = true;
            this.checkBoxSupplierEnable.CheckedChanged += new System.EventHandler(this.checkBoxSupplierEnable_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 15);
            this.label2.TabIndex = 43;
            this.label2.Text = "Вид лицензии";
            // 
            // checkBoxLicTypeEnable
            // 
            this.checkBoxLicTypeEnable.AutoSize = true;
            this.checkBoxLicTypeEnable.Location = new System.Drawing.Point(18, 203);
            this.checkBoxLicTypeEnable.Name = "checkBoxLicTypeEnable";
            this.checkBoxLicTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxLicTypeEnable.TabIndex = 8;
            this.checkBoxLicTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxLicTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxLicTypeEnable_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 15);
            this.label3.TabIndex = 45;
            this.label3.Text = "Департамент (отдел)";
            // 
            // checkBoxDepartmentEnable
            // 
            this.checkBoxDepartmentEnable.AutoSize = true;
            this.checkBoxDepartmentEnable.Location = new System.Drawing.Point(18, 247);
            this.checkBoxDepartmentEnable.Name = "checkBoxDepartmentEnable";
            this.checkBoxDepartmentEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDepartmentEnable.TabIndex = 10;
            this.checkBoxDepartmentEnable.UseVisualStyleBackColor = true;
            this.checkBoxDepartmentEnable.CheckedChanged += new System.EventHandler(this.checkBoxDepartment_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 268);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 15);
            this.label4.TabIndex = 47;
            this.label4.Text = "Номер документа-основания";
            // 
            // checkBoxDocNumberEnable
            // 
            this.checkBoxDocNumberEnable.AutoSize = true;
            this.checkBoxDocNumberEnable.Location = new System.Drawing.Point(18, 289);
            this.checkBoxDocNumberEnable.Name = "checkBoxDocNumberEnable";
            this.checkBoxDocNumberEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxDocNumberEnable.TabIndex = 12;
            this.checkBoxDocNumberEnable.UseVisualStyleBackColor = true;
            this.checkBoxDocNumberEnable.CheckedChanged += new System.EventHandler(this.checkBoxDocNumberEnable_CheckedChanged);
            // 
            // textBoxDocNumber
            // 
            this.textBoxDocNumber.Enabled = false;
            this.textBoxDocNumber.Location = new System.Drawing.Point(42, 285);
            this.textBoxDocNumber.Name = "textBoxDocNumber";
            this.textBoxDocNumber.Size = new System.Drawing.Size(437, 21);
            this.textBoxDocNumber.TabIndex = 13;
            // 
            // checkBoxLicDocTypeEnable
            // 
            this.checkBoxLicDocTypeEnable.AutoSize = true;
            this.checkBoxLicDocTypeEnable.Location = new System.Drawing.Point(18, 331);
            this.checkBoxLicDocTypeEnable.Name = "checkBoxLicDocTypeEnable";
            this.checkBoxLicDocTypeEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxLicDocTypeEnable.TabIndex = 14;
            this.checkBoxLicDocTypeEnable.UseVisualStyleBackColor = true;
            this.checkBoxLicDocTypeEnable.CheckedChanged += new System.EventHandler(this.checkBoxLicDocTypeEnable_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 309);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 15);
            this.label5.TabIndex = 51;
            this.label5.Text = "Вид документа-основания";
            // 
            // comboBoxLicDocType
            // 
            this.comboBoxLicDocType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLicDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLicDocType.Enabled = false;
            this.comboBoxLicDocType.FormattingEnabled = true;
            this.comboBoxLicDocType.Location = new System.Drawing.Point(43, 326);
            this.comboBoxLicDocType.Name = "comboBoxLicDocType";
            this.comboBoxLicDocType.Size = new System.Drawing.Size(436, 23);
            this.comboBoxLicDocType.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 395);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(182, 15);
            this.label9.TabIndex = 87;
            this.label9.Text = "Дата приобретения лицензии";
            // 
            // dateTimePickerBuyLicenseDate
            // 
            this.dateTimePickerBuyLicenseDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerBuyLicenseDate.Enabled = false;
            this.dateTimePickerBuyLicenseDate.Location = new System.Drawing.Point(108, 412);
            this.dateTimePickerBuyLicenseDate.Name = "dateTimePickerBuyLicenseDate";
            this.dateTimePickerBuyLicenseDate.Size = new System.Drawing.Size(371, 21);
            this.dateTimePickerBuyLicenseDate.TabIndex = 20;
            // 
            // checkBoxBuyLicenseDateEnable
            // 
            this.checkBoxBuyLicenseDateEnable.AutoSize = true;
            this.checkBoxBuyLicenseDateEnable.Location = new System.Drawing.Point(17, 416);
            this.checkBoxBuyLicenseDateEnable.Name = "checkBoxBuyLicenseDateEnable";
            this.checkBoxBuyLicenseDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBuyLicenseDateEnable.TabIndex = 18;
            this.checkBoxBuyLicenseDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxBuyLicenseDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxBuyLicenseDateEnable_CheckedChanged);
            // 
            // comboBoxOpBuyLicenseDate
            // 
            this.comboBoxOpBuyLicenseDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOpBuyLicenseDate.Enabled = false;
            this.comboBoxOpBuyLicenseDate.FormattingEnabled = true;
            this.comboBoxOpBuyLicenseDate.Items.AddRange(new object[] {
            "=",
            "≥",
            "≤"});
            this.comboBoxOpBuyLicenseDate.Location = new System.Drawing.Point(42, 412);
            this.comboBoxOpBuyLicenseDate.Name = "comboBoxOpBuyLicenseDate";
            this.comboBoxOpBuyLicenseDate.Size = new System.Drawing.Size(55, 23);
            this.comboBoxOpBuyLicenseDate.TabIndex = 19;
            // 
            // comboBoxOpExpireLicenseDate
            // 
            this.comboBoxOpExpireLicenseDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOpExpireLicenseDate.Enabled = false;
            this.comboBoxOpExpireLicenseDate.FormattingEnabled = true;
            this.comboBoxOpExpireLicenseDate.Items.AddRange(new object[] {
            "=",
            "≥",
            "≤"});
            this.comboBoxOpExpireLicenseDate.Location = new System.Drawing.Point(42, 454);
            this.comboBoxOpExpireLicenseDate.Name = "comboBoxOpExpireLicenseDate";
            this.comboBoxOpExpireLicenseDate.Size = new System.Drawing.Size(55, 23);
            this.comboBoxOpExpireLicenseDate.TabIndex = 22;
            // 
            // checkBoxExpireLicenseDateEnable
            // 
            this.checkBoxExpireLicenseDateEnable.AutoSize = true;
            this.checkBoxExpireLicenseDateEnable.Location = new System.Drawing.Point(17, 458);
            this.checkBoxExpireLicenseDateEnable.Name = "checkBoxExpireLicenseDateEnable";
            this.checkBoxExpireLicenseDateEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxExpireLicenseDateEnable.TabIndex = 21;
            this.checkBoxExpireLicenseDateEnable.UseVisualStyleBackColor = true;
            this.checkBoxExpireLicenseDateEnable.CheckedChanged += new System.EventHandler(this.checkBoxExpireLicenseDateEnable_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 437);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(195, 15);
            this.label6.TabIndex = 91;
            this.label6.Text = "Дата истечения срока действия";
            // 
            // dateTimePickerExpireLicenseDate
            // 
            this.dateTimePickerExpireLicenseDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerExpireLicenseDate.Enabled = false;
            this.dateTimePickerExpireLicenseDate.Location = new System.Drawing.Point(108, 454);
            this.dateTimePickerExpireLicenseDate.Name = "dateTimePickerExpireLicenseDate";
            this.dateTimePickerExpireLicenseDate.Size = new System.Drawing.Size(371, 21);
            this.dateTimePickerExpireLicenseDate.TabIndex = 23;
            // 
            // comboBoxLicKey
            // 
            this.comboBoxLicKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLicKey.Enabled = false;
            this.comboBoxLicKey.FormattingEnabled = true;
            this.comboBoxLicKey.Location = new System.Drawing.Point(45, 369);
            this.comboBoxLicKey.Name = "comboBoxLicKey";
            this.comboBoxLicKey.Size = new System.Drawing.Size(436, 23);
            this.comboBoxLicKey.TabIndex = 17;
            // 
            // checkBoxLicKeyEnable
            // 
            this.checkBoxLicKeyEnable.AutoSize = true;
            this.checkBoxLicKeyEnable.Location = new System.Drawing.Point(20, 373);
            this.checkBoxLicKeyEnable.Name = "checkBoxLicKeyEnable";
            this.checkBoxLicKeyEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxLicKeyEnable.TabIndex = 16;
            this.checkBoxLicKeyEnable.UseVisualStyleBackColor = true;
            this.checkBoxLicKeyEnable.CheckedChanged += new System.EventHandler(this.checkBoxLicKeyEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 352);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 15);
            this.label8.TabIndex = 94;
            this.label8.Text = "Лицензионный ключ";
            // 
            // SearchLicensesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(208)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(493, 529);
            this.Controls.Add(this.comboBoxLicKey);
            this.Controls.Add(this.checkBoxLicKeyEnable);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxOpExpireLicenseDate);
            this.Controls.Add(this.checkBoxExpireLicenseDateEnable);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePickerExpireLicenseDate);
            this.Controls.Add(this.comboBoxOpBuyLicenseDate);
            this.Controls.Add(this.checkBoxBuyLicenseDateEnable);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.dateTimePickerBuyLicenseDate);
            this.Controls.Add(this.checkBoxLicDocTypeEnable);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxLicDocType);
            this.Controls.Add(this.textBoxDocNumber);
            this.Controls.Add(this.checkBoxDocNumberEnable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBoxDepartmentEnable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxLicTypeEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxSupplierEnable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxDepartmentID);
            this.Controls.Add(this.comboBoxSupplierID);
            this.Controls.Add(this.comboBoxSoftwareName);
            this.Controls.Add(this.comboBoxLicType);
            this.Controls.Add(this.comboBoxSoftwareMaker);
            this.Controls.Add(this.checkBoxSoftwareMakerEnable);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.checkBoxSoftwareNameEnable);
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
            this.Name = "SearchLicensesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Фильтрация лицензий на ПО";
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
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBoxSoftwareMaker;
        private System.Windows.Forms.CheckBox checkBoxSoftwareMakerEnable;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBoxDepartmentID;
        private System.Windows.Forms.ComboBox comboBoxSupplierID;
        private System.Windows.Forms.ComboBox comboBoxSoftwareName;
        private System.Windows.Forms.ComboBox comboBoxLicType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxSupplierEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxLicTypeEnable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxDepartmentEnable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxDocNumberEnable;
        private System.Windows.Forms.TextBox textBoxDocNumber;
        private System.Windows.Forms.CheckBox checkBoxLicDocTypeEnable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxLicDocType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dateTimePickerBuyLicenseDate;
        private System.Windows.Forms.CheckBox checkBoxBuyLicenseDateEnable;
        private System.Windows.Forms.ComboBox comboBoxOpBuyLicenseDate;
        private System.Windows.Forms.ComboBox comboBoxOpExpireLicenseDate;
        private System.Windows.Forms.CheckBox checkBoxExpireLicenseDateEnable;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePickerExpireLicenseDate;
        private System.Windows.Forms.ComboBox comboBoxLicKey;
        private System.Windows.Forms.CheckBox checkBoxLicKeyEnable;
        private System.Windows.Forms.Label label8;

    }
}