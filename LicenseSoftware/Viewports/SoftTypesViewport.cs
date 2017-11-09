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
    internal sealed class SoftTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftType;
        private DataGridViewTextBoxColumn softType;
        #endregion Components

        #region Models

        private SoftTypesDataModel _softTypes;
        private readonly DataTable _snapshotSoftTypes = new DataTable("snapshotSoftTypes");
        #endregion Models

        #region Views

        private BindingSource _vSoftTypes;
        private BindingSource _vSnapshotSoftTypes;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftTypesViewport()
            : this(null)
        {
        }

        public SoftTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotSoftTypes.Locale = CultureInfo.InvariantCulture;
        }

        public SoftTypesViewport(SoftTypesViewport softTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softTypesViewport.DynamicFilter;
            StaticFilter = softTypesViewport.StaticFilter;
            ParentRow = softTypesViewport.ParentRow;
            ParentType = softTypesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = SoftTypesFromView();
            var listFromViewport = SoftTypesFromViewport();
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
                dataRowView["ID SoftType"], 
                dataRowView["SoftType"]
            };
        }

        private static bool ValidateViewportData(List<SoftType> list)
        {
            foreach (var softType in list)
            {
                if (softType.SoftTypeName == null)
                {
                    MessageBox.Show(@"Наименование вида программного обеспечения не может быть пустым",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softType.SoftTypeName != null && softType.SoftTypeName.Length > 400)
                {
                    MessageBox.Show(@"Длина вида программного обеспечения не может превышать 400 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftType RowToSoftType(DataRow row)
        {
            var softType = new SoftType
            {
                IdSoftType = ViewportHelper.ValueOrNull<int>(row, "ID SoftType"),
                SoftTypeName = ViewportHelper.ValueOrNull(row, "SoftType")
            };
            return softType;
        }

        private List<SoftType> SoftTypesFromViewport()
        {
            var list = new List<SoftType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                var st = new SoftType
                {
                    IdSoftType = ViewportHelper.ValueOrNull<int>(row, "idSoftType"),
                    SoftTypeName = ViewportHelper.ValueOrNull(row, "softType")
                };
                list.Add(st);
            }
            return list;
        }

        private List<SoftType> SoftTypesFromView()
        {
            var list = new List<SoftType>();
            for (var i = 0; i < _vSoftTypes.Count; i++)
            {
                var row = (DataRowView)_vSoftTypes[i];
                var st = new SoftType
                {
                    IdSoftType = ViewportHelper.ValueOrNull<int>(row, "ID SoftType"),
                    SoftTypeName = ViewportHelper.ValueOrNull(row, "SoftType")
                };
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotSoftTypes.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softTypes = SoftTypesDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            _softTypes.Select();

            _vSoftTypes = new BindingSource
            {
                DataMember = "SoftTypes",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _softTypes.Select().Columns.Count; i++)
                _snapshotSoftTypes.Columns.Add(new DataColumn(
                    _softTypes.Select().Columns[i].ColumnName, _softTypes.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vSoftTypes.Count; i++)
                _snapshotSoftTypes.Rows.Add(DataRowViewToArray((DataRowView)_vSoftTypes[i]));
            _vSnapshotSoftTypes = new BindingSource {DataSource = _snapshotSoftTypes};
            _vSnapshotSoftTypes.CurrentItemChanged += v_snapshotSoftTypes_CurrentItemChanged;

            dataGridView.DataSource = _vSnapshotSoftTypes;
            idSoftType.DataPropertyName = "ID SoftType";
            softType.DataPropertyName = "SoftType";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _softTypes.Select().RowChanged += SoftTypesViewport_RowChanged;
            _softTypes.Select().RowDeleting += SoftTypesViewport_RowDeleting;
            _softTypes.Select().RowDeleted += SoftTypesViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            _vSnapshotSoftTypes.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotSoftTypes.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotSoftTypes.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotSoftTypes.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotSoftTypes.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotSoftTypes.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotSoftTypes.Position > -1) && (_vSnapshotSoftTypes.Position < _vSnapshotSoftTypes.Count - 1);
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotSoftTypes.Position > -1) && (_vSnapshotSoftTypes.Position < _vSnapshotSoftTypes.Count - 1);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)_vSnapshotSoftTypes.AddNew();
            row.EndEdit();
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
            _softTypes.Select().RowChanged -= SoftTypesViewport_RowChanged;
            _softTypes.Select().RowDeleting -= SoftTypesViewport_RowDeleting;
            _softTypes.Select().RowDeleted -= SoftTypesViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotSoftTypes.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotSoftTypes[_vSnapshotSoftTypes.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotSoftTypes.Clear();
            for (var i = 0; i < _vSoftTypes.Count; i++)
                _snapshotSoftTypes.Rows.Add(DataRowViewToArray((DataRowView)_vSoftTypes[i]));
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
            var list = SoftTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = _softTypes.Select().Rows.Find(list[i].IdSoftType);
                if (row == null)
                {
                    var idSoftType = SoftTypesDataModel.Insert(list[i]);
                    if (idSoftType == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotSoftTypes[i])["ID SoftType"] = idSoftType;
                    _softTypes.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotSoftTypes[i]));
                }
                else
                {
                    if (RowToSoftType(row) == list[i])
                        continue;
                    if (SoftTypesDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["SoftType"] = list[i].SoftTypeName == null ? DBNull.Value : (object)list[i].SoftTypeName;
                }
            }
            list = SoftTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftType"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftType"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftType"].Value == list[i].IdSoftType))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (SoftTypesDataModel.Delete(list[i].IdSoftType.Value) == -1)
                {
                    _syncViews = true;
                    return;
                }
                _softTypes.Select().Rows.Find(list[i].IdSoftType).Delete();
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
            var viewport = new SoftTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void SoftTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
        }

        private void SoftTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = _vSnapshotSoftTypes.Find("ID SoftType", e.Row["ID SoftType"]);
                if (rowIndex != -1)
                    ((DataRowView)_vSnapshotSoftTypes[rowIndex]).Delete();
            }
        }

        private void SoftTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotSoftTypes.Find("ID SoftType", e.Row["ID SoftType"]);
            if (rowIndex == -1 && _vSoftTypes.Find("ID SoftType", e.Row["ID SoftType"]) != -1)
            {
                _snapshotSoftTypes.Rows.Add(e.Row["ID SoftType"], e.Row["SoftType"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = (DataRowView)_vSnapshotSoftTypes[rowIndex];
                    row["SoftType"] = e.Row["SoftType"];
                }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void v_snapshotSoftTypes_CurrentItemChanged(object sender, EventArgs e)
        {
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "softType":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования вида программного обеспечения не может превышать 400 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование вида программного обеспечения не может быть пустым";
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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftTypesViewport));
            dataGridView = new DataGridView();
            idSoftType = new DataGridViewTextBoxColumn();
            softType = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] {
            idSoftType,
            softType});
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView.Location = new System.Drawing.Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(654, 345);
            dataGridView.TabIndex = 8;
            // 
            // idSoftType
            // 
            idSoftType.Frozen = true;
            idSoftType.HeaderText = "Идентификатор";
            idSoftType.Name = "idSoftType";
            idSoftType.ReadOnly = true;
            idSoftType.Visible = false;
            // 
            // softType
            // 
            softType.HeaderText = "Наименование";
            softType.MinimumWidth = 100;
            softType.Name = "softType";
            // 
            // SoftTypesViewport
            // 
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(660, 351);
            Controls.Add(dataGridView);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "SoftTypesViewport";
            Padding = new Padding(3);
            Text = "Виды ПО";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);

        }
    }
}
