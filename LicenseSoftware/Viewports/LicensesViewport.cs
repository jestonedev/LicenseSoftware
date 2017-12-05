using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LicenseSoftware.DataModels.CalcDataModels;
using LicenseSoftware.DataModels.DataModels;
using LicenseSoftware.Viewport.SearchForms;

namespace LicenseSoftware.Viewport
{
    public sealed class LicensesViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView dataGridView;
        private GroupBox groupBox32;
        private bool is_editable;
        private GroupBox groupBox3;
        private Label label10;
        private DateTimePicker dateTimePickerExpireLicenseDate;
        private Label label9;
        private DateTimePicker dateTimePickerBuyLicenseDate;
        private ComboBox comboBoxDocTypeID;
        private Label label8;
        private TextBox textBoxDocNumber;
        private Label label1;
        private GroupBox groupBox1;
        private ComboBox comboBoxDepartmentID;
        private Label label4;
        private ComboBox comboBoxSupplierID;
        private Label label3;
        private ComboBox comboBoxSoftwareID;
        private Label label2;
        private GroupBox groupBox2;
        private TextBox textBoxDescription;
        private Label label7;
        private NumericUpDown numericUpDownInstallationsCount;
        private Label label6;
        private ComboBox comboBoxLicTypeID;
        private Label label5;
        private CheckBox checkBoxInstallCountEnable;
        private DataGridViewTextBoxColumn idLicense;
        private DataGridViewTextBoxColumn docNumber;
        private DataGridViewTextBoxColumn software;
        private DataGridViewTextBoxColumn department;
        private DataGridViewTextBoxColumn buyLicenseDate;
        private DataGridViewTextBoxColumn expireLicenseDate;
        private DataGridViewTextBoxColumn currentMaxInst;
        private ComboBox comboBoxSoftVersionID;
        private Label label11;
        #endregion Components

        #region Models

        private SoftLicensesDataModel _licenses;
        private SoftVersionsDataModel _softVersions;
        private SoftwareDataModel _softwareDataModel;
        private SoftLicTypesDataModel _softLicTypes;
        private SoftLicDocTypesDataModel _softLicDocTypes;
        private DepartmentsDataModel _departments;
        private SoftSuppliersDataModel _softSuppliers;
        private CalcDataModelSoftwareConcat _softwareConcat;
        #endregion Models

        #region Views

        private BindingSource _vLicenses;
        private BindingSource _vSoftLicTypes;
        private BindingSource _vSoftLicDocTypes;
        private BindingSource _vDepartments;
        private BindingSource _vSoftSuppliers;
        private BindingSource _vSoftware;
        private BindingSource _vSoftVersions;
        #endregion Views

        //State
        private ViewportState _viewportState = ViewportState.ReadState;


        private SearchForm _sSearchForm;

        private LicensesViewport()
            : this(null)
        {
        }

        public LicensesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public LicensesViewport(LicensesViewport licensesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = licensesViewport.DynamicFilter;
            StaticFilter = licensesViewport.StaticFilter;
            ParentRow = licensesViewport.ParentRow;
            ParentType = licensesViewport.ParentType;
        }

