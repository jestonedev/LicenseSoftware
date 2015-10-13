﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LicenseSoftware.DataModels;
using LicenseSoftware.CalcDataModels;
using System.Globalization;
using LicenseSoftware.Viewport;

namespace LicenseSoftware.SearchForms
{
    internal partial class SearchLicensesForm : SearchForm
    {
        SoftTypesDataModel softTypes = null;
        SoftMakersDataModel softMakers = null;
        SoftSuppliersDataModel softSuppliers = null;
        SoftLicTypesDataModel softLicTypes = null;
        DepartmentsDataModel departments = null;
        SoftLicDocTypesDataModel softLicDocTypes = null;
        SoftLicKeysDataModel softLicKeys = null;
        CalcDataModelSoftwareConcat software = null;


        BindingSource v_software = null;
        BindingSource v_softTypes = null;
        BindingSource v_softMakers = null;
        BindingSource v_softSuppliers = null;
        BindingSource v_softLicTypes = null;
        BindingSource v_departments = null;
        BindingSource v_softLicDocTypes = null;
        BindingSource v_softLicKeys = null;

        internal override string GetFilter()
        {
            string filter = "";
            IEnumerable<int> included_ids = null;
            if ((checkBoxLicTypeEnable.Checked) && (comboBoxLicType.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID LicType] = '{0}'", comboBoxLicType.SelectedValue.ToString());
            }
            if ((checkBoxLicDocTypeEnable.Checked) && (comboBoxLicDocType.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID DocType] = '{0}'", comboBoxLicDocType.SelectedValue.ToString());
            }
            if ((checkBoxSupplierEnable.Checked) && (comboBoxSupplierID.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID Supplier] = '{0}'", comboBoxSupplierID.SelectedValue.ToString());
            }
            if ((checkBoxDepartmentEnable.Checked) && (comboBoxDepartmentID.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                var selectedDepartments = DataModelHelper.GetDepartmentSubunits((int)comboBoxDepartmentID.SelectedValue).Union(new List<int> { (int)comboBoxDepartmentID.SelectedValue });
                var accessibleDepartments = from departments_row in DataModelHelper.FilterRows(DepartmentsDataModel.GetInstance().SelectVisibleDepartments())
                              where departments_row.Field<bool>("AllowSelect")
                              select departments_row.Field<int>("ID Department");
                var departments = selectedDepartments.Intersect(accessibleDepartments);
                filter += "[ID Department] IN ("; 
                foreach (int id in departments)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            if (checkBoxDocNumberEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "DocNumber LIKE'%{0}%'", textBoxDocNumber.Text.Trim().Replace("'", ""));
            }
            if ((checkBoxSoftwareNameEnable.Checked) && (comboBoxSoftwareName.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID Software] = '{0}'", comboBoxSoftwareName.SelectedValue.ToString());
            }
            if (checkBoxBuyLicenseDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "BuyLicenseDate {0} #{1}#",
                    ConvertDisplayEqExprToSql(
                        comboBoxOpBuyLicenseDate.SelectedItem.ToString()),
                        dateTimePickerBuyLicenseDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxExpireLicenseDateEnable.Checked)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "ExpireLicenseDate {0} #{1}#",
                    ConvertDisplayEqExprToSql(
                        comboBoxOpExpireLicenseDate.SelectedItem.ToString()),
                        dateTimePickerExpireLicenseDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxSoftwareMakerEnable.Checked && (comboBoxSoftwareMaker.SelectedValue != null))
            {
                IEnumerable<int> ids = DataModelHelper.GetSoftwareIDsBySoftMaker((int)comboBoxSoftwareMaker.SelectedValue);
                included_ids = DataModelHelper.Intersect(included_ids, ids);
            }
            if (checkBoxSoftwareTypeEnable.Checked && (comboBoxSoftwareType.SelectedValue != null))
            {
                IEnumerable<int> ids = DataModelHelper.GetSoftwareIDsBySoftType((int)comboBoxSoftwareType.SelectedValue);
                included_ids = DataModelHelper.Intersect(included_ids, ids);
            }
            if (checkBoxLicKeyEnable.Checked && (comboBoxLicKey.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID License] = '{0}'", comboBoxLicKey.SelectedValue.ToString());
            }
            if (included_ids != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "[ID Software] IN (0";
                foreach (int id in included_ids)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        public SearchLicensesForm()
        {
            InitializeComponent();
            softMakers = SoftMakersDataModel.GetInstance();
            softTypes = SoftTypesDataModel.GetInstance();
            softSuppliers = SoftSuppliersDataModel.GetInstance();
            softLicTypes = SoftLicTypesDataModel.GetInstance();
            softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();
            departments = DepartmentsDataModel.GetInstance();
            software = CalcDataModelSoftwareConcat.GetInstance();
            softLicKeys = SoftLicKeysDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            softMakers.Select();
            softTypes.Select();
            softSuppliers.Select();
            softLicTypes.Select();
            softLicDocTypes.Select();
            departments.Select();
            software.Select();
            softLicKeys.Select();

            v_software = new BindingSource();
            v_software.DataMember = "SoftwareConcat";
            v_software.DataSource = DataSetManager.DataSet;

            v_softMakers = new BindingSource();
            v_softMakers.DataMember = "SoftMakers";
            v_softMakers.DataSource = DataSetManager.DataSet;

            v_softTypes = new BindingSource();
            v_softTypes.DataMember = "SoftTypes";
            v_softTypes.DataSource = DataSetManager.DataSet;

            v_softSuppliers = new BindingSource();
            v_softSuppliers.DataMember = "SoftSuppliers";
            v_softSuppliers.DataSource = DataSetManager.DataSet;

            v_softLicTypes = new BindingSource();
            v_softLicTypes.DataMember = "SoftLicTypes";
            v_softLicTypes.DataSource = DataSetManager.DataSet;

            v_softLicDocTypes = new BindingSource();
            v_softLicDocTypes.DataMember = "SoftLicDocTypes";
            v_softLicDocTypes.DataSource = DataSetManager.DataSet;

            v_softLicKeys = new BindingSource();
            v_softLicKeys.DataMember = "SoftLicKeys";
            v_softLicKeys.DataSource = DataSetManager.DataSet;

            v_departments = new BindingSource();
            v_departments.DataSource = departments.SelectVisibleDepartments();

            comboBoxSoftwareName.DataSource = v_software;
            comboBoxSoftwareName.ValueMember = "ID Software";
            comboBoxSoftwareName.DisplayMember = "Software";

            comboBoxSoftwareMaker.DataSource = v_softMakers;
            comboBoxSoftwareMaker.ValueMember = "ID SoftMaker";
            comboBoxSoftwareMaker.DisplayMember = "SoftMaker";

            comboBoxSupplierID.DataSource = v_softSuppliers;
            comboBoxSupplierID.ValueMember = "ID Supplier";
            comboBoxSupplierID.DisplayMember = "Supplier";

            comboBoxSoftwareType.DataSource = v_softTypes;
            comboBoxSoftwareType.ValueMember = "ID SoftType";
            comboBoxSoftwareType.DisplayMember = "SoftType";

            comboBoxLicType.DataSource = v_softLicTypes;
            comboBoxLicType.ValueMember = "ID LicType";
            comboBoxLicType.DisplayMember = "LicType";

            comboBoxLicDocType.DataSource = v_softLicDocTypes;
            comboBoxLicDocType.ValueMember = "ID DocType";
            comboBoxLicDocType.DisplayMember = "DocType";

            comboBoxDepartmentID.DataSource = v_departments;
            comboBoxDepartmentID.ValueMember = "ID Department";
            comboBoxDepartmentID.DisplayMember = "Department";

            comboBoxLicKey.DataSource = v_softLicKeys;
            comboBoxLicKey.ValueMember = "ID License";
            comboBoxLicKey.DisplayMember = "LicKey";

            comboBoxOpBuyLicenseDate.SelectedIndex = 0;
            comboBoxOpExpireLicenseDate.SelectedIndex = 0;

            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    ComboBox comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if ((checkBoxSoftwareNameEnable.Checked) && String.IsNullOrEmpty(comboBoxSoftwareName.Text.Trim()))
            {
                MessageBox.Show("Выберите наименование ПО или уберите галочку поиска по наименованию ПО",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareName.Focus();
                return;
            }
            if ((checkBoxSoftwareTypeEnable.Checked) && (comboBoxSoftwareType.SelectedValue == null))
            {
                MessageBox.Show("Выберите вид ПО или уберите галочку поиска по виду ПО", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareType.Focus();
                return;
            }
            if ((checkBoxSoftwareMakerEnable.Checked) && (comboBoxSoftwareMaker.SelectedValue == null))
            {
                MessageBox.Show("Выберите разработчика ПО или уберите галочку поиска по разработчику ПО", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareMaker.Focus();
                return;
            }
            if ((checkBoxSupplierEnable.Checked) && (comboBoxSupplierID.SelectedValue == null))
            {
                MessageBox.Show("Выберите поставщика ПО или уберите галочку поиска по поставщику ПО", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSupplierID.Focus();
                return;
            }
            if ((checkBoxLicTypeEnable.Checked) && (comboBoxLicType.SelectedValue == null))
            {
                MessageBox.Show("Выберите вид лицензии или уберите галочку поиска по виду лицензии", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicType.Focus();
                return;
            }
            if ((checkBoxDepartmentEnable.Checked) && (comboBoxDepartmentID.SelectedValue == null))
            {
                MessageBox.Show("Выберите департамент-заказчик или уберите галочку поиска по департаменту", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentID.Focus();
                return;
            }
            if ((checkBoxDocNumberEnable.Checked) && (String.IsNullOrEmpty(textBoxDocNumber.Text.Trim())))
            {
                MessageBox.Show("Введите номер документа-основания или уберите галочку поиска по номеру документа-основания", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxDocNumber.Focus();
                return;
            }
            if ((checkBoxLicDocTypeEnable.Checked) && (comboBoxLicDocType.SelectedValue == null))
            {
                MessageBox.Show("Выберите вид документа-основания или уберите галочку поиска по виду документа-основания", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicDocType.Focus();
                return;
            }
            if ((checkBoxLicKeyEnable.Checked) && (comboBoxLicKey.SelectedValue == null))
            {
                MessageBox.Show("Выберите лицензионный ключ или уберите галочку поиска по лицензионному ключу", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicKey.Focus();
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private static string ConvertDisplayEqExprToSql(string expr)
        {
            switch (expr)
            {
                case "=": return "=";
                case "≥": return ">=";
                case "≤": return "<=";
                default:
                    throw new ViewportException("Неизвестный знак сравнения дат");
            }
        }

        private void checkBoxSoftwareTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSoftwareType.Enabled = checkBoxSoftwareTypeEnable.Checked;
        }

        private void checkBoxSoftwareNameEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSoftwareName.Enabled = checkBoxSoftwareNameEnable.Checked;
        }

        private void checkBoxSoftwareMakerEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSoftwareMaker.Enabled = checkBoxSoftwareMakerEnable.Checked;
        }

        private void checkBoxSupplierEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSupplierID.Enabled = checkBoxSupplierEnable.Checked;
        }

        private void checkBoxLicTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxLicType.Enabled = checkBoxLicTypeEnable.Checked;
        }

        private void checkBoxDepartment_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxDepartmentID.Enabled = checkBoxDepartmentEnable.Checked;
        }

        private void checkBoxDocNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDocNumber.Enabled = checkBoxDocNumberEnable.Checked;
        }

        private void checkBoxLicDocTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxLicDocType.Enabled = checkBoxLicDocTypeEnable.Checked;
        }

        private void checkBoxLicKeyEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxLicKey.Enabled = checkBoxLicKeyEnable.Checked;
        }

        private void checkBoxBuyLicenseDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerBuyLicenseDate.Enabled = comboBoxOpBuyLicenseDate.Enabled = checkBoxBuyLicenseDateEnable.Checked;
        }

        private void checkBoxExpireLicenseDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerExpireLicenseDate.Enabled = comboBoxOpExpireLicenseDate.Enabled = checkBoxExpireLicenseDateEnable.Checked;
        }

        private void comboBoxSoftwareName_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                string text = comboBoxSoftwareName.Text;
                int selectionStart = comboBoxSoftwareName.SelectionStart;
                int selectionLength = comboBoxSoftwareName.SelectionLength;
                v_software.Filter = "Software like '%" + comboBoxSoftwareName.Text + "%'";
                comboBoxSoftwareName.Text = text;
                comboBoxSoftwareName.SelectionStart = selectionStart;
                comboBoxSoftwareName.SelectionLength = selectionLength;
            }
        }

