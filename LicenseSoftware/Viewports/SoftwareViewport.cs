using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.DataModels.CalcDataModels;
using LicenseSoftware.DataModels.DataModels;
using LicenseSoftware.Viewport.SearchForms;

namespace LicenseSoftware.Viewport
{
    internal sealed class SoftwareViewport: Viewport
    {
        #region Components
        private TableLayoutPanel tableLayoutPanel14;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftware;
        private DataGridViewTextBoxColumn software;
        private DataGridViewTextBoxColumn idSoftType;
        private DataGridViewTextBoxColumn idSoftMaker;
        private Panel panel1;
        private TextBox textBoxSoftwareName;
        private Label label86;
        private Panel panel2;
        private ComboBox comboBoxSoftType;
        private Label label84;
        private Panel panel3;
        private Label label1;
        private ComboBox comboBoxSoftMaker;
        private GroupBox groupBox32;
        private TableLayoutPanel tableLayoutPanel1;
        #endregion Components

        #region Models

        private SoftwareDataModel _softwareDm;
        private SoftTypesDataModel _softTypes;
        private SoftMakersDataModel _softMakers;
        #endregion Models

        #region Views

        private BindingSource _vSoftware;
        private BindingSource _vSoftTypes;
        private BindingSource _vSoftMakers;
        #endregion Views

        //State
        private ViewportState _viewportState = ViewportState.ReadState;
        private bool _isEditable;


        private SearchForm _sSearchForm;

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
            DynamicFilter = softwareViewport.DynamicFilter;
            StaticFilter = softwareViewport.StaticFilter;
            ParentRow = softwareViewport.ParentRow;
            ParentType = softwareViewport.ParentType;
        }

        private void SetViewportCaption()
        {
            if (_viewportState == ViewportState.NewRowState)
                Text = @"Новое программное обеспечения";
            else
                if (_vSoftware.Position != -1)
                    Text = string.Format(CultureInfo.InvariantCulture, "Программное обеспечения №{0}", 
                        ((DataRowView)_vSoftware[_vSoftware.Position])["ID Software"]);
                else
                    Text = @"Исковые работы отсутствуют";
        }

        private void DataBind()
        {
            comboBoxSoftType.DataSource = _vSoftTypes;
            comboBoxSoftType.ValueMember = "ID SoftType";
            comboBoxSoftType.DisplayMember = "SoftType";
            comboBoxSoftType.DataBindings.Clear();
            comboBoxSoftType.DataBindings.Add("SelectedValue", _vSoftware, "ID SoftType", true, DataSourceUpdateMode.Never, DBNull.Value);

            comboBoxSoftMaker.DataSource = _vSoftMakers;
            comboBoxSoftMaker.ValueMember = "ID SoftMaker";
            comboBoxSoftMaker.DisplayMember = "SoftMaker";
            comboBoxSoftMaker.DataBindings.Clear();
            comboBoxSoftMaker.DataBindings.Add("SelectedValue", _vSoftware, "ID SoftMaker", true, DataSourceUpdateMode.Never, DBNull.Value);
            
            textBoxSoftwareName.DataBindings.Clear();
            textBoxSoftwareName.DataBindings.Add("Text", _vSoftware, "Software", true, DataSourceUpdateMode.Never, "");
        }

