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
    internal sealed class SoftwareViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView dataGridView;
        #endregion Components

        #region Models
        SoftwareDataModel softwareDM = null;
        SoftTypesDataModel softTypes = null;
        SoftMakersDataModel softMakers = null;
        #endregion Models

        #region Views
        BindingSource v_software = null;
        BindingSource v_softTypes = null;
        BindingSource v_softMakers = null;
        #endregion Views

        //State
        private ViewportState viewportState = ViewportState.ReadState;
        private GroupBox groupBox32;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private TextBox textBoxSoftwareName;
        private Label label87;
        private Label label86;
        private TextBox textBoxVersion;
        private Panel panel2;
        private ComboBox comboBoxSoftMaker;
        private Label label1;
        private ComboBox comboBoxSoftType;
        private Label label84;
        private bool is_editable = false;
        private DataGridViewTextBoxColumn idSoftware;
        private DataGridViewTextBoxColumn software;
        private DataGridViewTextBoxColumn version;
        private DataGridViewTextBoxColumn idSoftType;
        private DataGridViewTextBoxColumn idSoftMaker;


        private SearchForm sSearchForm = null;

        private SoftwareViewport()
            : this(null)
        {
        }

        public SoftwareViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
        }

        public SoftwareViewport(SoftwareViewport softwareViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = softwareViewport.DynamicFilter;
            this.StaticFilter = softwareViewport.StaticFilter;
            this.ParentRow = softwareViewport.ParentRow;
            this.ParentType = softwareViewport.ParentType;
        }

        private void SetViewportCaption()
        {
            if (viewportState == ViewportState.NewRowState)
                this.Text = "Новое программное обеспечения";
            else
                if (v_software.Position != -1)
                    this.Text = String.Format(CultureInfo.InvariantCulture, "Программное обеспечения №{0}", 
                        ((DataRowView)v_software[v_software.Position])["ID Software"]);
                else
                    this.Text = "Исковые работы отсутствуют";
        }

        private void DataBind()
        {
            comboBoxSoftType.DataSource = v_softTypes;
            comboBoxSoftType.ValueMember = "ID SoftType";
            comboBoxSoftType.DisplayMember = "SoftType";
            comboBoxSoftType.DataBindings.Clear();
            comboBoxSoftType.DataBindings.Add("SelectedValue", v_software, "ID SoftType", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxSoftMaker.DataSource = v_softMakers;
            comboBoxSoftMaker.ValueMember = "ID SoftMaker";
            comboBoxSoftMaker.DisplayMember = "SoftMaker";
            comboBoxSoftMaker.DataBindings.Clear();
            comboBoxSoftMaker.DataBindings.Add("SelectedValue", v_software, "ID SoftMaker", true, DataSourceUpdateMode.Never, DBNull.Value);
            
            textBoxSoftwareName.DataBindings.Clear();
            textBoxSoftwareName.DataBindings.Add("Text", v_software, "Software", true, DataSourceUpdateMode.Never, "");
            textBoxVersion.DataBindings.Clear();
            textBoxVersion.DataBindings.Add("Text", v_software, "Version", true, DataSourceUpdateMode.Never, "");
        }

        private void CheckViewportModifications()
        {
            if (!is_editable)
                return;
            if ((!this.ContainsFocus) || (dataGridView.Focused))
                return;
            if ((v_software.Position != -1) && (SoftwareFromView() != SoftwareFromViewport()))
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
                            if (softwareDM.EditingNewRecord)
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

        private void LocateSoftware(int id)
        {
            int Position = v_software.Find("ID Software", id);
            is_editable = false;
            if (Position > 0)
                v_software.Position = Position;
            is_editable = true;
        }

        private void ViewportFromSoftware(Software software)
        {
            comboBoxSoftType.SelectedValue = ViewportHelper.ValueOrDBNull(software.IdSoftType);
            comboBoxSoftMaker.SelectedValue = ViewportHelper.ValueOrDBNull(software.IdSoftMaker);
            textBoxSoftwareName.Text = software.SoftwareName;
            textBoxVersion.Text = software.Version;
        }

        private Software SoftwareFromViewport()
        {
            Software software = new Software();
            if (v_software.Position == -1)
                software.IdSoftware = null;
            else
                software.IdSoftware = ViewportHelper.ValueOrNull<int>((DataRowView)v_software[v_software.Position], "ID Software");
            software.IdSoftType = ViewportHelper.ValueOrNull<int>(comboBoxSoftType);
            software.IdSoftMaker = ViewportHelper.ValueOrNull<int>(comboBoxSoftMaker);
            software.Version = ViewportHelper.ValueOrNull(textBoxVersion);
            software.SoftwareName = ViewportHelper.ValueOrNull(textBoxSoftwareName);
            return software;
        }

        private Software SoftwareFromView()
        {
            Software software = new Software();
            DataRowView row = (DataRowView)v_software[v_software.Position];
            software.IdSoftware = ViewportHelper.ValueOrNull<int>(row, "ID Software");
            software.IdSoftType = ViewportHelper.ValueOrNull<int>(row, "ID SoftType");
            software.IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "ID SoftMaker");
            software.SoftwareName = ViewportHelper.ValueOrNull(row, "Software");
            software.Version = ViewportHelper.ValueOrNull(row, "Version");
            return software;
        }

        private static void FillRowFromSoftware(Software software, DataRowView row)
        {
            row.BeginEdit();
            row.BeginEdit();
            row["ID Software"] = ViewportHelper.ValueOrDBNull(software.IdSoftware);
            row["ID SoftType"] = ViewportHelper.ValueOrDBNull(software.IdSoftType);
            row["ID SoftMaker"] = ViewportHelper.ValueOrDBNull(software.IdSoftMaker);
            row["Software"] = ViewportHelper.ValueOrDBNull(software.SoftwareName);
            row["Version"] = ViewportHelper.ValueOrDBNull(software.Version);
            row.EndEdit();
        }

        private bool ValidateSoftware(Software software)
        {
            if (software.IdSoftType == null)
            {
                MessageBox.Show("Необходимо выбрать вид программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftType.Focus();
                return false;
            }
            if (software.IdSoftMaker == null)
            {
                MessageBox.Show("Необходимо выбрать разработчика программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftMaker.Focus();
                return false;
            }
            if (software.SoftwareName == null)
            {
                MessageBox.Show("Наименование программного обеспечения не может быть пустым", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSoftwareName.Focus();
                return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return v_software.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_software.MoveFirst();
            is_editable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_software.MoveLast();
            is_editable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_software.MoveNext();
            is_editable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            is_editable = false;
            v_software.MovePrevious();
            is_editable = true;
        }

        public override bool CanMoveFirst()
        {
            return v_software.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_software.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_software.Position > -1) && (v_software.Position < (v_software.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_software.Position > -1) && (v_software.Position < (v_software.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softwareDM = SoftwareDataModel.GetInstance();
            softTypes = SoftTypesDataModel.GetInstance();
            softMakers = SoftMakersDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            softwareDM.Select();
            softTypes.Select();
            softMakers.Select();

            DataSet ds = DataSetManager.DataSet;

            v_softTypes = new BindingSource();
            v_softTypes.DataMember = "SoftTypes";
            v_softTypes.DataSource = ds;

            v_softMakers = new BindingSource();
            v_softMakers.DataMember = "SoftMakers";
            v_softMakers.DataSource = ds;

            v_software = new BindingSource();
            v_software.CurrentItemChanged += new EventHandler(v_software_CurrentItemChanged);
            v_software.DataMember = "Software";
            v_software.DataSource = ds;
            v_software.Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_software.Filter += " AND ";
            v_software.Filter += DynamicFilter;

            DataBind();

            softwareDM.Select().RowChanged += SoftwareViewport_RowChanged;
            softwareDM.Select().RowDeleted += SoftwareViewport_RowDeleted;

            dataGridView.RowCount = v_software.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridView);
            is_editable = true;
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
                sSearchForm = new SearchSoftwareForm();
            if (sSearchForm.ShowDialog() != DialogResult.OK)
                return;
            DynamicFilter = sSearchForm.GetFilter();
            string Filter = StaticFilter;
            if (!String.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                Filter += " AND ";
            Filter += DynamicFilter;
            dataGridView.RowCount = 0;
            v_software.Filter = Filter;
            dataGridView.RowCount = v_software.Count;
        }

        public override void ClearSearch()
        {
            v_software.Filter = StaticFilter;
            dataGridView.RowCount = v_software.Count;
            DynamicFilter = "";
            MenuCallback.EditingStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.NavigationStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return (!softwareDM.EditingNewRecord) && AccessControl.HasPrivelege(Priveleges.DIRECTORIES_READ_WRITE);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            v_software.AddNew();
            dataGridView.Enabled = false;
            is_editable = true;
            softwareDM.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (v_software.Position != -1) && (!softwareDM.EditingNewRecord)
                && AccessControl.HasPrivelege(Priveleges.DIRECTORIES_READ_WRITE);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            is_editable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            Software software = SoftwareFromView();
            v_software.AddNew();
            dataGridView.Enabled = false;
            softwareDM.EditingNewRecord = true;
            ViewportFromSoftware(software);
            is_editable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show("Вы действительно хотите удалить эту запись?", "Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (SoftwareDataModel.Delete((int)((DataRowView)v_software.Current)["ID Software"]) == -1)
                    return;
                is_editable = false;
                ((DataRowView)v_software[v_software.Position]).Delete();
                is_editable = true;
                viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                CalcDataModelSoftwareConcat.GetInstance().Refresh(EntityType.Software, 
                    (int)((DataRowView)v_software.Current)["ID Software"], true);
            }
        }

        public override bool CanDeleteRecord()
        {
            return (v_software.Position > -1)
                && (viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.DIRECTORIES_READ_WRITE);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            SoftwareViewport viewport = new SoftwareViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (v_software.Count > 0)
                viewport.LocateSoftware((((DataRowView)v_software[v_software.Position])["ID Software"] as Int32?) ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((viewportState == ViewportState.NewRowState) || (viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.DIRECTORIES_READ_WRITE);
        }

        public override void SaveRecord()
        {
            Software software = SoftwareFromViewport();
            if (!ValidateSoftware(software))
                return;
            switch (viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show("Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    int idSoftware = SoftwareDataModel.Insert(software);
                    if (idSoftware == -1)
                        return;
                    DataRowView newRow;
                    software.IdSoftware = idSoftware;
                    is_editable = false;
                    if (v_software.Position == -1)
                        newRow = (DataRowView)v_software.AddNew();
                    else
                        newRow = ((DataRowView)v_software[v_software.Position]);
                    FillRowFromSoftware(software, newRow);
                    softwareDM.EditingNewRecord = false;
                    is_editable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (software.IdSoftware == null)
                    {
                        MessageBox.Show("Вы пытаетесь изменить запись о программном обеспечении без внутренного номера. " +
                            "Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (SoftwareDataModel.Update(software) == -1)
                        return;
                    DataRowView row = ((DataRowView)v_software[v_software.Position]);
                    is_editable = false;
                    FillRowFromSoftware(software, row);
                    break;
            }
            dataGridView.Enabled = true;
            is_editable = true;
            dataGridView.RowCount = v_software.Count;
            viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            if (CalcDataModelSoftwareConcat.HasInstance())
                CalcDataModelSoftwareConcat.GetInstance().Refresh(EntityType.Software, software.IdSoftware.Value, true);
        }

        public override void CancelRecord()
        {
            switch (viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    softwareDM.EditingNewRecord = false;
                    if (v_software.Position != -1)
                    {
                        is_editable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)v_software[v_software.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (v_software.Position != -1)
                            dataGridView.Rows[v_software.Position].Selected = true;
                    }
                    viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    is_editable = false;
                    DataBind();
                    viewportState = ViewportState.ReadState;
                    break;
            }
            is_editable = true; 
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool HasAssocLicenses()
        {
            return (v_software.Position > -1) &&
                AccessControl.HasPrivelege(Priveleges.LICENSES_READ);
        }

        public override void ShowAssocLicenses()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (v_software.Position == -1)
            {
                MessageBox.Show("Не выбрано программное обеспечение для отображения списка лицензий", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, ViewportType.LicensesViewport,
                "[ID Software] = " + Convert.ToInt32(((DataRowView)v_software[v_software.Position])["ID Software"], CultureInfo.InvariantCulture),
                ((DataRowView)v_software[v_software.Position]).Row,
                ParentTypeEnum.Software);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            softwareDM.Select().RowChanged -= SoftwareViewport_RowChanged;
            softwareDM.Select().RowDeleted -= SoftwareViewport_RowDeleted;
        }

        public override void ForceClose()
        {
            if (viewportState == ViewportState.NewRowState)
                softwareDM.EditingNewRecord = false;
            softwareDM.Select().RowChanged -= SoftwareViewport_RowChanged;
            softwareDM.Select().RowDeleted -= SoftwareViewport_RowDeleted;
            base.Close();
        }

        void SoftwareViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                dataGridView.RowCount = v_software.Count;
                dataGridView.Refresh();
                MenuCallback.ForceCloseDetachedViewports();
                if (Selected)
                    MenuCallback.StatusBarStateUpdate();
            }
        }

        void SoftwareViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = v_software.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        void v_software_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (v_software.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (v_software.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[1];
                }
                else
                    if (dataGridView.Rows[v_software.Position].Selected != true)
                    {
                        dataGridView.Rows[v_software.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[v_software.Position].Cells[1];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            if (v_software.Position == -1)
                return;
            if (viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            viewportState = ViewportState.ReadState;
            is_editable = true;
        }

        private void comboBoxSoftType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void comboBoxSoftMaker_SelectedValueChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void textBoxSoftwareName_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        private void textBoxVersion_TextChanged(object sender, EventArgs e)
        {
            CheckViewportModifications();
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (v_software.Count <= e.RowIndex) return;
            switch (this.dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "idSoftware":
                    e.Value = ((DataRowView)v_software[e.RowIndex])["ID Software"];
                    break;
                case "software":
                    e.Value = ((DataRowView)v_software[e.RowIndex])["Software"];
                    break;
                case "version":
                    e.Value = ((DataRowView)v_software[e.RowIndex])["Version"];
                    break;
                case "idSoftType":
                    int row_index = v_softTypes.Find("ID SoftType", ((DataRowView)v_software[e.RowIndex])["ID SoftType"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_softTypes[row_index])["SoftType"];
                    break;
                case "idSoftMaker":
                    row_index = v_softMakers.Find("ID SoftMaker", ((DataRowView)v_software[e.RowIndex])["ID SoftMaker"]);
                    if (row_index != -1)
                        e.Value = ((DataRowView)v_softMakers[row_index])["SoftMaker"];
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                v_software.Position = dataGridView.SelectedRows[0].Index;
            else
                v_software.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                v_software.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + ((way == SortOrder.Ascending) ? "ASC" : "DESC");
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftwareViewport));
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox32 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxSoftwareName = new System.Windows.Forms.TextBox();
            this.label87 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.textBoxVersion = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBoxSoftMaker = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxSoftType = new System.Windows.Forms.ComboBox();
            this.label84 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idSoftware = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.software = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idSoftType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idSoftMaker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel14.SuspendLayout();
            this.groupBox32.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel14.Controls.Add(this.groupBox32, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 2;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(818, 340);
            this.tableLayoutPanel14.TabIndex = 0;
            // 
            // groupBox32
            // 
            this.tableLayoutPanel14.SetColumnSpan(this.groupBox32, 2);
            this.groupBox32.Controls.Add(this.tableLayoutPanel1);
            this.groupBox32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox32.Location = new System.Drawing.Point(3, 3);
            this.groupBox32.Name = "groupBox32";
            this.groupBox32.Size = new System.Drawing.Size(812, 88);
            this.groupBox32.TabIndex = 0;
            this.groupBox32.TabStop = false;
            this.groupBox32.Text = "Сведения о программном обеспечении";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(806, 68);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxSoftwareName);
            this.panel1.Controls.Add(this.label87);
            this.panel1.Controls.Add(this.label86);
            this.panel1.Controls.Add(this.textBoxVersion);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 68);
            this.panel1.TabIndex = 0;
            // 
            // textBoxSoftwareName
            // 
            this.textBoxSoftwareName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSoftwareName.Location = new System.Drawing.Point(121, 8);
            this.textBoxSoftwareName.MaxLength = 500;
            this.textBoxSoftwareName.Name = "textBoxSoftwareName";
            this.textBoxSoftwareName.Size = new System.Drawing.Size(267, 21);
            this.textBoxSoftwareName.TabIndex = 0;
            this.textBoxSoftwareName.TextChanged += new System.EventHandler(this.textBoxSoftwareName_TextChanged);
            // 
            // label87
            // 
            this.label87.AutoSize = true;
            this.label87.Location = new System.Drawing.Point(10, 41);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(70, 15);
            this.label87.TabIndex = 74;
            this.label87.Text = "Версия ПО";
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(10, 10);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(95, 15);
            this.label86.TabIndex = 73;
            this.label86.Text = "Наименование";
            // 
            // textBoxVersion
            // 
            this.textBoxVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxVersion.Location = new System.Drawing.Point(121, 37);
            this.textBoxVersion.MaxLength = 50;
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.Size = new System.Drawing.Size(267, 21);
            this.textBoxVersion.TabIndex = 1;
            this.textBoxVersion.TextChanged += new System.EventHandler(this.textBoxVersion_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.comboBoxSoftMaker);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.comboBoxSoftType);
            this.panel2.Controls.Add(this.label84);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(403, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(403, 68);
            this.panel2.TabIndex = 1;
            // 
            // comboBoxSoftMaker
            // 
            this.comboBoxSoftMaker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSoftMaker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSoftMaker.FormattingEnabled = true;
            this.comboBoxSoftMaker.Location = new System.Drawing.Point(127, 37);
            this.comboBoxSoftMaker.Name = "comboBoxSoftMaker";
            this.comboBoxSoftMaker.Size = new System.Drawing.Size(266, 23);
            this.comboBoxSoftMaker.TabIndex = 1;
            this.comboBoxSoftMaker.SelectedValueChanged += new System.EventHandler(this.comboBoxSoftMaker_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 15);
            this.label1.TabIndex = 73;
            this.label1.Text = "Разработчик ПО";
            // 
            // comboBoxSoftType
            // 
            this.comboBoxSoftType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSoftType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSoftType.FormattingEnabled = true;
            this.comboBoxSoftType.Location = new System.Drawing.Point(127, 8);
            this.comboBoxSoftType.Name = "comboBoxSoftType";
            this.comboBoxSoftType.Size = new System.Drawing.Size(266, 23);
            this.comboBoxSoftType.TabIndex = 0;
            this.comboBoxSoftType.SelectedIndexChanged += new System.EventHandler(this.comboBoxSoftType_SelectedIndexChanged);
            // 
            // label84
            // 
            this.label84.AutoSize = true;
            this.label84.Location = new System.Drawing.Point(10, 10);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(50, 15);
            this.label84.TabIndex = 71;
            this.label84.Text = "Вид ПО";
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
            this.idSoftware,
            this.software,
            this.version,
            this.idSoftType,
            this.idSoftMaker});
            this.tableLayoutPanel14.SetColumnSpan(this.dataGridView, 2);
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 97);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(812, 240);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView_CellValueNeeded);
            this.dataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_ColumnHeaderMouseClick);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // idSoftware
            // 
            this.idSoftware.Frozen = true;
            this.idSoftware.HeaderText = "Идентификатор";
            this.idSoftware.Name = "idSoftware";
            this.idSoftware.ReadOnly = true;
            this.idSoftware.Visible = false;
            // 
            // software
            // 
            this.software.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.software.HeaderText = "Наименование ПО";
            this.software.MinimumWidth = 300;
            this.software.Name = "software";
            this.software.ReadOnly = true;
            // 
            // version
            // 
            this.version.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.version.HeaderText = "Версия ПО";
            this.version.MinimumWidth = 150;
            this.version.Name = "version";
            this.version.ReadOnly = true;
            this.version.Width = 150;
            // 
            // idSoftType
            // 
            this.idSoftType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.idSoftType.HeaderText = "Вид ПО";
            this.idSoftType.MinimumWidth = 150;
            this.idSoftType.Name = "idSoftType";
            this.idSoftType.ReadOnly = true;
            this.idSoftType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.idSoftType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.idSoftType.Width = 150;
            // 
            // idSoftMaker
            // 
            this.idSoftMaker.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.idSoftMaker.HeaderText = "Разработчик ПО";
            this.idSoftMaker.MinimumWidth = 150;
            this.idSoftMaker.Name = "idSoftMaker";
            this.idSoftMaker.ReadOnly = true;
            this.idSoftMaker.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.idSoftMaker.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.idSoftMaker.Width = 150;
            // 
            // SoftwareViewport
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(800, 300);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(824, 346);
            this.Controls.Add(this.tableLayoutPanel14);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftwareViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Программное обеспечение";
            this.tableLayoutPanel14.ResumeLayout(false);
            this.groupBox32.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
