using System;
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
    internal partial class SearchInstallationsForm : SearchForm
    {
        SoftTypesDataModel softTypes = null;
        SoftMakersDataModel softMakers = null;
        SoftSuppliersDataModel softSuppliers = null;
        SoftLicTypesDataModel softLicTypes = null;
        DepartmentsDataModel departments = null;
        DevicesDataModel devices = null;
        SoftLicDocTypesDataModel softLicDocTypes = null;
        CalcDataModelSoftwareConcat software = null;
        SoftLicKeysDataModel softLicKeys = null;
        SoftInstallatorsDataModel softInstallators = null;


        BindingSource v_software = null;
        BindingSource v_softTypes = null;
        BindingSource v_softMakers = null;
        BindingSource v_softSuppliers = null;
        BindingSource v_softLicTypes = null;
        BindingSource v_departmentsLic = null;
        BindingSource v_departmentsInstall = null;
        BindingSource v_devices = null;
        BindingSource v_devicesInvNum = null;
        BindingSource v_devicesSerialNum = null;
        BindingSource v_softLicDocTypes = null;
        BindingSource v_softLicKeys = null;
        BindingSource v_softInstallators = null;

        internal override string GetFilter()
        {
            var filter = "";
            IEnumerable<int> includedLicensesIds = null;
            if ((checkBoxSoftwareNameEnable.Checked) && (comboBoxSoftwareName.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID Software") == (int)comboBoxSoftwareName.SelectedValue, Entities.EntityType.Software);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);    
            }
            if ((checkBoxSoftwareMakerEnable.Checked) && (comboBoxSoftwareMaker.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID SoftMaker") == (int)comboBoxSoftwareMaker.SelectedValue, Entities.EntityType.Software);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if ((checkBoxSoftwareTypeEnable.Checked) && (comboBoxSoftwareType.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID SoftType") == (int)comboBoxSoftwareType.SelectedValue, Entities.EntityType.Software);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if ((checkBoxSupplierEnable.Checked) && (comboBoxSupplierID.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID Supplier") == (int)comboBoxSupplierID.SelectedValue, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if ((checkBoxLicTypeEnable.Checked) && (comboBoxLicType.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID LicType") == (int)comboBoxLicType.SelectedValue, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if ((checkBoxLicDocTypeEnable.Checked) && (comboBoxLicDocType.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => row.Field<int>("ID DocType") == (int)comboBoxLicDocType.SelectedValue, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if ((checkBoxDepartmentLicEnable.Checked) && (comboBoxDepartmentLicID.SelectedValue != null))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => DataModelHelper.GetDepartmentSubunits((int)comboBoxDepartmentLicID.SelectedValue).Union(
                        new List<int> { (int)comboBoxDepartmentLicID.SelectedValue }).Contains(
                            row.Field<int>("ID Department")), Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if ((checkBoxDocNumberEnable.Checked) && (!String.IsNullOrEmpty(textBoxDocNumber.Text.Trim())))
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => { return row.Field<string>("DocNumber").ToUpper(CultureInfo.InvariantCulture)
                        .Contains(textBoxDocNumber.Text.Trim().ToUpper(CultureInfo.InvariantCulture));
                    }, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxExpireLicenseDateEnable.Checked)
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row => {
                        if (row.Field<DateTime?>("ExpireLicenseDate") != null)
                        {
                            switch (comboBoxOpExpireLicenseDate.SelectedItem.ToString())
                            {
                                case "=": return row.Field<DateTime>("ExpireLicenseDate") == dateTimePickerExpireLicenseDate.Value;
                                case "≥": return row.Field<DateTime>("ExpireLicenseDate") >= dateTimePickerExpireLicenseDate.Value;
                                case "≤": return row.Field<DateTime>("ExpireLicenseDate") <= dateTimePickerExpireLicenseDate.Value;
                            }
                            return false;
                        }
                        else
                            return false;
                    }, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            if (checkBoxBuyLicenseDateEnable.Checked)
            {
                var ids = DataModelHelper.GetLicenseIDsByCondition(
                    row =>
                    {
                        if (row.Field<DateTime?>("BuyLicenseDate") != null)
                        {
                            switch (comboBoxOpBuyLicenseDate.SelectedItem.ToString())
                            {
                                case "=": return row.Field<DateTime>("BuyLicenseDate") == dateTimePickerBuyLicenseDate.Value;
                                case "≥": return row.Field<DateTime>("BuyLicenseDate") >= dateTimePickerBuyLicenseDate.Value;
                                case "≤": return row.Field<DateTime>("BuyLicenseDate") <= dateTimePickerBuyLicenseDate.Value;
                            }
                            return false;
                        }
                        else
                            return false;
                    }, Entities.EntityType.License);
                includedLicensesIds = DataModelHelper.Intersect(includedLicensesIds, ids);
            }
            var allowedDepartments = from departments_row in DataModelHelper.FilterRows(DepartmentsDataModel.GetInstance().SelectVisibleDepartments())
                                    where departments_row.Field<bool>("AllowSelect")
                                    select departments_row.Field<int>("ID Department");
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
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "InstallationDate {0} #{1}#",
                    ConvertDisplayEqExprToSql(
                        comboBoxOpInstallDate.SelectedItem.ToString()),
                        dateTimePickerInstallDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            if ((checkBoxInstallatorEnable.Checked) && (comboBoxInstallator.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID Installator] = '{0}'", comboBoxInstallator.SelectedValue.ToString());
            }
            if ((checkBoxLicKeyEnable.Checked) && (comboBoxLicKey.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID LicenseKey] = '{0}'", comboBoxLicKey.SelectedValue.ToString());
            }
            if ((checkBoxComputerEnable.Checked) && (comboBoxComputer.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID Computer] = '{0}'", comboBoxComputer.SelectedValue.ToString());
            }
            if ((checkBoxInvNumEnable.Checked) && (comboBoxInvNum.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID Computer] = '{0}'", comboBoxInvNum.SelectedValue.ToString());
            }
            if ((checkBoxSerialNumEnable.Checked) && (comboBoxSerialNum.SelectedValue != null))
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += String.Format(CultureInfo.InvariantCulture, "[ID Computer] = '{0}'", comboBoxSerialNum.SelectedValue.ToString());
            }
            if ((checkBoxDepartmentInstallEnable.Checked) && (comboBoxDepartmentInstallID.SelectedValue != null))
            {
                var computerIds = DataModelHelper.GetComputerIDsByDepartment((int)comboBoxDepartmentInstallID.SelectedValue);
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "[ID Computer] IN (0";
                foreach (var id in computerIds)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            if (includedLicensesIds != null)
            {
                if (!String.IsNullOrEmpty(filter.Trim()))
                    filter += " AND ";
                filter += "[ID License] IN (0";
                foreach (var id in includedLicensesIds)
                    filter += id.ToString(CultureInfo.InvariantCulture) + ",";
                filter = filter.TrimEnd(new char[] { ',' }) + ")";
            }
            return filter;
        }

        public SearchInstallationsForm()
        {
            InitializeComponent();
            softMakers = SoftMakersDataModel.GetInstance();
            softTypes = SoftTypesDataModel.GetInstance();
            softSuppliers = SoftSuppliersDataModel.GetInstance();
            softLicTypes = SoftLicTypesDataModel.GetInstance();
            softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();
            departments = DepartmentsDataModel.GetInstance();
            devices = DevicesDataModel.GetInstance();
            software = CalcDataModelSoftwareConcat.GetInstance();
            softLicKeys = SoftLicKeysDataModel.GetInstance();
            softInstallators = SoftInstallatorsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            softMakers.Select();
            softTypes.Select();
            softSuppliers.Select();
            softLicTypes.Select();
            softLicDocTypes.Select();
            departments.Select();
            devices.Select();
            software.Select();
            softLicKeys.Select();
            softInstallators.Select();

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

            v_softLicKeys = new BindingSource();
            v_softLicKeys.DataMember = "SoftLicKeys";
            v_softLicKeys.DataSource = DataSetManager.DataSet;

            v_softLicDocTypes = new BindingSource();
            v_softLicDocTypes.DataMember = "SoftLicDocTypes";
            v_softLicDocTypes.DataSource = DataSetManager.DataSet;

            v_departmentsInstall = new BindingSource();
            v_departmentsInstall.DataSource = departments.SelectVisibleDepartments();

            v_departmentsLic = new BindingSource();
            v_departmentsLic.DataSource = departments.SelectVisibleDepartments();

            v_devices = new BindingSource();
            v_devices.DataMember = "Devices";
            v_devices.DataSource = DataSetManager.DataSet;
            v_devices.Filter = DepartmentFilter();

            v_devicesInvNum = new BindingSource();
            v_devicesInvNum.DataMember = "Devices";
            v_devicesInvNum.DataSource = DataSetManager.DataSet;
            v_devicesInvNum.Filter = DepartmentFilter();

            v_devicesSerialNum = new BindingSource();
            v_devicesSerialNum.DataMember = "Devices";
            v_devicesSerialNum.DataSource = DataSetManager.DataSet;
            v_devicesSerialNum.Filter = DepartmentFilter();

            v_softInstallators = new BindingSource();
            v_softInstallators.DataMember = "SoftInstallators";
            v_softInstallators.DataSource = DataSetManager.DataSet;

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

            comboBoxLicKey.DataSource = v_softLicKeys;
            comboBoxLicKey.ValueMember = "ID LicenseKey";
            comboBoxLicKey.DisplayMember = "LicKey";

            comboBoxInstallator.DataSource = v_softInstallators;
            comboBoxInstallator.ValueMember = "ID Installator";
            comboBoxInstallator.DisplayMember = "FullName";

            comboBoxLicDocType.DataSource = v_softLicDocTypes;
            comboBoxLicDocType.ValueMember = "ID DocType";
            comboBoxLicDocType.DisplayMember = "DocType";

            comboBoxDepartmentLicID.DataSource = v_departmentsLic;
            comboBoxDepartmentLicID.ValueMember = "ID Department";
            comboBoxDepartmentLicID.DisplayMember = "Department";

            comboBoxDepartmentInstallID.DataSource = v_departmentsInstall;
            comboBoxDepartmentInstallID.ValueMember = "ID Department";
            comboBoxDepartmentInstallID.DisplayMember = "Department";

            comboBoxComputer.DataSource = v_devices;
            comboBoxComputer.ValueMember = "ID Device";
            comboBoxComputer.DisplayMember = "Device Name";

            comboBoxSerialNum.DataSource = v_devicesSerialNum;
            comboBoxSerialNum.ValueMember = "ID Device";
            comboBoxSerialNum.DisplayMember = "SerialNumber";

            comboBoxInvNum.DataSource = v_devicesInvNum;
            comboBoxInvNum.ValueMember = "ID Device";
            comboBoxInvNum.DisplayMember = "InventoryNumber";

            comboBoxOpBuyLicenseDate.SelectedIndex = 0;
            comboBoxOpExpireLicenseDate.SelectedIndex = 0;
            comboBoxOpInstallDate.SelectedIndex = 0;

            foreach (Control control in this.Controls)
                control.KeyDown += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    if (comboBox != null && comboBox.DroppedDown)
                        return;
                    if (e.KeyCode == Keys.Enter)
                        vButtonSearch_Click(sender, e);
                    else
                        if (e.KeyCode == Keys.Escape)
                            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                };
        }

        private string DepartmentFilter()
        {
            var DepartmentFilter = "[ID Department] IN (0";
            for (var i = 0; i < v_departmentsInstall.Count; i++)
                if ((bool)((DataRowView)v_departmentsInstall[i])["AllowSelect"])
                    DepartmentFilter += ((DataRowView)v_departmentsInstall[i])["ID Department"] + ",";
            DepartmentFilter = DepartmentFilter.TrimEnd(',');
            DepartmentFilter += ")";
            return DepartmentFilter;
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
            if ((checkBoxDepartmentLicEnable.Checked) && (comboBoxDepartmentLicID.SelectedValue == null))
            {
                MessageBox.Show("Выберите департамент-заказчик лицензии или уберите галочку поиска по департаменту-заказчику лицензии", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentLicID.Focus();
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
            if ((checkBoxDepartmentInstallEnable.Checked) && (comboBoxDepartmentInstallID.SelectedValue == null))
            {
                MessageBox.Show("Выберите департамент, в котором произведена установка ПО, или уберите галочку поиска по департаменту, в котором произведена установка ПО", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentInstallID.Focus();
                return;
            }
            if ((checkBoxComputerEnable.Checked) && (comboBoxComputer.SelectedValue == null))
            {
                MessageBox.Show("Выберите компьютер, на который произведена установка ПО, или уберите галочку поиска по компьютеру, на который произведена установка ПО",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxComputer.Focus();
                return;
            }
            if ((checkBoxInvNumEnable.Checked) && (comboBoxInvNum.SelectedValue == null))
            {
                MessageBox.Show("Укажите инвентарный номер компьютера или уберите галочку поиска по инвентарному номеру",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxInvNum.Focus();
                return;
            }
            if ((checkBoxSerialNumEnable.Checked) && (comboBoxSerialNum.SelectedValue == null))
            {
                MessageBox.Show("Укажите серийный номер компьютера или уберите галочку поиска по серийному номеру",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSerialNum.Focus();
                return;
            }
            if ((checkBoxLicKeyEnable.Checked) && (comboBoxLicKey.SelectedValue == null))
            {
                MessageBox.Show("Выберите лицензионный ключ или уберите галочку поиска по лицензионному ключу",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicKey.Focus();
                return;
            }
            if ((checkBoxInstallatorEnable.Checked) && (comboBoxInstallator.SelectedValue == null))
            {
                MessageBox.Show("Выберите установщика ПО или уберите галочку поиска по установщику ПО",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxInstallator.Focus();
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
            comboBoxComputer.Enabled = checkBoxComputerEnable.Checked;
        }

        private void checkBoxSerialNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSerialNum.Enabled = checkBoxSerialNumEnable.Checked;
        }

        private void checkBoxInvNumEnable_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxInvNum.Enabled = checkBoxInvNumEnable.Checked;
        }

        private void checkBoxLicKeyEnable_CheckedChanged(object sender, EventArgs e)
        {
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

        private void comboBoxSoftwareName_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxSoftwareName.Text;
                var selectionStart = comboBoxSoftwareName.SelectionStart;
                var selectionLength = comboBoxSoftwareName.SelectionLength;
                v_software.Filter = "Software like '%" + comboBoxSoftwareName.Text + "%'";
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
                    comboBoxComputer.SelectedItem = v_devices[v_devices.Position];
                comboBoxComputer.Text = ((DataRowView)v_devices[v_devices.Position])["Device Name"].ToString();
            }
            if (comboBoxComputer.SelectedItem == null)
            {
                v_devices.Filter = "";
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
                v_devices.Filter = "[Device Name] like '%" + comboBoxComputer.Text + "%'";
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
                    comboBoxInvNum.SelectedItem = v_devicesInvNum[v_devicesInvNum.Position];
                comboBoxInvNum.Text = ((DataRowView)v_devicesInvNum[v_devicesInvNum.Position])["InventoryNumber"].ToString();
            }
            if (comboBoxInvNum.SelectedItem == null)
            {
                v_devicesInvNum.Filter = "";
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
                v_devicesInvNum.Filter = "[InventoryNumber] like '%" + comboBoxInvNum.Text + "%'";
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
                    comboBoxSerialNum.SelectedItem = v_devicesSerialNum[v_devicesSerialNum.Position];
                comboBoxSerialNum.Text = ((DataRowView)v_devicesSerialNum[v_devicesSerialNum.Position])["SerialNumber"].ToString();
            }
            if (comboBoxSerialNum.SelectedItem == null)
            {
                v_devicesSerialNum.Filter = "";
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
                v_devicesSerialNum.Filter = "[SerialNumber] like '%" + comboBoxSerialNum.Text + "%'";
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

        private void comboBoxLicKey_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxLicKey.Text;
                var selectionStart = comboBoxLicKey.SelectionStart;
                var selectionLength = comboBoxLicKey.SelectionLength;
                v_softLicKeys.Filter = "[LicKey] like '%" + comboBoxLicKey.Text + "%'";
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
    }
}
