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
    internal partial class SearchInstallationsForm : SearchForm
    {
        private  DevicesDataModel _devices;
        private  CalcDataModelSoftwareConcat _software;
        private  CalcDataModelLicKeyConcat _softLicKeys;


        private  BindingSource _vSoftware;
        private readonly BindingSource _vDepartmentsInstall;
        private readonly BindingSource _vDepartmentsLic;
        private  BindingSource _vDevices;
        private  BindingSource _vDevicesInvNum;
        private  BindingSource _vDevicesSerialNum;
        private  BindingSource _vSoftLicKeys;

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedLicensesIds = null;
            if (checkBoxSoftwareNameEnable.Checked && (comboBoxSoftwareName.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID Version") == (int)comboBoxSoftwareName.SelectedValue, Entities.EntityType.SoftVersion);
                includedLicensesIds = DataModelHelper.Intersect(null, ids);    
            }
            if (checkBoxSoftwareMakerEnable.Checked && (comboBoxSoftwareMaker.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID SoftMaker") == (int)comboBoxSoftwareMaker.SelectedValue, Entities.EntityType.Software);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxSoftwareTypeEnable.Checked && (comboBoxSoftwareType.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID SoftType") == (int)comboBoxSoftwareType.SelectedValue, Entities.EntityType.Software);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxSupplierEnable.Checked && (comboBoxSupplierID.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID Supplier") == (int)comboBoxSupplierID.SelectedValue, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxLicTypeEnable.Checked && (comboBoxLicType.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID LicType") == (int)comboBoxLicType.SelectedValue, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxLicDocTypeEnable.Checked && (comboBoxLicDocType.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID DocType") == (int)comboBoxLicDocType.SelectedValue, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxDepartmentLicEnable.Checked && (comboBoxDepartmentLicID.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => DataModelHelper.GetDepartmentSubunits((int)comboBoxDepartmentLicID.SelectedValue).Union(
                        new List<int> { (int)comboBoxDepartmentLicID.SelectedValue }).Contains(
                            row.Field<int>("ID Department")), Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxDocNumberEnable.Checked && !string.IsNullOrEmpty(textBoxDocNumber.Text.Trim()))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<string>("DocNumber").ToUpper(CultureInfo.InvariantCulture)
                        .Contains(textBoxDocNumber.Text.Trim().ToUpper(CultureInfo.InvariantCulture)), Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxExpireLicenseDateEnable.Checked)
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row =>
                    {
                        if (row.Field<DateTime?>("ExpireLicenseDate") == null) return false;
                        switch (comboBoxOpExpireLicenseDate.SelectedItem.ToString())
                        {
                            case "=": return row.Field<DateTime>("ExpireLicenseDate") == dateTimePickerExpireLicenseDate.Value;
                            case "≥": return row.Field<DateTime>("ExpireLicenseDate") >= dateTimePickerExpireLicenseDate.Value;
                            case "≤": return row.Field<DateTime>("ExpireLicenseDate") <= dateTimePickerExpireLicenseDate.Value;
                        }
                        return false;
                    }, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxBuyLicenseDateEnable.Checked)
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row =>
                    {
                        if (row.Field<DateTime?>("BuyLicenseDate") == null) return false;
                        switch (comboBoxOpBuyLicenseDate.SelectedItem.ToString())
                        {
                            case "=": return row.Field<DateTime>("BuyLicenseDate") == dateTimePickerBuyLicenseDate.Value;
                            case "≥": return row.Field<DateTime>("BuyLicenseDate") >= dateTimePickerBuyLicenseDate.Value;
                            case "≤": return row.Field<DateTime>("BuyLicenseDate") <= dateTimePickerBuyLicenseDate.Value;
                        }
                        return false;
                    }, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            var allowedDepartments = from departmentsRow in DataModelHelper.FilterRows(DepartmentsDataModel.GetInstance().SelectVisibleDepartments())
                                    where departmentsRow.Field<bool>("AllowSelect")
                                    select departmentsRow.Field<int>("ID Department");
            var allowedComputers =
                from devicesRow in DataModelHelper.FilterRows(DevicesDataModel.GetInstance().Select())
                join depRow in allowedDepartments
                    on devicesRow.Field<int>("ID Department") equals depRow
                select devicesRow.Field<int>("ID Device");
            if (!string.IsNullOrEmpty(filter.Trim()))
                filter += " AND ";
            filter += "[ID Computer] IN (0";
            foreach (var id in allowedComputers)
                filter += id.ToString(CultureInfo.InvariantCulture) + ",";
            filter = filter.TrimEnd(',') + ")";
            if (checkBoxInstallDateEnable.Checked)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "InstallationDate {0} #{1}#",
                    ConvertDisplayEqExprToSql(
                        comboBoxOpInstallDate.SelectedItem.ToString()),
                        dateTimePickerInstallDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if (checkBoxInstallatorEnable.Checked && (comboBoxInstallator.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID Installator] = '{0}'", comboBoxInstallator.SelectedValue);
            }
            if (checkBoxLicKeyEnable.Checked && (comboBoxLicKey.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID LicenseKey] = '{0}'", comboBoxLicKey.SelectedValue);
            }
            if (checkBoxComputerEnable.Checked && (comboBoxComputer.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID Computer] = '{0}'", comboBoxComputer.SelectedValue);
            }
            if (checkBoxInvNumEnable.Checked && (comboBoxInvNum.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID Computer] = '{0}'", comboBoxInvNum.SelectedValue);
            }
            if (checkBoxSerialNumEnable.Checked && (comboBoxSerialNum.SelectedValue != null))
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += string.Format(CultureInfo.InvariantCulture, "[ID Computer] = '{0}'", comboBoxSerialNum.SelectedValue);
            }
            if (checkBoxDepartmentInstallEnable.Checked && (comboBoxDepartmentInstallID.SelectedValue != null))
            {
                var computerIds = DataModelHelper.GetComputerIDsByDepartment((int)comboBoxDepartmentInstallID.SelectedValue);
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "[ID Computer] IN (0";
                foreach (var id in computerIds)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            if (includedLicensesIds != null)
            {
                if (!string.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "[ID License] IN (0";
                foreach (var id in includedLicensesIds)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(',') + ")";
            }
            return filter;
        }

        public SearchInstallationsForm()
        {
            InitializeComponent();
            var softMakers = SoftMakersDataModel.GetInstance();
            var softTypes = SoftTypesDataModel.GetInstance();
            var softSuppliers = SoftSuppliersDataModel.GetInstance();
            var softLicTypes = SoftLicTypesDataModel.GetInstance();
            var softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();
            var departments = DepartmentsDataModel.GetInstance();
            var softInstallators = SoftInstallatorsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            softMakers.Select();
            softTypes.Select();
            softSuppliers.Select();
            softLicTypes.Select();
            softLicDocTypes.Select();
            departments.Select();
            softInstallators.Select();

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

            _vDepartmentsInstall = new BindingSource {DataSource = departments.SelectVisibleDepartments()};

            _vDepartmentsLic = new BindingSource {DataSource = departments.SelectVisibleDepartments()};

            var vSoftInstallators = new BindingSource
            {
                DataMember = "SoftInstallators",
                DataSource = DataSetManager.DataSet
            };

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

            comboBoxInstallator.DataSource = vSoftInstallators;
            comboBoxInstallator.ValueMember = "ID Installator";
            comboBoxInstallator.DisplayMember = "FullName";

            comboBoxLicDocType.DataSource = vSoftLicDocTypes;
            comboBoxLicDocType.ValueMember = "ID DocType";
            comboBoxLicDocType.DisplayMember = "DocType";

            comboBoxDepartmentLicID.DataSource = _vDepartmentsLic;
            comboBoxDepartmentLicID.ValueMember = "ID Department";
            comboBoxDepartmentLicID.DisplayMember = "Department";

            comboBoxDepartmentInstallID.DataSource = _vDepartmentsInstall;
            comboBoxDepartmentInstallID.ValueMember = "ID Department";
            comboBoxDepartmentInstallID.DisplayMember = "Department";

            comboBoxOpBuyLicenseDate.SelectedIndex = 0;
            comboBoxOpExpireLicenseDate.SelectedIndex = 0;
            comboBoxOpInstallDate.SelectedIndex = 0;

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

        private string DepartmentFilter()
        {
            var departmentFilter = "[ID Department] IN (0";
            for (var i = 0; i < _vDepartmentsInstall.Count; i++)
                if ((bool)((DataRowView)_vDepartmentsInstall[i])["AllowSelect"])
                    departmentFilter += ((DataRowView)_vDepartmentsInstall[i])["ID Department"] + ",";
            departmentFilter = departmentFilter.TrimEnd(',');
            departmentFilter += ")";
            return departmentFilter;
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
            if (checkBoxDepartmentLicEnable.Checked && (comboBoxDepartmentLicID.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите департамент-заказчик лицензии или уберите галочку поиска по департаменту-заказчику лицензии", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentLicID.Focus();
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
            if (checkBoxDepartmentInstallEnable.Checked && (comboBoxDepartmentInstallID.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите департамент, в котором произведена установка ПО, или уберите галочку поиска по департаменту, в котором произведена установка ПО", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentInstallID.Focus();
                return;
            }
            if (checkBoxComputerEnable.Checked && (comboBoxComputer.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите компьютер, на который произведена установка ПО, или уберите галочку поиска по компьютеру, на который произведена установка ПО",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxComputer.Focus();
                return;
            }
            if (checkBoxInvNumEnable.Checked && (comboBoxInvNum.SelectedValue == null))
            {
                MessageBox.Show(@"Укажите инвентарный номер компьютера или уберите галочку поиска по инвентарному номеру",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxInvNum.Focus();
                return;
            }
            if (checkBoxSerialNumEnable.Checked && (comboBoxSerialNum.SelectedValue == null))
            {
                MessageBox.Show(@"Укажите серийный номер компьютера или уберите галочку поиска по серийному номеру",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSerialNum.Focus();
                return;
            }
            if (checkBoxLicKeyEnable.Checked && (comboBoxLicKey.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите лицензионный ключ или уберите галочку поиска по лицензионному ключу",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicKey.Focus();
                return;
            }
            if (checkBoxInstallatorEnable.Checked && (comboBoxInstallator.SelectedValue == null))
            {
                MessageBox.Show(@"Выберите установщика ПО или уберите галочку поиска по установщику ПО",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxInstallator.Focus();
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
            }
            comboBoxSoftwareName.DataSource = _vSoftware;
            comboBoxSoftwareName.ValueMember = "ID Version";
            comboBoxSoftwareName.DisplayMember = "Software";
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
            comboBoxDepartmentLicID.Enabled = checkBoxDepartmentLicEnable.Checked;
        }

        private void checkBoxDocNumberEnable_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDocNumber.Enabled = checkBoxDocNumberEnable.Checked;
        }

        private void checkBoxLicDocTypeEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxLicDocType.Enabled = checkBoxLicDocTypeEnable.Checked;
        }

        private void checkBoxBuyLicenseDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerBuyLicenseDate.Enabled = comboBoxOpBuyLicenseDate.Enabled = checkBoxBuyLicenseDateEnable.Checked;
        }

        private void checkBoxExpireLicenseDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerExpireLicenseDate.Enabled = comboBoxOpExpireLicenseDate.Enabled = checkBoxExpireLicenseDateEnable.Checked;
        }

        private void checkBoxDepartmentInstallEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxDepartmentInstallID.Enabled = checkBoxDepartmentInstallEnable.Checked;
        }

        private void checkBoxComputerEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxComputer.DataSource == null)
            {
                _devices = DevicesDataModel.GetInstance();
                _devices.Select();
                _vDevices = new BindingSource
                {
                    DataMember = "Devices",
                    DataSource = DataSetManager.DataSet,
                    Filter = DepartmentFilter()
                };
                comboBoxComputer.DataSource = _vDevices;
                comboBoxComputer.ValueMember = "ID Device";
                comboBoxComputer.DisplayMember = "Device Name";
            }
            comboBoxComputer.Enabled = checkBoxComputerEnable.Checked;
        }

        private void checkBoxSerialNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxSerialNum.DataSource == null)
            {
                _devices = DevicesDataModel.GetInstance();
                _devices.Select();
                _vDevicesSerialNum = new BindingSource
                {
                    DataMember = "Devices",
                    DataSource = DataSetManager.DataSet,
                    Filter = DepartmentFilter()
                };

                comboBoxSerialNum.DataSource = _vDevicesSerialNum;
                comboBoxSerialNum.ValueMember = "ID Device";
                comboBoxSerialNum.DisplayMember = "SerialNumber";
            }
            comboBoxSerialNum.Enabled = checkBoxSerialNumEnable.Checked;
        }

        private void checkBoxInvNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxInvNum.DataSource == null)
            {
                _devices = DevicesDataModel.GetInstance();
                _devices.Select();
                _vDevicesInvNum = new BindingSource
                {
                    DataMember = "Devices",
                    DataSource = DataSetManager.DataSet,
                    Filter = DepartmentFilter()
                };

                comboBoxInvNum.DataSource = _vDevicesInvNum;
                comboBoxInvNum.ValueMember = "ID Device";
                comboBoxInvNum.DisplayMember = "InventoryNumber";
            }

            comboBoxInvNum.Enabled = checkBoxInvNumEnable.Checked;
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
                comboBoxLicKey.ValueMember = "ID LicenseKey";
                comboBoxLicKey.DisplayMember = "LicKey";   
            }
            comboBoxLicKey.Enabled = checkBoxLicKeyEnable.Checked;
        }

        private void checkBoxInstallatorEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxInstallator.Enabled = checkBoxInstallatorEnable.Checked;
        }

        private void checkBoxInstallDateEnable_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerInstallDate.Enabled = comboBoxOpInstallDate.Enabled = checkBoxInstallDateEnable.Checked;
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

        private void comboBoxSoftwareName_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxSoftwareName.Items.Count == 0)
                comboBoxSoftwareName.SelectedIndex = -1;
        }

        private void comboBoxComputer_Leave(object sender, EventArgs e)
        {
            if (comboBoxComputer.Items.Count > 0)
            {
                if (comboBoxComputer.SelectedItem == null)
                    comboBoxComputer.SelectedItem = _vDevices[_vDevices.Position];
                comboBoxComputer.Text = ((DataRowView)_vDevices[_vDevices.Position])["Device Name"].ToString();
            }
            if (comboBoxComputer.SelectedItem == null)
            {
                _vDevices.Filter = "";
                comboBoxComputer.Text = "";
                comboBoxComputer.SelectedItem = null;
            }
        }

        private void comboBoxComputer_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxComputer.Text;
                var selectionStart = comboBoxComputer.SelectionStart;
                var selectionLength = comboBoxComputer.SelectionLength;
                _vDevices.Filter = "[Device Name] like '%" + comboBoxComputer.Text + "%'";
                comboBoxComputer.Text = text;
                comboBoxComputer.SelectionStart = selectionStart;
                comboBoxComputer.SelectionLength = selectionLength;
            }
        }

        private void comboBoxComputer_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxComputer.Items.Count == 0)
                comboBoxComputer.SelectedIndex = -1;
        }

        private void comboBoxInvNum_Leave(object sender, EventArgs e)
        {
            if (comboBoxInvNum.Items.Count > 0)
            {
                if (comboBoxInvNum.SelectedItem == null)
                    comboBoxInvNum.SelectedItem = _vDevicesInvNum[_vDevicesInvNum.Position];
                comboBoxInvNum.Text = ((DataRowView)_vDevicesInvNum[_vDevicesInvNum.Position])["InventoryNumber"].ToString();
            }
            if (comboBoxInvNum.SelectedItem == null)
            {
                _vDevicesInvNum.Filter = "";
                comboBoxInvNum.Text = "";
                comboBoxInvNum.SelectedItem = null;
            }
        }

        private void comboBoxInvNum_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxInvNum.Text;
                var selectionStart = comboBoxInvNum.SelectionStart;
                var selectionLength = comboBoxInvNum.SelectionLength;
                _vDevicesInvNum.Filter = "[InventoryNumber] like '%" + comboBoxInvNum.Text + "%'";
                comboBoxInvNum.Text = text;
                comboBoxInvNum.SelectionStart = selectionStart;
                comboBoxInvNum.SelectionLength = selectionLength;
            }
        }

        private void comboBoxInvNum_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxInvNum.Items.Count == 0)
                comboBoxInvNum.SelectedIndex = -1;
        }

        private void comboBoxSerialNum_Leave(object sender, EventArgs e)
        {
            if (comboBoxSerialNum.Items.Count > 0)
            {
                if (comboBoxSerialNum.SelectedItem == null)
                    comboBoxSerialNum.SelectedItem = _vDevicesSerialNum[_vDevicesSerialNum.Position];
                comboBoxSerialNum.Text = ((DataRowView)_vDevicesSerialNum[_vDevicesSerialNum.Position])["SerialNumber"].ToString();
            }
            if (comboBoxSerialNum.SelectedItem == null)
            {
                _vDevicesSerialNum.Filter = "";
                comboBoxSerialNum.Text = "";
                comboBoxSerialNum.SelectedItem = null;
            }
        }

        private void comboBoxSerialNum_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxSerialNum.Text;
                var selectionStart = comboBoxSerialNum.SelectionStart;
                var selectionLength = comboBoxSerialNum.SelectionLength;
                _vDevicesSerialNum.Filter = "[SerialNumber] like '%" + comboBoxSerialNum.Text + "%'";
                comboBoxSerialNum.Text = text;
                comboBoxSerialNum.SelectionStart = selectionStart;
                comboBoxSerialNum.SelectionLength = selectionLength;
            }
        }

        private void comboBoxSerialNum_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxSerialNum.Items.Count == 0)
                comboBoxSerialNum.SelectedIndex = -1;
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

        private void comboBoxLicKey_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxLicKey.Text;
                var selectionStart = comboBoxLicKey.SelectionStart;
                var selectionLength = comboBoxLicKey.SelectionLength;
                _vSoftLicKeys.Filter = "[LicKey] like '%" + comboBoxLicKey.Text + "%'";
                comboBoxLicKey.Text = text;
                comboBoxLicKey.SelectionStart = selectionStart;
                comboBoxLicKey.SelectionLength = selectionLength;
            }
        }

        private void comboBoxLicKey_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxLicKey.Items.Count == 0)
                comboBoxLicKey.SelectedIndex = -1;
        }

        private void comboBoxDepartmentInstallID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxDepartmentInstallID.Text;
                var selectionStart = comboBoxDepartmentInstallID.SelectionStart;
                var selectionLength = comboBoxDepartmentInstallID.SelectionLength;
                _vDepartmentsInstall.Filter = "Department like '%" + comboBoxDepartmentInstallID.Text + "%' OR Department not like '    %'";
                comboBoxDepartmentInstallID.Text = text;
                comboBoxDepartmentInstallID.SelectionStart = selectionStart;
                comboBoxDepartmentInstallID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxDepartmentInstallID_Leave(object sender, EventArgs e)
        {
            if (comboBoxSupplierID.Items.Count > 0)
            {
                if (comboBoxDepartmentInstallID.SelectedItem == null)
                    comboBoxDepartmentInstallID.SelectedItem = _vDepartmentsInstall[_vDepartmentsInstall.Position];
                comboBoxDepartmentInstallID.Text = ((DataRowView)_vDepartmentsInstall[_vDepartmentsInstall.Position])["Department"].ToString();
            }
            if (comboBoxDepartmentInstallID.SelectedItem == null)
            {
                comboBoxDepartmentInstallID.Text = "";
                _vDepartmentsInstall.Filter = "";
            }
        }

        private void comboBoxDepartmentInstallID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxDepartmentInstallID.Items.Count == 0)
                comboBoxDepartmentInstallID.SelectedIndex = -1;
        }

        private void comboBoxDepartmentLicID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxDepartmentLicID.Items.Count == 0)
                comboBoxDepartmentLicID.SelectedIndex = -1;
        }

        private void comboBoxDepartmentLicID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxDepartmentLicID.Text;
                var selectionStart = comboBoxDepartmentLicID.SelectionStart;
                var selectionLength = comboBoxDepartmentLicID.SelectionLength;
                _vDepartmentsLic.Filter = "Department like '%" + comboBoxDepartmentLicID.Text + "%' OR Department not like '    %'";
                comboBoxDepartmentLicID.Text = text;
                comboBoxDepartmentLicID.SelectionStart = selectionStart;
                comboBoxDepartmentLicID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxDepartmentLicID_Leave(object sender, EventArgs e)
        {
            if (comboBoxSupplierID.Items.Count > 0)
            {
                if (comboBoxDepartmentLicID.SelectedItem == null)
                    comboBoxDepartmentLicID.SelectedItem = _vDepartmentsLic[_vDepartmentsLic.Position];
                comboBoxDepartmentLicID.Text = ((DataRowView)_vDepartmentsLic[_vDepartmentsLic.Position])["Department"].ToString();
            }
            if (comboBoxDepartmentLicID.SelectedItem == null)
            {
                comboBoxDepartmentLicID.Text = "";
                _vDepartmentsLic.Filter = "";
            }
        }
    }
}
