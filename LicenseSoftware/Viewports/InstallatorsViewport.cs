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
    internal sealed class InstallatorsViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idInstallator;
        private DataGridViewTextBoxColumn fullName;
        private DataGridViewTextBoxColumn profession;
        private DataGridViewCheckBoxColumn inactive;
        #endregion Components

        #region Models

        private SoftInstallatorsDataModel _installators;
        private readonly DataTable _snapshotInstallators = new DataTable("snapshotInstallators");
        #endregion Models

        #region Views

        private BindingSource _vInstallators;
        private BindingSource _vSnapshotInstallators;
        #endregion Views



        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private InstallatorsViewport()
            : this(null)
        {
        }

        public InstallatorsViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            _snapshotInstallators.Locale = CultureInfo.InvariantCulture;
        }

        public InstallatorsViewport(InstallatorsViewport installatorsViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = installatorsViewport.DynamicFilter;
            StaticFilter = installatorsViewport.StaticFilter;
            ParentRow = installatorsViewport.ParentRow;
            ParentType = installatorsViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = InstallatorsFromView();
            var listFromViewport = InstallatorsFromViewport();
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
                dataRowView["ID Installator"], 
                dataRowView["FullName"],
                dataRowView["Profession"],
                ViewportHelper.ValueOrNull<bool>(dataRowView,"inactive") == true
            };
        }

        private static bool ValidateViewportData(List<SoftInstallator> list)
        {
            foreach (var installator in list)
            {
                if (installator.FullName == null)
                {
                    MessageBox.Show(@"ФИО установщика не может быть пустым", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftInstallator RowToInstallator(DataRow row)
        {
            var si = new SoftInstallator
            {
                IdInstallator = ViewportHelper.ValueOrNull<int>(row, "ID Installator"),
                FullName = ViewportHelper.ValueOrNull(row, "FullName"),
                Profession = ViewportHelper.ValueOrNull(row, "Profession"),
                Inactive = ViewportHelper.ValueOrNull<bool>(row, "Inactive")
            };
            return si;
        }

        private List<SoftInstallator> InstallatorsFromViewport()
        {
            var list = new List<SoftInstallator>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].IsNewRow) continue;
                var row = dataGridView.Rows[i];
                var si = new SoftInstallator
                {
                    IdInstallator = ViewportHelper.ValueOrNull<int>(row, "idInstallator"),
                    FullName = ViewportHelper.ValueOrNull(row, "FullName"),
                    Profession = ViewportHelper.ValueOrNull(row, "Profession"),
                    Inactive = ViewportHelper.ValueOrNull<bool>(row, "Inactive") == true
                };
                list.Add(si);
            }
            return list;
        }

        private List<SoftInstallator> InstallatorsFromView()
        {
            var list = new List<SoftInstallator>();
            for (var i = 0; i < _vInstallators.Count; i++)
            {
                var row = (DataRowView)_vInstallators[i];
                var si = new SoftInstallator
                {
                    IdInstallator = ViewportHelper.ValueOrNull<int>(row, "ID Installator"),
                    FullName = ViewportHelper.ValueOrNull(row, "FullName"),
                    Profession = ViewportHelper.ValueOrNull(row, "Profession"),
                    Inactive = ViewportHelper.ValueOrNull<bool>(row, "Inactive") == true
                };
                list.Add(si);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return _vSnapshotInstallators.Count;
        }

        public override void MoveFirst()
        {
            _vSnapshotInstallators.MoveFirst();
        }

        public override void MoveLast()
        {
            _vSnapshotInstallators.MoveLast();
        }

        public override void MoveNext()
        {
            _vSnapshotInstallators.MoveNext();
        }

        public override void MovePrev()
        {
            _vSnapshotInstallators.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return _vSnapshotInstallators.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return _vSnapshotInstallators.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (_vSnapshotInstallators.Position > -1) && (_vSnapshotInstallators.Position < _vSnapshotInstallators.Count - 1);
        }

        public override bool CanMoveLast()
        {
            return (_vSnapshotInstallators.Position > -1) && (_vSnapshotInstallators.Position < _vSnapshotInstallators.Count - 1);
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            _installators = SoftInstallatorsDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            _installators.Select();

            _vInstallators = new BindingSource
            {
                DataMember = "SoftInstallators",
                DataSource = DataSetManager.DataSet
            };

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < _installators.Select().Columns.Count; i++)
                _snapshotInstallators.Columns.Add(new DataColumn(
                    _installators.Select().Columns[i].ColumnName, _installators.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < _vInstallators.Count; i++)
                _snapshotInstallators.Rows.Add(DataRowViewToArray((DataRowView)_vInstallators[i]));
            _vSnapshotInstallators = new BindingSource {DataSource = _snapshotInstallators};
            _vSnapshotInstallators.CurrentItemChanged += v_snapshotInstallators_CurrentItemChanged;

            dataGridView.DataSource = _vSnapshotInstallators;
            idInstallator.DataPropertyName = "ID Installator";
            fullName.DataPropertyName = "fullName";
            profession.DataPropertyName = "profession";
            inactive.DataPropertyName = "inactive";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            _installators.Select().RowChanged += InstallatorsViewport_RowChanged;
            _installators.Select().RowDeleting += InstallatorsViewport_RowDeleting;
            _installators.Select().RowDeleted += InstallatorsViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)_vSnapshotInstallators.AddNew();
            if (row != null) row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (_vSnapshotInstallators.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)_vSnapshotInstallators[_vSnapshotInstallators.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            _snapshotInstallators.Clear();
            for (var i = 0; i < _vInstallators.Count; i++)
                _snapshotInstallators.Rows.Add(DataRowViewToArray((DataRowView)_vInstallators[i]));
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
            var list = InstallatorsFromViewport();
            if (!ValidateViewportData(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = _installators.Select().Rows.Find(list[i].IdInstallator ?? 0);
                if (row == null)
                {
                    var idInstallator = SoftInstallatorsDataModel.Insert(list[i]);
                    if (idInstallator == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)_vSnapshotInstallators[i])["ID Installator"] = idInstallator;
                    _installators.Select().Rows.Add(DataRowViewToArray((DataRowView)_vSnapshotInstallators[i]));
                }
                else
                {

                    if (RowToInstallator(row) == list[i])
                        continue;
                    if (SoftInstallatorsDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["FullName"] = list[i].FullName == null ? DBNull.Value : (object)list[i].FullName;
                    row["Profession"] = list[i].Profession == null ? DBNull.Value : (object)list[i].Profession;
                    row["Inactive"] = list[i].Inactive == null ? DBNull.Value : (object)list[i].Inactive;
                }
            }
            list = InstallatorsFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idInstallator"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idInstallator"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idInstallator"].Value == list[i].IdInstallator))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (SoftInstallatorsDataModel.Delete(list[i].IdInstallator.Value) == -1)
                {
                    _syncViews = true;
                    return;
                }
                _installators.Select().Rows.Find(list[i].IdInstallator).Delete();
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
            var viewport = new InstallatorsViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения об установщиках в базу данных в базу данных?", @"Внимание",
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
            _installators.Select().RowChanged -= InstallatorsViewport_RowChanged;
            _installators.Select().RowDeleting -= InstallatorsViewport_RowDeleting;
            _installators.Select().RowDeleted -= InstallatorsViewport_RowDeleted;
            base.OnClosing(e);
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "fullName":
                    cell.ErrorText = string.IsNullOrEmpty(cell.Value.ToString().Trim()) ? "ФИО установщика не может быть пустым" : "";
                    break;
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InstallatorsViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (!Selected) return;
            MenuCallback.EditingStateUpdate();
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
        }

        private void InstallatorsViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = _vSnapshotInstallators.Find("ID Installator", e.Row["ID Installator"]);
            if (rowIndex != -1)
                ((DataRowView)_vSnapshotInstallators[rowIndex]).Delete();
        }

        private void InstallatorsViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = _vSnapshotInstallators.Find("ID Installator", e.Row["ID Installator"]);
            if (rowIndex == -1 && _vInstallators.Find("ID Installator", e.Row["ID Installator"]) != -1)
            {
                _snapshotInstallators.Rows.Add(e.Row["ID Installator"], e.Row["FullName"], e.Row["Profession"], e.Row["Inactive"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = (DataRowView)_vSnapshotInstallators[rowIndex];
                    row["FullName"] = e.Row["FullName"];
                    row["Profession"] = e.Row["Profession"];
                    row["Inactive"] = e.Row["Inactive"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void v_snapshotInstallators_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallatorsViewport));
            dataGridView = new DataGridView();
            idInstallator = new DataGridViewTextBoxColumn();
            fullName = new DataGridViewTextBoxColumn();
            profession = new DataGridViewTextBoxColumn();
            inactive = new DataGridViewCheckBoxColumn();
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
            idInstallator,
            fullName,
            profession,
            inactive});
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView.Location = new System.Drawing.Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.Size = new System.Drawing.Size(648, 281);
            dataGridView.TabIndex = 8;
            // 
            // idInstallator
            // 
            idInstallator.Frozen = true;
            idInstallator.HeaderText = "Идентификатор";
            idInstallator.Name = "idInstallator";
            idInstallator.ReadOnly = true;
            idInstallator.Visible = false;
            // 
            // fullName
            // 
            fullName.HeaderText = "ФИО установщика";
            fullName.MaxInputLength = 128;
            fullName.MinimumWidth = 150;
            fullName.Name = "fullName";
            // 
            // profession
            // 
            profession.HeaderText = "Должность";
            profession.MaxInputLength = 128;
            profession.MinimumWidth = 150;
            profession.Name = "profession";
            // 
            // inactive
            // 
            inactive.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            inactive.HeaderText = "Неактивный";
            inactive.Name = "inactive";
            inactive.Resizable = DataGridViewTriState.True;
            inactive.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // InstallatorsViewport
            // 
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(654, 287);
            Controls.Add(dataGridView);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "InstallatorsViewport";
            Padding = new Padding(3);
            Text = "Исполнители";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);

        }
    }
}
