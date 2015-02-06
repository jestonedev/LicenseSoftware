﻿using LicenseSoftware.CalcDataModels;
using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using LicenseSoftware.SearchForms;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LicenseSoftware.Viewport
{
    internal sealed class InstallationsViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView dataGridView;
        private Label label9;
        private DateTimePicker dateTimePickerInstallDate;
        private ComboBox comboBoxLicKeysID;
        private Label label8;
        private GroupBox groupBox1;
        private ComboBox comboBoxComputerID;
        private Label label4;
        private ComboBox comboBoxLicenseID;
        private Label label3;
        private ComboBox comboBoxSoftwareID;
        private Label label2;
        private GroupBox groupBox2;
        private ComboBox comboBoxInstallatorID;
        private Label label5;
        #endregion Components

        #region Models

        CalcDataModelSoftwareConcat softwareDM = null;
        CalcDataModelLicensesConcat licenses = null;
        SoftLicKeysDataModel softLicKeys = null;
        SoftInstallationsDataModel softInstallations = null;
        DepartmentsDataModel departments = null;
        SoftInstallatorsDataModel softInstallators = null;
        DevicesDataModel devices = null;
        #endregion Models

        #region Views
        BindingSource v_software = null;
        BindingSource v_licenses = null;
        BindingSource v_softLicKeys = null;
        BindingSource v_softInstallations = null;
        BindingSource v_departments = null;
        BindingSource v_softInstallators = null;
        BindingSource v_devices = null;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;

        private bool is_editable = false;
        private SearchForm sSearchForm = null;
        private DataGridViewTextBoxColumn idInstallation;
        private DataGridViewTextBoxColumn software;
        private DataGridViewTextBoxColumn department;
        private DataGridViewTextBoxColumn computer;
        private DataGridViewTextBoxColumn installationDate;
        private DataGridViewTextBoxColumn license;
        private DataGridViewTextBoxColumn licKey;
        private bool is_first_visibility = true;

        private InstallationsViewport()
            : this(null)
        {
        }

        public InstallationsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public InstallationsViewport(InstallationsViewport installationsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = installationsViewport.DynamicFilter;
            this.StaticFilter = installationsViewport.StaticFilter;
            this.ParentRow = installationsViewport.ParentRow;
            this.ParentType = installationsViewport.ParentType;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                {
                    this.Text = String.Format(CultureInfo.InvariantCulture, "Установки по лицензии №{0}", ParentRow["ID License"]);
                }
                else
                    this.Text = "Новая установка";
            }
            else
                if (v_softInstallations.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Установка №{0} по лицензии №{1}",
                            ((DataRowView)v_softInstallations[v_softInstallations.Position])["ID Installation"], ParentRow["ID License"]);
                    else
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Установка №{0}",
                            ((DataRowView)v_softInstallations[v_softInstallations.Position])["ID Installation"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Установки по лицензии №{0} отсутствуют", ParentRow["ID License"]);
                    else
                        this.Text = "Установки отсутствуют";
                }
        }

        private void SelectCurrentAutoCompleteValue()
        {
            if (v_softInstallations.Position == -1)
                return;
            DataRowView installationRow = (DataRowView)v_softInstallations[v_softInstallations.Position];
            if ((comboBoxSoftwareID.DataSource != null) && (comboBoxLicenseID.DataSource != null) && (comboBoxLicKeysID.DataSource != null) &&
                !String.IsNullOrEmpty(comboBoxSoftwareID.ValueMember) &&
                !String.IsNullOrEmpty(comboBoxLicenseID.ValueMember) &&
                !String.IsNullOrEmpty(comboBoxLicKeysID.ValueMember))
            {
                v_software.Filter = "";
                v_licenses.Filter = "";
                v_softLicKeys.Filter = "";
                int? idSoftware = null;
                int? idLicense = null;
                int? idLicKey = null;
                if (installationRow["ID License"] != DBNull.Value)
                    idLicense = Convert.ToInt32(installationRow["ID License"], CultureInfo.InvariantCulture);
                else
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                        idLicense = Convert.ToInt32(ParentRow["ID License"], CultureInfo.InvariantCulture);
                if (idLicense != null)
                {
                    int index = v_licenses.Find("ID License", idLicense);
                    if (index != -1)
                        idSoftware = (int)((DataRowView)v_licenses[index])["ID Software"];
                    if (installationRow["ID LicenseKey"] != DBNull.Value)
                        idLicKey = Convert.ToInt32(installationRow["ID LicenseKey"], CultureInfo.InvariantCulture);
                }
                comboBoxSoftwareID.SelectedValue = (object)idSoftware ?? DBNull.Value;
                v_licenses.Filter = DepartmentFilter() + " AND [ID Software] = " + ((DataRowView)v_software[v_software.Position])["ID Software"];
                comboBoxLicenseID.SelectedValue = (object)idLicense ?? DBNull.Value;
                if (v_licenses.Position != -1)
                {
                    v_softLicKeys.Filter = "[ID License] = " + ((DataRowView)v_licenses[v_licenses.Position])["ID License"];
                    comboBoxLicKeysID.SelectedValue = (object)idLicKey ?? DBNull.Value;
                }
                else
                    comboBoxLicKeysID.SelectedValue = DBNull.Value;
            }
            if (comboBoxComputerID.DataSource != null && !String.IsNullOrEmpty(comboBoxComputerID.ValueMember))
            {
                v_devices.Filter = DepartmentFilter();
                int? idComputer = null;
                if (installationRow["ID Computer"] != DBNull.Value)
                    idComputer = (int)installationRow["ID Computer"];
                comboBoxComputerID.SelectedValue = (object)idComputer ?? DBNull.Value;
            }
        }

        private void DataBind()
        {
            comboBoxComputerID.DataSource = v_devices;
            comboBoxComputerID.ValueMember = "ID Device";
            comboBoxComputerID.DisplayMember = "Device Name";

            comboBoxInstallatorID.DataSource = v_softInstallators;
            comboBoxInstallatorID.ValueMember = "ID Installator";
            comboBoxInstallatorID.DisplayMember = "FullName";
            comboBoxInstallatorID.DataBindings.Clear();
            comboBoxInstallatorID.DataBindings.Add("SelectedValue", v_softInstallations, "ID Installator", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxLicKeysID.DataSource = v_softLicKeys;
            comboBoxLicKeysID.ValueMember = "ID LicenseKey";
            comboBoxLicKeysID.DisplayMember = "LicKey";

            comboBoxLicenseID.DataSource = v_licenses;
            comboBoxLicenseID.ValueMember = "ID License";
            comboBoxLicenseID.DisplayMember = "License";

            comboBoxSoftwareID.DataSource = v_software;
            comboBoxSoftwareID.ValueMember = "ID Software";
            comboBoxSoftwareID.DisplayMember = "Software";

            dateTimePickerInstallDate.DataBindings.Clear();
            dateTimePickerInstallDate.DataBindings.Add("Value", v_softInstallations, "InstallationDate", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_softInstallations.Position != -1) && (InstallationFromView() != InstallationFromViewport()))
            {
                if (viewportState == ViewportState.ReadState)
                {
                    viewportState = ViewportState.ModifyRowState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (viewportState == ViewportState.ModifyRowState)
                {
                    viewportState = ViewportState.ReadState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = true;
                }
            }
        }

        private bool ChangeViewportStateTo(ViewportState state)
        {
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            if (viewportState == ViewportState.ReadState)
                                return true;
                            else
                                return false;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            if (softInstallations.EditingNewRecord)
                                return false;
                            else
                            {
                                viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.NewRowState);
                            else
                                return false;
                    }
                    break;
                case ViewportState.ModifyRowState: ;
                    switch (viewportState)
                    {
                        case ViewportState.ReadState:
                            viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.ModifyRowState);
                            else
                                return false;
                    }
                    break;
            }
            return false;
        }

        private void LocateInstallation(int id)
        {
            int Position = v_softInstallations.Find("ID Installation", id);
            is_editable = false;
            if (Position > 0)
                v_softInstallations.Position = Position;
            is_editable = true;
        }

        private void ViewportFromInstallation(SoftInstallation installation)
        {
            if (installation.IdLicense != null)
            {
                v_software.Filter = "";
                v_licenses.Filter = "";
                v_softLicKeys.Filter = "";
                int index = v_licenses.Find("ID License", installation.IdLicense);
                if (index != -1)
                {
                    comboBoxSoftwareID.SelectedValue = ((DataRowView)v_licenses[index])["ID Software"];
                    comboBoxLicenseID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdLicense);
                    comboBoxLicKeysID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdLicenseKey);
                }
            }

            comboBoxComputerID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdComputer);
            comboBoxInstallatorID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdInstallator);
            dateTimePickerInstallDate.Value = ViewportHelper.ValueOrDefault(installation.InstallationDate);
        }

        private SoftInstallation InstallationFromViewport()
        {
            SoftInstallation installation = new SoftInstallation();
            if (v_softInstallations.Position == -1)
                installation.IdInstallation = null;
            else
                installation.IdInstallation = ViewportHelper.ValueOrNull<int>((DataRowView)v_softInstallations[v_softInstallations.Position], "ID Installation");
            installation.IdLicense = ViewportHelper.ValueOrNull<int>(comboBoxLicenseID);
            installation.IdInstallator = ViewportHelper.ValueOrNull<int>(comboBoxInstallatorID);
            installation.IdLicenseKey = ViewportHelper.ValueOrNull<int>(comboBoxLicKeysID);
            installation.IdComputer = ViewportHelper.ValueOrNull<int>(comboBoxComputerID);
            installation.InstallationDate = ViewportHelper.ValueOrNull(dateTimePickerInstallDate);
            return installation;
        }

        private SoftInstallation InstallationFromView()
        {
            SoftInstallation installation = new SoftInstallation();
            DataRowView row = (DataRowView)v_softInstallations[v_softInstallations.Position];
            installation.IdInstallation = ViewportHelper.ValueOrNull<int>(row, "ID Installation");
            installation.IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License");
            installation.IdComputer = ViewportHelper.ValueOrNull<int>(row, "ID Computer");
            installation.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey");
            installation.IdInstallator = ViewportHelper.ValueOrNull<int>(row, "ID Installator");
            installation.InstallationDate = ViewportHelper.ValueOrNull<DateTime>(row, "InstallationDate");
            return installation;
        }

        private static void FillRowFromInstallation(SoftInstallation installation, DataRowView row)
        {
            row.BeginEdit();
            row["ID Installation"] = ViewportHelper.ValueOrDBNull(installation.IdInstallation);
            row["ID License"] = ViewportHelper.ValueOrDBNull(installation.IdLicense);
            row["ID Computer"] = ViewportHelper.ValueOrDBNull(installation.IdComputer);
            row["ID LicenseKey"] = ViewportHelper.ValueOrDBNull(installation.IdLicenseKey);
            row["ID Installator"] = ViewportHelper.ValueOrDBNull(installation.IdInstallator);
            row["InstallationDate"] = ViewportHelper.ValueOrDBNull(installation.InstallationDate);
            row.EndEdit();
        }

        private bool ValidateInstallation(SoftInstallation installation)
        {
            if (installation.IdLicense == null)
            {
                MessageBox.Show("Необходимо выбрать лицензию на программное обеспечение, по которой осуществляется установка", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicenseID.Focus();
                return false;
            }
            if (installation.IdComputer == null)
            {
                MessageBox.Show("Необходимо выбрать компьютер, на который производится установка программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxComputerID.Focus();
                return false;
            }
            if (installation.IdInstallator == null)
            {
                MessageBox.Show("Необходимо выбрать установщика программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicenseID.Focus();
                return false;
            }
            if (installation.IdLicenseKey != null && (int)SoftLicensesDataModel.GetInstance().Select().Rows.Find(installation.IdLicense)["ID LicType"] != 1 &&
                !DataModelHelper.KeyIsFree(installation.IdLicenseKey.Value))
            {
                DialogResult result = MessageBox.Show("Данный лицензионный ключ уже используется. Вы уверены, что хотите продолжить сохранение?", "Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result != DialogResult.Yes)
                    return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return v_softInstallations.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_softInstallations.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_softInstallations.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_softInstallations.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_softInstallations.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_softInstallations.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_softInstallations.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_softInstallations.Position > -1) && (v_softInstallations.Position < (v_softInstallations.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_softInstallations.Position > -1) && (v_softInstallations.Position < (v_softInstallations.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softwareDM = CalcDataModelSoftwareConcat.GetInstance();
            licenses = CalcDataModelLicensesConcat.GetInstance();
            softLicKeys = SoftLicKeysDataModel.GetInstance();
            devices = DevicesDataModel.GetInstance();
            departments = DepartmentsDataModel.GetInstance();
            softInstallators = SoftInstallatorsDataModel.GetInstance();
            softInstallations = SoftInstallationsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            softwareDM.Select();
            licenses.Select();
            softLicKeys.Select();
            devices.Select();
            departments.Select();
            softInstallators.Select();
            softInstallations.Select();

            DataSet ds = DataSetManager.DataSet;

            v_departments = new BindingSource();
            v_departments.DataSource = departments.SelectVisibleDepartments();

            v_devices = new BindingSource();
            v_devices.DataMember = "Devices";
            v_devices.DataSource = ds;
            v_devices.Filter = DepartmentFilter();

            v_software = new BindingSource();
            v_software.DataMember = "SoftwareConcat";
            v_software.DataSource = ds;

            v_licenses = new BindingSource();
            v_licenses.DataMember = "LicensesConcat";
            v_licenses.DataSource = ds;
            v_licenses.Filter = DepartmentFilter();

            v_softLicKeys = new BindingSource();
            v_softLicKeys.DataMember = "SoftLicKeys";
            v_softLicKeys.DataSource = ds;

            v_softInstallators = new BindingSource();
            v_softInstallators.DataMember = "SoftInstallators";
            v_softInstallators.DataSource = ds;

            v_softInstallations = new BindingSource();
            v_softInstallations.CurrentItemChanged += new EventHandler(v_softInstallations_CurrentItemChanged);
            v_softInstallations.DataMember = "SoftInstallations";
            v_softInstallations.DataSource = ds;
            RebuildFilter();

            DataBind();

            softInstallations.Select().RowChanged += InstallationsViewport_RowChanged;
            softInstallations.Select().RowDeleted += InstallationsViewport_RowDeleted;

            softLicKeys.Select().RowChanged += softLicKeys_RowChanged;
            softLicKeys.Select().RowDeleted += softLicKeys_RowDeleted;

            dataGridView.RowCount = v_softInstallations.Count;
            SetViewportCaption();

            softwareDM.RefreshEvent += softwareDM_RefreshEvent;
            licenses.RefreshEvent += licenses_RefreshEvent;

            devices.Select().RowChanged += Devices_RowChanged;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            is_editable = true;
        }

        private void RebuildFilter()
        {
            string Filter = StaticFilter;
            // Фильтрация по правам на департаменты
            if (!String.IsNullOrEmpty(Filter))
                Filter += " AND ";
            Filter += ComputerFilter();
            if (!String.IsNullOrEmpty(Filter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            v_softInstallations.Filter = Filter;
        }

        private string ComputerFilter()
        {
            string DeviceFilter = v_devices.Filter;
            int DevicePosition = v_devices.Position;
            v_devices.Filter = DepartmentFilter();
            string Filter = "[ID Computer] IN (0";
            for (int i = 0; i < v_devices.Count; i++)
                Filter += ((DataRowView)v_devices[i])["ID Device"] + ",";
            Filter = Filter.TrimEnd(',');
            Filter += ")";
            v_devices.Filter = DeviceFilter;
            v_devices.Position = DevicePosition;
            return Filter;
        }

        private string DepartmentFilter()
        {
            string DepartmentFilter = "[ID Department] IN (0";
            for (int i = 0; i < v_departments.Count; i++)
                if ((bool)((DataRowView)v_departments[i])["AllowSelect"])
                    DepartmentFilter += ((DataRowView)v_departments[i])["ID Department"] + ",";
            DepartmentFilter = DepartmentFilter.TrimEnd(',');
            DepartmentFilter += ")";
            return DepartmentFilter;
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (!String.IsNullOrEmpty(DynamicFilter))
                return true;
            else
                return false;
        }

        public override void SearchRecord()
        {
            if (sSearchForm == null)
                sSearchForm = new SearchInstallationsForm();
            if (sSearchForm.ShowDialog() != DialogResult.OK)
                return;
            DynamicFilter = sSearchForm.GetFilter();
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            v_softInstallations.Filter = Filter;
            dataGridView.RowCount = v_softInstallations.Count;
        }

        public override void ClearSearch()
        {
            v_softInstallations.Filter = StaticFilter;
            dataGridView.RowCount = v_softInstallations.Count;
            DynamicFilter = "";
            MenuCallback.EditingStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.NavigationStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return (!softInstallations.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.INSTALLATIONS_READ_WRITE);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            v_softInstallations.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.License)
            {
                comboBoxSoftwareID.SelectedValue = ParentRow["ID Software"];
                comboBoxLicenseID.SelectedValue = ParentRow["ID License"];
            }
            dataGridView.Enabled = false;
            is_editable = true;
            softInstallations.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_softInstallations.Position != -1) && (!softInstallations.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.INSTALLATIONS_READ_WRITE);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            SoftInstallation installation = InstallationFromView();
            v_softInstallations.AddNew();
            dataGridView.Enabled = false;
            softInstallations.EditingNewRecord = true;
            ViewportFromInstallation(installation);
            is_editable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (SoftInstallationsDataModel.Delete((int)((DataRowView)v_softInstallations.Current)["ID Installation"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_softInstallations[v_softInstallations.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            return (v_softInstallations.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.INSTALLATIONS_READ_WRITE);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            InstallationsViewport viewport = new InstallationsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_softInstallations.Count > 0)
                viewport.LocateInstallation((((DataRowView)v_softInstallations[v_softInstallations.Position])["ID Installation"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.INSTALLATIONS_READ_WRITE);
        }

        public override void SaveRecord()
        {
            SoftInstallation installation = InstallationFromViewport();
            if (!ValidateInstallation(installation))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int idInstallation = SoftInstallationsDataModel.Insert(installation);
                    if (idInstallation == -1)
                        return;
                    DataRowView newRow;
                    installation.IdInstallation = idInstallation;
                    is_editable = false;
                    if (v_softInstallations.Position == -1)
                        newRow = (DataRowView)v_softInstallations.AddNew();
                    else
                        newRow = ((DataRowView)v_softInstallations[v_softInstallations.Position]);
                    FillRowFromInstallation(installation, newRow);
                    softInstallations.EditingNewRecord = false;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (installation.IdLicense == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись об установке по лицензии без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (SoftInstallationsDataModel.Update(installation) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_softInstallations[v_softInstallations.Position]);
                    is_editable = false;
                    FillRowFromInstallation(installation, row);
                    break;
            }
            dataGridView.Enabled = true;
            is_editable = true;
            dataGridView.RowCount = v_softInstallations.Count;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    softInstallations.EditingNewRecord = false;
                    if (v_softInstallations.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_softInstallations[v_softInstallations.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (v_softInstallations.Position != -1)
                            dataGridView.Rows[v_softInstallations.Position].Selected = true;
                    }
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    SelectCurrentAutoCompleteValue();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            softInstallations.Select().RowChanged -= InstallationsViewport_RowChanged;
            softInstallations.Select().RowDeleted -= InstallationsViewport_RowDeleted;
            softwareDM.RefreshEvent -= softwareDM_RefreshEvent;
            licenses.RefreshEvent -= licenses_RefreshEvent;
            devices.Select().RowChanged -= Devices_RowChanged;
            softLicKeys.Select().RowChanged -= softLicKeys_RowChanged;
            softLicKeys.Select().RowDeleted -= softLicKeys_RowDeleted;
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                softInstallations.EditingNewRecord = false;
            softInstallations.Select().RowChanged -= InstallationsViewport_RowChanged;
            softInstallations.Select().RowDeleted -= InstallationsViewport_RowDeleted;
            softwareDM.RefreshEvent -= softwareDM_RefreshEvent;
            licenses.RefreshEvent -= licenses_RefreshEvent;
            devices.Select().RowChanged -= Devices_RowChanged;
            softLicKeys.Select().RowChanged -= softLicKeys_RowChanged;
            softLicKeys.Select().RowDeleted -= softLicKeys_RowDeleted;
            base.Close();
        }

        void softwareDM_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        void licenses_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        void InstallationsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridView.RowCount = v_softInstallations.Count;
                dataGridView.Refresh();
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        void InstallationsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_softInstallations.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void softLicKeys_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.Refresh();
        }

        private void softLicKeys_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            dataGridView.Refresh();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        private void Devices_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                RebuildFilter();
        }

        void v_softInstallations_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption(); 
            SelectCurrentAutoCompleteValue();
            if (v_softInstallations.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_softInstallations.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[1];
                }
                else
                    if (dataGridView.Rows[v_softInstallations.Position].Selected != true)
                    {
                        dataGridView.Rows[v_softInstallations.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[v_softInstallations.Position].Cells[1];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            if (v_softInstallations.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void comboBoxSoftwareID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                string text = comboBoxSoftwareID.Text;
                int selectionStart = comboBoxSoftwareID.SelectionStart;
                int selectionLength = comboBoxSoftwareID.SelectionLength;
                v_software.Filter = "Software like '%" + comboBoxSoftwareID.Text + "%'";
                comboBoxSoftwareID.Text = text;
                comboBoxSoftwareID.SelectionStart = selectionStart;
                comboBoxSoftwareID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxSoftwareID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxSoftwareID.Items.Count == 0)
                comboBoxSoftwareID.SelectedIndex = -1;
        }

        private void comboBoxSoftwareID_Leave(object sender, EventArgs e)
        {
            if (comboBoxSoftwareID.Items.Count > 0)
            {
                if (comboBoxSoftwareID.SelectedItem == null)
                    comboBoxSoftwareID.SelectedItem = v_software[v_software.Position];
            }
            if (comboBoxSoftwareID.SelectedItem == null)
            {
                comboBoxSoftwareID.Text = "";
                v_software.Filter = "";
            }
        }

        private void comboBoxLicenseID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                string text = comboBoxLicenseID.Text;
                int selectionStart = comboBoxLicenseID.SelectionStart;
                int selectionLength = comboBoxLicenseID.SelectionLength;
                v_licenses.Filter = DepartmentFilter() + " AND [ID Software] = " + (comboBoxSoftwareID.SelectedValue != null ? comboBoxSoftwareID.SelectedValue : "0") + " AND License like '%" + comboBoxLicenseID.Text + "%'";
                comboBoxLicenseID.Text = text;
                comboBoxLicenseID.SelectionStart = selectionStart;
                comboBoxLicenseID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxLicenseID_Leave(object sender, EventArgs e)
        {
            if (comboBoxLicenseID.Items.Count > 0)
            {
                if (comboBoxLicenseID.SelectedItem == null)
                    comboBoxLicenseID.SelectedItem = v_licenses[v_licenses.Position];
            }
            if (comboBoxLicenseID.SelectedItem == null)
            {
                comboBoxLicenseID.Text = "";
                v_licenses.Filter = DepartmentFilter() + " AND [ID Software] = " + (comboBoxSoftwareID.SelectedValue != null ? comboBoxSoftwareID.SelectedValue : "0");
            }
        }

        private void comboBoxLicenseID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxLicenseID.Items.Count == 0)
                comboBoxLicenseID.SelectedIndex = -1;
        }

        private void comboBoxLicKeysID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                string text = comboBoxLicKeysID.Text;
                int selectionStart = comboBoxLicKeysID.SelectionStart;
                int selectionLength = comboBoxLicKeysID.SelectionLength;
                v_softLicKeys.Filter = "[ID License] = " + (comboBoxLicenseID.SelectedValue != null ? comboBoxLicenseID.SelectedValue : "0") + " AND LicKey like '%" + comboBoxLicKeysID.Text + "%'";
                comboBoxLicKeysID.Text = text;
                comboBoxLicKeysID.SelectionStart = selectionStart;
                comboBoxLicKeysID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxLicKeysID_Leave(object sender, EventArgs e)
        {
            if (comboBoxLicKeysID.SelectedItem == null)
            {
                comboBoxLicKeysID.Text = "";
                v_softLicKeys.Filter = "[ID License] = " + (comboBoxLicenseID.SelectedValue != null ? comboBoxLicenseID.SelectedValue : "0");
            }
            if (String.IsNullOrEmpty(comboBoxLicKeysID.Text))
                comboBoxLicKeysID.SelectedItem = null;
        }

        private void comboBoxLicKeysID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxLicKeysID.Items.Count == 0)
                comboBoxLicKeysID.SelectedIndex = -1;
        }

        private void comboBoxComputerID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                string text = comboBoxComputerID.Text;
                int selectionStart = comboBoxComputerID.SelectionStart;
                int selectionLength = comboBoxComputerID.SelectionLength;
                v_devices.Filter = "[Device Name] like '%" + comboBoxComputerID.Text + "%'";
                comboBoxComputerID.Text = text;
                comboBoxComputerID.SelectionStart = selectionStart;
                comboBoxComputerID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxComputerID_Leave(object sender, EventArgs e)
        {
            if (comboBoxComputerID.Items.Count > 0)
            {
                if (comboBoxComputerID.SelectedItem == null)
                    comboBoxComputerID.SelectedItem = v_devices[v_devices.Position];
            }
            if (comboBoxComputerID.SelectedItem == null)
            {
                comboBoxComputerID.Text = "";
                v_devices.Filter = DepartmentFilter();
            }
        }

        private void comboBoxComputerID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxComputerID.Items.Count == 0)
                comboBoxComputerID.SelectedIndex = -1;
        }

        private void comboBoxComputerID_SelectedValueChanged(object sender, EventArgs e)
        {

            CheckViewportModifications();
        }

        private void comboBoxInstallatorID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dateTimePickerInstallationDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxSoftwareID_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((comboBoxSoftwareID.DataSource != null) && (comboBoxLicenseID.DataSource != null) && (comboBoxLicKeysID.DataSource != null) &&
                !String.IsNullOrEmpty(comboBoxSoftwareID.ValueMember))
            {
                int? idSoftware = (int?)comboBoxSoftwareID.SelectedValue;
                if (idSoftware != null)
                    v_licenses.Filter = DepartmentFilter() + " AND [ID Software] = " + idSoftware.ToString();
                else
                    v_licenses.Filter = "1 = 0";
            }
        }

        private void comboBoxLicenseID_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((comboBoxSoftwareID.DataSource != null) && (comboBoxLicenseID.DataSource != null) && (comboBoxLicKeysID.DataSource != null) &&
                !String.IsNullOrEmpty(comboBoxLicenseID.ValueMember))
            {
                int? idLicense = (int?)comboBoxLicenseID.SelectedValue;
                if (idLicense != null)
                    v_softLicKeys.Filter = "[ID License] = " + idLicense.ToString();
                else
                    v_softLicKeys.Filter = "1 = 0";
            }
            CheckViewportModifications();
        }

        private void comboBoxLicKeysID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_softInstallations.Count <= e.RowIndex) return;
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "idInstallation":
                    e.Value = ((DataRowView)v_softInstallations[e.RowIndex])["ID Installation"];
                    break;
                case "installationDate":
                    e.Value = ((DataRowView)v_softInstallations[e.RowIndex])["InstallationDate"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_softInstallations[e.RowIndex])["InstallationDate"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "software":
                    DataRow row = licenses.Select().Rows.Find(((DataRowView)v_softInstallations[e.RowIndex])["ID License"]);
                    if (row != null)
                    {
                        row = softwareDM.Select().Rows.Find(row["ID Software"]);
                        if (row != null)
                            e.Value = row["Software"];
                    }
                    break;
                case "department":
                    row = devices.Select().Rows.Find(((DataRowView)v_softInstallations[e.RowIndex])["ID Computer"]);
                    if (row != null)
                    {
                        int row_index = v_departments.Find("ID Department", row["ID Department"]);
                        if (row_index != -1)
                            e.Value = ((DataRowView)v_departments[row_index])["Department"].ToString().Trim();
                    }
                    break;
                case "computer":
                    row = devices.Select().Rows.Find(((DataRowView)v_softInstallations[e.RowIndex])["ID Computer"]);
                    if (row != null)
                        e.Value = row["Device Name"];
                    break;
                case "license":
                    row = licenses.Select().Rows.Find(((DataRowView)v_softInstallations[e.RowIndex])["ID License"]);
                    if (row != null)
                        e.Value = row["License"];
                    break;
                case "licKey":
                    row = softLicKeys.Select().Rows.Find(((DataRowView)v_softInstallations[e.RowIndex])["ID LicenseKey"]);
                    if (row != null)
                        e.Value = row["LicKey"];
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                v_softInstallations.Position = dataGridView.SelectedRows[0].Index;
            else
                v_softInstallations.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_softInstallations.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
            dataGridView.Refresh();
        }

        private void comboBoxSoftwareID_VisibleChanged(object sender, EventArgs e)
        {
            if (is_first_visibility)
            {
                SelectCurrentAutoCompleteValue();
                is_first_visibility = false;
            }
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationsViewport));
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxLicenseID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLicKeysID = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxSoftwareID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxComputerID = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxInstallatorID = new System.Windows.Forms.ComboBox();
            this.dateTimePickerInstallDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.idInstallation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.software = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.department = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.computer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.installationDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.license = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.licKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel14.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 2;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 119F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(650, 316);
            this.tableLayoutPanel14.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idInstallation,
            this.software,
            this.department,
            this.computer,
            this.installationDate,
            this.license,
            this.licKey});
            this.tableLayoutPanel14.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 122);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(644, 191);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxLicenseID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxLicKeysID);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.comboBoxSoftwareID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(319, 113);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сведения о лицензии";
            // 
            // comboBoxLicenseID
            // 
            this.comboBoxLicenseID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLicenseID.FormattingEnabled = true;
            this.comboBoxLicenseID.Location = new System.Drawing.Point(161, 51);
            this.comboBoxLicenseID.Name = "comboBoxLicenseID";
            this.comboBoxLicenseID.Size = new System.Drawing.Size(146, 23);
            this.comboBoxLicenseID.TabIndex = 1;
            this.comboBoxLicenseID.DropDownClosed += new System.EventHandler(this.comboBoxLicenseID_DropDownClosed);
            this.comboBoxLicenseID.SelectedValueChanged += new System.EventHandler(this.comboBoxLicenseID_SelectedValueChanged);
            this.comboBoxLicenseID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxLicenseID_KeyUp);
            this.comboBoxLicenseID.Leave += new System.EventHandler(this.comboBoxLicenseID_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 15);
            this.label3.TabIndex = 75;
            this.label3.Text = "Лицензия";
            // 
            // comboBoxLicKeysID
            // 
            this.comboBoxLicKeysID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLicKeysID.FormattingEnabled = true;
            this.comboBoxLicKeysID.Location = new System.Drawing.Point(161, 80);
            this.comboBoxLicKeysID.Name = "comboBoxLicKeysID";
            this.comboBoxLicKeysID.Size = new System.Drawing.Size(146, 23);
            this.comboBoxLicKeysID.TabIndex = 2;
            this.comboBoxLicKeysID.DropDownClosed += new System.EventHandler(this.comboBoxLicKeysID_DropDownClosed);
            this.comboBoxLicKeysID.SelectedValueChanged += new System.EventHandler(this.comboBoxLicKeysID_SelectedValueChanged);
            this.comboBoxLicKeysID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxLicKeysID_KeyUp);
            this.comboBoxLicKeysID.Leave += new System.EventHandler(this.comboBoxLicKeysID_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 15);
            this.label8.TabIndex = 86;
            this.label8.Text = "Лицензионный ключ";
            // 
            // comboBoxSoftwareID
            // 
            this.comboBoxSoftwareID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSoftwareID.FormattingEnabled = true;
            this.comboBoxSoftwareID.Location = new System.Drawing.Point(161, 22);
            this.comboBoxSoftwareID.Name = "comboBoxSoftwareID";
            this.comboBoxSoftwareID.Size = new System.Drawing.Size(146, 23);
            this.comboBoxSoftwareID.TabIndex = 0;
            this.comboBoxSoftwareID.DropDownClosed += new System.EventHandler(this.comboBoxSoftwareID_DropDownClosed);
            this.comboBoxSoftwareID.SelectedValueChanged += new System.EventHandler(this.comboBoxSoftwareID_SelectedValueChanged);
            this.comboBoxSoftwareID.VisibleChanged += new System.EventHandler(this.comboBoxSoftwareID_VisibleChanged);
            this.comboBoxSoftwareID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxSoftwareID_KeyUp);
            this.comboBoxSoftwareID.Leave += new System.EventHandler(this.comboBoxSoftwareID_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 73;
            this.label2.Text = "Наименование ПО";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxComputerID);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.comboBoxInstallatorID);
            this.groupBox2.Controls.Add(this.dateTimePickerInstallDate);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(328, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(319, 113);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Сведения об установке";
            // 
            // comboBoxComputerID
            // 
            this.comboBoxComputerID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxComputerID.FormattingEnabled = true;
            this.comboBoxComputerID.Location = new System.Drawing.Point(161, 22);
            this.comboBoxComputerID.Name = "comboBoxComputerID";
            this.comboBoxComputerID.Size = new System.Drawing.Size(145, 23);
            this.comboBoxComputerID.TabIndex = 0;
            this.comboBoxComputerID.DropDownClosed += new System.EventHandler(this.comboBoxComputerID_DropDownClosed);
            this.comboBoxComputerID.SelectedValueChanged += new System.EventHandler(this.comboBoxComputerID_SelectedValueChanged);
            this.comboBoxComputerID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxComputerID_KeyUp);
            this.comboBoxComputerID.Leave += new System.EventHandler(this.comboBoxComputerID_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 15);
            this.label4.TabIndex = 77;
            this.label4.Text = "Компьютер";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 15);
            this.label9.TabIndex = 85;
            this.label9.Text = "Дата установки";
            // 
            // comboBoxInstallatorID
            // 
            this.comboBoxInstallatorID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxInstallatorID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInstallatorID.FormattingEnabled = true;
            this.comboBoxInstallatorID.Location = new System.Drawing.Point(161, 80);
            this.comboBoxInstallatorID.Name = "comboBoxInstallatorID";
            this.comboBoxInstallatorID.Size = new System.Drawing.Size(145, 23);
            this.comboBoxInstallatorID.TabIndex = 2;
            this.comboBoxInstallatorID.SelectedIndexChanged += new System.EventHandler(this.comboBoxInstallatorID_SelectedIndexChanged);
            // 
            // dateTimePickerInstallDate
            // 
            this.dateTimePickerInstallDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerInstallDate.Location = new System.Drawing.Point(161, 53);
            this.dateTimePickerInstallDate.Name = "dateTimePickerInstallDate";
            this.dateTimePickerInstallDate.Size = new System.Drawing.Size(145, 21);
            this.dateTimePickerInstallDate.TabIndex = 1;
            this.dateTimePickerInstallDate.ValueChanged += new System.EventHandler(this.dateTimePickerInstallationDate_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 15);
            this.label5.TabIndex = 77;
            this.label5.Text = "Установщик";
            // 
            // idInstallation
            // 
            this.idInstallation.Frozen = true;
            this.idInstallation.HeaderText = "Идентификатор";
            this.idInstallation.Name = "idInstallation";
            this.idInstallation.ReadOnly = true;
            this.idInstallation.Visible = false;
            // 
            // software
            // 
            this.software.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.software.HeaderText = "Наименование ПО";
            this.software.MinimumWidth = 300;
            this.software.Name = "software";
            this.software.ReadOnly = true;
            this.software.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // department
            // 
            this.department.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.department.HeaderText = "Департамент";
            this.department.MinimumWidth = 300;
            this.department.Name = "department";
            this.department.ReadOnly = true;
            this.department.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.department.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // computer
            // 
            this.computer.HeaderText = "Компьютер";
            this.computer.MinimumWidth = 200;
            this.computer.Name = "computer";
            this.computer.ReadOnly = true;
            this.computer.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.computer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // installationDate
            // 
            this.installationDate.HeaderText = "Дата установки";
            this.installationDate.MinimumWidth = 130;
            this.installationDate.Name = "installationDate";
            this.installationDate.ReadOnly = true;
            // 
            // license
            // 
            this.license.HeaderText = "Лицензия";
            this.license.MinimumWidth = 250;
            this.license.Name = "license";
            this.license.ReadOnly = true;
            // 
            // licKey
            // 
            this.licKey.HeaderText = "Лицензионный ключ";
            this.licKey.MinimumWidth = 150;
            this.licKey.Name = "licKey";
            this.licKey.ReadOnly = true;
            // 
            // InstallationsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(650, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(652, 322);
            this.Controls.Add(this.tableLayoutPanel14);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InstallationsViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Установки ПО";
            this.tableLayoutPanel14.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}