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
    internal sealed class SoftLicDocTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftLicDocType;
        private DataGridViewTextBoxColumn softLicDocType;
        #endregion Components

        #region Models

        private SoftLicDocTypesDataModel _softLicDocTypes;
        private readonly DataTable _snapshotSoftLicDocTypes = new DataTable("snapshotSoftLicDocTypes");
        #endregion Models

        #region Views

        private BindingSource _vSoftLicDocTypes;
        private BindingSource _vSnapshotSoftLicDocTypes;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftLicDocTypesViewport()
            : this(null)
        {
        }

        public SoftLicDocTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotSoftLicDocTypes.Locale = CultureInfo.InvariantCulture;
        }

        public SoftLicDocTypesViewport(SoftLicDocTypesViewport softLicDocTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softLicDocTypesViewport.DynamicFilter;
            StaticFilter = softLicDocTypesViewport.StaticFilter;
            ParentRow = softLicDocTypesViewport.ParentRow;
            ParentType = softLicDocTypesViewport.ParentType;
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
                dataRowView["ID DocType"], 
                dataRowView["DocType"]
            };
        }

        private static bool ValidateViewportData(List<SoftLicDocType> list)
        {
            foreach (var softLicDocType in list)
            {
                if (softLicDocType.DocType == null)
                {
                    MessageBox.Show(@"Наименование вида документа-основания на приобретение лицензии не может быть пустым",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softLicDocType.DocType != null && softLicDocType.DocType.Length > 400)
                {
                    MessageBox.Show(@"Длина наименования вида документа-основания на приобретение лицензии не может превышать 400 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftLicDocType RowToSoftLicDocType(DataRow row)
        {
            var softLicDocType = new SoftLicDocType
            {
                IdDocType = ViewportHelper.ValueOrNull<int>(row, "ID DocType"),
                DocType = ViewportHelper.ValueOrNull(row, "DocType")
            };
            return softLicDocType;
        }

        private List<SoftLicDocType> SoftLicTypesFromViewport()
        {
            var list = new List<SoftLicDocType>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                var st = new SoftLicDocType
                {
                    IdDocType = ViewportHelper.ValueOrNull<int>(row, "idSoftLicDocType"),
                    DocType = ViewportHelper.ValueOrNull(row, "softLicDocType")
                };
                list.Add(st);
            }
            return list;
        }

        private List<SoftLicDocType> SoftLicTypesFromView()
        {
            var list = new List<SoftLicDocType>();
            for (var i = 0; i < _vSoftLicDocTypes.Count; i++)
            {
                var row = (DataRowView)_vSoftLicDocTypes[i];
                var st = new SoftLicDocType
                {
                    IdDocType = ViewportHelper.ValueOrNull<int>(row, "ID DocType"),
                    DocType = ViewportHelper.ValueOrNull(row, "DocType")
                };
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotSoftLicDocTypes.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            _softLicDocTypes.Select();

            _vSoftLicDocTypes = new BindingSource
            {
                DataMember = "SoftLicDocTypes",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _softLicDocTypes.Select().Columns.Count; i++)
                _snapshotSoftLicDocTypes.Columns.Add(new DataColumn(
                    _softLicDocTypes.Select().Columns[i].ColumnName, _softLicDocTypes.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vSoftLicDocTypes.Count; i++)
                _snapshotSoftLicDocTypes.Rows.Add(DataRowViewToArray(((DataRowView)_vSoftLicDocTypes[i])));
            _vSnapshotSoftLicDocTypes = new BindingSource {DataSource = _snapshotSoftLicDocTypes};
            _vSnapshotSoftLicDocTypes.CurrentItemChanged += v_snapshotSoftLicTypes_CurrentItemChanged;

            dataGridView.DataSource = _vSnapshotSoftLicDocTypes;
            idSoftLicDocType.DataPropertyName = "ID DocType";
            softLicDocType.DataPropertyName = "DocType";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _softLicDocTypes.Select().RowChanged += SoftLicDocTypesViewport_RowChanged;
            _softLicDocTypes.Select().RowDeleting += SoftLicDocTypesViewport_RowDeleting;
            _softLicDocTypes.Select().RowDeleted += SoftLicDocTypesViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            _vSnapshotSoftLicDocTypes.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotSoftLicDocTypes.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotSoftLicDocTypes.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotSoftLicDocTypes.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotSoftLicDocTypes.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotSoftLicDocTypes.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotSoftLicDocTypes.Position > -1) && (_vSnapshotSoftLicDocTypes.Position < (_vSnapshotSoftLicDocTypes.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotSoftLicDocTypes.Position > -1) && (_vSnapshotSoftLicDocTypes.Position < (_vSnapshotSoftLicDocTypes.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)_vSnapshotSoftLicDocTypes.AddNew();
            if (row != null) row.EndEdit();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
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
                        e.Cancel = true;
                        return;
                }
            }
            _softLicDocTypes.Select().RowChanged -= SoftLicDocTypesViewport_RowChanged;
            _softLicDocTypes.Select().RowDeleting -= SoftLicDocTypesViewport_RowDeleting;
            _softLicDocTypes.Select().RowDeleted -= SoftLicDocTypesViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotSoftLicDocTypes.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotSoftLicDocTypes[_vSnapshotSoftLicDocTypes.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotSoftLicDocTypes.Clear();
            for (var i = 0; i < _vSoftLicDocTypes.Count; i++)
                _snapshotSoftLicDocTypes.Rows.Add(DataRowViewToArray((DataRowView)_vSoftLicDocTypes[i]));
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
                var row = _softLicDocTypes.Select().Rows.Find(list[i].IdDocType);
                if (row == null)
                {
                    var idSoftLicDocType = SoftLicDocTypesDataModel.Insert(list[i]);
                    if (idSoftLicDocType == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotSoftLicDocTypes[i])["ID DocType"] = idSoftLicDocType;
                    _softLicDocTypes.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotSoftLicDocTypes[i]));
                }
                else
                {

                    if (RowToSoftLicDocType(row) == list[i])
                        continue;
                    if (SoftLicDocTypesDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["DocType"] = list[i].DocType == null ? DBNull.Value : (object)list[i].DocType;
                }
            }
            list = SoftLicTypesFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftLicDocType"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftLicDocType"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftLicDocType"].Value == list[i].IdDocType))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (SoftLicDocTypesDataModel.Delete(list[i].IdDocType.Value) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    _softLicDocTypes.Select().Rows.Find(list[i].IdDocType).Delete();
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
            var viewport = new SoftLicDocTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void SoftLicDocTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        private void SoftLicDocTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                var rowIndex = _vSnapshotSoftLicDocTypes.Find("ID DocType", e.Row["ID DocType"]);
                if (rowIndex != -1)
                    ((DataRowView)_vSnapshotSoftLicDocTypes[rowIndex]).Delete();
            }
        }

        private void SoftLicDocTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotSoftLicDocTypes.Find("ID DocType", e.Row["ID DocType"]);
            if (rowIndex == -1 && _vSoftLicDocTypes.Find("ID DocType", e.Row["ID DocType"]) != -1)
            {
                _snapshotSoftLicDocTypes.Rows.Add(e.Row["ID DocType"], e.Row["DocType"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)_vSnapshotSoftLicDocTypes[rowIndex]);
                    row["DocType"] = e.Row["DocType"];
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
                case "softLicDocType":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования вида документа-основания на приобретение лицензии не может превышать 400 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование вида документа-основания на приобретение лицензии не может быть пустым";
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftLicDocTypesViewport));
            dataGridView = new DataGridView();
            idSoftLicDocType = new DataGridViewTextBoxColumn();
            softLicDocType = new DataGridViewTextBoxColumn();
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
            dataGridView.Columns.AddRange(idSoftLicDocType, softLicDocType);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView.Location = new System.Drawing.Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(654, 345);
            dataGridView.TabIndex = 8;
            // 
            // idSoftLicDocType
            // 
            idSoftLicDocType.Frozen = true;
            idSoftLicDocType.HeaderText = "Идентификатор";
            idSoftLicDocType.Name = "idSoftLicDocType";
            idSoftLicDocType.ReadOnly = true;
            idSoftLicDocType.Visible = false;
            // 
            // softLicDocType
            // 
            softLicDocType.HeaderText = "Наименование";
            softLicDocType.MinimumWidth = 100;
            softLicDocType.Name = "softLicDocType";
            // 
            // SoftLicDocTypesViewport
            // 
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(660, 351);
            Controls.Add(dataGridView);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "SoftLicDocTypesViewport";
            Padding = new Padding(3);
            Text = "Документы-основания";
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
