using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.DataModels.DataModels;

namespace LicenseSoftware.Viewport
{
    internal sealed class SoftLicTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftLicType;
        private DataGridViewTextBoxColumn softLicType;
        private DataGridViewCheckBoxColumn softLicKeyDuplicateAllowed;
        #endregion Components

        #region Models

        private SoftLicTypesDataModel _softLicTypes;
        private readonly DataTable _snapshotSoftLicTypes = new DataTable("snapshotSoftLicTypes");
        #endregion Models

        #region Views

        private BindingSource _vSoftLicTypes;
        private BindingSource _vSnapshotSoftLicTypes;
        #endregion Models


        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftLicTypesViewport()
            : this(null)
        {
        }

        public SoftLicTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotSoftLicTypes.Locale = CultureInfo.InvariantCulture;
        }

        public SoftLicTypesViewport(SoftLicTypesViewport softLicTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softLicTypesViewport.DynamicFilter;
            StaticFilter = softLicTypesViewport.StaticFilter;
            ParentRow = softLicTypesViewport.ParentRow;
            ParentType = softLicTypesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = SoftLicTypesFromView();
            var listFromViewport = SoftLicTypesFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            for (var i = 0; i < listFromView.Count; i++)
            {
                var founded = false;
                for (var j = 0; j < listFromViewport.Count; j++)
                    if (listFromView[i] == listFromViewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new[] { 
                dataRowView["ID LicType"], 
                dataRowView["LicType"],
                ViewportHelper.ValueOrNull<bool>(dataRowView,"LicKeyDuplicateAllowed") == true
            };
        }

        private static bool ValidateViewportData(List<SoftLicType> list)
        {
            foreach (var softLicType in list)
            {
                if (softLicType.LicType == null)
                {
                    MessageBox.Show(@"Наименование вида лицензии на ПО не может быть пустым",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softLicType.LicType != null && softLicType.LicType.Length > 50)
                {
                    MessageBox.Show(@"Длина наименования вида лицензии не может превышать 50 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftLicType RowToSoftLicType(DataRow row)
        {
            var softLicType = new SoftLicType
            {
                IdLicType = ViewportHelper.ValueOrNull<int>(row, "ID LicType"),
                LicType = ViewportHelper.ValueOrNull(row, "LicType"),
                LicKeyDuplicateAllowed = ViewportHelper.ValueOrNull<bool>(row, "LicKeyDuplicateAllowed")
            };
            return softLicType;
        }

        private List<SoftLicType> SoftLicTypesFromViewport()
        {
            var list = new List<SoftLicType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var st = new SoftLicType();
                var row = dataGridView.Rows[i];
                st.IdLicType = ViewportHelper.ValueOrNull<int>(row, "idSoftLicType");
                st.LicType = ViewportHelper.ValueOrNull(row, "softLicType");
                st.LicKeyDuplicateAllowed = ViewportHelper.ValueOrNull<bool>(row, "softLicKeyDuplicateAllowed") == true;
                list.Add(st);
            }
            return list;
        }

        private List<SoftLicType> SoftLicTypesFromView()
        {
            var list = new List<SoftLicType>();
            for (var i = 0; i < _vSoftLicTypes.Count; i++)
            {
                var st = new SoftLicType();
                var row = (DataRowView)_vSoftLicTypes[i];
                st.IdLicType = ViewportHelper.ValueOrNull<int>(row, "ID LicType");
                st.LicType = ViewportHelper.ValueOrNull(row, "LicType");
                st.LicKeyDuplicateAllowed = ViewportHelper.ValueOrNull<bool>(row, "LicKeyDuplicateAllowed") == true;
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotSoftLicTypes.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softLicTypes = SoftLicTypesDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            _softLicTypes.Select();

            _vSoftLicTypes = new BindingSource
            {
                DataMember = "SoftLicTypes",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _softLicTypes.Select().Columns.Count; i++)
                _snapshotSoftLicTypes.Columns.Add(new DataColumn(
                    _softLicTypes.Select().Columns[i].ColumnName, _softLicTypes.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vSoftLicTypes.Count; i++)
                _snapshotSoftLicTypes.Rows.Add(DataRowViewToArray((DataRowView)_vSoftLicTypes[i]));
            _vSnapshotSoftLicTypes = new BindingSource {DataSource = _snapshotSoftLicTypes};
            _vSnapshotSoftLicTypes.CurrentItemChanged += v_snapshotSoftLicTypes_CurrentItemChanged;

            dataGridView.DataSource = _vSnapshotSoftLicTypes;
            idSoftLicType.DataPropertyName = "ID LicType";
            softLicType.DataPropertyName = "LicType";
            softLicKeyDuplicateAllowed.DataPropertyName = "LicKeyDuplicateAllowed";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _softLicTypes.Select().RowChanged += SoftLicTypesViewport_RowChanged;
            _softLicTypes.Select().RowDeleting += SoftLicTypesViewport_RowDeleting;
            _softLicTypes.Select().RowDeleted += SoftLicTypesViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            _vSnapshotSoftLicTypes.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotSoftLicTypes.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotSoftLicTypes.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotSoftLicTypes.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotSoftLicTypes.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotSoftLicTypes.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotSoftLicTypes.Position > -1) && (_vSnapshotSoftLicTypes.Position < (_vSnapshotSoftLicTypes.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotSoftLicTypes.Position > -1) && (_vSnapshotSoftLicTypes.Position < (_vSnapshotSoftLicTypes.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)_vSnapshotSoftLicTypes.AddNew();
            if (row != null) row.EndEdit();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                    SaveRecord();
                else
                    if (result == DialogResult.No)
                        CancelRecord();
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
            }
            _softLicTypes.Select().RowChanged -= SoftLicTypesViewport_RowChanged;
            _softLicTypes.Select().RowDeleting -= SoftLicTypesViewport_RowDeleting;
            _softLicTypes.Select().RowDeleted -= SoftLicTypesViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotSoftLicTypes.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotSoftLicTypes[_vSnapshotSoftLicTypes.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotSoftLicTypes.Clear();
            for (var i = 0; i < _vSoftLicTypes.Count; i++)
                _snapshotSoftLicTypes.Rows.Add(DataRowViewToArray(((DataRowView)_vSoftLicTypes[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            dataGridView.EndEdit();
            _syncViews = false;
            var list = SoftLicTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = _softLicTypes.Select().Rows.Find(list[i].IdLicType);
                if (row == null)
                {
                    var idSoftLicType = SoftLicTypesDataModel.Insert(list[i]);
                    if (idSoftLicType == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotSoftLicTypes[i])["ID LicType"] = idSoftLicType;
                    _softLicTypes.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotSoftLicTypes[i]));
                }
                else
                {

                    if (RowToSoftLicType(row) == list[i])
                        continue;
                    if (SoftLicTypesDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["LicType"] = list[i].LicType == null ? DBNull.Value : (object)list[i].LicType;
                    row["LicKeyDuplicateAllowed"] = list[i].LicKeyDuplicateAllowed == null ? DBNull.Value : (object)list[i].LicKeyDuplicateAllowed;
                }
            }
            list = SoftLicTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftLicType"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftLicType"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftLicType"].Value == list[i].IdLicType))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (SoftLicTypesDataModel.Delete(list[i].IdLicType.Value) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    _softLicTypes.Select().Rows.Find(list[i].IdLicType).Delete();
                }
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new SoftLicTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void SoftLicTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        private void SoftLicTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = _vSnapshotSoftLicTypes.Find("ID LicType", e.Row["ID LicType"]);
            if (rowIndex != -1)
                ((DataRowView)_vSnapshotSoftLicTypes[rowIndex]).Delete();
        }

        private void SoftLicTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotSoftLicTypes.Find("ID LicType", e.Row["ID LicType"]);
            if (rowIndex == -1 && _vSoftLicTypes.Find("ID LicType", e.Row["ID LicType"]) != -1)
            {
                _snapshotSoftLicTypes.Rows.Add(e.Row["ID LicType"], e.Row["LicType"], e.Row["LicKeyDuplicateAllowed"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)_vSnapshotSoftLicTypes[rowIndex]);
                    row["LicType"] = e.Row["LicType"];
                    row["LicKeyDuplicateAllowed"] = e.Row["LicKeyDuplicateAllowed"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void v_snapshotSoftLicTypes_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "softLicType":
                    if (cell.Value.ToString().Trim().Length > 50)
                        cell.ErrorText = "Длина наименования вида лицензии на программное обеспечение не может превышать 50 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование вида лицензии на программное обеспечение не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftLicTypesViewport));
            dataGridView = new DataGridView();
            idSoftLicType = new DataGridViewTextBoxColumn();
            softLicType = new DataGridViewTextBoxColumn();
            softLicKeyDuplicateAllowed = new DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] {
            idSoftLicType,
            softLicType,
            softLicKeyDuplicateAllowed});
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView.Location = new System.Drawing.Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(693, 345);
            dataGridView.TabIndex = 8;
            // 
            // idSoftLicType
            // 
            idSoftLicType.Frozen = true;
            idSoftLicType.HeaderText = "Идентификатор органа";
            idSoftLicType.Name = "idSoftLicType";
            idSoftLicType.ReadOnly = true;
            idSoftLicType.Visible = false;
            // 
            // softLicType
            // 
            softLicType.HeaderText = "Наименование";
            softLicType.MinimumWidth = 100;
            softLicType.Name = "softLicType";
            // 
            // softLicKeyDuplicateAllowed
            // 
            softLicKeyDuplicateAllowed.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            softLicKeyDuplicateAllowed.FillWeight = 1F;
            softLicKeyDuplicateAllowed.HeaderText = "Разрешить дублирование лиц. ключей";
            softLicKeyDuplicateAllowed.MinimumWidth = 200;
            softLicKeyDuplicateAllowed.Name = "softLicKeyDuplicateAllowed";
            softLicKeyDuplicateAllowed.Resizable = DataGridViewTriState.True;
            softLicKeyDuplicateAllowed.SortMode = DataGridViewColumnSortMode.Automatic;
            softLicKeyDuplicateAllowed.Width = 200;
            // 
            // SoftLicTypesViewport
            // 
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(699, 351);
            Controls.Add(dataGridView);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "SoftLicTypesViewport";
            Padding = new Padding(3);
            Text = "Виды лицензий на ПО";
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
