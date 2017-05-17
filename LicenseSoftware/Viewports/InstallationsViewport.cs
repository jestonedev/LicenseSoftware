using LicenseSoftware.CalcDataModels;
using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using LicenseSoftware.SearchForms;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DataModels.DataModels;

namespace LicenseSoftware.Viewport
{
    public sealed class InstallationsViewport: Viewport
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
        private DataGridViewTextBoxColumn idInstallation;
        private DataGridViewTextBoxColumn software;
        private DataGridViewTextBoxColumn licKey;
        private DataGridViewTextBoxColumn department;
        private DataGridViewTextBoxColumn computer;
        private DataGridViewTextBoxColumn serialNum;
        private DataGridViewTextBoxColumn invNum;
        private DataGridViewTextBoxColumn installationDate;
        private DataGridViewTextBoxColumn license;
        private Label label1;
        private TextBox textBoxDescription;
        private ComboBox comboBoxSoftVersionID;
        private Label label6;
        #endregion Components

        #region Models

        private CalcDataModelSoftwareConcat _softwareConcat;
        private CalcDataModelLicensesConcat _licenses;
        private SoftLicKeysDataModel _softLicKeys;
        private SoftInstallationsDataModel _softInstallations;
        private DepartmentsDataModel _departments;
        private SoftInstallatorsDataModel _softInstallators;
        private DevicesDataModel _devices;
        private SoftVersionsDataModel _softVersions;
        private SoftwareDataModel _softwareDataModel;
        #endregion Models

        #region Views

        private BindingSource _vLicenses;
        private BindingSource _vSoftLicKeys;
        private BindingSource _vSoftInstallations;
        private BindingSource _vDepartments;
        private BindingSource _vSoftInstallators;
        private BindingSource _vDevices;
        private BindingSource _vSoftware;
        private BindingSource _vSoftVersions;
        #endregion Views

        //State
        private ViewportState _viewportState = ViewportState.ReadState;

