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
    internal sealed class SoftSuppliersViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftSupplier;
        private DataGridViewTextBoxColumn softSupplier;
        #endregion Components

        #region Models

        private SoftSuppliersDataModel _softSuppliers;
        private readonly DataTable _snapshotSoftSuppliers = new DataTable("snapshotSoftSuppliers");
        #endregion Models

        #region Views

        private BindingSource _vSoftSuppliers;
        private BindingSource _vSnapshotSoftSuppliers;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftSuppliersViewport()
            : this(null)
        {
        }

        public SoftSuppliersViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotSoftSuppliers.Locale = CultureInfo.InvariantCulture;
        }

        public SoftSuppliersViewport(SoftSuppliersViewport softSuppliersViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softSuppliersViewport.DynamicFilter;
            StaticFilter = softSuppliersViewport.StaticFilter;
            ParentRow = softSuppliersViewport.ParentRow;
            ParentType = softSuppliersViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = SoftSuppliersFromView();
            var listFromViewport = SoftSuppliersFromViewport();
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
                dataRowView["ID Supplier"], 
                dataRowView["Supplier"]
            };
        }

        private static bool ValidateViewportData(List<SoftSupplier> list)
        {
            foreach (var softSupplier in list)
            {
                if (softSupplier.SoftSupplierName == null)
                {
                    MessageBox.Show(@"Наименование поставщика ПО не может быть пустым",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softSupplier.SoftSupplierName != null && softSupplier.SoftSupplierName.Length > 400)
                {
                    MessageBox.Show(@"Длина наименования поставщика ПО не может превышать 400 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftSupplier RowToSoftSupplier(DataRow row)
        {
            var softSupplier = new SoftSupplier
            {
                IdSoftSupplier = ViewportHelper.ValueOrNull<int>(row, "ID Supplier"),
                SoftSupplierName = ViewportHelper.ValueOrNull(row, "Supplier")
            };
            return softSupplier;
        }

        private List<SoftSupplier> SoftSuppliersFromViewport()
        {
            var list = new List<SoftSupplier>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                var st = new SoftSupplier
                {
                    IdSoftSupplier = ViewportHelper.ValueOrNull<int>(row, "idSoftSupplier"),
                    SoftSupplierName = ViewportHelper.ValueOrNull(row, "softSupplier")
                };
                list.Add(st);
            }
            return list;
        }

        private List<SoftSupplier> SoftSuppliersFromView()
        {
            var list = new List<SoftSupplier>();
            for (var i = 0; i < _vSoftSuppliers.Count; i++)
            {
                var row = (DataRowView)_vSoftSuppliers[i];
                var st = new SoftSupplier
                {
                    IdSoftSupplier = ViewportHelper.ValueOrNull<int>(row, "ID Supplier"),
                    SoftSupplierName = ViewportHelper.ValueOrNull(row, "Supplier")
                };
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotSoftSuppliers.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softSuppliers = SoftSuppliersDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            _softSuppliers.Select();

            _vSoftSuppliers = new BindingSource
            {
                DataMember = "SoftSuppliers",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _softSuppliers.Select().Columns.Count; i++)
                _snapshotSoftSuppliers.Columns.Add(new DataColumn(
                    _softSuppliers.Select().Columns[i].ColumnName, _softSuppliers.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vSoftSuppliers.Count; i++)
                _snapshotSoftSuppliers.Rows.Add(DataRowViewToArray((DataRowView)_vSoftSuppliers[i]));
            _vSnapshotSoftSuppliers = new BindingSource {DataSource = _snapshotSoftSuppliers};
            _vSnapshotSoftSuppliers.CurrentItemChanged += v_snapshotSuppliers_CurrentItemChanged;

            dataGridView.DataSource = _vSnapshotSoftSuppliers;
            idSoftSupplier.DataPropertyName = "ID Supplier";
            softSupplier.DataPropertyName = "Supplier";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _softSuppliers.Select().RowChanged += SoftSuppliersViewport_RowChanged;
            _softSuppliers.Select().RowDeleting += SoftSuppliersViewport_RowDeleting;
            _softSuppliers.Select().RowDeleted += SoftSuppliersViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            _vSnapshotSoftSuppliers.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotSoftSuppliers.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotSoftSuppliers.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotSoftSuppliers.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotSoftSuppliers.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotSoftSuppliers.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotSoftSuppliers.Position > -1) && (_vSnapshotSoftSuppliers.Position < _vSnapshotSoftSuppliers.Count - 1);
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotSoftSuppliers.Position > -1) && (_vSnapshotSoftSuppliers.Position < _vSnapshotSoftSuppliers.Count - 1);
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)_vSnapshotSoftSuppliers.AddNew();
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
            _softSuppliers.Select().RowChanged -= SoftSuppliersViewport_RowChanged;
            _softSuppliers.Select().RowDeleting -= SoftSuppliersViewport_RowDeleting;
            _softSuppliers.Select().RowDeleted -= SoftSuppliersViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotSoftSuppliers.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotSoftSuppliers[_vSnapshotSoftSuppliers.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotSoftSuppliers.Clear();
            for (var i = 0; i < _vSoftSuppliers.Count; i++)
                _snapshotSoftSuppliers.Rows.Add(DataRowViewToArray((DataRowView)_vSoftSuppliers[i]));
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
            var list = SoftSuppliersFromViewport();
            if (!ValidateViewportData(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = _softSuppliers.Select().Rows.Find(list[i].IdSoftSupplier);
                if (row == null)
                {
                    var idSoftSupplier = SoftSuppliersDataModel.Insert(list[i]);
                    if (idSoftSupplier == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotSoftSuppliers[i])["ID Supplier"] = idSoftSupplier;
                    _softSuppliers.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotSoftSuppliers[i]));
                }
                else
                {

                    if (RowToSoftSupplier(row) == list[i])
                        continue;
                    if (SoftSuppliersDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["Supplier"] = list[i].SoftSupplierName == null ? DBNull.Value : (object)list[i].SoftSupplierName;
                }
            }
            list = SoftSuppliersFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftSupplier"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftSupplier"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftSupplier"].Value == list[i].IdSoftSupplier))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (SoftSuppliersDataModel.Delete(list[i].IdSoftSupplier.Value) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    _softSuppliers.Select().Rows.Find(list[i].IdSoftSupplier).Delete();
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
            var viewport = new SoftSuppliersViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void SoftSuppliersViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
        }

        private void SoftSuppliersViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = _vSnapshotSoftSuppliers.Find("ID Supplier", e.Row["ID Supplier"]);
            if (rowIndex != -1)
                ((DataRowView)_vSnapshotSoftSuppliers[rowIndex]).Delete();
        }

        private void SoftSuppliersViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotSoftSuppliers.Find("ID Supplier", e.Row["ID Supplier"]);
            if (rowIndex == -1 && _vSoftSuppliers.Find("ID Supplier", e.Row["ID Supplier"]) != -1)
            {
                _snapshotSoftSuppliers.Rows.Add(e.Row["ID Supplier"], e.Row["Supplier"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = (DataRowView)_vSnapshotSoftSuppliers[rowIndex];
                    row["Supplier"] = e.Row["Supplier"];
                }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
        }

        private void v_snapshotSuppliers_CurrentItemChanged(object sender, EventArgs e)
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
                case "softSupplier":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования поставщика ПО не может превышать 400 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование поставщика ПО не может быть пустым";
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftSuppliersViewport));
            dataGridView = new DataGridView();
            idSoftSupplier = new DataGridViewTextBoxColumn();
            softSupplier = new DataGridViewTextBoxColumn();
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)204);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Padding = new Padding(0, 2, 0, 2);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(idSoftSupplier, softSupplier);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView.Location = new System.Drawing.Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(654, 345);
            dataGridView.TabIndex = 8;
            // 
            // idSoftSupplier
            // 
            idSoftSupplier.Frozen = true;
            idSoftSupplier.HeaderText = "Идентификатор";
            idSoftSupplier.Name = "idSoftSupplier";
            idSoftSupplier.ReadOnly = true;
            idSoftSupplier.Visible = false;
            // 
            // softSupplier
            // 
            softSupplier.HeaderText = "Наименование";
            softSupplier.MinimumWidth = 100;
            softSupplier.Name = "softSupplier";
            // 
            // SoftSuppliersViewport
            // 
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(660, 351);
            Controls.Add(dataGridView);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)204);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "SoftSuppliersViewport";
            Padding = new Padding(3);
            Text = "Поставщики ПО";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);

        }
    }
}
