using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using DataModels.DataModels;

namespace LicenseSoftware.Viewport
{
    internal sealed class SoftLicTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models

        private SoftLicTypesDataModel softLicTypes;
        private DataTable snapshotSoftLicTypes = new DataTable("snapshotSoftLicTypes");
        #endregion Models

        #region Views

        private BindingSource v_softLicTypes;
        private BindingSource v_snapshotSoftLicTypes;
        #endregion Models
        private DataGridViewTextBoxColumn idSoftLicType;
        private DataGridViewTextBoxColumn softLicType;
        private DataGridViewCheckBoxColumn softLicKeyDuplicateAllowed;


        //Флаг разрешения синхронизации snapshot и original моделей
        private bool sync_views = true;

        private SoftLicTypesViewport()
            : this(null)
        {
        }

        public SoftLicTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotSoftLicTypes.Locale = CultureInfo.InvariantCulture;
        }

        public SoftLicTypesViewport(SoftLicTypesViewport softLicTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = softLicTypesViewport.DynamicFilter;
            this.StaticFilter = softLicTypesViewport.StaticFilter;
            this.ParentRow = softLicTypesViewport.ParentRow;
            this.ParentType = softLicTypesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var list_from_view = SoftLicTypesFromView();
            var list_from_viewport = SoftLicTypesFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            var founded = false;
            for (var i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (var j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
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
            for (var i = 0; i < v_softLicTypes.Count; i++)
            {
                var st = new SoftLicType();
                var row = (DataRowView)v_softLicTypes[i];
                st.IdLicType = ViewportHelper.ValueOrNull<int>(row, "ID LicType");
                st.LicType = ViewportHelper.ValueOrNull(row, "LicType");
                st.LicKeyDuplicateAllowed = ViewportHelper.ValueOrNull<bool>(row, "LicKeyDuplicateAllowed") == true;
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotSoftLicTypes.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softLicTypes = SoftLicTypesDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            softLicTypes.Select();

            v_softLicTypes = new BindingSource();
            v_softLicTypes.DataMember = "SoftLicTypes";
            v_softLicTypes.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < softLicTypes.Select().Columns.Count; i++)
                snapshotSoftLicTypes.Columns.Add(new DataColumn(
                    softLicTypes.Select().Columns[i].ColumnName, softLicTypes.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_softLicTypes.Count; i++)
                snapshotSoftLicTypes.Rows.Add(DataRowViewToArray((DataRowView)v_softLicTypes[i]));
            v_snapshotSoftLicTypes = new BindingSource {DataSource = snapshotSoftLicTypes};
            v_snapshotSoftLicTypes.CurrentItemChanged += v_snapshotSoftLicTypes_CurrentItemChanged;

            dataGridView.DataSource = v_snapshotSoftLicTypes;
            idSoftLicType.DataPropertyName = "ID LicType";
            softLicType.DataPropertyName = "LicType";
            softLicKeyDuplicateAllowed.DataPropertyName = "LicKeyDuplicateAllowed";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            softLicTypes.Select().RowChanged += SoftLicTypesViewport_RowChanged;
            softLicTypes.Select().RowDeleting += SoftLicTypesViewport_RowDeleting;
            softLicTypes.Select().RowDeleted += SoftLicTypesViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            v_snapshotSoftLicTypes.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotSoftLicTypes.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotSoftLicTypes.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotSoftLicTypes.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotSoftLicTypes.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotSoftLicTypes.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotSoftLicTypes.Position > -1) && (v_snapshotSoftLicTypes.Position < (v_snapshotSoftLicTypes.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotSoftLicTypes.Position > -1) && (v_snapshotSoftLicTypes.Position < (v_snapshotSoftLicTypes.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            var row = (DataRowView)v_snapshotSoftLicTypes.AddNew();
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
            softLicTypes.Select().RowChanged -= SoftLicTypesViewport_RowChanged;
            softLicTypes.Select().RowDeleting -= SoftLicTypesViewport_RowDeleting;
            softLicTypes.Select().RowDeleted -= SoftLicTypesViewport_RowDeleted;
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotSoftLicTypes.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotSoftLicTypes[v_snapshotSoftLicTypes.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotSoftLicTypes.Clear();
            for (var i = 0; i < v_softLicTypes.Count; i++)
                snapshotSoftLicTypes.Rows.Add(DataRowViewToArray(((DataRowView)v_softLicTypes[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void SaveRecord()
        {
            dataGridView.EndEdit();
            sync_views = false;
            var list = SoftLicTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = softLicTypes.Select().Rows.Find(list[i].IdLicType);
                if (row == null)
                {
                    var idSoftLicType = SoftLicTypesDataModel.Insert(list[i]);
                    if (idSoftLicType == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshotSoftLicTypes[i])["ID LicType"] = idSoftLicType;
                    softLicTypes.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotSoftLicTypes[i]));
                }
                else
                {

                    if (RowToSoftLicType(row) == list[i])
                        continue;
                    if (SoftLicTypesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
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
                        sync_views = true;
                        return;
                    }
                    softLicTypes.Select().Rows.Find(list[i].IdLicType).Delete();
                }
            }
            sync_views = true;
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
            if (!sync_views)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = v_snapshotSoftLicTypes.Find("ID LicType", e.Row["ID LicType"]);
            if (rowIndex != -1)
                ((DataRowView)v_snapshotSoftLicTypes[rowIndex]).Delete();
        }

        private void SoftLicTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            var row_index = v_snapshotSoftLicTypes.Find("ID LicType", e.Row["ID LicType"]);
            if (row_index == -1 && v_softLicTypes.Find("ID LicType", e.Row["ID LicType"]) != -1)
            {
                snapshotSoftLicTypes.Rows.Add(e.Row["ID LicType"], e.Row["LicType"], e.Row["LicKeyDuplicateAllowed"]);
            }
            else
                if (row_index != -1)
                {
                    var row = ((DataRowView)v_snapshotSoftLicTypes[row_index]);
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftLicTypesViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idSoftLicType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.softLicType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.softLicKeyDuplicateAllowed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
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
            this.idSoftLicType,
            this.softLicType,
            this.softLicKeyDuplicateAllowed});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(693, 345);
            this.dataGridView.TabIndex = 8;
            // 
            // idSoftLicType
            // 
            this.idSoftLicType.Frozen = true;
            this.idSoftLicType.HeaderText = "Идентификатор органа";
            this.idSoftLicType.Name = "idSoftLicType";
            this.idSoftLicType.ReadOnly = true;
            this.idSoftLicType.Visible = false;
            // 
            // softLicType
            // 
            this.softLicType.HeaderText = "Наименование";
            this.softLicType.MinimumWidth = 100;
            this.softLicType.Name = "softLicType";
            // 
            // softLicKeyDuplicateAllowed
            // 
            this.softLicKeyDuplicateAllowed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.softLicKeyDuplicateAllowed.FillWeight = 1F;
            this.softLicKeyDuplicateAllowed.HeaderText = "Разрешить дублирование лиц. ключей";
            this.softLicKeyDuplicateAllowed.MinimumWidth = 200;
            this.softLicKeyDuplicateAllowed.Name = "softLicKeyDuplicateAllowed";
            this.softLicKeyDuplicateAllowed.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.softLicKeyDuplicateAllowed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.softLicKeyDuplicateAllowed.Width = 200;
            // 
            // SoftLicTypesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(699, 351);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftLicTypesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Виды лицензий на ПО";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
