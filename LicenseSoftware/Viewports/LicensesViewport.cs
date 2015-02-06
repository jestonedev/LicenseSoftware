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
using System.Text;
using System.Windows.Forms;

namespace LicenseSoftware.Viewport
{
    internal sealed class LicensesViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView dataGridView;
        private GroupBox groupBox32;
        private bool is_editable = false;
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
        #endregion Components

        #region Models
        SoftLicensesDataModel licenses = null;
        SoftLicTypesDataModel softLicTypes = null;
        SoftLicDocTypesDataModel softLicDocTypes = null;
        DepartmentsDataModel departments = null;
        SoftSuppliersDataModel softSuppliers = null;
        CalcDataModelSoftwareConcat softwareDM = null;
        #endregion Models

        #region Views
        BindingSource v_licenses = null;
        BindingSource v_softLicTypes = null;
        BindingSource v_softLicDocTypes = null;
        BindingSource v_departments = null;
        BindingSource v_softSuppliers = null;
        BindingSource v_software = null;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private DataGridViewTextBoxColumn idLicense;
        private DataGridViewTextBoxColumn docNumber;
        private DataGridViewTextBoxColumn software;
        private DataGridViewTextBoxColumn department;
        private DataGridViewTextBoxColumn buyLicenseDate;
        private DataGridViewTextBoxColumn expireLicenseDate;


