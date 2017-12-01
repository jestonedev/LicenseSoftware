using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LicenseSoftware.DataModels;
using LicenseSoftware.DataModels.CalcDataModels;
using LicenseSoftware.DataModels.DataModels;

namespace LicenseSoftware.Viewport.SearchForms
{
    internal partial class SearchLicensesForm : SearchForm
    {
        private CalcDataModelLicKeyConcat _softLicKeys;
        private CalcDataModelSoftwareConcat _software;


        private BindingSource _vSoftware;
        private BindingSource _vSoftLicKeys;
        private BindingSource _vDepartments;

        internal override string GetFilter()
        {

            var filter = "";
            List<int> includedSoftwareIds = null;
            if (checkBoxLicTypeEnable.Checked && (comboBoxLicType.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID LicType] = '{0}'", comboBoxLicType.SelectedValue);
            }
            if (checkBoxLicDocTypeEnable.Checked && (comboBoxLicDocType.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID DocType] = '{0}'", comboBoxLicDocType.SelectedValue);
            }
            if (checkBoxSupplierEnable.Checked && (comboBoxSupplierID.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID Supplier] = '{0}'", comboBoxSupplierID.SelectedValue);
            }
            if (checkBoxDepartmentEnable.Checked && (comboBoxDepartmentID.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                var selectedDepartments = DataModelHelper.GetDepartmentSubunits((int)comboBoxDepartmentID.SelectedValue).
                    Union(new List<int> { (int)comboBoxDepartmentID.SelectedValue });
                var accessibleDepartments = from departmentsRow in DataModelHelper.
                                                FilterRows(DepartmentsDataModel.GetInstance().SelectVisibleDepartments())
                              where departmentsRow.Field<bool>("AllowSelect")
                              select departmentsRow.Field<int>("ID Department");
                var departments = selectedDepartments.Intersect(accessibleDepartments).ToList();
                if (!departments.Any())
                    throw new ViewportException("Вы не состоите ни в одном из департаментов.");
                filter += "[ID Department] IN (";
                foreach (var id in departments)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            else
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                var accessibleDepartments = (from departmentsRow in DataModelHelper.FilterRows(DepartmentsDataModel.GetInstance().SelectVisibleDepartments())
                                            where departmentsRow.Field<bool>("AllowSelect")
                                            select departmentsRow.Field<int>("ID Department")).ToList();
                if (!accessibleDepartments.Any())
                    throw new ViewportException("Вы не состоите ни в одном из департаментов.");
                filter += "[ID Department] IN (";
                foreach (var id in accessibleDepartments)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            if (checkBoxDocNumberEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "DocNumber LIKE'%{0}%'", textBoxDocNumber.Text.Trim().Replace("'", ""));
            }
            if (checkBoxSoftwareNameEnable.Checked && (comboBoxSoftwareName.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID Version] = '{0}'", comboBoxSoftwareName.SelectedValue);
            }
            if (checkBoxBuyLicenseDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "BuyLicenseDate {0} #{1}#",
                    ConvertDisplayEqExprToSql(
                        comboBoxOpBuyLicenseDate.SelectedItem.ToString()),
                        dateTimePickerBuyLicenseDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxExpireLicenseDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "ExpireLicenseDate {0} #{1}#",
                    ConvertDisplayEqExprToSql(
                        comboBoxOpExpireLicenseDate.SelectedItem.ToString()),
                        dateTimePickerExpireLicenseDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxSoftwareMakerEnable.Checked && (comboBoxSoftwareMaker.SelectedValue != null))
            {
                var ids = DataModelHelper.GetSoftwareIDsBySoftMaker((int)comboBoxSoftwareMaker.SelectedValue);
                includedSoftwareIds = DataModelHelper.Intersect(null, ids).ToList();
            }
            if (checkBoxSoftwareTypeEnable.Checked && (comboBoxSoftwareType.SelectedValue != null))
            {
                var ids = DataModelHelper.GetSoftwareIDsBySoftType((int)comboBoxSoftwareType.SelectedValue);
                includedSoftwareIds = DataModelHelper.Intersect(includedSoftwareIds, ids).ToList();
            }
            if (checkBoxLicKeyEnable.Checked && (comboBoxLicKey.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID License] = '{0}'", comboBoxLicKey.SelectedValue);
            }
            if (checkBoxOnlyAvailableInstallations.Checked)
            {
                var installationsCount =
                    from installRow in DataModelHelper.FilterRows(SoftInstallationsDataModel.GetInstance().Select())
                    group installRow by installRow.Field<int>("ID License")
                    into gs
                    select new
                    {
                        idLicense = gs.Key,
                        istallationsCount = gs.Count()
                    };
                var notAvailableLicenses =
                    (from licensesRow in DataModelHelper.FilterRows(SoftLicensesDataModel.GetInstance().Select())
                    join installRow in installationsCount
                        on licensesRow.Field<int>("ID License") equals installRow.idLicense
                    where licensesRow.Field<int?>("InstallationsCount") != null &&
                        (licensesRow.Field<int>("InstallationsCount") - installRow.istallationsCount <= 0)
                    select licensesRow.Field<int>("ID License")).ToList();
                if (notAvailableLicenses.Any())
                {
                    if (!string.IsNullOrEmpty(filter.Trim()))
                        filter += " AND ";
                    filter += "[ID License] NOT IN (0";
                    foreach (var id in notAvailableLicenses)
                        filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                    filter = filter.TrimEnd(',') + ")";
                }
            }
            if (includedSoftwareIds == null || !includedSoftwareIds.Any()) return filter;     
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            filter += "[ID Version] IN (0";
            foreach (var id in includedSoftwareIds)
                filter += id.ToString(CultureInfo.InvariantCulture) + ",";
            filter = filter.TrimEnd(',') + ")";
            return filter;
        }

        public SearchLicensesForm()
        {
            InitializeComponent();
            var softMakers = SoftMakersDataModel.GetInstance();
            var softTypes = SoftTypesDataModel.GetInstance();
            var softSuppliers = SoftSuppliersDataModel.GetInstance();
            var softLicTypes = SoftLicTypesDataModel.GetInstance();
            var softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();
            var departments = DepartmentsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            softMakers.Select();
            softTypes.Select();
            softSuppliers.Select();
            softLicTypes.Select();
            softLicDocTypes.Select();
            departments.Select();

            var vSoftMakers = new BindingSource
            {
                DataMember = "SoftMakers",
                DataSource = DataSetManager.DataSet
            };

            var vSoftTypes = new BindingSource
            {
                DataMember = "SoftTypes",
                DataSource = DataSetManager.DataSet
            };

            var vSoftSuppliers = new BindingSource
            {
                DataMember = "SoftSuppliers",
                DataSource = DataSetManager.DataSet
            };

            var vSoftLicTypes = new BindingSource
            {
                DataMember = "SoftLicTypes",
                DataSource = DataSetManager.DataSet
            };

            var vSoftLicDocTypes = new BindingSource
            {
                DataMember = "SoftLicDocTypes",
                DataSource = DataSetManager.DataSet
            };

            _vDepartments = new BindingSource { DataSource = departments.SelectVisibleDepartments() };

            comboBoxDepartmentID.DataSource = _vDepartments;
            comboBoxDepartmentID.ValueMember = "ID Department";
            comboBoxDepartmentID.DisplayMember = "Department";   

            comboBoxSoftwareMaker.DataSource = vSoftMakers;
            comboBoxSoftwareMaker.ValueMember = "ID SoftMaker";
            comboBoxSoftwareMaker.DisplayMember = "SoftMaker";

            comboBoxSupplierID.DataSource = vSoftSuppliers;
            comboBoxSupplierID.ValueMember = "ID Supplier";
            comboBoxSupplierID.DisplayMember = "Supplier";

            comboBoxSoftwareType.DataSource = vSoftTypes;
            comboBoxSoftwareType.ValueMember = "ID SoftType";
            comboBoxSoftwareType.DisplayMember = "SoftType";

            comboBoxLicType.DataSource = vSoftLicTypes;
            comboBoxLicType.ValueMember = "ID LicType";
            comboBoxLicType.DisplayMember = "LicType";

            comboBoxLicDocType.DataSource = vSoftLicDocTypes;
            comboBoxLicDocType.ValueMember = "ID DocType";
            comboBoxLicDocType.DisplayMember = "DocType";

            comboBoxOpBuyLicenseDate.SelectedIndex = 0;
            comboBoxOpExpireLicenseDate.SelectedIndex = 0;

            foreach (Control control in Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            DialogResult = DialogResult.Cancel;
                };
        }

        private void vButtonSearch_Click(object sender, EventArgs e)
        {
            if (checkBoxSoftwareNameEnable.Checked && string.IsNullOrEmpty(comboBoxSoftwareName.Text.Trim()))
            {
                MessageBox.Show(@"Выберите наименование ПО или уберите галочку поиска по наименованию ПО",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareName.Focus();
                return;
            }
            if (checkBoxSoftwareTypeEnable.Checked && (comboBoxSoftwareType.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите вид ПО или уберите галочку поиска по виду ПО", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareType.Focus();
                return;
            }
            if (checkBoxSoftwareMakerEnable.Checked && (comboBoxSoftwareMaker.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите разработчика ПО или уберите галочку поиска по разработчику ПО", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareMaker.Focus();
                return;
            }
            if (checkBoxSupplierEnable.Checked && (comboBoxSupplierID.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите поставщика ПО или уберите галочку поиска по поставщику ПО", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSupplierID.Focus();
                return;
            }
            if (checkBoxLicTypeEnable.Checked && (comboBoxLicType.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите вид лицензии или уберите галочку поиска по виду лицензии", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicType.Focus();
                return;
            }
            if (checkBoxDepartmentEnable.Checked && (comboBoxDepartmentID.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите департамент-заказчик или уберите галочку поиска по департаменту", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentID.Focus();
                return;
            }
            if (checkBoxDocNumberEnable.Checked && string.IsNullOrEmpty(textBoxDocNumber.Text.Trim()))
            {
                MessageBox.Show(@"Введите номер документа-основания или уберите галочку поиска по номеру документа-основания", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxDocNumber.Focus();
                return;
            }
            if (checkBoxLicDocTypeEnable.Checked && (comboBoxLicDocType.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите вид документа-основания или уберите галочку поиска по виду документа-основания", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicDocType.Focus();
                return;
            }
            if (checkBoxLicKeyEnable.Checked && (comboBoxLicKey.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите лицензионный ключ или уберите галочку поиска по лицензионному ключу", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicKey.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
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
            if (comboBoxSoftwareName.DataSource == null)
            {
                _software = CalcDataModelSoftwareConcat.GetInstance();
                _software.Select();

                _vSoftware = new BindingSource
                {
                    DataMember = "SoftwareConcat",
                    DataSource = DataSetManager.DataSet
                };
                comboBoxSoftwareName.DataSource = _vSoftware;
                comboBoxSoftwareName.ValueMember = "ID Version";
                comboBoxSoftwareName.DisplayMember = "Software";
            }

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
            if (comboBoxLicKey.DataSource == null)
            {
                _softLicKeys = CalcDataModelLicKeyConcat.GetInstance();
                _softLicKeys.Select();
                _vSoftLicKeys = new BindingSource
                {
                    DataMember = "LicKeyConcat",
                    DataSource = DataSetManager.DataSet
                };

                comboBoxLicKey.DataSource = _vSoftLicKeys;
                comboBoxLicKey.ValueMember = "ID License";
                comboBoxLicKey.DisplayMember = "LicKey";
            }

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
                var text = comboBoxSoftwareName.Text;
                var selectionStart = comboBoxSoftwareName.SelectionStart;
                var selectionLength = comboBoxSoftwareName.SelectionLength;
                _vSoftware.Filter = "Software like '%" + comboBoxSoftwareName.Text + "%'";
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
                    comboBoxSoftwareName.SelectedItem = _vSoftware[_vSoftware.Position];
                comboBoxSoftwareName.Text = ((DataRowView)_vSoftware[_vSoftware.Position])["Software"].ToString();
            }
            if (comboBoxSoftwareName.SelectedItem == null)
            {
                _vSoftware.Filter = "";
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
                var text = comboBoxLicKey.Text;
                var selectionStart = comboBoxLicKey.SelectionStart;
                var selectionLength = comboBoxLicKey.SelectionLength;
                _vSoftLicKeys.Filter = "LicKey like '%" + comboBoxLicKey.Text + "%'";
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
                    comboBoxLicKey.SelectedItem = _vSoftLicKeys[_vSoftLicKeys.Position];
                comboBoxLicKey.Text = ((DataRowView)_vSoftLicKeys[_vSoftLicKeys.Position])["LicKey"].ToString();
            }
            if (comboBoxLicKey.SelectedItem == null)
            {
                _vSoftLicKeys.Filter = "";
                comboBoxLicKey.Text = "";
                comboBoxLicKey.SelectedItem = null;
            }
        }

        private void comboBoxLicKey_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxLicKey.Items.Count == 0)
                comboBoxLicKey.SelectedIndex = -1;
        }

        private void comboBoxDepartmentID_Leave(object sender, EventArgs e)
        {
            if (comboBoxSupplierID.Items.Count > 0)
            {
                if (comboBoxDepartmentID.SelectedItem == null)
                    comboBoxDepartmentID.SelectedItem = _vDepartments[_vDepartments.Position];
                comboBoxDepartmentID.Text = ((DataRowView)_vDepartments[_vDepartments.Position])["Department"].ToString();
            }
            if (comboBoxDepartmentID.SelectedItem == null)
            {
                comboBoxDepartmentID.Text = "";
                _vDepartments.Filter = "";
            }
        }

        private void comboBoxDepartmentID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxDepartmentID.Text;
                var selectionStart = comboBoxDepartmentID.SelectionStart;
                var selectionLength = comboBoxDepartmentID.SelectionLength;
                _vDepartments.Filter = "Department like '%" + comboBoxDepartmentID.Text + "%' OR Department not like '    %'";
                comboBoxDepartmentID.Text = text;
                comboBoxDepartmentID.SelectionStart = selectionStart;
                comboBoxDepartmentID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxDepartmentID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxDepartmentID.Items.Count == 0)
                comboBoxDepartmentID.SelectedIndex = -1;
        }
    }
}
