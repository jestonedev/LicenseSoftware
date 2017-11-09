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
    internal sealed class SoftMakersViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftMaker;
        private DataGridViewTextBoxColumn softMaker;
        #endregion Components

        #region Models

        private SoftMakersDataModel _softMakers;
        private readonly DataTable _snapshotSoftMakers = new DataTable("snapshotSoftMakers");
        #endregion Models

        #region Views

        private BindingSource _vSoftMakers;
        private BindingSource _vSnapshotSoftMakers;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftMakersViewport()
            : this(null)
        {
        }

        public SoftMakersViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotSoftMakers.Locale = CultureInfo.InvariantCulture;
        }

        public SoftMakersViewport(SoftMakersViewport softMakersViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softMakersViewport.DynamicFilter;
            StaticFilter = softMakersViewport.StaticFilter;
            ParentRow = softMakersViewport.ParentRow;
            ParentType = softMakersViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = SoftMakersFromView();
            var listFromViewport = SoftMakersFromViewport();
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
                dataRowView["ID SoftMaker"], 
                dataRowView["SoftMaker"]
            };
        }

        private static bool ValidateViewportData(List<SoftMaker> list)
        {
            foreach (var softMaker in list)
            {
                if (softMaker.SoftMakerName == null)
                {
                    MessageBox.Show(@"Наименование разработчика ПО не может быть пустым",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softMaker.SoftMakerName != null && softMaker.SoftMakerName.Length > 400)
                {
                    MessageBox.Show(@"Длина наименования разработчика ПО не может превышать 400 символов",
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftMaker RowToSoftType(DataRow row)
        {
            var softMaker = new SoftMaker
            {
                IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "ID SoftMaker"),
                SoftMakerName = ViewportHelper.ValueOrNull(row, "SoftMaker")
            };
            return softMaker;
        }

        private List<SoftMaker> SoftMakersFromViewport()
        {
            var list = new List<SoftMaker>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var row = dataGridView.Rows[i];
                    var st = new SoftMaker
                    {
                        IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "idSoftMaker"),
                        SoftMakerName = ViewportHelper.ValueOrNull(row, "softMaker")
                    };
                    list.Add(st);
                }
            }
            return list;
        }

        private List<SoftMaker> SoftMakersFromView()
        {
            var list = new List<SoftMaker>();
            for (var i = 0; i < _vSoftMakers.Count; i++)
            {
                var row = (DataRowView)_vSoftMakers[i];
                var st = new SoftMaker
                {
                    IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "ID SoftMaker"),
                    SoftMakerName = ViewportHelper.ValueOrNull(row, "SoftMaker")
                };
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotSoftMakers.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _softMakers = SoftMakersDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            _softMakers.Select();

            _vSoftMakers = new BindingSource
            {
                DataMember = "SoftMakers",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _softMakers.Select().Columns.Count; i++)
                _snapshotSoftMakers.Columns.Add(new DataColumn(
                    _softMakers.Select().Columns[i].ColumnName, _softMakers.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vSoftMakers.Count; i++)
                _snapshotSoftMakers.Rows.Add(DataRowViewToArray(((DataRowView)_vSoftMakers[i])));
            _vSnapshotSoftMakers = new BindingSource {DataSource = _snapshotSoftMakers};
            _vSnapshotSoftMakers.CurrentItemChanged += v_snapshotSoftMakers_CurrentItemChanged;

            dataGridView.DataSource = _vSnapshotSoftMakers;
            idSoftMaker.DataPropertyName = "ID SoftMaker";
            softMaker.DataPropertyName = "SoftMaker";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _softMakers.Select().RowChanged += SoftMakersViewport_RowChanged;
            _softMakers.Select().RowDeleting += SoftMakersViewport_RowDeleting;
            _softMakers.Select().RowDeleted += SoftMakersViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            _vSnapshotSoftMakers.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotSoftMakers.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotSoftMakers.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotSoftMakers.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotSoftMakers.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotSoftMakers.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotSoftMakers.Position > -1) && (_vSnapshotSoftMakers.Position < (_vSnapshotSoftMakers.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotSoftMakers.Position > -1) && (_vSnapshotSoftMakers.Position < (_vSnapshotSoftMakers.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)_vSnapshotSoftMakers.AddNew();
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
            _softMakers.Select().RowChanged -= SoftMakersViewport_RowChanged;
            _softMakers.Select().RowDeleting -= SoftMakersViewport_RowDeleting;
            _softMakers.Select().RowDeleted -= SoftMakersViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotSoftMakers.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotSoftMakers[_vSnapshotSoftMakers.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotSoftMakers.Clear();
            for (var i = 0; i < _vSoftMakers.Count; i++)
                _snapshotSoftMakers.Rows.Add(DataRowViewToArray(((DataRowView)_vSoftMakers[i])));
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
            var list = SoftMakersFromViewport();
            if (!ValidateViewportData(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = _softMakers.Select().Rows.Find(list[i].IdSoftMaker);
                if (row == null)
                {
                    var idSoftMaker = SoftMakersDataModel.Insert(list[i]);
                    if (idSoftMaker == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotSoftMakers[i])["ID SoftMaker"] = idSoftMaker;
                    _softMakers.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotSoftMakers[i]));
                }
                else
                {

                    if (RowToSoftType(row) == list[i])
                        continue;
                    if (SoftMakersDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["SoftMaker"] = list[i].SoftMakerName == null ? DBNull.Value : (object)list[i].SoftMakerName;
                }
            }
            list = SoftMakersFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftMaker"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftMaker"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftMaker"].Value == list[i].IdSoftMaker))
                        rowIndex = j;
                if (rowIndex == -1)
                {
                    if (SoftMakersDataModel.Delete(list[i].IdSoftMaker.Value) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    _softMakers.Select().Rows.Find(list[i].IdSoftMaker).Delete();
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
            var viewport = new SoftMakersViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        private void SoftMakersViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
        }

        private void SoftMakersViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = _vSnapshotSoftMakers.Find("ID SoftMaker", e.Row["ID SoftMaker"]);
            if (rowIndex != -1)
                ((DataRowView)_vSnapshotSoftMakers[rowIndex]).Delete();
        }

        private void SoftMakersViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotSoftMakers.Find("ID SoftMaker", e.Row["ID SoftMaker"]);
            if (rowIndex == -1 && _vSoftMakers.Find("ID SoftMaker", e.Row["ID SoftMaker"]) != -1)
            {
                _snapshotSoftMakers.Rows.Add(e.Row["ID SoftMaker"], e.Row["SoftMaker"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)_vSnapshotSoftMakers[rowIndex]);
                    row["SoftMaker"] = e.Row["SoftMaker"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void v_snapshotSoftMakers_CurrentItemChanged(object sender, EventArgs e)
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
                case "softMaker":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования разработчика ПО не может превышать 400 символов";
                    else
                        if (string.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование разработчика ПО не может быть пустым";
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftMakersViewport));
            dataGridView = new DataGridView();
            idSoftMaker = new DataGridViewTextBoxColumn();
            softMaker = new DataGridViewTextBoxColumn();
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
            idSoftMaker,
            softMaker});
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView.Location = new System.Drawing.Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(654, 345);
            dataGridView.TabIndex = 8;
            // 
            // idSoftMaker
            // 
            idSoftMaker.Frozen = true;
            idSoftMaker.HeaderText = "Идентификатор";
            idSoftMaker.Name = "idSoftMaker";
            idSoftMaker.ReadOnly = true;
            idSoftMaker.Visible = false;
            // 
            // softMaker
            // 
            softMaker.HeaderText = "Наименование";
            softMaker.MinimumWidth = 100;
            softMaker.Name = "softMaker";
            // 
            // SoftMakersViewport
            // 
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(660, 351);
            Controls.Add(dataGridView);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "SoftMakersViewport";
            Padding = new Padding(3);
            Text = "Разработчики ПО";
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