        private SearchForm sSearchForm = null;

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
            this.DynamicFilter = licensesViewport.DynamicFilter;
            this.StaticFilter = licensesViewport.StaticFilter;
            this.ParentRow = licensesViewport.ParentRow;
            this.ParentType = licensesViewport.ParentType;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
            {
                if ((ParentRow != null) && (ParentType == ParentTypeEnum.Software))
                {
                    this.Text = String.Format(CultureInfo.InvariantCulture, "Лицензия на ПО №{0}", ParentRow["ID Software"]);
                }
                else
                    this.Text = "Новая лицензия на ПО";
            }
            else
                if (v_licenses.Position != -1)
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Software))
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Лицензия №{0} на ПО №{1}",
                            ((DataRowView)v_licenses[v_licenses.Position])["ID License"], ParentRow["ID Software"]);
                    else
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Лицензия №{0}", ((DataRowView)v_licenses[v_licenses.Position])["ID License"]);
                }
                else
                {
                    if ((ParentRow != null) && (ParentType == ParentTypeEnum.Software))
                        this.Text = String.Format(CultureInfo.InvariantCulture, "Лицензии на ПО №{0} отсутствуют", ParentRow["ID Software"]);
                    else
                        this.Text = "Лицензии отсутствуют";
                }
        }

        private void DataBind()
        {
            comboBoxDepartmentID.DataSource = v_departments;
            comboBoxDepartmentID.ValueMember = "ID Department";
            comboBoxDepartmentID.DisplayMember = "Department";
            comboBoxDepartmentID.DataBindings.Clear();
            comboBoxDepartmentID.DataBindings.Add("SelectedValue", v_licenses, "ID Department", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxDocTypeID.DataSource = v_softLicDocTypes;
            comboBoxDocTypeID.ValueMember = "ID DocType";
            comboBoxDocTypeID.DisplayMember = "DocType";
            comboBoxDocTypeID.DataBindings.Clear();
            comboBoxDocTypeID.DataBindings.Add("SelectedValue", v_licenses, "ID DocType", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxLicTypeID.DataSource = v_softLicTypes;
            comboBoxLicTypeID.ValueMember = "ID LicType";
            comboBoxLicTypeID.DisplayMember = "LicType";
            comboBoxLicTypeID.DataBindings.Clear();
            comboBoxLicTypeID.DataBindings.Add("SelectedValue", v_licenses, "ID LicType", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxSoftwareID.DataSource = v_software;
            comboBoxSoftwareID.ValueMember = "ID Software";
            comboBoxSoftwareID.DisplayMember = "Software";
            comboBoxSoftwareID.DataBindings.Clear();
            comboBoxSoftwareID.DataBindings.Add("SelectedValue", v_licenses, "ID Software", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxSupplierID.DataSource = v_softSuppliers;
            comboBoxSupplierID.ValueMember = "ID Supplier";
            comboBoxSupplierID.DisplayMember = "Supplier";
            comboBoxSupplierID.DataBindings.Clear();
            comboBoxSupplierID.DataBindings.Add("SelectedValue", v_licenses, "ID Supplier", true, DataSourceUpdateMode.Never, DBNull.Value);

            textBoxDocNumber.DataBindings.Clear();
            textBoxDocNumber.DataBindings.Add("Text", v_licenses, "DocNumber", true, DataSourceUpdateMode.Never, "");
            textBoxDescription.DataBindings.Clear();
            textBoxDescription.DataBindings.Add("Text", v_licenses, "Description", true, DataSourceUpdateMode.Never, "");

            numericUpDownInstallationsCount.DataBindings.Clear();
            numericUpDownInstallationsCount.DataBindings.Add("Value", v_licenses, "InstallationsCount", true, DataSourceUpdateMode.Never, 1);

            dateTimePickerBuyLicenseDate.DataBindings.Clear();
            dateTimePickerBuyLicenseDate.DataBindings.Add("Value", v_licenses, "BuyLicenseDate", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
            dateTimePickerExpireLicenseDate.DataBindings.Clear();
            dateTimePickerExpireLicenseDate.DataBindings.Add("Value", v_licenses, "ExpireLicenseDate", true, DataSourceUpdateMode.Never, DateTime.Now.Date);
        }

        private void UnbindedCheckBoxesUpdate()
        {
            DataRowView row = (v_licenses.Position >= 0) ? (DataRowView)v_licenses[v_licenses.Position] : null;
            if ((v_licenses.Position >= 0) && (row["ExpireLicenseDate"] != DBNull.Value))
                dateTimePickerExpireLicenseDate.Checked = true;
            else
            {
                dateTimePickerExpireLicenseDate.Value = DateTime.Now.Date;
                dateTimePickerExpireLicenseDate.Checked = false;
            }
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_licenses.Position != -1) && (LicenseFromView() != LicenseFromViewport()))
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
                            if (licenses.EditingNewRecord)
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

        private void LocateLicense(int id)
        {
            int Position = v_licenses.Find("ID License", id);
            is_editable = false;
            if (Position > 0)
                v_licenses.Position = Position;
            is_editable = true;
        }

        private void ViewportFromLicense(SoftLicense license)
        {
            comboBoxDepartmentID.SelectedValue = ViewportHelper.ValueOrDBNull(license.IdDepartment);
            comboBoxDocTypeID.SelectedValue = ViewportHelper.ValueOrDBNull(license.IdDocType);
            comboBoxLicTypeID.SelectedValue = ViewportHelper.ValueOrDBNull(license.IdLicType);
            comboBoxSupplierID.SelectedValue = ViewportHelper.ValueOrDBNull(license.IdSupplier);
            comboBoxSoftwareID.SelectedValue = ViewportHelper.ValueOrDBNull(license.IdSoftware);
            textBoxDocNumber.Text = license.DocNumber;
            textBoxDescription.Text = license.Description;
            dateTimePickerBuyLicenseDate.Value = ViewportHelper.ValueOrDefault(license.BuyLicenseDate);
            dateTimePickerExpireLicenseDate.Value = ViewportHelper.ValueOrDefault(license.ExpireLicenseDate);
            numericUpDownInstallationsCount.Value = ViewportHelper.ValueOrDefault(license.InstallationsCount);
        }

        private SoftLicense LicenseFromViewport()
        {
            SoftLicense softLicense = new SoftLicense();
            if (v_licenses.Position == -1)
                softLicense.IdLicense = null;
            else
                softLicense.IdLicense = ViewportHelper.ValueOrNull<int>((DataRowView)v_licenses[v_licenses.Position], "ID License");
            softLicense.IdSoftware = ViewportHelper.ValueOrNull<int>(comboBoxSoftwareID);
            softLicense.IdLicType = ViewportHelper.ValueOrNull<int>(comboBoxLicTypeID);
            softLicense.IdDocType = ViewportHelper.ValueOrNull<int>(comboBoxDocTypeID);
            softLicense.IdSupplier = ViewportHelper.ValueOrNull<int>(comboBoxSupplierID);
            softLicense.IdDepartment = ViewportHelper.ValueOrNull<int>(comboBoxDepartmentID);
            softLicense.DocNumber = ViewportHelper.ValueOrNull(textBoxDocNumber);
            softLicense.Description = ViewportHelper.ValueOrNull(textBoxDescription);
            softLicense.BuyLicenseDate = ViewportHelper.ValueOrNull(dateTimePickerBuyLicenseDate);
            softLicense.ExpireLicenseDate = ViewportHelper.ValueOrNull(dateTimePickerExpireLicenseDate);
            softLicense.InstallationsCount = (int)numericUpDownInstallationsCount.Value;
            return softLicense;
        }

        private SoftLicense LicenseFromView()
        {
            SoftLicense softLicense = new SoftLicense();
            DataRowView row = (DataRowView)v_licenses[v_licenses.Position];
            softLicense.IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License");
            softLicense.IdSoftware = ViewportHelper.ValueOrNull<int>(row, "ID Software");
            softLicense.IdLicType = ViewportHelper.ValueOrNull<int>(row, "ID LicType");
            softLicense.IdDocType = ViewportHelper.ValueOrNull<int>(row, "ID DocType");
            softLicense.IdSupplier = ViewportHelper.ValueOrNull<int>(row, "ID Supplier");
            softLicense.IdDepartment = ViewportHelper.ValueOrNull<int>(row, "ID Department");
            softLicense.DocNumber = ViewportHelper.ValueOrNull(row, "DocNumber");
            softLicense.Description = ViewportHelper.ValueOrNull(row, "Description");
            softLicense.BuyLicenseDate = ViewportHelper.ValueOrNull<DateTime>(row, "BuyLicenseDate");
            softLicense.ExpireLicenseDate = ViewportHelper.ValueOrNull<DateTime>(row, "ExpireLicenseDate");
            softLicense.InstallationsCount = ViewportHelper.ValueOrNull<int>(row, "InstallationsCount");
            return softLicense;
        }

        private static void FillRowFromLicense(SoftLicense softLicense, DataRowView row)
        {
            row.BeginEdit();
            row["ID License"] = ViewportHelper.ValueOrDBNull(softLicense.IdLicense);
            row["ID Software"] = ViewportHelper.ValueOrDBNull(softLicense.IdSoftware);
            row["ID LicType"] = ViewportHelper.ValueOrDBNull(softLicense.IdLicType);
            row["ID DocType"] = ViewportHelper.ValueOrDBNull(softLicense.IdDocType);
            row["ID Supplier"] = ViewportHelper.ValueOrDBNull(softLicense.IdSupplier);
            row["ID Department"] = ViewportHelper.ValueOrDBNull(softLicense.IdDepartment);
            row["DocNumber"] = ViewportHelper.ValueOrDBNull(softLicense.DocNumber);
            row["Description"] = ViewportHelper.ValueOrDBNull(softLicense.Description);
            row["BuyLicenseDate"] = ViewportHelper.ValueOrDBNull(softLicense.BuyLicenseDate);
            row["ExpireLicenseDate"] = ViewportHelper.ValueOrDBNull(softLicense.ExpireLicenseDate);
            row["InstallationsCount"] = ViewportHelper.ValueOrDBNull(softLicense.InstallationsCount);
            row.EndEdit();
        }

        private bool ValidateLicense(SoftLicense softLicense)
        {
            if (softLicense.IdSoftware == null)
            {
                MessageBox.Show("Необходимо выбрать программное обеспечение, на которое заводится лицензия", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftwareID.Focus();
                return false;
            }
            if (softLicense.IdSupplier == null)
            {
                MessageBox.Show("Необходимо выбрать поставщика программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSupplierID.Focus();
                return false;
            }
            if (softLicense.IdDepartment == null)
            {
                MessageBox.Show("Необходимо выбрать департамент-заказчик программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxDepartmentID.Focus();
                return false;
            } else
                if (!(bool)((DataRowView)v_departments[
                    v_departments.Find("ID Department", softLicense.IdDepartment)])["AllowSelect"])
                {
                    MessageBox.Show("У вас нет прав на подачу заявок на данный департамент", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    comboBoxDepartmentID.Focus();
                    return false;
                }
            if (softLicense.IdLicType == null)
            {
                MessageBox.Show("Необходимо выбрать вид лицензии на программное обеспечение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxLicTypeID.Focus();
                return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return v_licenses.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_licenses.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_licenses.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_licenses.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_licenses.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_licenses.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_licenses.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_licenses.Position > -1) && (v_licenses.Position < (v_licenses.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_licenses.Position > -1) && (v_licenses.Position < (v_licenses.Count - 1));
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
            softLicTypes = SoftLicTypesDataModel.GetInstance();
            softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();
            softSuppliers = SoftSuppliersDataModel.GetInstance();
            departments = DepartmentsDataModel.GetInstance();
            licenses = SoftLicensesDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            softLicTypes.Select();
            softLicDocTypes.Select();
            softSuppliers.Select();
            licenses.Select();
            softwareDM.Select();

            DataSet ds = DataSetManager.DataSet;

            v_softLicTypes = new BindingSource();
            v_softLicTypes.DataMember = "SoftLicTypes";
            v_softLicTypes.DataSource = ds;

            v_softLicDocTypes = new BindingSource();
            v_softLicDocTypes.DataMember = "SoftLicDocTypes";
            v_softLicDocTypes.DataSource = ds;

            v_softSuppliers = new BindingSource();
            v_softSuppliers.DataMember = "SoftSuppliers";
            v_softSuppliers.DataSource = ds;

            v_departments = new BindingSource();
            v_departments.DataSource = departments.SelectVisibleDepartments();

            v_software = new BindingSource();
            v_software.DataMember = "SoftwareConcat";
            v_software.DataSource = ds;

            v_licenses = new BindingSource();
            v_licenses.CurrentItemChanged += new EventHandler(v_licenses_CurrentItemChanged);
            v_licenses.DataMember = "SoftLicenses";
            v_licenses.DataSource = ds;
            RebuildFilter();

            DataBind();

            licenses.Select().RowChanged += LicensesViewport_RowChanged;
            licenses.Select().RowDeleted += LicensesViewport_RowDeleted;
            departments.Select().RowChanged += Departments_RowChanged;
            departments.Select().RowDeleted += Departments_RowDeleted;

            dataGridView.RowCount = v_licenses.Count;
            SetViewportCaption();

            softwareDM.RefreshEvent += softwareDM_RefreshEvent;
            ViewportHelper.SetDoubleBuffered(dataGridView);
            is_editable = true;
        }

        private void Departments_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            v_departments.DataSource = departments.SelectVisibleDepartments();
            RebuildFilter();
        }

        private void Departments_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            v_departments.DataSource = departments.SelectVisibleDepartments();
            RebuildFilter();
        }

        private void RebuildFilter()
        {
            string Filter = StaticFilter;
            // Фильтрация по правам на департаменты
            if (!String.IsNullOrEmpty(Filter))
                Filter += " AND ";
            Filter += "[ID Department] IN (0";
            for (int i = 0; i < v_departments.Count; i++)
                if ((bool)((DataRowView)v_departments[i])["AllowSelect"])
                    Filter += ((DataRowView)v_departments[i])["ID Department"] + ",";
            Filter = Filter.TrimEnd(',');
            Filter += ")";
            if (!String.IsNullOrEmpty(Filter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            v_licenses.Filter = Filter;
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
                sSearchForm = new SearchLicensesForm();
            if (sSearchForm.ShowDialog() != DialogResult.OK)
                return;
            DynamicFilter = sSearchForm.GetFilter();
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            v_licenses.Filter = Filter;
            dataGridView.RowCount = v_licenses.Count;
        }

        public override void ClearSearch()
        {
            v_licenses.Filter = StaticFilter;
            dataGridView.RowCount = v_licenses.Count;
            DynamicFilter = "";
            MenuCallback.EditingStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.NavigationStateUpdate();
        }

        void softwareDM_RefreshEvent(object sender, EventArgs e)
        {
            dataGridView.Refresh();
        }

        public override bool CanInsertRecord()
        {
            return (!licenses.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.LICENSES_READ_WRITE);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            v_licenses.AddNew();
            if (ParentRow != null && ParentType == ParentTypeEnum.Software)
                comboBoxSoftwareID.SelectedValue = ParentRow["ID Software"];
            dataGridView.Enabled = false;
            is_editable = true;
            licenses.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_licenses.Position != -1) && (!licenses.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.LICENSES_READ_WRITE);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            SoftLicense license = LicenseFromView();
            v_licenses.AddNew();
            dataGridView.Enabled = false;
            licenses.EditingNewRecord = true;
            ViewportFromLicense(license);
            is_editable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (SoftLicensesDataModel.Delete((int)((DataRowView)v_licenses.Current)["ID License"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_licenses[v_licenses.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
            }
            if (CalcDataModelLicensesConcat.HasInstance())
                CalcDataModelLicensesConcat.GetInstance().Refresh(EntityType.License, (int)((DataRowView)v_licenses.Current)["ID License"], true);
        }

        public override bool CanDeleteRecord()
        {
            return (v_licenses.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.LICENSES_READ_WRITE);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            LicensesViewport viewport = new LicensesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_licenses.Count > 0)
                viewport.LocateLicense((((DataRowView)v_licenses[v_licenses.Position])["ID License"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.LICENSES_READ_WRITE);
        }

        public override void SaveRecord()
        {
            SoftLicense softLicense = LicenseFromViewport();
            if (!ValidateLicense(softLicense))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int idLicenses = SoftLicensesDataModel.Insert(softLicense);
                    if (idLicenses == -1)
                        return;
                    DataRowView newRow;
                    softLicense.IdLicense = idLicenses;
                    is_editable = false;
                    if (v_licenses.Position == -1)
                        newRow = (DataRowView)v_licenses.AddNew();
                    else
                        newRow = ((DataRowView)v_licenses[v_licenses.Position]);
                    FillRowFromLicense(softLicense, newRow);
                    licenses.EditingNewRecord = false;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (softLicense.IdSoftware == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о лицензии на программное обеспечение без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (SoftLicensesDataModel.Update(softLicense) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_licenses[v_licenses.Position]);
                    is_editable = false;
                    FillRowFromLicense(softLicense, row);
                    break;
            }
            dataGridView.Enabled = true;
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            dataGridView.RowCount = v_licenses.Count;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            if (CalcDataModelLicensesConcat.HasInstance())
                CalcDataModelLicensesConcat.GetInstance().Refresh(EntityType.License, softLicense.IdLicense, true);
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    licenses.EditingNewRecord = false;
                    if (v_licenses.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_licenses[v_licenses.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (v_licenses.Position != -1)
                            dataGridView.Rows[v_licenses.Position].Selected = true;
                    }
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    v_software.Filter = "";
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            UnbindedCheckBoxesUpdate();
            is_editable = true;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool HasAssocInstallations()
        {
            return (v_licenses.Position > -1) &&
                AccessControl.HasPrivelege(Priveleges.INSTALLATIONS_READ);
        }

        public override bool HasAssocLicKeys()
        {
            return (v_licenses.Position > -1);
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
            if (v_licenses.Position == -1)
            {
                MessageBox.Show("Не выбрана лицензия на программное обеспечение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, viewportType,
                "[ID License] = " + Convert.ToInt32(((DataRowView)v_licenses[v_licenses.Position])["ID License"], CultureInfo.InvariantCulture),
                ((DataRowView)v_licenses[v_licenses.Position]).Row,
                ParentTypeEnum.License);
        }    

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            licenses.Select().RowChanged -= LicensesViewport_RowChanged;
            licenses.Select().RowDeleted -= LicensesViewport_RowDeleted;
            departments.Select().RowChanged -= Departments_RowChanged;
            departments.Select().RowDeleted -= Departments_RowDeleted;
            softwareDM.RefreshEvent -= softwareDM_RefreshEvent;
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                licenses.EditingNewRecord = false;
            licenses.Select().RowChanged -= LicensesViewport_RowChanged;
            licenses.Select().RowDeleted -= LicensesViewport_RowDeleted;
            departments.Select().RowChanged -= Departments_RowChanged;
            departments.Select().RowDeleted -= Departments_RowDeleted;
            softwareDM.RefreshEvent -= softwareDM_RefreshEvent;
            base.Close();
        }

        void LicensesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridView.RowCount = v_licenses.Count;
                dataGridView.Refresh();
                UnbindedCheckBoxesUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        void LicensesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_licenses.Count;
            UnbindedCheckBoxesUpdate();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            UnbindedCheckBoxesUpdate();
            base.OnVisibleChanged(e);
        }

        void v_licenses_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            v_software.Filter = "";
            if (v_licenses.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_licenses.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[1];
                }
                else
                    if (dataGridView.Rows[v_licenses.Position].Selected != true)
                    {
                        dataGridView.Rows[v_licenses.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[v_licenses.Position].Cells[1];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            UnbindedCheckBoxesUpdate();
            if (v_licenses.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
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

        private void comboBoxDocTypeID_SelectedIndexChanged(object sender, EventArgs e)
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
                comboBoxSoftwareID.Text = ((DataRowView)v_software[v_software.Position])["Software"].ToString();
            }
            if (comboBoxSoftwareID.SelectedItem == null)
            {
                comboBoxSoftwareID.Text = "";
                v_software.Filter = "";
            }
        }

        private void comboBoxSupplierID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxDepartmentID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxLicTypeID_SelectedIndexChanged(object sender, EventArgs e)
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

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_licenses.Count <= e.RowIndex) return;
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "idLicense":
                    e.Value = ((DataRowView)v_licenses[e.RowIndex])["ID License"];
                    break;
                case "docNumber":
                    e.Value = ((DataRowView)v_licenses[e.RowIndex])["DocNumber"];
                    break;
                case "expireLicenseDate":
                    e.Value = ((DataRowView)v_licenses[e.RowIndex])["ExpireLicenseDate"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_licenses[e.RowIndex])["ExpireLicenseDate"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "buyLicenseDate":
                    e.Value = ((DataRowView)v_licenses[e.RowIndex])["BuyLicenseDate"] == DBNull.Value ? "" :
                        ((DateTime)((DataRowView)v_licenses[e.RowIndex])["BuyLicenseDate"]).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case "software":
                    DataRow row = softwareDM.Select().Rows.Find(((DataRowView)v_licenses[e.RowIndex])["ID Software"]);
                    if (row != null)
                        e.Value = row["Software"];
                    break;
                case "department":
                    int row_index = v_departments.Find("ID Department", ((DataRowView)v_licenses[e.RowIndex])["ID Department"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_departments[row_index])["Department"].ToString().Trim();
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                v_licenses.Position = dataGridView.SelectedRows[0].Index;
            else
                v_licenses.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_licenses.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            if (dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                changeSortColumn(SortOrder.Descending);
            else
                changeSortColumn(SortOrder.Ascending);
            dataGridView.Refresh();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxDepartmentID = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxSupplierID = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSoftwareID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
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
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 119F));
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
            this.groupBox3.Location = new System.Drawing.Point(402, 122);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(394, 83);
            this.groupBox3.TabIndex = 3;
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
            this.groupBox32.Location = new System.Drawing.Point(3, 122);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(393, 83);
            this.groupBox32.TabIndex = 2;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Документе-основании на приобретение";
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
            this.comboBoxDocTypeID.SelectedIndexChanged += new System.EventHandler(this.comboBoxDocTypeID_SelectedIndexChanged);
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
            this.idLicense,
            this.docNumber,
            this.software,
            this.department,
            this.buyLicenseDate,
            this.expireLicenseDate});
            this.tableLayoutPanel14.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 211);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(793, 256);
            this.dataGridView.TabIndex = 4;
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
            this.docNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.docNumber.HeaderText = "Документ-основание";
            this.docNumber.MinimumWidth = 100;
            this.docNumber.Name = "docNumber";
            this.docNumber.ReadOnly = true;
            this.docNumber.Width = 155;
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
            this.department.HeaderText = "Департамент-заказчик";
            this.department.MinimumWidth = 300;
            this.department.Name = "department";
            this.department.ReadOnly = true;
            this.department.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.department.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // buyLicenseDate
            // 
            this.buyLicenseDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
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
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxDepartmentID);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBoxSupplierID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxSoftwareID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 113);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сведения о ПО";
            // 
            // comboBoxDepartmentID
            // 
            this.comboBoxDepartmentID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDepartmentID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDepartmentID.FormattingEnabled = true;
            this.comboBoxDepartmentID.Location = new System.Drawing.Point(161, 80);
            this.comboBoxDepartmentID.Name = "comboBoxDepartmentID";
            this.comboBoxDepartmentID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxDepartmentID.TabIndex = 2;
            this.comboBoxDepartmentID.SelectedIndexChanged += new System.EventHandler(this.comboBoxDepartmentID_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 15);
            this.label4.TabIndex = 77;
            this.label4.Text = "Департамент-заказчик";
            // 
            // comboBoxSupplierID
            // 
            this.comboBoxSupplierID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSupplierID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSupplierID.FormattingEnabled = true;
            this.comboBoxSupplierID.Location = new System.Drawing.Point(161, 51);
            this.comboBoxSupplierID.Name = "comboBoxSupplierID";
            this.comboBoxSupplierID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxSupplierID.TabIndex = 1;
            this.comboBoxSupplierID.SelectedIndexChanged += new System.EventHandler(this.comboBoxSupplierID_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 54);
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
            this.comboBoxSoftwareID.Location = new System.Drawing.Point(161, 22);
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
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 73;
            this.label2.Text = "Наименование ПО";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxDescription);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDownInstallationsCount);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comboBoxLicTypeID);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(402, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(394, 113);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Общие сведения о лицензии";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(161, 81);
            this.textBoxDescription.MaxLength = 500;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(220, 21);
            this.textBoxDescription.TabIndex = 2;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 15);
            this.label7.TabIndex = 82;
            this.label7.Text = "Примечание";
            // 
            // numericUpDownInstallationsCount
            // 
            this.numericUpDownInstallationsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownInstallationsCount.Location = new System.Drawing.Point(161, 51);
            this.numericUpDownInstallationsCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownInstallationsCount.Name = "numericUpDownInstallationsCount";
            this.numericUpDownInstallationsCount.Size = new System.Drawing.Size(220, 21);
            this.numericUpDownInstallationsCount.TabIndex = 1;
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
            this.label6.Location = new System.Drawing.Point(7, 54);
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
            this.comboBoxLicTypeID.Location = new System.Drawing.Point(161, 22);
            this.comboBoxLicTypeID.Name = "comboBoxLicTypeID";
            this.comboBoxLicTypeID.Size = new System.Drawing.Size(220, 23);
            this.comboBoxLicTypeID.TabIndex = 0;
            this.comboBoxLicTypeID.SelectedIndexChanged += new System.EventHandler(this.comboBoxLicTypeID_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 25);
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