        private void CheckViewportModifications()
        {
            if (!_isEditable)
                return;
            if (!ContainsFocus || dataGridView.Focused)
                return;
            if ((_vSoftware.Position != -1) && (SoftwareFromView() != SoftwareFromViewport()))
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
            if (!AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite))
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
                            switch (result)
                            {
                                case DialogResult.Yes:
                                    SaveRecord();
                                    break;
                                case DialogResult.No:
                                    CancelRecord();
                                    break;
                                default:
                                    return false;
                            }
                            return _viewportState == ViewportState.ReadState;
                    }
                    break;
                case ViewportState.NewRowState:
                    switch (_viewportState)
                    {
                        case ViewportState.ReadState:
                            if (_softwareDm.EditingNewRecord)
                                return false;
                            _viewportState = ViewportState.NewRowState;
                            return true;
                        case ViewportState.NewRowState:
                            return true;
                        case ViewportState.ModifyRowState:
                            var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            switch (result)
                            {
                                case DialogResult.Yes:
                                    SaveRecord();
                                    break;
                                case DialogResult.No:
                                    CancelRecord();
                                    break;
                                default:
                                    return false;
                            }
                            return _viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.NewRowState);
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
                            switch (result)
                            {
                                case DialogResult.Yes:
                                    SaveRecord();
                                    break;
                                case DialogResult.No:
                                    CancelRecord();
                                    break;
                                default:
                                    return false;
                            }
                            return _viewportState == ViewportState.ReadState && ChangeViewportStateTo(ViewportState.ModifyRowState);
                    }
                    break;
            }
            return false;
        }

        private void LocateSoftware(int id)
        {
            var position = _vSoftware.Find("ID Software", id);
            _isEditable = false;
            if (position > 0)
                _vSoftware.Position = position;
            _isEditable = true;
        }

        private void ViewportFromSoftware(Software software)
        {
            comboBoxSoftType.SelectedValue = ViewportHelper.ValueOrDbNull(software.IdSoftType);
            comboBoxSoftMaker.SelectedValue = ViewportHelper.ValueOrDbNull(software.IdSoftMaker);
            textBoxSoftwareName.Text = software.SoftwareName;
        }

        private Software SoftwareFromViewport()
        {
            var software = new Software
            {
                IdSoftware = _vSoftware.Position == -1
                    ? null
                    : ViewportHelper.ValueOrNull<int>((DataRowView) _vSoftware[_vSoftware.Position], "ID Software"),
                IdSoftType = ViewportHelper.ValueOrNull<int>(comboBoxSoftType),
                IdSoftMaker = ViewportHelper.ValueOrNull<int>(comboBoxSoftMaker),
                SoftwareName = ViewportHelper.ValueOrNull(textBoxSoftwareName)
            };
            return software;
        }

        private Software SoftwareFromView()
        {
            var row = (DataRowView)_vSoftware[_vSoftware.Position];
            var software = new Software
            {
                IdSoftware = ViewportHelper.ValueOrNull<int>(row, "ID Software"),
                IdSoftType = ViewportHelper.ValueOrNull<int>(row, "ID SoftType"),
                IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "ID SoftMaker"),
                SoftwareName = ViewportHelper.ValueOrNull(row, "Software")
            };
            return software;
        }

        private static void FillRowFromSoftware(Software software, DataRowView row)
        {
            row.BeginEdit();
            row["ID Software"] = ViewportHelper.ValueOrDbNull(software.IdSoftware);
            row["ID SoftType"] = ViewportHelper.ValueOrDbNull(software.IdSoftType);
            row["ID SoftMaker"] = ViewportHelper.ValueOrDbNull(software.IdSoftMaker);
            row["Software"] = ViewportHelper.ValueOrDbNull(software.SoftwareName);
            row.EndEdit();
        }

        private bool ValidateSoftware(Software software)
        {
            if (software.IdSoftType == null)
            {
                MessageBox.Show(@"Необходимо выбрать вид программного обеспечения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftType.Focus();
                return false;
            }
            if (software.IdSoftMaker == null)
            {
                MessageBox.Show(@"Необходимо выбрать разработчика программного обеспечения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                comboBoxSoftMaker.Focus();
                return false;
            }
            if (software.SoftwareName == null)
            {
                MessageBox.Show(@"Наименование программного обеспечения не может быть пустым", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                textBoxSoftwareName.Focus();
                return false;
            }
            return true;
        }

        public override int GetRecordCount()
        {
            return _vSoftware.Count;
        }

        public override void MoveFirst()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftware.MoveFirst();
            _isEditable = true;
        }

        public override void MoveLast()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftware.MoveLast();
            _isEditable = true;
        }

        public override void MoveNext()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftware.MoveNext();
            _isEditable = true;
        }

        public override void MovePrev()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            _isEditable = false;
            _vSoftware.MovePrevious();
            _isEditable = true;
        }

        public override bool CanMoveFirst()
        {
            return _vSoftware.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSoftware.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSoftware.Position > -1) && (_vSoftware.Position < _vSoftware.Count - 1);
        }

        public override bool CanMoveLast()
        {
            return (_vSoftware.Position > -1) && (_vSoftware.Position < _vSoftware.Count - 1);
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softwareDm = SoftwareDataModel.GetInstance();
            _softTypes = SoftTypesDataModel.GetInstance();
            _softMakers = SoftMakersDataModel.GetInstance();

            // Ожидаем дозагрузки, если это необходимо
            _softwareDm.Select();
            _softTypes.Select();
            _softMakers.Select();

            var ds = DataSetManager.DataSet;

            _vSoftTypes = new BindingSource
            {
                DataMember = "SoftTypes",
                DataSource = ds
            };

            _vSoftMakers = new BindingSource
            {
                DataMember = "SoftMakers",
                DataSource = ds
            };

            _vSoftware = new BindingSource();
            _vSoftware.CurrentItemChanged += v_software_CurrentItemChanged;
            _vSoftware.DataMember = "Software";
            _vSoftware.DataSource = ds;
            _vSoftware.Filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                _vSoftware.Filter += " AND ";
            _vSoftware.Filter += DynamicFilter;

            DataBind();

            _softwareDm.Select().RowChanged += SoftwareViewport_RowChanged;
            _softwareDm.Select().RowDeleted += SoftwareViewport_RowDeleted;

            dataGridView.RowCount = _vSoftware.Count;
            SetViewportCaption();
            ViewportHelper.SetDoubleBuffered(dataGridView);
            _isEditable = true;
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
                _sSearchForm = new SearchSoftwareForm();
            if (_sSearchForm.ShowDialog() != DialogResult.OK)
                return;
            DynamicFilter = _sSearchForm.GetFilter();
            var filter = StaticFilter;
            if (!string.IsNullOrEmpty(StaticFilter) && !string.IsNullOrEmpty(DynamicFilter))
                filter += " AND ";
            filter += DynamicFilter;
            dataGridView.RowCount = 0;
            _vSoftware.Filter = filter;
            dataGridView.RowCount = _vSoftware.Count;
        }

        public override void ClearSearch()
        {
            _vSoftware.Filter = StaticFilter;
            dataGridView.RowCount = _vSoftware.Count;
            DynamicFilter = "";
            MenuCallback.EditingStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.RelationsStateUpdate();
            MenuCallback.NavigationStateUpdate();
        }

        public override bool CanInsertRecord()
        {
            return !_softwareDm.EditingNewRecord && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            _isEditable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            _vSoftware.AddNew();
            dataGridView.Enabled = false;
            _isEditable = true;
            _softwareDm.EditingNewRecord = true;
        }

        public override bool CanCopyRecord()
        {
            return (_vSoftware.Position != -1) && !_softwareDm.EditingNewRecord
                && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void CopyRecord()
        {
            if (!ChangeViewportStateTo(ViewportState.NewRowState))
                return;
            _isEditable = false;
            dataGridView.RowCount = dataGridView.RowCount + 1;
            var software = SoftwareFromView();
            _vSoftware.AddNew();
            dataGridView.Enabled = false;
            _softwareDm.EditingNewRecord = true;
            ViewportFromSoftware(software);
            _isEditable = true;
        }

        public override void DeleteRecord()
        {
            if (MessageBox.Show(@"Вы действительно хотите удалить эту запись?", @"Внимание", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (SoftwareDataModel.Delete((int)((DataRowView)_vSoftware.Current)["ID Software"]) == -1)
                    return;
                _isEditable = false;
                ((DataRowView)_vSoftware[_vSoftware.Position]).Delete();
                _isEditable = true;
                _viewportState = ViewportState.ReadState;
                MenuCallback.EditingStateUpdate();
                MenuCallback.ForceCloseDetachedViewports();
                if (CalcDataModelSoftwareConcat.HasInstance())
                    CalcDataModelSoftwareConcat.GetInstance().Refresh(EntityType.Software, (int)((DataRowView)_vSoftware.Current)["ID Software"], true);
            }
        }

        public override bool CanDeleteRecord()
        {
            return (_vSoftware.Position > -1)
                && (_viewportState != ViewportState.NewRowState)
                && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new SoftwareViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            if (_vSoftware.Count > 0)
                viewport.LocateSoftware(((DataRowView)_vSoftware[_vSoftware.Position])["ID Software"] as int? ?? -1);
            return viewport;
        }

        public override bool CanCancelRecord()
        {
            return (_viewportState == ViewportState.NewRowState) || (_viewportState == ViewportState.ModifyRowState);
        }

        public override bool CanSaveRecord()
        {
            return ((_viewportState == ViewportState.NewRowState) || (_viewportState == ViewportState.ModifyRowState))
                && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            var software = SoftwareFromViewport();
            if (!ValidateSoftware(software))
                return;
            switch (_viewportState)
            {
                case ViewportState.ReadState:
                    MessageBox.Show(@"Нельзя сохранить неизмененные данные. Если вы видите это сообщение, обратитесь к системному администратору", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    break;
                case ViewportState.NewRowState:
                    var idSoftware = SoftwareDataModel.Insert(software);
                    if (idSoftware == -1)
                        return;
                    DataRowView newRow;
                    software.IdSoftware = idSoftware;
                    _isEditable = false;
                    if (_vSoftware.Position == -1)
                        newRow = (DataRowView)_vSoftware.AddNew();
                    else
                        newRow = (DataRowView)_vSoftware[_vSoftware.Position];
                    FillRowFromSoftware(software, newRow);
                    _softwareDm.EditingNewRecord = false;
                    _isEditable = true;
                    break;
                case ViewportState.ModifyRowState:
                    if (software.IdSoftware == null)
                    {
                        MessageBox.Show(@"Вы пытаетесь изменить запись о программном обеспечении без внутренного номера. " +
                            @"Если вы видите это сообщение, обратитесь к системному администратору", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    if (SoftwareDataModel.Update(software) == -1)
                        return;
                    var row = (DataRowView)_vSoftware[_vSoftware.Position];
                    _isEditable = false;
                    FillRowFromSoftware(software, row);
                    break;
            }
            dataGridView.Enabled = true;
            _isEditable = true;
            dataGridView.RowCount = _vSoftware.Count;
            _viewportState = ViewportState.ReadState;
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
            if (CalcDataModelSoftwareConcat.HasInstance())
                CalcDataModelSoftwareConcat.GetInstance().Refresh(EntityType.Software, software.IdSoftware.Value, true);
        }

        public override void CancelRecord()
        {
            switch (_viewportState)
            {
                case ViewportState.ReadState: return;
                case ViewportState.NewRowState:
                    _softwareDm.EditingNewRecord = false;
                    if (_vSoftware.Position != -1)
                    {
                        _isEditable = false;
                        dataGridView.Enabled = true;
                        ((DataRowView)_vSoftware[_vSoftware.Position]).Delete();
                        dataGridView.RowCount = dataGridView.RowCount - 1;
                        if (_vSoftware.Position != -1)
                            dataGridView.Rows[_vSoftware.Position].Selected = true;
                    }
                    _viewportState = ViewportState.ReadState;
                    break;
                case ViewportState.ModifyRowState:
                    dataGridView.Enabled = true;
                    _isEditable = false;
                    DataBind();
                    _viewportState = ViewportState.ReadState;
                    break;
            }
            _isEditable = true; 
            MenuCallback.EditingStateUpdate();
            SetViewportCaption();
        }

        public override bool HasAssocLicenses()
        {
            return (_vSoftware.Position > -1) &&
                AccessControl.HasPrivelege(Priveleges.LicensesRead);
        }

        public override void ShowAssocLicenses()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (_vSoftware.Position == -1)
            {
                MessageBox.Show(@"Не выбрано программное обеспечение для отображения списка лицензий", @"Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, ViewportType.LicensesViewport,
                "[ID Software] = " + Convert.ToInt32(((DataRowView)_vSoftware[_vSoftware.Position])["ID Software"], CultureInfo.InvariantCulture),
                ((DataRowView)_vSoftware[_vSoftware.Position]).Row,
                ParentTypeEnum.Software);
        }

        public override bool HasAssocSoftVersions()
        {
            return _vSoftware.Position > -1;
        }

        public override void ShowAssocSoftVersions()
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                return;
            if (_vSoftware.Position == -1)
            {
                MessageBox.Show(@"Не выбрано программное обеспечение для отображения списка версий", @"Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            ShowAssocViewport(MenuCallback, ViewportType.SoftVersionsViewport,
                "[ID Software] = " + Convert.ToInt32(((DataRowView)_vSoftware[_vSoftware.Position])["ID Software"], CultureInfo.InvariantCulture),
                ((DataRowView)_vSoftware[_vSoftware.Position]).Row,
                ParentTypeEnum.Software);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!ChangeViewportStateTo(ViewportState.ReadState))
                e.Cancel = true;
            _softwareDm.Select().RowChanged -= SoftwareViewport_RowChanged;
            _softwareDm.Select().RowDeleted -= SoftwareViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override void ForceClose()
        {
            if (_viewportState == ViewportState.NewRowState)
                _softwareDm.EditingNewRecord = false;
            _softwareDm.Select().RowChanged -= SoftwareViewport_RowChanged;
            _softwareDm.Select().RowDeleted -= SoftwareViewport_RowDeleted;
            Close();
        }

        private void SoftwareViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action != DataRowAction.Delete) return;
            dataGridView.RowCount = _vSoftware.Count;
            dataGridView.Refresh();
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void SoftwareViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change || e.Action == DataRowAction.ChangeCurrentAndOriginal || e.Action == DataRowAction.ChangeOriginal)
                dataGridView.Refresh();
            dataGridView.RowCount = _vSoftware.Count;
            if (Selected)
                MenuCallback.StatusBarStateUpdate();
        }

        private void v_software_CurrentItemChanged(object sender, EventArgs e)
        {
            SetViewportCaption();
            if (_vSoftware.Position == -1 || dataGridView.RowCount == 0)
                dataGridView.ClearSelection();
            else
                if (_vSoftware.Position >= dataGridView.RowCount)
                {
                    dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
                    dataGridView.CurrentCell = dataGridView.Rows[dataGridView.RowCount - 1].Cells[1];
                }
                else
                    if (dataGridView.Rows[_vSoftware.Position].Selected != true)
                    {
                        dataGridView.Rows[_vSoftware.Position].Selected = true;
                        dataGridView.CurrentCell = dataGridView.Rows[_vSoftware.Position].Cells[1];
                    }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
            if (_vSoftware.Position == -1)
                return;
            if (_viewportState == ViewportState.NewRowState)
                return;
            dataGridView.Enabled = true;
            _viewportState = ViewportState.ReadState;
            _isEditable = true;
        }

        private void comboBoxSoftType_SelectedValueChanged(object sender, EventArgs e)
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

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_vSoftware.Count <= e.RowIndex) return;
            switch (dataGridView.Columns[e.ColumnIndex].Name)
            {
                case "idSoftware":
                    e.Value = ((DataRowView)_vSoftware[e.RowIndex])["ID Software"];
                    break;
                case "software":
                    e.Value = ((DataRowView)_vSoftware[e.RowIndex])["Software"];
                    break;
                case "version":
                    e.Value = ((DataRowView)_vSoftware[e.RowIndex])["Version"];
                    break;
                case "idSoftType":
                    var rowIndex = _vSoftTypes.Find("ID SoftType", ((DataRowView)_vSoftware[e.RowIndex])["ID SoftType"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSoftTypes[rowIndex])["SoftType"];
                    break;
                case "idSoftMaker":
                    rowIndex = _vSoftMakers.Find("ID SoftMaker", ((DataRowView)_vSoftware[e.RowIndex])["ID SoftMaker"]);
                    if (rowIndex != -1)
                        e.Value = ((DataRowView)_vSoftMakers[rowIndex])["SoftMaker"];
                    break;
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
                _vSoftware.Position = dataGridView.SelectedRows[0].Index;
            else
                _vSoftware.Position = -1;
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;
            Func<SortOrder, bool> changeSortColumn = (way) =>
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                _vSoftware.Sort = dataGridView.Columns[e.ColumnIndex].Name + " " + (way == SortOrder.Ascending ? "ASC" : "DESC");
                dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = way;
                return true;
            };
            changeSortColumn(dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending);
            dataGridView.Refresh();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftwareViewport));
            tableLayoutPanel14 = new TableLayoutPanel();
            groupBox32 = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel2 = new Panel();
            comboBoxSoftType = new ComboBox();
            label84 = new Label();
            panel3 = new Panel();
            label1 = new Label();
            comboBoxSoftMaker = new ComboBox();
            panel1 = new Panel();
            textBoxSoftwareName = new TextBox();
            label86 = new Label();
            dataGridView = new DataGridView();
            idSoftware = new DataGridViewTextBoxColumn();
            software = new DataGridViewTextBoxColumn();
            idSoftType = new DataGridViewTextBoxColumn();
            idSoftMaker = new DataGridViewTextBoxColumn();
            tableLayoutPanel14.SuspendLayout();
            groupBox32.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.ColumnCount = 1;
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel14.Controls.Add(groupBox32, 0, 0);
            tableLayoutPanel14.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel14.Dock = DockStyle.Fill;
            tableLayoutPanel14.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.RowCount = 2;
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel14.Size = new System.Drawing.Size(1002, 724);
            tableLayoutPanel14.TabIndex = 0;
            // 
            // groupBox32
            // 
            groupBox32.Controls.Add(tableLayoutPanel1);
            groupBox32.Dock = DockStyle.Fill;
            groupBox32.Location = new System.Drawing.Point(3, 3);
            groupBox32.Name = "groupBox32";
            groupBox32.Size = new System.Drawing.Size(996, 88);
            groupBox32.TabIndex = 1;
            groupBox32.TabStop = false;
            groupBox32.Text = "Сведения о программном обеспечении";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(panel2, 0, 1);
            tableLayoutPanel1.Controls.Add(panel3, 0, 1);
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new System.Drawing.Size(990, 68);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(comboBoxSoftType);
            panel2.Controls.Add(label84);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new System.Drawing.Point(0, 34);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(495, 34);
            panel2.TabIndex = 76;
            // 
            // comboBoxSoftType
            // 
            comboBoxSoftType.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left 
                                                     | AnchorStyles.Right);
            comboBoxSoftType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSoftType.FormattingEnabled = true;
            comboBoxSoftType.Location = new System.Drawing.Point(112, 5);
            comboBoxSoftType.Name = "comboBoxSoftType";
            comboBoxSoftType.Size = new System.Drawing.Size(380, 23);
            comboBoxSoftType.TabIndex = 0;
            comboBoxSoftType.SelectedValueChanged += comboBoxSoftType_SelectedValueChanged;
            // 
            // label84
            // 
            label84.AutoSize = true;
            label84.Location = new System.Drawing.Point(10, 9);
            label84.Name = "label84";
            label84.Size = new System.Drawing.Size(50, 15);
            label84.TabIndex = 71;
            label84.Text = "Вид ПО";
            // 
            // panel3
            // 
            panel3.Controls.Add(label1);
            panel3.Controls.Add(comboBoxSoftMaker);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new System.Drawing.Point(495, 34);
            panel3.Margin = new Padding(0);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(495, 34);
            panel3.TabIndex = 75;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(103, 15);
            label1.TabIndex = 73;
            label1.Text = "Разработчик ПО";
            // 
            // comboBoxSoftMaker
            // 
            comboBoxSoftMaker.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left 
                                                      | AnchorStyles.Right);
            comboBoxSoftMaker.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSoftMaker.FormattingEnabled = true;
            comboBoxSoftMaker.Location = new System.Drawing.Point(109, 5);
            comboBoxSoftMaker.Name = "comboBoxSoftMaker";
            comboBoxSoftMaker.Size = new System.Drawing.Size(380, 23);
            comboBoxSoftMaker.TabIndex = 1;
            comboBoxSoftMaker.SelectedValueChanged += comboBoxSoftMaker_SelectedValueChanged;
            // 
            // panel1
            // 
            tableLayoutPanel1.SetColumnSpan(panel1, 2);
            panel1.Controls.Add(textBoxSoftwareName);
            panel1.Controls.Add(label86);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(990, 34);
            panel1.TabIndex = 0;
            // 
            // textBoxSoftwareName
            // 
            textBoxSoftwareName.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left 
                                                        | AnchorStyles.Right);
            textBoxSoftwareName.Location = new System.Drawing.Point(112, 8);
            textBoxSoftwareName.MaxLength = 500;
            textBoxSoftwareName.Name = "textBoxSoftwareName";
            textBoxSoftwareName.Size = new System.Drawing.Size(871, 21);
            textBoxSoftwareName.TabIndex = 0;
            textBoxSoftwareName.TextChanged += textBoxSoftwareName_TextChanged;
            // 
            // label86
            // 
            label86.AutoSize = true;
            label86.Location = new System.Drawing.Point(10, 10);
            label86.Name = "label86";
            label86.Size = new System.Drawing.Size(95, 15);
            label86.TabIndex = 73;
            label86.Text = "Наименование";
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)204);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] {
            idSoftware,
            software,
            idSoftType,
            idSoftMaker});
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new System.Drawing.Point(3, 97);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(996, 624);
            dataGridView.TabIndex = 0;
            dataGridView.VirtualMode = true;
            dataGridView.CellValueNeeded += dataGridView_CellValueNeeded;
            dataGridView.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridView.DataError += dataGridView_DataError;
            dataGridView.SelectionChanged += dataGridView_SelectionChanged;
            // 
            // idSoftware
            // 
            idSoftware.Frozen = true;
            idSoftware.HeaderText = "Идентификатор";
            idSoftware.Name = "idSoftware";
            idSoftware.ReadOnly = true;
            idSoftware.Visible = false;
            // 
            // software
            // 
            software.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            software.HeaderText = "Наименование ПО";
            software.MinimumWidth = 300;
            software.Name = "software";
            software.ReadOnly = true;
            // 
            // idSoftType
            // 
            idSoftType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            idSoftType.HeaderText = "Вид ПО";
            idSoftType.MinimumWidth = 200;
            idSoftType.Name = "idSoftType";
            idSoftType.ReadOnly = true;
            idSoftType.Resizable = DataGridViewTriState.True;
            idSoftType.SortMode = DataGridViewColumnSortMode.NotSortable;
            idSoftType.Width = 200;
            // 
            // idSoftMaker
            // 
            idSoftMaker.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            idSoftMaker.HeaderText = "Разработчик ПО";
            idSoftMaker.MinimumWidth = 200;
            idSoftMaker.Name = "idSoftMaker";
            idSoftMaker.ReadOnly = true;
            idSoftMaker.Resizable = DataGridViewTriState.True;
            idSoftMaker.SortMode = DataGridViewColumnSortMode.NotSortable;
            idSoftMaker.Width = 200;
            // 
            // SoftwareViewport
            // 
            AutoScroll = true;
            AutoScrollMinSize = new System.Drawing.Size(800, 300);
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(1008, 730);
            Controls.Add(tableLayoutPanel14);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)204);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "SoftwareViewport";
            Padding = new Padding(3);
            Text = "Программное обеспечение";
            tableLayoutPanel14.ResumeLayout(false);
            groupBox32.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);

        }
    }
}