        private void SetViewportCaption()
        {
            if (_viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Software))
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "Новая лицензия на ПО №{0}", ParentRow["ID Software"]);
                } else
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.SoftVersion))
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "Новая лицензия на ПО №{0} версии {1}", ParentRow["ID Software"], ParentRow["Version"]);
                }
                else
                    Text = @"Новая лицензия на ПО";
            }
            else
                if (_vLicenses.Position != -1)
                {
                    var currentIdLicense = ((DataRowView) _vLicenses[_vLicenses.Position])["ID License"];
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Software))
                    {
                        Text = string.Format(CultureInfo.InvariantCulture, "Лицензия №{0} на ПО №{1}",
                            currentIdLicense, ParentRow["ID Software"]);
                    }
                    else
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.SoftVersion))
                    {
                        Text = string.Format(CultureInfo.InvariantCulture, "Лицензия №{0} на ПО №{1} версии {2}",
                             currentIdLicense, ParentRow["ID Software"], ParentRow["Version"]);
                    }
                    else
                        Text = string.Format(CultureInfo.InvariantCulture, "Лицензия №{0}", currentIdLicense);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Software))
                    {
                        Text = string.Format(CultureInfo.InvariantCulture, "Лицензии на ПО №{0} отсутствуют", ParentRow["ID Software"]);
                    }
                    else if ((ParentRow != null) && (ParentType == ParentTypeEnum.SoftVersion))
                    {
                        Text = string.Format(CultureInfo.InvariantCulture, "Лицензии на ПО №{0} версии {1} отсутствуют",
                            ParentRow["ID Software"], ParentRow["Version"]);
                    }
                    else
                    {
                        Text = @"Лицензии отсутствуют";
                    }
                }
        }

        private void DataBind()
        {
            comboBoxDepartmentID.DataSource = _vDepartments;
            comboBoxDepartmentID.ValueMember = "ID Department";
            comboBoxDepartmentID.DisplayMember = "Department";
            comboBoxDepartmentID.DataBindings.Clear();
            comboBoxDepartmentID.DataBindings.Add("SelectedValue", _vLicenses, "ID Department", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxDocTypeID.DataSource = _vSoftLicDocTypes;
            comboBoxDocTypeID.ValueMember = "ID DocType";
            comboBoxDocTypeID.DisplayMember = "DocType";
            comboBoxDocTypeID.DataBindings.Clear();
            comboBoxDocTypeID.DataBindings.Add("SelectedValue", _vLicenses, "ID DocType", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxLicTypeID.DataSource = _vSoftLicTypes;
            comboBoxLicTypeID.ValueMember = "ID LicType";
            comboBoxLicTypeID.DisplayMember = "LicType";
            comboBoxLicTypeID.DataBindings.Clear();
            comboBoxLicTypeID.DataBindings.Add("SelectedValue", _vLicenses, "ID LicType", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxSoftwareID.DataSource = _vSoftware;
            comboBoxSoftwareID.ValueMember = "ID Software";
            comboBoxSoftwareID.DisplayMember = "Software";

            comboBoxSoftVersionID.DataSource = _vSoftVersions;
            comboBoxSoftVersionID.ValueMember = "ID Version";
            comboBoxSoftVersionID.DisplayMember = "Version";
            comboBoxSoftVersionID.DataBindings.Clear();
            comboBoxSoftVersionID.DataBindings.Add("SelectedValue", _vLicenses, "ID Version", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxSupplierID.DataSource = _vSoftSuppliers;
            comboBoxSupplierID.ValueMember = "ID Supplier";
            comboBoxSupplierID.DisplayMember = "Supplier";
            comboBoxSupplierID.DataBindings.Clear();
            comboBoxSupplierID.DataBindings.Add("SelectedValue", _vLicenses, "ID Supplier", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxDocNumber.DataBindings.Clear();
            textBoxDocNumber.DataBindings.Add("Text", _vLicenses, "DocNumber", true, DataSourceUpdateMode.Never, "");
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", _vLicenses, "Description", true, DataSourceUpdateMode.Never, "");

            numericUpDownInstallationsCount.DataBindings.Clear();
            numericUpDownInstallationsCount.DataBindings.Add("Value", _vLicenses, "InstallationsCount", true, DataSourceUpdateMode.Never, 1);

            dateTimePickerBuyLicenseDate.DataBindings.Clear();
            dateTimePickerBuyLicenseDate.DataBindings.Add("Value", _vLicenses, "BuyLicenseDate", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
            dateTimePickerExpireLicenseDate.DataBindings.Clear();
            dateTimePickerExpireLicenseDate.DataBindings.Add("Value", _vLicenses, "ExpireLicenseDate", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            var row = _vLicenses.Position >= 0 ? (DataRowView)_vLicenses[_vLicenses.Position] : null;
            if (row == null)
            {
                return;
            }
            if ((_vLicenses.Position >= 0) && (row["ExpireLicenseDate"] != DBNull.Value))
                dateTimePickerExpireLicenseDate.Checked = true;
            else
            {
                dateTimePickerExpireLicenseDate.Value = DateTime.Now.Date;
                dateTimePickerExpireLicenseDate.Checked = false;
            }
            if ((_vLicenses.Position >= 0) && (row["InstallationsCount"] != DBNull.Value))
                checkBoxInstallCountEnable.Checked = true;
            else
            {
                numericUpDownInstallationsCount.Value = 0;
                checkBoxInstallCountEnable.Checked = false;
            }
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!ContainsFocus) || (dataGridView.Focused))
                return;
            if ((_vLicenses.Position != -1) && (LicenseFromView() != LicenseFromViewport()))
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
            if (!AccessControl.HasPrivelege(Priveleges.LicensesReadWrite))
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
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.Yes)
                                SaveRecord();
                            else
                                if (result == DialogResult.No)
                                    CancelRecord();
                                else return false;
                            return _viewportState == ViewportState.ReadState;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (_viewportState)
                    {
                        case ViewportState.ReadState:
                            if (_licenses.EditingNewRecord)
                                return false;
                            _viewportState = ViewportState.NewRowState;
                            return true;
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
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
                            return false;
                    }
                    break;
                case ViewportState.ModifyRowState: 
                    switch (_viewportState)
                    {
                        case ViewportState.ReadState:
                            _viewportState = ViewportState.ModifyRowState;
                            return true;
                        case ViewportState.ModifyRowState:
                            return true;
                        case ViewportState.NewRowState:
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
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

        private void LocateLicense(int id)
        {
            var position = _vLicenses.Find("ID License", id);
            is_editable = false;
            if (position > 0)
                _vLicenses.Position = position;
            is_editable = true;
        }

        private void ViewportFromLicense(SoftLicense license)
        {
            if (license.IdVersion != null)
            {
                var softVersions = _softVersions.Select();
                var softVersionRow = softVersions.Rows.Find(license.IdVersion);
                if (softVersionRow != null)
                {
                    _vSoftware.Filter = "";
                    comboBoxSoftwareID.SelectedValue = softVersionRow["ID Software"];
                    comboBoxSoftVersionID.SelectedValue = ViewportHelper.ValueOrDbNull(license.IdVersion);
                }
            }

            comboBoxDocTypeID.SelectedValue = ViewportHelper.ValueOrDbNull(license.IdDocType);
            comboBoxLicTypeID.SelectedValue = ViewportHelper.ValueOrDbNull(license.IdLicType);
            comboBoxSupplierID.SelectedValue = ViewportHelper.ValueOrDbNull(license.IdSupplier);
            comboBoxDepartmentID.SelectedValue = ViewportHelper.ValueOrDbNull(license.IdDepartment);
            textBoxDocNumber.Text = license.DocNumber;
            textBoxDescription.Text = license.Description;
            dateTimePickerBuyLicenseDate.Value = ViewportHelper.ValueOrDefault(license.BuyLicenseDate);
            dateTimePickerExpireLicenseDate.Value = ViewportHelper.ValueOrDefault(license.ExpireLicenseDate);
            numericUpDownInstallationsCount.Value = ViewportHelper.ValueOrDefault(license.InstallationsCount);
        }

        private SoftLicense LicenseFromViewport()
        {
            var softLicense = new SoftLicense
            {
                IdLicense =
                    _vLicenses.Position == -1
                        ? null
                        : ViewportHelper.ValueOrNull<int>((DataRowView) _vLicenses[_vLicenses.Position], "ID License"),
                IdVersion = ViewportHelper.ValueOrNull<int>(comboBoxSoftVersionID),
                IdLicType = ViewportHelper.ValueOrNull<int>(comboBoxLicTypeID),
                IdDocType = ViewportHelper.ValueOrNull<int>(comboBoxDocTypeID),
                IdSupplier = ViewportHelper.ValueOrNull<int>(comboBoxSupplierID),
                IdDepartment = ViewportHelper.ValueOrNull<int>(comboBoxDepartmentID),
                DocNumber = ViewportHelper.ValueOrNull(textBoxDocNumber),
                Description = ViewportHelper.ValueOrNull(textBoxDescription),
                BuyLicenseDate = ViewportHelper.ValueOrNull(dateTimePickerBuyLicenseDate),
                ExpireLicenseDate = ViewportHelper.ValueOrNull(dateTimePickerExpireLicenseDate)
            };
            if (checkBoxInstallCountEnable.Checked)
                softLicense.InstallationsCount = (int)numericUpDownInstallationsCount.Value;
            else
                softLicense.InstallationsCount = null;
            return softLicense;
        }

        private SoftLicense LicenseFromView()
        {
            var row = (DataRowView)_vLicenses[_vLicenses.Position];
            var softLicense = new SoftLicense
            {
                IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License"),
                IdVersion = ViewportHelper.ValueOrNull<int>(row, "ID Version"),
                IdLicType = ViewportHelper.ValueOrNull<int>(row, "ID LicType"),
                IdDocType = ViewportHelper.ValueOrNull<int>(row, "ID DocType"),
                IdSupplier = ViewportHelper.ValueOrNull<int>(row, "ID Supplier"),
                IdDepartment = ViewportHelper.ValueOrNull<int>(row, "ID Department"),
                DocNumber = ViewportHelper.ValueOrNull(row, "DocNumber"),
                Description = ViewportHelper.ValueOrNull(row, "Description"),
                BuyLicenseDate = ViewportHelper.ValueOrNull<DateTime>(row, "BuyLicenseDate"),
                ExpireLicenseDate = ViewportHelper.ValueOrNull<DateTime>(row, "ExpireLicenseDate"),
                InstallationsCount = ViewportHelper.ValueOrNull<int>(row, "InstallationsCount")
            };
            return softLicense;
        }

        private static void FillRowFromLicense(SoftLicense softLicense, DataRowView row)
        {
            row.BeginEdit();
            row["ID License"] = ViewportHelper.ValueOrDbNull(softLicense.IdLicense);
            row["ID Version"] = ViewportHelper.ValueOrDbNull(softLicense.IdVersion);
            row["ID LicType"] = ViewportHelper.ValueOrDbNull(softLicense.IdLicType);
            row["ID DocType"] = ViewportHelper.ValueOrDbNull(softLicense.IdDocType);
            row["ID Supplier"] = ViewportHelper.ValueOrDbNull(softLicense.IdSupplier);
            row["ID Department"] = ViewportHelper.ValueOrDbNull(softLicense.IdDepartment);
            row["DocNumber"] = ViewportHelper.ValueOrDbNull(softLicense.DocNumber);
            row["Description"] = ViewportHelper.ValueOrDbNull(softLicense.Description);
            row["BuyLicenseDate"] = ViewportHelper.ValueOrDbNull(softLicense.BuyLicenseDate);
            row["ExpireLicenseDate"] = ViewportHelper.ValueOrDbNull(softLicense.ExpireLicenseDate);
            row["InstallationsCount"] = ViewportHelper.ValueOrDbNull(softLicense.InstallationsCount);
            row.EndEdit();
        }

        private bool ValidateLicense(SoftLicense softLicense)
        {
            if (softLicense.IdVersion == null)
            {
                MessageBox.Show(@"Необходимо выбрать программное обеспечение, на которое заводится лицензия, и его версию", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareID.Focus();
                return false;
            }
            if (softLicense.IdSupplier == null)
            {
                MessageBox.Show(@"Необходимо выбрать поставщика программного обеспечения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSupplierID.Focus();
                return false;
            }
            if (softLicense.IdDepartment == null)
            {
                MessageBox.Show(@"Необходимо выбрать департамент-заказчик программного обеспечения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentID.Focus();
                return false;
            } else
                if (!(bool)((DataRowView)_vDepartments[
                    _vDepartments.Find("ID Department", softLicense.IdDepartment)])["AllowSelect"])
                {
                    MessageBox.Show(@"У вас нет прав на подачу заявок на данный департамент", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    comboBoxDepartmentID.Focus();
                    return false;
                }
            if (softLicense.IdLicType == null)
            {
                MessageBox.Show(@"Необходимо выбрать вид лицензии на программное обеспечение", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicTypeID.Focus();
                return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return _vLicenses.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            _vLicenses.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            _vLicenses.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            _vLicenses.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            _vLicenses.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return _vLicenses.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vLicenses.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vLicenses.Position > -1) && (_vLicenses.Position < (_vLicenses.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (_vLicenses.Position > -1) && (_vLicenses.Position < (_vLicenses.Count - 1));
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
            _softLicTypes = SoftLicTypesDataModel.GetInstance();
            _softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();
            _softSuppliers = SoftSuppliersDataModel.GetInstance();
            _departments = DepartmentsDataModel.GetInstance();
            _licenses = SoftLicensesDataModel.GetInstance();
            _softwareDataModel = SoftwareDataModel.GetInstance();
            _softVersions = SoftVersionsDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            _softLicTypes.Select();
            _softLicDocTypes.Select();
            _softSuppliers.Select();
            _licenses.Select();
            _softwareConcat.Select();
            _softwareDataModel.Select();
            _softVersions.Select();

            var ds = DataSetManager.DataSet;

            _vSoftLicTypes = new BindingSource
            {
                DataMember = "SoftLicTypes",
                DataSource = ds
            };

            _vSoftLicDocTypes = new BindingSource
            {
                DataMember = "SoftLicDocTypes",
                DataSource = ds
            };

            _vSoftSuppliers = new BindingSource
            {
                DataMember = "SoftSuppliers",
                DataSource = ds
            };

            _vDepartments = new BindingSource {DataSource = _departments.SelectVisibleDepartments()};

            _vSoftware = new BindingSource
            {
                DataMember = "Software",
                DataSource = ds
            };

            _vSoftVersions = new BindingSource
            {
                DataMember = "Software_SoftVersions",
                DataSource = _vSoftware
            };

            _vLicenses = new BindingSource();
            _vLicenses.CurrentItemChanged += v_licenses_CurrentItemChanged;
            _vLicenses.DataMember = "SoftLicenses";
            _vLicenses.DataSource = ds;
            RebuildFilter();

            DataBind();

            _licenses.Select().RowChanged += LicensesViewport_RowChanged;
            _licenses.Select().RowDeleted += LicensesViewport_RowDeleted;
            _departments.Select().RowChanged += Departments_RowChanged;
            _departments.Select().RowDeleted += Departments_RowDeleted;

            dataGridView.RowCount = _vLicenses.Count;
            SetViewportCaption();

            _softwareConcat.RefreshEvent += SoftwareConcatRefreshEvent;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            is_editable = true;
        }

        private void Departments_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            _vDepartments.DataSource = _departments.SelectVisibleDepartments();
            RebuildFilter();
        }

        private void Departments_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            _vDepartments.DataSource = _departments.SelectVisibleDepartments();
            RebuildFilter();
        }

        private void RebuildFilter()
        {
            var filter = "";
            if (ParentType == ParentTypeEnum.SoftVersion && ParentRow != null)
            {
                filter = StaticFilter;
            }
            else
            if (ParentType == ParentTypeEnum.Software && ParentRow != null)
            {
                filter = "[ID Version] IN (" + (from row in DataModelHelper.FilterRows(_softVersions.Select())
                    where row.Field<int>("ID Software") == (int)ParentRow["ID Software"]
                    select row.Field<int>("ID Version").ToString()).Concat(new []{ "0" })
                    .Aggregate((acc, v) => acc + "," + v)+")";
            }
            // Фильтрация по правам на департаменты
            if (!string.IsNullOrEmpty(filter))
                filter += " AND ";
            filter += "[ID Department] IN (0";
            for (var i = 0; i < _vDepartments.Count; i++)
                if ((bool)((DataRowView)_vDepartments[i])["AllowSelect"])
                    filter += ((DataRowView)_vDepartments[i])["ID Department"] + ",";
            filter = filter.TrimEnd(',');
            filter += ")";
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            _vLicenses.Filter = filter;
        }

        public override bool CanSearchRecord()
        {
            return true;
        }

        public override bool SearchedRecords()
        {
            return !string.IsNullOrEmpty(DynamicFilter);
        }

        public override void SearchRecord()
        {
            if (_sSearchForm == null)
                _sSearchForm = new SearchLicensesForm();
            if (_sSearchForm.ShowDialog() != DialogResult.OK)
                return;
            try 
            {
                DynamicFilter = _sSearchForm.GetFilter();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message, @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            dataGridView.RowCount = 0;
            RebuildFilter();
            dataGridView.RowCount = _vLicenses.Count;
        }

        public override void ClearSearch()
        {
            DynamicFilter = "";
            RebuildFilter();
            dataGridView.RowCount = _vLicenses.Count;
            MenuCallback.EditingStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.NavigationStateUpdate();
        }

        private void SoftwareConcatRefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        public override bool CanInsertRecord()
        {
            return !_licenses.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            _vLicenses.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.Software)
            {
                comboBoxSoftwareID.SelectedValue = ParentRow["ID Software"];
            }
            if (ParentRow != null && ParentType == ParentTypeEnum.SoftVersion)
            {
                comboBoxSoftwareID.SelectedValue = ParentRow["ID Software"];
                comboBoxSoftVersionID.SelectedValue = ParentRow["ID Version"];
            }
            dataGridView.Enabled = false;
            is_editable = true;
            _licenses.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (_vLicenses.Position != -1) && (!_licenses.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            var license = LicenseFromView();
            _vLicenses.AddNew();
            dataGridView.Enabled = false;
            _licenses.EditingNewRecord = true;
            ViewportFromLicense(license);
            is_editable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (SoftLicensesDataModel.Delete((int)((DataRowView)_vLicenses.Current)["ID License"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)_vLicenses[_vLicenses.Position]).Delete();
                is_editable = true;
                _viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
            if (CalcDataModelLicensesConcat.HasInstance())
                CalcDataModelLicensesConcat.GetInstance().Refresh(EntityType.License, (int)((DataRowView)_vLicenses.Current)["ID License"], true);
        }

        public override bool CanDeleteRecord()
        {
            return (_vLicenses.Position > -1)
                && (_viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new LicensesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (_vLicenses.Count > 0)
                viewport.LocateLicense((((DataRowView)_vLicenses[_vLicenses.Position])["ID License"] as int?) ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (_viewportState == ViewportState.NewRowState) || (_viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((_viewportState == ViewportState.NewRowState) || (_viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void SaveRecord()
        {
            var softLicense = LicenseFromViewport();
            if (!ValidateLicense(softLicense))
                return;
            switch (_viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idLicenses = SoftLicensesDataModel.Insert(softLicense);
                    if (idLicenses == -1)
                        return;
                    DataRowView newRow;
                    softLicense.IdLicense = idLicenses;
                    is_editable = false;
                    if (_vLicenses.Position == -1)
                        newRow = (DataRowView)_vLicenses.AddNew();
                    else
                        newRow = ((DataRowView)_vLicenses[_vLicenses.Position]);
                    FillRowFromLicense(softLicense, newRow);
                    _licenses.EditingNewRecord = false;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (softLicense.IdVersion == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о лицензии на программное обеспечение без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (SoftLicensesDataModel.Update(softLicense) == -1)
                        return;
                    var row = ((DataRowView)_vLicenses[_vLicenses.Position]);
                    is_editable = false;
                    FillRowFromLicense(softLicense, row);
                    break;
            }
            dataGridView.Enabled = true;
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            dataGridView.RowCount = _vLicenses.Count;
            _viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            if (CalcDataModelLicensesConcat.HasInstance())
                CalcDataModelLicensesConcat.GetInstance().Refresh(EntityType.License, softLicense.IdLicense, true);
        }

        public override void CancelRecord()
        {
            switch (_viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    _licenses.EditingNewRecord = false;
                    if (_vLicenses.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)_vLicenses[_vLicenses.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (_vLicenses.Position != -1)
                            dataGridView.Rows[_vLicenses.Position].Selected = true;
                    }
                    _viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    _vSoftware.Filter = "";
                    _vSoftSuppliers.Filter = "";
                    _vDepartments.Filter = "";
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    _viewportState = ViewportState.ReadState;
                    SelectCurrentSoftware();
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        private void SelectCurrentSoftware()
        {
            if (comboBoxSoftwareID.DataSource == null || comboBoxSoftVersionID.DataSource == null) return;
            if (_vLicenses.Position == -1) return;
            int? idVersion = null;
            var row = (DataRowView)_vLicenses[_vLicenses.Position];
            if (row["ID Version"] != DBNull.Value)
            {
                idVersion = Convert.ToInt32(row["ID Version"], CultureInfo.InvariantCulture);
            }
            else
            if (ParentRow != null && ParentType == ParentTypeEnum.SoftVersion)
            {
                idVersion = Convert.ToInt32(ParentRow["ID Version"], CultureInfo.InvariantCulture);
            }
            int? idSoftware = null;
            if (idVersion != null)
            {
                var versionRow = _softVersions.Select().Rows.Find(idVersion);
                if (versionRow != null && versionRow["ID Software"] != DBNull.Value)
                {
                    idSoftware = Convert.ToInt32(versionRow["ID Software"]);
                }
            }
            _vSoftware.Filter = "";
            is_editable = false;
            if (idSoftware != null)
            {
                comboBoxSoftwareID.SelectedValue = idSoftware;
            }
            else
            {
                comboBoxSoftwareID.SelectedValue = DBNull.Value;
            }
            if (idVersion != null)
            {
                comboBoxSoftVersionID.SelectedValue = idVersion;
            }
            else
            {
                comboBoxSoftVersionID.SelectedValue = DBNull.Value;
            }
            is_editable = true;
        }

        public override bool HasAssocInstallations()
        {
            return (_vLicenses.Position > -1) &&
                AccessControl.HasPrivelege(Priveleges.InstallationsRead);
        }

        public override bool HasAssocLicKeys()
        {
            return (_vLicenses.Position > -1);
        }

        public override void ShowAssocLicKeys()
        {
            ShowAssocViewport(ViewportType.LicenseKeysViewport);
        }

        public override void ShowAssocInstallations()
        {
            ShowAssocViewport(ViewportType.InstallationsViewport);
        }

        private void ShowAssocViewport(ViewportType viewportType)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (_vLicenses.Position == -1)
            {
                MessageBox.Show(@"Не выбрана лицензия на программное обеспечение", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "[ID License] = " + Convert.ToInt32(((DataRowView)_vLicenses[_vLicenses.Position])["ID License"], CultureInfo.InvariantCulture),
                ((DataRowView)_vLicenses[_vLicenses.Position]).Row,
                ParentTypeEnum.License);
        }    

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            _licenses.Select().RowChanged -= LicensesViewport_RowChanged;
            _licenses.Select().RowDeleted -= LicensesViewport_RowDeleted;
            _departments.Select().RowChanged -= Departments_RowChanged;
            _departments.Select().RowDeleted -= Departments_RowDeleted;
            _softwareConcat.RefreshEvent -= SoftwareConcatRefreshEvent;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (_viewportState == ViewportState.NewRowState)
                _licenses.EditingNewRecord = false;
            _licenses.Select().RowChanged -= LicensesViewport_RowChanged;
            _licenses.Select().RowDeleted -= LicensesViewport_RowDeleted;
            _departments.Select().RowChanged -= Departments_RowChanged;
            _departments.Select().RowDeleted -= Departments_RowDeleted;
            _softwareConcat.RefreshEvent -= SoftwareConcatRefreshEvent;
            Close();
        }

        private void LicensesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridView.RowCount = _vLicenses.Count;
                dataGridView.Refresh();
                UnbindedCheckBoxesUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        private void LicensesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = _vLicenses.Count;
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            SelectCurrentSoftware();
            base.OnVisibleChanged(e);
        }

        private void v_licenses_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            SelectCurrentSoftware();
            _vDepartments.Filter = "";
            _vSoftSuppliers.Filter = "";
            if (_vLicenses.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (_vLicenses.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[1];
                }
                else
                    if (dataGridView.Rows[_vLicenses.Position].Selected != true)
                    {
                        dataGridView.Rows[_vLicenses.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[_vLicenses.Position].Cells[1];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (_vLicenses.Position == -1)
                return;
            if (_viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            _viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void numericUpDownInstallationsCount_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void textBoxDocNumber_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxDocTypeID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxSoftwareID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
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
                comboBoxSoftwareID.Text = ((DataRowView)_vSoftware[_vSoftware.Position])["Software"].ToString();
            }
            if (comboBoxSoftwareID.SelectedItem == null)
            {
                comboBoxSoftwareID.Text = "";
                _vSoftware.Filter = "";
            }
        }

        private void comboBoxSoftVersionID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxSupplierID_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode == Keys.Back) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
            {
                var text = comboBoxSupplierID.Text;
                var selectionStart = comboBoxSupplierID.SelectionStart;
                var selectionLength = comboBoxSupplierID.SelectionLength;
                _vSoftSuppliers.Filter = "Supplier like '%" + comboBoxSupplierID.Text + "%'";
                comboBoxSupplierID.Text = text;
                comboBoxSupplierID.SelectionStart = selectionStart;
                comboBoxSupplierID.SelectionLength = selectionLength;
            }
        }

        private void comboBoxSupplierID_Leave(object sender, EventArgs e)
        {
            if (comboBoxSupplierID.Items.Count > 0)
            {
                if (comboBoxSupplierID.SelectedItem == null)
                    comboBoxSupplierID.SelectedItem = _vSoftSuppliers[_vSoftSuppliers.Position];
                comboBoxSupplierID.Text = ((DataRowView)_vSoftSuppliers[_vSoftSuppliers.Position])["Supplier"].ToString();
            }
            if (comboBoxSupplierID.SelectedItem == null)
            {
                comboBoxSupplierID.Text = "";
                _vSoftSuppliers.Filter = "";
            }
        }

        private void comboBoxSupplierID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxSupplierID.Items.Count == 0)
                comboBoxSupplierID.SelectedIndex = -1;
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

        private void comboBoxDepartmentID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxDepartmentID.Items.Count == 0)
                comboBoxDepartmentID.SelectedIndex = -1;
        }

        private void comboBoxSupplierID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxDepartmentID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxLicTypeID_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dateTimePickerBuyLicenseDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void dateTimePickerExpireLicenseDate_ValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void checkBoxInstallCountEnable_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownInstallationsCount.Enabled = checkBoxInstallCountEnable.Checked;
            CheckViewportModifications();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_vLicenses.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "idLicense":
                    e.Value = ((DataRowView)_vLicenses[e.RowIndex])["ID License"];
                    break;
                case "docNumber":
                    e.Value = ((DataRowView)_vLicenses[e.RowIndex])["DocNumber"];
                    break;
                case "expireLicenseDate":
                    e.Value = ((DataRowView)_vLicenses[e.RowIndex])["ExpireLicenseDate"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)_vLicenses[e.RowIndex])["ExpireLicenseDate"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "buyLicenseDate":
                    e.Value = ((DataRowView)_vLicenses[e.RowIndex])["BuyLicenseDate"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)_vLicenses[e.RowIndex])["BuyLicenseDate"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "software":
                    var row = _softwareConcat.Select().Rows.Find(((DataRowView)_vLicenses[e.RowIndex])["ID Version"]);
                    if (row != null)
                        e.Value = row["Software"];
                    break;
                case "department":
                    var rowIndex = _vDepartments.Find("ID Department", ((DataRowView)_vLicenses[e.RowIndex])["ID Department"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vDepartments[rowIndex])["Department"].ToString().Trim();
                    break;
                case "currentMaxInst":
                    var installations = SoftInstallationsDataModel.GetInstance().Select();
                    var licenseRow = (DataRowView) _vLicenses[e.RowIndex];
                    var currentCount = 0;
                    if (licenseRow["ID License"] != DBNull.Value)
                    {
                        currentCount = (from installRow in DataModelHelper.FilterRows(installations)
                                        where installRow.Field<int>("ID License") == (int?)licenseRow["ID License"]
                                        select installRow).Count();                    
                    }
                    int maxCount;
                    int.TryParse(((DataRowView)_vLicenses[e.RowIndex])["InstallationsCount"].ToString(),out maxCount) ;
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = currentCount == maxCount ? 
                        System.Drawing.Color.LightPink : System.Drawing.Color.White;
                    e.Value = currentCount + " / " + maxCount;
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                _vLicenses.Position = dataGridView.SelectedRows[0].Index;
            else
                _vLicenses.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                _vLicenses.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        public override List<string> GetIdLicenses()
        {
            var idList = new List<string>();
            for (var i = 0; i < dataGridView.RowCount; i++)
            {
                idList.Add(dataGridView["idLicense", i].Value.ToString());
            }
            return idList;
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicensesViewport));
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.dateTimePickerExpireLicenseDate = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.dateTimePickerBuyLicenseDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.comboBoxDocTypeID = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxDocNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idLicense = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.docNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.software = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.department = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buyLicenseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.expireLicenseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currentMaxInst = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxSoftVersionID = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxSupplierID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSoftwareID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxDepartmentID = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxInstallCountEnable = new System.Windows.Forms.CheckBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownInstallationsCount = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxLicTypeID = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel14.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox32.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInstallationsCount)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Controls.Add(this.groupBox3, 1, 1);
            this.tableLayoutPanel14.Controls.Add(this.groupBox32, 0, 1);
            this.tableLayoutPanel14.Controls.Add(this.dataGridView, 0, 2);
            this.tableLayoutPanel14.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 3;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(799, 470);
            this.tableLayoutPanel14.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.dateTimePickerExpireLicenseDate);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.dateTimePickerBuyLicenseDate);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(402, 148);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(394, 83);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Срок действия лицензии";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 15);
            this.label10.TabIndex = 87;
            this.label10.Text = "Дата истечения";
            // 
            // dateTimePickerExpireLicenseDate
            // 
            this.dateTimePickerExpireLicenseDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerExpireLicenseDate.Checked = false;
            this.dateTimePickerExpireLicenseDate.Location = new System.Drawing.Point(161, 51);
            this.dateTimePickerExpireLicenseDate.Name = "dateTimePickerExpireLicenseDate";
            this.dateTimePickerExpireLicenseDate.ShowCheckBox = true;
            this.dateTimePickerExpireLicenseDate.Size = new System.Drawing.Size(220, 21);
            this.dateTimePickerExpireLicenseDate.TabIndex = 1;
            this.dateTimePickerExpireLicenseDate.ValueChanged += new System.EventHandler(this.dateTimePickerExpireLicenseDate_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(124, 15);
            this.label9.TabIndex = 85;
            this.label9.Text = "Дата приобретения";
            // 
            // dateTimePickerBuyLicenseDate
            // 
            this.dateTimePickerBuyLicenseDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerBuyLicenseDate.Location = new System.Drawing.Point(161, 22);
            this.dateTimePickerBuyLicenseDate.Name = "dateTimePickerBuyLicenseDate";
            this.dateTimePickerBuyLicenseDate.Size = new System.Drawing.Size(220, 21);
            this.dateTimePickerBuyLicenseDate.TabIndex = 0;
            this.dateTimePickerBuyLicenseDate.ValueChanged += new System.EventHandler(this.dateTimePickerBuyLicenseDate_ValueChanged);
            // 
            // groupBox32
            // 
            this.groupBox32.Controls.Add(this.comboBoxDocTypeID);
            this.groupBox32.Controls.Add(this.label8);
            this.groupBox32.Controls.Add(this.textBoxDocNumber);
            this.groupBox32.Controls.Add(this.label1);
            this.groupBox32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox32.Location = new System.Drawing.Point(3, 148);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(393, 83);
            this.groupBox32.TabIndex = 2;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Документ-основание на приобретение";
            // 
            // comboBoxDocTypeID
            // 
            this.comboBoxDocTypeID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDocTypeID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDocTypeID.FormattingEnabled = true;
            this.comboBoxDocTypeID.Location = new System.Drawing.Point(161, 51);
            this.comboBoxDocTypeID.Name = "comboBoxDocTypeID";
            this.comboBoxDocTypeID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxDocTypeID.TabIndex = 1;
            this.comboBoxDocTypeID.SelectedValueChanged += new System.EventHandler(this.comboBoxDocTypeID_SelectedValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 15);
            this.label8.TabIndex = 86;
            this.label8.Text = "Вид документа";
            // 
            // textBoxDocNumber
            // 
            this.textBoxDocNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocNumber.Location = new System.Drawing.Point(161, 22);
            this.textBoxDocNumber.MaxLength = 500;
            this.textBoxDocNumber.Name = "textBoxDocNumber";
            this.textBoxDocNumber.Size = new System.Drawing.Size(220, 21);
            this.textBoxDocNumber.TabIndex = 0;
            this.textBoxDocNumber.TextChanged += new System.EventHandler(this.textBoxDocNumber_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 15);
            this.label1.TabIndex = 84;
            this.label1.Text = "Номер документа";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
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
            this.idLicense,
            this.docNumber,
            this.software,
            this.department,
            this.buyLicenseDate,
            this.expireLicenseDate,
            this.currentMaxInst});
            this.tableLayoutPanel14.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 237);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(793, 230);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // idLicense
            // 
            this.idLicense.Frozen = true;
            this.idLicense.HeaderText = "Идентификатор";
            this.idLicense.Name = "idLicense";
            this.idLicense.ReadOnly = true;
            this.idLicense.Visible = false;
            // 
            // docNumber
            // 
            this.docNumber.HeaderText = "Документ-основание";
            this.docNumber.MinimumWidth = 150;
            this.docNumber.Name = "docNumber";
            this.docNumber.ReadOnly = true;
            this.docNumber.Width = 150;
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
            // department
            // 
            this.department.HeaderText = "Департамент-заказчик";
            this.department.MinimumWidth = 300;
            this.department.Name = "department";
            this.department.ReadOnly = true;
            this.department.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.department.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.department.Width = 300;
            // 
            // buyLicenseDate
            // 
            this.buyLicenseDate.HeaderText = "Дата приобретения";
            this.buyLicenseDate.MinimumWidth = 150;
            this.buyLicenseDate.Name = "buyLicenseDate";
            this.buyLicenseDate.ReadOnly = true;
            this.buyLicenseDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.buyLicenseDate.Width = 150;
            // 
            // expireLicenseDate
            // 
            this.expireLicenseDate.HeaderText = "Дата истечения";
            this.expireLicenseDate.MinimumWidth = 150;
            this.expireLicenseDate.Name = "expireLicenseDate";
            this.expireLicenseDate.ReadOnly = true;
            this.expireLicenseDate.Width = 150;
            // 
            // currentMaxInst
            // 
            this.currentMaxInst.HeaderText = "Установок тек./макс.";
            this.currentMaxInst.MinimumWidth = 160;
            this.currentMaxInst.Name = "currentMaxInst";
            this.currentMaxInst.ReadOnly = true;
            this.currentMaxInst.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.currentMaxInst.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.currentMaxInst.Width = 160;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxSoftVersionID);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.comboBoxSupplierID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxSoftwareID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 139);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сведения о ПО";
            // 
            // comboBoxSoftVersionID
            // 
            this.comboBoxSoftVersionID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSoftVersionID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSoftVersionID.FormattingEnabled = true;
            this.comboBoxSoftVersionID.Location = new System.Drawing.Point(161, 54);
            this.comboBoxSoftVersionID.Name = "comboBoxSoftVersionID";
            this.comboBoxSoftVersionID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxSoftVersionID.TabIndex = 1;
            this.comboBoxSoftVersionID.SelectedValueChanged += new System.EventHandler(this.comboBoxSoftVersionID_SelectedValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 57);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 15);
            this.label11.TabIndex = 77;
            this.label11.Text = "Версия ПО";
            // 
            // comboBoxSupplierID
            // 
            this.comboBoxSupplierID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSupplierID.FormattingEnabled = true;
            this.comboBoxSupplierID.Location = new System.Drawing.Point(161, 84);
            this.comboBoxSupplierID.Name = "comboBoxSupplierID";
            this.comboBoxSupplierID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxSupplierID.TabIndex = 2;
            this.comboBoxSupplierID.DropDownClosed += new System.EventHandler(this.comboBoxSupplierID_DropDownClosed);
            this.comboBoxSupplierID.SelectedValueChanged += new System.EventHandler(this.comboBoxSupplierID_SelectedValueChanged);
            this.comboBoxSupplierID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxSupplierID_KeyUp);
            this.comboBoxSupplierID.Leave += new System.EventHandler(this.comboBoxSupplierID_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 15);
            this.label3.TabIndex = 75;
            this.label3.Text = "Поставщик ПО";
            // 
            // comboBoxSoftwareID
            // 
            this.comboBoxSoftwareID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSoftwareID.FormattingEnabled = true;
            this.comboBoxSoftwareID.Location = new System.Drawing.Point(161, 25);
            this.comboBoxSoftwareID.Name = "comboBoxSoftwareID";
            this.comboBoxSoftwareID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxSoftwareID.TabIndex = 0;
            this.comboBoxSoftwareID.DropDownClosed += new System.EventHandler(this.comboBoxSoftwareID_DropDownClosed);
            this.comboBoxSoftwareID.SelectedValueChanged += new System.EventHandler(this.comboBoxSoftwareID_SelectedValueChanged);
            this.comboBoxSoftwareID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxSoftwareID_KeyUp);
            this.comboBoxSoftwareID.Leave += new System.EventHandler(this.comboBoxSoftwareID_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 73;
            this.label2.Text = "Наименование ПО";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxDepartmentID);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.checkBoxInstallCountEnable);
            this.groupBox2.Controls.Add(this.textBoxDescription);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDownInstallationsCount);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comboBoxLicTypeID);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(402, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(394, 139);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Общие сведения о лицензии";
            // 
            // comboBoxDepartmentID
            // 
            this.comboBoxDepartmentID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDepartmentID.FormattingEnabled = true;
            this.comboBoxDepartmentID.Location = new System.Drawing.Point(161, 25);
            this.comboBoxDepartmentID.Name = "comboBoxDepartmentID";
            this.comboBoxDepartmentID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxDepartmentID.TabIndex = 0;
            this.comboBoxDepartmentID.DropDownClosed += new System.EventHandler(this.comboBoxDepartmentID_DropDownClosed);
            this.comboBoxDepartmentID.SelectedValueChanged += new System.EventHandler(this.comboBoxDepartmentID_SelectedValueChanged);
            this.comboBoxDepartmentID.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxDepartmentID_KeyUp);
            this.comboBoxDepartmentID.Leave += new System.EventHandler(this.comboBoxDepartmentID_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 15);
            this.label4.TabIndex = 77;
            this.label4.Text = "Заказчик";
            // 
            // checkBoxInstallCountEnable
            // 
            this.checkBoxInstallCountEnable.AutoSize = true;
            this.checkBoxInstallCountEnable.Location = new System.Drawing.Point(165, 88);
            this.checkBoxInstallCountEnable.Name = "checkBoxInstallCountEnable";
            this.checkBoxInstallCountEnable.Size = new System.Drawing.Size(15, 14);
            this.checkBoxInstallCountEnable.TabIndex = 2;
            this.checkBoxInstallCountEnable.UseVisualStyleBackColor = true;
            this.checkBoxInstallCountEnable.CheckedChanged += new System.EventHandler(this.checkBoxInstallCountEnable_CheckedChanged);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 114);
            this.textBoxDescription.MaxLength = 500;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(220, 21);
            this.textBoxDescription.TabIndex = 4;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 116);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 15);
            this.label7.TabIndex = 82;
            this.label7.Text = "Примечание";
            // 
            // numericUpDownInstallationsCount
            // 
            this.numericUpDownInstallationsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownInstallationsCount.Enabled = false;
            this.numericUpDownInstallationsCount.Location = new System.Drawing.Point(186, 84);
            this.numericUpDownInstallationsCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownInstallationsCount.Name = "numericUpDownInstallationsCount";
            this.numericUpDownInstallationsCount.Size = new System.Drawing.Size(195, 21);
            this.numericUpDownInstallationsCount.TabIndex = 3;
            this.numericUpDownInstallationsCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownInstallationsCount.ValueChanged += new System.EventHandler(this.numericUpDownInstallationsCount_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 87);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 15);
            this.label6.TabIndex = 79;
            this.label6.Text = "Количество установок";
            // 
            // comboBoxLicTypeID
            // 
            this.comboBoxLicTypeID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLicTypeID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLicTypeID.FormattingEnabled = true;
            this.comboBoxLicTypeID.Location = new System.Drawing.Point(161, 54);
            this.comboBoxLicTypeID.Name = "comboBoxLicTypeID";
            this.comboBoxLicTypeID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxLicTypeID.TabIndex = 1;
            this.comboBoxLicTypeID.SelectedValueChanged += new System.EventHandler(this.comboBoxLicTypeID_SelectedValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 15);
            this.label5.TabIndex = 77;
            this.label5.Text = "Вид лицензии";
            // 
            // LicensesViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(650, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(805, 476);
            this.Controls.Add(this.tableLayoutPanel14);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LicensesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Лицензии на ПО";
            this.tableLayoutPanel14.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox32.ResumeLayout(false);
            this.groupBox32.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInstallationsCount)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