        private bool _isEditable;
        private SearchForm _sSearchForm;
        private bool _isFirstVisibility = true;

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
            DynamicFilter = installationsViewport.DynamicFilter;
            StaticFilter = installationsViewport.StaticFilter;
            ParentRow = installationsViewport.ParentRow;
            ParentType = installationsViewport.ParentType;
        }

        private void SetViewportCaption()
        {
            if (_viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "Установки по лицензии №{0}", ParentRow["ID License"]);
                }
                else
                    Text = "Новая установка";
            }
            else
                if (_vSoftInstallations.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                        Text = string.Format(CultureInfo.InvariantCulture, "Установка №{0} по лицензии №{1}",
                            ((DataRowView)_vSoftInstallations[_vSoftInstallations.Position])["ID Installation"], ParentRow["ID License"]);
                    else
                        Text = string.Format(CultureInfo.InvariantCulture, "Установка №{0}",
                            ((DataRowView)_vSoftInstallations[_vSoftInstallations.Position])["ID Installation"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                        Text = string.Format(CultureInfo.InvariantCulture, "Установки по лицензии №{0} отсутствуют", ParentRow["ID License"]);
                    else
                        Text = "Установки отсутствуют";
                }
        }

        private void SelectCurrentAutoCompleteValue()
        {
            if (_vSoftInstallations.Position == -1)
                return;
            var installationRow = (DataRowView)_vSoftInstallations[_vSoftInstallations.Position];
            if ((comboBoxSoftwareID.DataSource == null) || (comboBoxLicenseID.DataSource == null) ||
                (comboBoxLicKeysID.DataSource == null) || comboBoxSoftVersionID.DataSource == null)
                return;
            if (string.IsNullOrEmpty(comboBoxSoftwareID.ValueMember) || string.IsNullOrEmpty(comboBoxLicenseID.ValueMember) ||
                string.IsNullOrEmpty(comboBoxLicKeysID.ValueMember) || string.IsNullOrEmpty(comboBoxSoftVersionID.ValueMember))
                return;
            int? idSoftware = null;
            int? idVersion = null;
            int? idLicense = null;
            int? idLicKey = null;
            if (installationRow["ID License"] != DBNull.Value)
                idLicense = Convert.ToInt32(installationRow["ID License"], CultureInfo.InvariantCulture);
            else
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.License))
                    idLicense = Convert.ToInt32(ParentRow["ID License"], CultureInfo.InvariantCulture);
            if (idLicense != null)
            {
                var licesneRow = _licenses.Select().Rows.Find(idLicense);
                if (licesneRow != null)
                    idVersion = (int)licesneRow["ID Version"];
            }
            if (idVersion != null)
            {
                var versionRow = _softVersions.Select().Rows.Find(idVersion);
                if (versionRow != null)
                    idSoftware = (int)versionRow["ID Software"];
            }
            if (installationRow["ID LicenseKey"] != DBNull.Value)
                idLicKey = Convert.ToInt32(installationRow["ID LicenseKey"], CultureInfo.InvariantCulture);
            _vSoftware.Filter = "";
            comboBoxSoftwareID.SelectedValue = (object)idSoftware ?? DBNull.Value;
            comboBoxSoftVersionID.SelectedValue = (object)idVersion ?? DBNull.Value;
            comboBoxLicenseID.SelectedValue = (object)idLicense ?? DBNull.Value;
            if (_vLicenses.Position != -1)
            {
                _vSoftLicKeys.Filter = "[ID License] = " + ((DataRowView)_vLicenses[_vLicenses.Position])["ID License"]
                     + " AND [ID LicenseKey] IN (0" + GetLicKeysFilter((int)((DataRowView)_vLicenses[_vLicenses.Position])["ID License"]) + ")";
                comboBoxLicKeysID.SelectedValue = (object)idLicKey ?? DBNull.Value;
            }
            else
                comboBoxLicKeysID.SelectedValue = DBNull.Value;
            if (comboBoxComputerID.DataSource != null && !string.IsNullOrEmpty(comboBoxComputerID.ValueMember))
            {
                _vDevices.Filter = GetAllowedDepartmentFilter();
                int? idComputer = null;
                if (installationRow["ID Computer"] != DBNull.Value)
                    idComputer = (int)installationRow["ID Computer"];
                comboBoxComputerID.SelectedValue = (object)idComputer ?? DBNull.Value;
            }
        }

        private void DataBind()
        {
            comboBoxComputerID.DataSource = _vDevices;
            comboBoxComputerID.ValueMember = "ID Device";
            comboBoxComputerID.DisplayMember = "Device Name";
            
            comboBoxInstallatorID.DataSource = _vSoftInstallators;
            comboBoxInstallatorID.ValueMember = "ID Installator";
            comboBoxInstallatorID.DisplayMember = "FullName";
            comboBoxInstallatorID.DataBindings.Clear();
            comboBoxInstallatorID.DataBindings.Add("SelectedValue", _vSoftInstallations, "ID Installator", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxLicKeysID.DataSource = _vSoftLicKeys;
            comboBoxLicKeysID.ValueMember = "ID LicenseKey";
            comboBoxLicKeysID.DisplayMember = "LicKey";

            comboBoxLicenseID.DataSource = _vLicenses;
            comboBoxLicenseID.ValueMember = "ID License";
            comboBoxLicenseID.DisplayMember = "License";

            comboBoxSoftwareID.DataSource = _vSoftware;
            comboBoxSoftwareID.ValueMember = "ID Software";
            comboBoxSoftwareID.DisplayMember = "Software";

            comboBoxSoftVersionID.DataSource = _vSoftVersions;
            comboBoxSoftVersionID.ValueMember = "ID Version";
            comboBoxSoftVersionID.DisplayMember = "Version";

            dateTimePickerInstallDate.DataBindings.Clear();
            dateTimePickerInstallDate.DataBindings.Add("Value", _vSoftInstallations, "InstallationDate", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
            
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", _vSoftInstallations, "Description", true, DataSourceUpdateMode.Never, "");
        }

        private void CheckViewportModifications()
        {
            if (!_isEditable)
                return;
            if ((!ContainsFocus) || (dataGridView.Focused))
                return;
            if ((_vSoftInstallations.Position != -1) && (InstallationFromView() != InstallationFromViewport()))
            {
                if (_viewportState == ViewportState.ReadState)
                {
                    _viewportState = ViewportState.ModifyRowState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = false;
                }
            }
            else
            {
                if (_viewportState == ViewportState.ModifyRowState)
                {
                    _viewportState = ViewportState.ReadState;
                    MenuCallback.EditingStateUpdate();
                    dataGridView.Enabled = true;
                }
            }
        }

        private bool ChangeViewportStateTo(ViewportState state)
        {
            if (!AccessControl.HasPrivelege(Priveleges.InstallationsReadWrite))
            {
                _viewportState = ViewportState.ReadState;
                return true;
            }
            switch (state)
            {
                case ViewportState.ReadState:
                    switch (_viewportState)
                    {
                        case ViewportState.ReadState:
                            return true;
                        case ViewportState.NewRowState:
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            if (_viewportState == ViewportState.ReadState)
                                return true;
                            else
                                return false;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (_viewportState)
                    {
                        case ViewportState.ReadState:
                            if (_softInstallations.EditingNewRecord)
                                return false;
                            else
                            {
                                _viewportState = ViewportState.NewRowState;
                                return true;
                            }
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (_viewportState == ViewportState.ReadState)
                                return ChangeViewportStateTo(ViewportState.NewRowState);
                            else
                                return false;
                    }
                    break;
                case ViewportState.ModifyRowState: ;
                    switch (_viewportState)
                    {
                        case ViewportState.ReadState:
                            _viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else
                                    return false;
                            if (_viewportState == ViewportState.ReadState)
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
            var Position = _vSoftInstallations.Find("ID Installation", id);
            _isEditable = false;
            if (Position > 0)
                _vSoftInstallations.Position = Position;
            _isEditable = true;
        }

        private void ViewportFromInstallation(SoftInstallation installation)
        {
            if (installation.IdLicense != null)
            {
                _vSoftware.Filter = "";
                _vSoftVersions.Filter = "";
                _vLicenses.Filter = "";
                _vSoftLicKeys.Filter = "";
                var index = _vLicenses.Find("ID License", installation.IdLicense);
                if (index != -1)
                {
                    var licenseRow = (DataRowView) _vLicenses[index];
                    var softVersionRow = _softVersions.Select().Rows.Find(licenseRow["ID Version"]);
                    if (softVersionRow != null)
                    {
                        comboBoxSoftwareID.SelectedValue = softVersionRow["ID Software"];
                        comboBoxSoftVersionID.SelectedValue = licenseRow["ID Version"];
                        comboBoxLicenseID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdLicense);
                        comboBoxLicKeysID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdLicenseKey);   
                    }
                }
            }

            comboBoxComputerID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdComputer);
            comboBoxInstallatorID.SelectedValue = ViewportHelper.ValueOrDBNull(installation.IdInstallator);
            dateTimePickerInstallDate.Value = ViewportHelper.ValueOrDefault(installation.InstallationDate);
            textBoxDescription.Text = installation.Description;
        }

        private SoftInstallation InstallationFromViewport()
        {
            var installation = new SoftInstallation
            {
                IdInstallation =
                    _vSoftInstallations.Position == -1
                        ? null
                        : ViewportHelper.ValueOrNull<int>(
                            (DataRowView) _vSoftInstallations[_vSoftInstallations.Position], "ID Installation"),
                IdLicense = ViewportHelper.ValueOrNull<int>(comboBoxLicenseID),
                IdInstallator = ViewportHelper.ValueOrNull<int>(comboBoxInstallatorID),
                IdLicenseKey = ViewportHelper.ValueOrNull<int>(comboBoxLicKeysID),
                IdComputer = ViewportHelper.ValueOrNull<int>(comboBoxComputerID),
                InstallationDate = ViewportHelper.ValueOrNull(dateTimePickerInstallDate),
                Description = ViewportHelper.ValueOrNull(textBoxDescription)
            };
            return installation;
        }

        private SoftInstallation InstallationFromView()
        {
            var installation = new SoftInstallation();
            var row = (DataRowView)_vSoftInstallations[_vSoftInstallations.Position];
            installation.IdInstallation = ViewportHelper.ValueOrNull<int>(row, "ID Installation");
            installation.IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License");
            installation.IdComputer = ViewportHelper.ValueOrNull<int>(row, "ID Computer");
            installation.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey");
            installation.IdInstallator = ViewportHelper.ValueOrNull<int>(row, "ID Installator");
            installation.InstallationDate = ViewportHelper.ValueOrNull<DateTime>(row, "InstallationDate");
            installation.Description = ViewportHelper.ValueOrNull(row, "Description");
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
            row["Description"] = ViewportHelper.ValueOrDBNull(installation.Description);
            row.EndEdit();
        }

        private bool ValidateInstallation(SoftInstallation installation)
        {
            if (installation.IdLicense == null)
            {
                MessageBox.Show(@"Необходимо выбрать лицензию на программное обеспечение, по которой осуществляется установка", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicenseID.Focus();
                return false;
            }
            if (installation.IdComputer == null)
            {
                MessageBox.Show(@"Необходимо выбрать компьютер, на который производится установка программного обеспечения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxComputerID.Focus();
                return false;
            }
            if (installation.IdInstallator == null)
            {
                MessageBox.Show(@"Необходимо выбрать установщика программного обеспечения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicenseID.Focus();
                return false;
            }
            if (installation.IdLicenseKey != null && (int)SoftLicensesDataModel.GetInstance().Select().Rows.Find(installation.IdLicense)["ID LicType"] != 1 &&
                !DataModelHelper.KeyIsFree(installation.IdLicenseKey.Value))
            {
                var result = MessageBox.Show(@"Данный лицензионный ключ уже используется. Вы уверены, что хотите продолжить сохранение?", @"Внимание",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result != DialogResult.Yes)
                    return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return _vSoftInstallations.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftInstallations.MoveFirst();
            _isEditable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftInstallations.MoveLast();
            _isEditable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftInstallations.MoveNext();
            _isEditable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftInstallations.MovePrevious();
            _isEditable = true;
        }

        public override bool CanMoveFirst()
        {
            return _vSoftInstallations.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSoftInstallations.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSoftInstallations.Position > -1) && (_vSoftInstallations.Position < (_vSoftInstallations.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (_vSoftInstallations.Position > -1) && (_vSoftInstallations.Position < (_vSoftInstallations.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softwareConcat = CalcDataModelSoftwareConcat.GetInstance();
            _licenses = CalcDataModelLicensesConcat.GetInstance();
            _softLicKeys = SoftLicKeysDataModel.GetInstance();
            _devices = DevicesDataModel.GetInstance();
            _departments = DepartmentsDataModel.GetInstance();
            _softInstallators = SoftInstallatorsDataModel.GetInstance();
            _softInstallations = SoftInstallationsDataModel.GetInstance();
            _softVersions = SoftVersionsDataModel.GetInstance();
            _softwareDataModel = SoftwareDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            _softwareDataModel.Select();
            _softVersions.Select();
            _licenses.Select();
            _softLicKeys.Select();
            _devices.Select();
            _departments.Select();
            _softInstallators.Select();
            _softInstallations.Select();
            _softwareConcat.Select();

            var ds = DataSetManager.DataSet;

            _vDepartments = new BindingSource {DataSource = _departments.SelectVisibleDepartments()};

            _vDevices = new BindingSource
            {
                DataMember = "Devices",
                DataSource = ds,
                Filter = GetAllowedDepartmentFilter()
            };

            _vSoftware = new BindingSource
            {
                DataMember = "Software",
                DataSource = ds
            };

            _vSoftVersions = new BindingSource
            {
                DataMember = "SoftVersions",
                DataSource = ds
            };

            _vLicenses = new BindingSource
            {
                DataMember = "LicensesConcat",
                DataSource = ds,
                Filter = GetAllowedDepartmentFilter()
            };

            _vSoftLicKeys = new BindingSource
            {
                DataMember = "SoftLicKeys",
                DataSource = ds
            };

            _vSoftInstallators = new BindingSource
            {
                DataMember = "SoftInstallators",
                DataSource = ds
            };

            _vSoftInstallations = new BindingSource();
            _vSoftInstallations.CurrentItemChanged += v_softInstallations_CurrentItemChanged;
            _vSoftInstallations.DataMember = "SoftInstallations";
            _vSoftInstallations.DataSource = ds;
            RebuildFilter();

            DataBind();

            _softInstallations.Select().RowChanged += InstallationsViewport_RowChanged;
            _softInstallations.Select().RowDeleted += InstallationsViewport_RowDeleted;

            _softLicKeys.Select().RowChanged += softLicKeys_RowChanged;
            _softLicKeys.Select().RowDeleted += softLicKeys_RowDeleted;

            dataGridView.RowCount = _vSoftInstallations.Count;
            SetViewportCaption();

            _softwareConcat.RefreshEvent += SoftwareConcatRefreshEvent;
            _licenses.RefreshEvent += licenses_RefreshEvent;

            _devices.Select().RowChanged += Devices_RowChanged;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            _isEditable = true;
        }

        private void RebuildFilter()
        {
            var filter = StaticFilter;
            // Фильтрация по правам на департаменты
            if (!string.IsNullOrEmpty(filter))
                filter += " AND ";
            filter += ComputerFilter();
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            _vSoftInstallations.Filter = filter;
        }

        private string ComputerFilter()
        {
            var deviceFilter = _vDevices.Filter;
            var devicePosition = _vDevices.Position;
            _vDevices.Filter = GetAllowedDepartmentFilter();
            var filter = "[ID Computer] IN (0";
            for (var i = 0; i < _vDevices.Count; i++)
                filter += ((DataRowView)_vDevices[i])["ID Device"] + ",";
            filter = filter.TrimEnd(',');
            filter += ")";
            _vDevices.Filter = deviceFilter;
            _vDevices.Position = devicePosition;
            return filter;
        }

        private string GetAllowedDepartmentFilter()
        {
            var departmentFilter = "[ID Department] IN (0";
            for (var i = 0; i < _vDepartments.Count; i++)
                if ((bool)((DataRowView)_vDepartments[i])["AllowSelect"])
                    departmentFilter += ((DataRowView)_vDepartments[i])["ID Department"] + ",";
            departmentFilter = departmentFilter.TrimEnd(',');
            departmentFilter += ")";
            return departmentFilter;
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            if (!string.IsNullOrEmpty(DynamicFilter))
                return true;
            else
                return false;
        }

        public override void SearchRecord()
        {
            if (_sSearchForm == null)
                _sSearchForm = new SearchInstallationsForm();
            if (_sSearchForm.ShowDialog() != DialogResult.OK)
                return;
            DynamicFilter = _sSearchForm.GetFilter();
            var Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            _vSoftInstallations.Filter = Filter;
            dataGridView.RowCount = _vSoftInstallations.Count;
        }

        public override void ClearSearch()
        {
            DynamicFilter = "";
            RebuildFilter();
            dataGridView.RowCount = _vSoftInstallations.Count;
            MenuCallback.EditingStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.NavigationStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return (!_softInstallations.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.InstallationsReadWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            _isEditable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            _vSoftInstallations.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.License)
            {
                var versionRow = _softVersions.Select().Rows.Find((int) ParentRow["ID Version"]);
                if (versionRow != null)
                {
                    comboBoxSoftwareID.SelectedValue = versionRow["ID Software"];
                }
                else
                {
                    comboBoxSoftwareID.SelectedValue = DBNull.Value;
                }
                comboBoxSoftVersionID.SelectedValue = ParentRow["ID Version"];
                comboBoxLicenseID.SelectedValue = ParentRow["ID License"];
            }
            _vSoftInstallators.Filter = "[ID User] = " + AccessControl.UserID + " AND " + "Inactive = 0";
            ChangeCbEditing(comboBoxComputerID, true);
            ChangeCbEditing(comboBoxInstallatorID, true);
            dataGridView.Enabled = false;
            _isEditable = true;
            _softInstallations.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (_vSoftInstallations.Position != -1) && (!_softInstallations.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.InstallationsReadWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            _isEditable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            var installation = InstallationFromView();
            _vSoftInstallations.AddNew();
            _vSoftInstallators.Filter = "[ID User] = " + AccessControl.UserID + " AND " + "Inactive = 0";
            ChangeCbEditing(comboBoxComputerID, true);
            ChangeCbEditing(comboBoxInstallatorID, true);
            dataGridView.Enabled = false;
            _softInstallations.EditingNewRecord = true;
            ViewportFromInstallation(installation);
            _isEditable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (SoftInstallationsDataModel.Delete((int)((DataRowView)_vSoftInstallations.Current)["ID Installation"]) == -1)
                    return;
                _isEditable = false;
                ((DataRowView)_vSoftInstallations[_vSoftInstallations.Position]).Delete();
                _isEditable = true;
                _viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
        }

        public override bool CanDeleteRecord()
        {
            return (_vSoftInstallations.Position > -1)
                && (_viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.InstallationsReadWrite);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new InstallationsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (_vSoftInstallations.Count > 0)
                viewport.LocateInstallation((((DataRowView)_vSoftInstallations[_vSoftInstallations.Position])["ID Installation"] as int?) ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (_viewportState == ViewportState.NewRowState) || (_viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((_viewportState == ViewportState.NewRowState) || (_viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.InstallationsReadWrite);
        }

        public override void SaveRecord()
        {
            var installation = InstallationFromViewport();
            if (!ValidateInstallation(installation))
                return;
            switch (_viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idInstallation = SoftInstallationsDataModel.Insert(installation);
                    if (idInstallation == -1)
                        return;
                    DataRowView newRow;
                    installation.IdInstallation = idInstallation;
                    _isEditable = false;
                    if (_vSoftInstallations.Position == -1)
                        newRow = (DataRowView)_vSoftInstallations.AddNew();
                    else
                        newRow = ((DataRowView)_vSoftInstallations[_vSoftInstallations.Position]);
                    FillRowFromInstallation(installation, newRow);
                    _softInstallations.EditingNewRecord = false;
                    _isEditable = true;
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
                    var row = ((DataRowView)_vSoftInstallations[_vSoftInstallations.Position]);
                    _isEditable = false;
                    FillRowFromInstallation(installation, row);
                    break;
            }
            dataGridView.Enabled = true;
            _isEditable = true;
            dataGridView.RowCount = _vSoftInstallations.Count;
            _viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override void CancelRecord()
        {
            switch (_viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    _softInstallations.EditingNewRecord = false;
                    if (_vSoftInstallations.Position != -1)
                    {
                        _isEditable = false;
                        dataGridView.Enabled = true;
                        var row = (DataRowView)_vSoftInstallations[_vSoftInstallations.Position];
                        row.Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (_vSoftInstallations.Position != -1)
                        {
                            dataGridView.Rows[_vSoftInstallations.Position].Selected = true;
                            var idInstallator = 0;
                            var currentRow = (DataRowView) _vSoftInstallations[_vSoftInstallations.Position];
                            if (currentRow["ID Installator"] != DBNull.Value)
                                idInstallator = (int) currentRow["ID Installator"];
                            _vSoftInstallators.Filter =
                                string.Format("Inactive = 0 OR [ID Installator] = {0}", idInstallator);
                        }
                    }
                    ChangeCbEditing(comboBoxComputerID, false);
                    ChangeCbEditing(comboBoxInstallatorID, false);
                    _viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    _isEditable = false;
                    DataBind();
                    SelectCurrentAutoCompleteValue();
                    _viewportState = ViewportState.ReadState;
                    break;
            }
            _isEditable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            _softInstallations.Select().RowChanged -= InstallationsViewport_RowChanged;
            _softInstallations.Select().RowDeleted -= InstallationsViewport_RowDeleted;
            _softwareConcat.RefreshEvent -= SoftwareConcatRefreshEvent;
            _licenses.RefreshEvent -= licenses_RefreshEvent;
            _devices.Select().RowChanged -= Devices_RowChanged;
            _softLicKeys.Select().RowChanged -= softLicKeys_RowChanged;
            _softLicKeys.Select().RowDeleted -= softLicKeys_RowDeleted;
        }

        public override void ForceClose()
        {
            if (_viewportState == ViewportState.NewRowState)
                _softInstallations.EditingNewRecord = false;
            _softInstallations.Select().RowChanged -= InstallationsViewport_RowChanged;
            _softInstallations.Select().RowDeleted -= InstallationsViewport_RowDeleted;
            _softwareConcat.RefreshEvent -= SoftwareConcatRefreshEvent;
            _licenses.RefreshEvent -= licenses_RefreshEvent;
            _devices.Select().RowChanged -= Devices_RowChanged;
            _softLicKeys.Select().RowChanged -= softLicKeys_RowChanged;
            _softLicKeys.Select().RowDeleted -= softLicKeys_RowDeleted;
            Close();
        }

        private void SoftwareConcatRefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void licenses_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        private void InstallationsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridView.RowCount = _vSoftInstallations.Count;
                dataGridView.Refresh();
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        private void InstallationsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = _vSoftInstallations.Count;
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

        private void Devices_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
                RebuildFilter();
        }

        private void v_softInstallations_CurrentItemChanged(object sender, EventArgs e)
        {
            var currentIsEditable = _isEditable;
            _isEditable = false;
            SetViewportCaption(); 
            SelectCurrentAutoCompleteValue();
            if (_vSoftInstallations.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (_vSoftInstallations.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[1];
                }
                else
                    if (dataGridView.Rows[_vSoftInstallations.Position].Selected != true)
                    {
                        dataGridView.Rows[_vSoftInstallations.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[_vSoftInstallations.Position].Cells[1];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            if (_vSoftInstallations.Position == -1)
                return;
            if (_viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            _viewportState = ViewportState.ReadState;
            _isEditable = currentIsEditable;
        }

        private void comboBoxSoftwareID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxSoftwareID.Text;
                var selectionStart = comboBoxSoftwareID.SelectionStart;
                var selectionLength = comboBoxSoftwareID.SelectionLength;
                _vSoftware.Filter = "Software like '%" + comboBoxSoftwareID.Text + "%'";
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
                    comboBoxSoftwareID.SelectedItem = _vSoftware[_vSoftware.Position];
            }
            if (comboBoxSoftwareID.SelectedItem == null)
            {
                comboBoxSoftwareID.Text = "";
                _vSoftware.Filter = "";
            }
        }

        private void comboBoxLicenseID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxLicenseID.Text;
                var selectionStart = comboBoxLicenseID.SelectionStart;
                var selectionLength = comboBoxLicenseID.SelectionLength;
                _vLicenses.Filter = GetAllowedDepartmentFilter() + " AND [ID Software] = " + (comboBoxSoftwareID.SelectedValue != null ? comboBoxSoftwareID.SelectedValue : "0") + " AND License like '%" + comboBoxLicenseID.Text + "%'";
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
                    comboBoxLicenseID.SelectedItem = _vLicenses[_vLicenses.Position];
            }
            if (comboBoxLicenseID.SelectedItem == null)
            {
                comboBoxLicenseID.Text = "";
                _vLicenses.Filter = GetAllowedDepartmentFilter() + " AND [ID Version] = " + (comboBoxSoftVersionID.SelectedValue ?? "0");
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
                var text = comboBoxLicKeysID.Text;
                var selectionStart = comboBoxLicKeysID.SelectionStart;
                var selectionLength = comboBoxLicKeysID.SelectionLength;
                _vSoftLicKeys.Filter = "[ID License] = " + (comboBoxLicenseID.SelectedValue != null ? comboBoxLicenseID.SelectedValue : "0") 
                    + " AND LicKey like '%" + comboBoxLicKeysID.Text + "%'"
                    + " AND [ID LicenseKey] IN (0" + GetLicKeysFilter((comboBoxLicenseID.SelectedValue != null ? (int)comboBoxLicenseID.SelectedValue : 0)) + ")";
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
                _vSoftLicKeys.Filter = "[ID License] = " + (comboBoxLicenseID.SelectedValue != null ? comboBoxLicenseID.SelectedValue : "0")
                     + " AND [ID LicenseKey] IN (0" + GetLicKeysFilter((comboBoxLicenseID.SelectedValue != null ? (int)comboBoxLicenseID.SelectedValue : 0)) + ")";
            }
            if (string.IsNullOrEmpty(comboBoxLicKeysID.Text))
                comboBoxLicKeysID.SelectedItem = null;
        }

        private void comboBoxLicKeysID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxLicKeysID.Items.Count == 0)
                comboBoxLicKeysID.SelectedIndex = -1;
        }

        private void comboBoxComputerID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode < Keys.A || e.KeyCode > Keys.Z) && (e.KeyCode != Keys.Back) &&
                (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
                return;
            var text = comboBoxComputerID.Text;
            var selectionStart = comboBoxComputerID.SelectionStart;
            var selectionLength = comboBoxComputerID.SelectionLength;
            if (!string.IsNullOrEmpty(comboBoxComputerID.Text))
            {
                var deviceFilter = "[Device Name] like '%" + comboBoxComputerID.Text + "%'";
                var filter = GetDeviceFilter();
                filter = !string.IsNullOrEmpty(filter) ? 
                    string.Format("({0}) AND {1}", filter, deviceFilter) : 
                    deviceFilter;
                _vDevices.Filter = filter;
            }
            comboBoxComputerID.Text = text;
            comboBoxComputerID.SelectionStart = selectionStart;
            comboBoxComputerID.SelectionLength = selectionLength;
        }

        private void comboBoxComputerID_Leave(object sender, EventArgs e)
        {
            if (comboBoxComputerID.Items.Count > 0)
            {
                if (comboBoxComputerID.SelectedItem == null)
                    comboBoxComputerID.SelectedItem = _vDevices[_vDevices.Position];
            }
            if (comboBoxComputerID.SelectedItem == null)
            {
                comboBoxComputerID.Text = "";
                _vDevices.Filter = GetDeviceFilter();
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
            if ((comboBoxSoftwareID.DataSource != null) && (comboBoxSoftVersionID.DataSource != null) &&
                (comboBoxLicenseID.DataSource != null) && (comboBoxLicKeysID.DataSource != null) &&
                !string.IsNullOrEmpty(comboBoxSoftwareID.ValueMember))
            {
                var idSoftware = (int?)comboBoxSoftwareID.SelectedValue;
                if (idSoftware != null)
                    _vSoftVersions.Filter = "[ID Software] = " + idSoftware;
                else
                    _vSoftVersions.Filter = "1 = 0";
            }
            CheckViewportModifications();
        }

        private void comboBoxSoftVersionID_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((comboBoxSoftwareID.DataSource != null) && (comboBoxSoftVersionID.DataSource != null) && 
                (comboBoxLicenseID.DataSource != null) && (comboBoxLicKeysID.DataSource != null) &&
                !string.IsNullOrEmpty(comboBoxSoftVersionID.ValueMember))
            {
                var idVersion = (int?)comboBoxSoftVersionID.SelectedValue;
                if (idVersion != null)
                    _vLicenses.Filter = GetAllowedDepartmentFilter() + " AND [ID Version] = " + idVersion;
                else
                    _vLicenses.Filter = "1 = 0";
            }
            CheckViewportModifications();
        }

        private void comboBoxLicenseID_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((comboBoxSoftwareID.DataSource != null) && (comboBoxSoftVersionID.DataSource != null) && 
                (comboBoxLicenseID.DataSource != null) && (comboBoxLicKeysID.DataSource != null) &&
                !string.IsNullOrEmpty(comboBoxLicenseID.ValueMember))
            {
                var idLicense = (int?)comboBoxLicenseID.SelectedValue;
                if (idLicense != null)
                {
                    _vSoftLicKeys.Filter = "[ID License] = " + idLicense + " AND [ID LicenseKey] IN (0" +
                                           GetLicKeysFilter(idLicense.Value) + ")";
                    _vDevices.Filter = GetDeviceFilter();
                }
                else
                    _vSoftLicKeys.Filter = "1 = 0";
            }
            CheckViewportModifications();
        }

        private string GetDeviceFilter()
        {
            var filter = GetAllowedDepartmentFilter();
            var licenseId = comboBoxLicenseID.SelectedValue;
            if (licenseId != null)
            {
                var vLicensesRowIndex = _vLicenses.Find("ID License", licenseId);
                if (vLicensesRowIndex != -1)
                {
                    var vLicensesRow = (DataRowView)_vLicenses[vLicensesRowIndex];
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter += " AND ";
                    }
                    var idDepartment = (int) vLicensesRow["ID Department"];
                    filter += string.Format("[ID Department] IN (0{0})",
                        DataModelHelper.GetDepartmentSubunits(idDepartment).Concat(new[] {idDepartment}).
                            Select(v => v.ToString()).Aggregate((acc, v) => acc + "," + v));
                }
            }

            var vSoftInstallationsRow = _vSoftInstallations.Position > -1
                ? (DataRowView) _vSoftInstallations[_vSoftInstallations.Position] : null;
            if (vSoftInstallationsRow != null && vSoftInstallationsRow["ID Computer"] != DBNull.Value)
            {
                var idComputer = (int) vSoftInstallationsRow["ID Computer"];
                filter = !string.IsNullOrEmpty(filter) ? 
                    string.Format("({0}) OR ([ID Device] = {1})", filter, idComputer) : 
                    string.Format("([ID Device] = {0})", idComputer);
            }
            return filter;
        }

        private string GetLicKeysFilter(int idLicense)
        {
            var licKeyIds = DataModelHelper.LicKeyIdsNotUsed(idLicense);
            var licKeys = "";
            foreach (var licKey in licKeyIds)
                licKeys += licKey.ToString(CultureInfo.InvariantCulture) + ",";
            if (_vSoftInstallations.Position != -1)
            {
                var row = (DataRowView)_vSoftInstallations[_vSoftInstallations.Position];
                if (row["ID LicenseKey"] != DBNull.Value)
                    licKeys += row["ID LicenseKey"].ToString();
            }
            return licKeys;
        }

        private void comboBoxLicKeysID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_vSoftInstallations.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "idInstallation":
                    e.Value = ((DataRowView)_vSoftInstallations[e.RowIndex])["ID Installation"];
                    break;
                case "installationDate":
                    e.Value = ((DataRowView)_vSoftInstallations[e.RowIndex])["InstallationDate"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)_vSoftInstallations[e.RowIndex])["InstallationDate"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "software":
                    var row = _licenses.Select().Rows.Find(((DataRowView)_vSoftInstallations[e.RowIndex])["ID License"]);
                    if (row != null)
                    {
                        row = _softwareConcat.Select().Rows.Find(row["ID Version"]);
                        if (row != null)
                            e.Value = row["Software"];
                    }
                    break;
                case "department":
                    row = _devices.Select().Rows.Find(((DataRowView)_vSoftInstallations[e.RowIndex])["ID Computer"]);
                    if (row != null)
                    {
                        var rowIndex = _vDepartments.Find("ID Department", row["ID Department"]);
                        if (rowIndex != -1)
                            e.Value = ((DataRowView)_vDepartments[rowIndex])["Department"].ToString().Trim();
                    }
                    break;
                case "computer":
                    row = _devices.Select().Rows.Find(((DataRowView)_vSoftInstallations[e.RowIndex])["ID Computer"]);
                    if (row != null)
                        e.Value = row["Device Name"];
                    break;
                case "invNum":
                    row = _devices.Select().Rows.Find(((DataRowView)_vSoftInstallations[e.RowIndex])["ID Computer"]);
                    if (row != null)
                        e.Value = row["InventoryNumber"];
                    break;
                case "serialNum":
                    row = _devices.Select().Rows.Find(((DataRowView)_vSoftInstallations[e.RowIndex])["ID Computer"]);
                    if (row != null)
                        e.Value = row["SerialNumber"];
                    break;
                case "license":
                    row = _licenses.Select().Rows.Find(((DataRowView)_vSoftInstallations[e.RowIndex])["ID License"]);
                    if (row != null)
                        e.Value = row["License"];
                    break;
                case "licKey":
                    row = _softLicKeys.Select().Rows.Find(((DataRowView)_vSoftInstallations[e.RowIndex])["ID LicenseKey"]);
                    if (row != null)
                        e.Value = row["LicKey"];
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                _vSoftInstallations.Position = dataGridView.SelectedRows[0].Index;
            else
                _vSoftInstallations.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                _vSoftInstallations.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
            if (_isFirstVisibility)
            {
                SelectCurrentAutoCompleteValue();
                _isFirstVisibility = false;
            }
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationsViewport));
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idInstallation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.software = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.licKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.department = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.computer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serialNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.invNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.installationDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.license = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxSoftVersionID = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxLicenseID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLicKeysID = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxSoftwareID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.comboBoxComputerID = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxInstallatorID = new System.Windows.Forms.ComboBox();
            this.dateTimePickerInstallDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
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
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(1019, 479);
            this.tableLayoutPanel14.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
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
            this.licKey,
            this.department,
            this.computer,
            this.serialNum,
            this.invNum,
            this.installationDate,
            this.license});
            this.tableLayoutPanel14.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 148);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1013, 328);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
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
            this.software.HeaderText = "Наименование ПО";
            this.software.MinimumWidth = 300;
            this.software.Name = "software";
            this.software.ReadOnly = true;
            this.software.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.software.Width = 300;
            // 
            // licKey
            // 
            this.licKey.HeaderText = "Лицензионный ключ";
            this.licKey.MinimumWidth = 150;
            this.licKey.Name = "licKey";
            this.licKey.ReadOnly = true;
            this.licKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.licKey.Width = 150;
            // 
            // department
            // 
            this.department.HeaderText = "Кабинет";
            this.department.MinimumWidth = 80;
            this.department.Name = "department";
            this.department.ReadOnly = true;
            this.department.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.department.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.department.Width = 80;
            // 
            // computer
            // 
            this.computer.HeaderText = "Имя ПК";
            this.computer.MinimumWidth = 80;
            this.computer.Name = "computer";
            this.computer.ReadOnly = true;
            this.computer.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.computer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.computer.Width = 80;
            // 
            // serialNum
            // 
            this.serialNum.HeaderText = "Серийный №";
            this.serialNum.MinimumWidth = 150;
            this.serialNum.Name = "serialNum";
            this.serialNum.ReadOnly = true;
            this.serialNum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.serialNum.Width = 150;
            // 
            // invNum
            // 
            this.invNum.HeaderText = "Инвентарный №";
            this.invNum.MinimumWidth = 130;
            this.invNum.Name = "invNum";
            this.invNum.ReadOnly = true;
            this.invNum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.invNum.Width = 130;
            // 
            // installationDate
            // 
            this.installationDate.HeaderText = "Дата установки";
            this.installationDate.MinimumWidth = 80;
            this.installationDate.Name = "installationDate";
            this.installationDate.ReadOnly = true;
            this.installationDate.Width = 80;
            // 
            // license
            // 
            this.license.HeaderText = "Лицензия";
            this.license.MinimumWidth = 250;
            this.license.Name = "license";
            this.license.ReadOnly = true;
            this.license.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.license.Width = 250;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxSoftVersionID);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.comboBoxLicenseID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxLicKeysID);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.comboBoxSoftwareID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(503, 139);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сведения о лицензии";
            // 
            // comboBoxSoftVersionID
            // 
            this.comboBoxSoftVersionID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSoftVersionID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSoftVersionID.FormattingEnabled = true;
            this.comboBoxSoftVersionID.Location = new System.Drawing.Point(161, 50);
            this.comboBoxSoftVersionID.Name = "comboBoxSoftVersionID";
            this.comboBoxSoftVersionID.Size = new System.Drawing.Size(330, 23);
            this.comboBoxSoftVersionID.TabIndex = 1;
            this.comboBoxSoftVersionID.SelectedValueChanged += new System.EventHandler(this.comboBoxSoftVersionID_SelectedValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 15);
            this.label6.TabIndex = 88;
            this.label6.Text = "Версия ПО";
            // 
            // comboBoxLicenseID
            // 
            this.comboBoxLicenseID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLicenseID.FormattingEnabled = true;
            this.comboBoxLicenseID.Location = new System.Drawing.Point(161, 78);
            this.comboBoxLicenseID.Name = "comboBoxLicenseID";
            this.comboBoxLicenseID.Size = new System.Drawing.Size(330, 23);
            this.comboBoxLicenseID.TabIndex = 2;
            this.comboBoxLicenseID.DropDownClosed += new System.EventHandler(this.comboBoxLicenseID_DropDownClosed);
            this.comboBoxLicenseID.SelectedValueChanged += new System.EventHandler(this.comboBoxLicenseID_SelectedValueChanged);
            this.comboBoxLicenseID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxLicenseID_KeyUp);
            this.comboBoxLicenseID.Leave += new System.EventHandler(this.comboBoxLicenseID_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 81);
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
            this.comboBoxLicKeysID.Location = new System.Drawing.Point(161, 107);
            this.comboBoxLicKeysID.Name = "comboBoxLicKeysID";
            this.comboBoxLicKeysID.Size = new System.Drawing.Size(330, 23);
            this.comboBoxLicKeysID.TabIndex = 3;
            this.comboBoxLicKeysID.DropDownClosed += new System.EventHandler(this.comboBoxLicKeysID_DropDownClosed);
            this.comboBoxLicKeysID.SelectedValueChanged += new System.EventHandler(this.comboBoxLicKeysID_SelectedValueChanged);
            this.comboBoxLicKeysID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxLicKeysID_KeyUp);
            this.comboBoxLicKeysID.Leave += new System.EventHandler(this.comboBoxLicKeysID_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 110);
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
            this.comboBoxSoftwareID.Size = new System.Drawing.Size(330, 23);
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
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxDescription);
            this.groupBox2.Controls.Add(this.comboBoxComputerID);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.comboBoxInstallatorID);
            this.groupBox2.Controls.Add(this.dateTimePickerInstallDate);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(512, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(504, 139);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Сведения об установке";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 87;
            this.label1.Text = "Примечание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 109);
            this.textBoxDescription.MaxLength = 2048;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(330, 21);
            this.textBoxDescription.TabIndex = 3;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // comboBoxComputerID
            // 
            this.comboBoxComputerID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxComputerID.Enabled = false;
            this.comboBoxComputerID.FormattingEnabled = true;
            this.comboBoxComputerID.Location = new System.Drawing.Point(161, 22);
            this.comboBoxComputerID.Name = "comboBoxComputerID";
            this.comboBoxComputerID.Size = new System.Drawing.Size(330, 23);
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
            this.comboBoxInstallatorID.Enabled = false;
            this.comboBoxInstallatorID.FormattingEnabled = true;
            this.comboBoxInstallatorID.Location = new System.Drawing.Point(161, 80);
            this.comboBoxInstallatorID.Name = "comboBoxInstallatorID";
            this.comboBoxInstallatorID.Size = new System.Drawing.Size(330, 23);
            this.comboBoxInstallatorID.TabIndex = 2;
            this.comboBoxInstallatorID.SelectedIndexChanged += new System.EventHandler(this.comboBoxInstallatorID_SelectedIndexChanged);
            // 
            // dateTimePickerInstallDate
            // 
            this.dateTimePickerInstallDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerInstallDate.Location = new System.Drawing.Point(161, 53);
            this.dateTimePickerInstallDate.Name = "dateTimePickerInstallDate";
            this.dateTimePickerInstallDate.Size = new System.Drawing.Size(330, 21);
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
            // InstallationsViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(650, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1025, 485);
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

        private void ChangeCbEditing(ComboBox control,bool state)
        {
            control.Enabled = state;
        }
        public override List<string> GetIdInstallations()
        {
            var idList = new List<string>();
            for (int i = 0; i < this.dataGridView.RowCount; i++)
            {
                idList.Add(this.dataGridView["idInstallation", i].Value.ToString());
            }
            return idList;
        }
    }
}