        private void comboBoxSoftwareName_Leave(object sender, EventArgs e)
        {
            if (comboBoxSoftwareName.Items.Count > 0)
            {
                if (comboBoxSoftwareName.SelectedItem == null)
                    comboBoxSoftwareName.SelectedItem = v_software[v_software.Position];
                comboBoxSoftwareName.Text = ((DataRowView)v_software[v_software.Position])["Software"].ToString();
            }
            if (comboBoxSoftwareName.SelectedItem == null)
            {
                v_software.Filter = "";
                comboBoxSoftwareName.Text = "";
                comboBoxSoftwareName.SelectedItem = null;
            }
        }

        private void comboBoxSoftwareName_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxSoftwareName.Items.Count == 0)
                comboBoxSoftwareName.SelectedIndex = -1;
        }

        private void comboBoxLicKey_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                   || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                string text = comboBoxLicKey.Text;
                int selectionStart = comboBoxLicKey.SelectionStart;
                int selectionLength = comboBoxLicKey.SelectionLength;
                v_softLicKeys.Filter = "LicKey like '%" + comboBoxLicKey.Text + "%'";
                comboBoxLicKey.Text = text;
                comboBoxLicKey.SelectionStart = selectionStart;
                comboBoxLicKey.SelectionLength = selectionLength;
            }
        }

        private void comboBoxLicKey_Leave(object sender, EventArgs e)
        {
            if (comboBoxLicKey.Items.Count > 0)
            {
                if (comboBoxLicKey.SelectedItem == null)
                    comboBoxLicKey.SelectedItem = v_softLicKeys[v_softLicKeys.Position];
                comboBoxLicKey.Text = ((DataRowView)v_softLicKeys[v_softLicKeys.Position])["LicKey"].ToString();
            }
            if (comboBoxLicKey.SelectedItem == null)
            {
                v_softLicKeys.Filter = "";
                comboBoxLicKey.Text = "";
                comboBoxLicKey.SelectedItem = null;
            }
        }

        private void comboBoxLicKey_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxLicKey.Items.Count == 0)
                comboBoxLicKey.SelectedIndex = -1;
        }
    }
}
