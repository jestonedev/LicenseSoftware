using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
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
    internal sealed class SoftSuppliersViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftSupplier;
        private DataGridViewTextBoxColumn softSupplier;
        #endregion Components

        #region Models
        SoftSuppliersDataModel softSuppliers = null;
        DataTable snapshotSoftSuppliers = new DataTable("snapshotSoftSuppliers");
        #endregion Models

        #region Views
        BindingSource v_softSuppliers = null;
        BindingSource v_snapshotSoftSuppliers = null;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private SoftSuppliersViewport()
            : this(null)
        {
        }

        public SoftSuppliersViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotSoftSuppliers.Locale = CultureInfo.InvariantCulture;
        }

        public SoftSuppliersViewport(SoftSuppliersViewport softSuppliersViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = softSuppliersViewport.DynamicFilter;
            this.StaticFilter = softSuppliersViewport.StaticFilter;
            this.ParentRow = softSuppliersViewport.ParentRow;
            this.ParentType = softSuppliersViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<SoftSupplier> list_from_view = SoftSuppliersFromView();
            List<SoftSupplier> list_from_viewport = SoftSuppliersFromViewport();
            if (list_from_view.Count != list_from_viewport.Count)
                return true;
            bool founded = false;
            for (int i = 0; i < list_from_view.Count; i++)
            {
                founded = false;
                for (int j = 0; j < list_from_viewport.Count; j++)
                    if (list_from_view[i] == list_from_viewport[j])
                        founded = true;
                if (!founded)
                    return true;
            }
            return false;
        }

        private static object[] DataRowViewToArray(DataRowView dataRowView)
        {
            return new object[] { 
                dataRowView["ID Supplier"], 
                dataRowView["Supplier"]
            };
        }

        private static bool ValidateViewportData(List<SoftSupplier> list)
        {
            foreach (SoftSupplier softSupplier in list)
            {
                if (softSupplier.SoftSupplierName == null)
                {
                    MessageBox.Show("Наименование поставщика ПО не может быть пустым",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softSupplier.SoftSupplierName != null && softSupplier.SoftSupplierName.Length > 400)
                {
                    MessageBox.Show("Длина наименования поставщика ПО не может превышать 400 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftSupplier RowToSoftSupplier(DataRow row)
        {
            SoftSupplier softSupplier = new SoftSupplier();
            softSupplier.IdSoftSupplier = ViewportHelper.ValueOrNull<int>(row, "ID Supplier");
            softSupplier.SoftSupplierName = ViewportHelper.ValueOrNull(row, "Supplier");
            return softSupplier;
        }

        private List<SoftSupplier> SoftSuppliersFromViewport()
        {
            List<SoftSupplier> list = new List<SoftSupplier>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SoftSupplier st = new SoftSupplier();
                    DataGridViewRow row = dataGridView.Rows[i];
                    st.IdSoftSupplier = ViewportHelper.ValueOrNull<int>(row, "idSoftSupplier");
                    st.SoftSupplierName = ViewportHelper.ValueOrNull(row, "softSupplier");
                    list.Add(st);
                }
            }
            return list;
        }

        private List<SoftSupplier> SoftSuppliersFromView()
        {
            List<SoftSupplier> list = new List<SoftSupplier>();
            for (int i = 0; i < v_softSuppliers.Count; i++)
            {
                SoftSupplier st = new SoftSupplier();
                DataRowView row = ((DataRowView)v_softSuppliers[i]);
                st.IdSoftSupplier = ViewportHelper.ValueOrNull<int>(row, "ID Supplier");
                st.SoftSupplierName = ViewportHelper.ValueOrNull(row, "Supplier");
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotSoftSuppliers.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softSuppliers = SoftSuppliersDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            softSuppliers.Select();

            v_softSuppliers = new BindingSource();
            v_softSuppliers.DataMember = "SoftSuppliers";
            v_softSuppliers.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < softSuppliers.Select().Columns.Count; i++)
                snapshotSoftSuppliers.Columns.Add(new DataColumn(
                    softSuppliers.Select().Columns[i].ColumnName, softSuppliers.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_softSuppliers.Count; i++)
                snapshotSoftSuppliers.Rows.Add(DataRowViewToArray(((DataRowView)v_softSuppliers[i])));
            v_snapshotSoftSuppliers = new BindingSource();
            v_snapshotSoftSuppliers.DataSource = snapshotSoftSuppliers;
            v_snapshotSoftSuppliers.CurrentItemChanged += new EventHandler(v_snapshotSuppliers_CurrentItemChanged);

            dataGridView.DataSource = v_snapshotSoftSuppliers;
            idSoftSupplier.DataPropertyName = "ID Supplier";
            softSupplier.DataPropertyName = "Supplier";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            softSuppliers.Select().RowChanged += new DataRowChangeEventHandler(SoftSuppliersViewport_RowChanged);
            softSuppliers.Select().RowDeleting += new DataRowChangeEventHandler(SoftSuppliersViewport_RowDeleting);
            softSuppliers.Select().RowDeleted += SoftSuppliersViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            v_snapshotSoftSuppliers.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotSoftSuppliers.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotSoftSuppliers.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotSoftSuppliers.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotSoftSuppliers.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotSoftSuppliers.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotSoftSuppliers.Position > -1) && (v_snapshotSoftSuppliers.Position < (v_snapshotSoftSuppliers.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotSoftSuppliers.Position > -1) && (v_snapshotSoftSuppliers.Position < (v_snapshotSoftSuppliers.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DIRECTORIES_READ_WRITE);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshotSoftSuppliers.AddNew();
            row.EndEdit();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (e == null)
                return;
            if (SnapshotHasChanges())
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в базу данных?", "Внимание",
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
            softSuppliers.Select().RowChanged -= new DataRowChangeEventHandler(SoftSuppliersViewport_RowChanged);
            softSuppliers.Select().RowDeleting -= new DataRowChangeEventHandler(SoftSuppliersViewport_RowDeleting);
            softSuppliers.Select().RowDeleted -= new DataRowChangeEventHandler(SoftSuppliersViewport_RowDeleted);
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotSoftSuppliers.Position != -1) && AccessControl.HasPrivelege(Priveleges.DIRECTORIES_READ_WRITE);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotSoftSuppliers[v_snapshotSoftSuppliers.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotSoftSuppliers.Clear();
            for (int i = 0; i < v_softSuppliers.Count; i++)
                snapshotSoftSuppliers.Rows.Add(DataRowViewToArray(((DataRowView)v_softSuppliers[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() && AccessControl.HasPrivelege(Priveleges.DIRECTORIES_READ_WRITE);
        }

        public override void SaveRecord()
        {
            sync_views = false;
            List<SoftSupplier> list = SoftSuppliersFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = softSuppliers.Select().Rows.Find(((SoftSupplier)list[i]).IdSoftSupplier);
                if (row == null)
                {
                    int idSoftSupplier = SoftSuppliersDataModel.Insert(list[i]);
                    if (idSoftSupplier == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshotSoftSuppliers[i])["ID Supplier"] = idSoftSupplier;
                    softSuppliers.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotSoftSuppliers[i]));
                }
                else
                {

                    if (RowToSoftSupplier(row) == list[i])
                        continue;
                    if (SoftSuppliersDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["Supplier"] = list[i].SoftSupplierName == null ? DBNull.Value : (object)list[i].SoftSupplierName;
                }
            }
            list = SoftSuppliersFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftSupplier"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftSupplier"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftSupplier"].Value == list[i].IdSoftSupplier))
                        row_index = j;
                if (row_index == -1)
                {
                    if (SoftSuppliersDataModel.Delete(list[i].IdSoftSupplier.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    softSuppliers.Select().Rows.Find(((SoftSupplier)list[i]).IdSoftSupplier).Delete();
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
            SoftSuppliersViewport viewport = new SoftSuppliersViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        void SoftSuppliersViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void SoftSuppliersViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshotSoftSuppliers.Find("ID Supplier", e.Row["ID Supplier"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshotSoftSuppliers[row_index]).Delete();
            }
        }

        void SoftSuppliersViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshotSoftSuppliers.Find("ID Supplier", e.Row["ID Supplier"]);
            if (row_index == -1 && v_softSuppliers.Find("ID Supplier", e.Row["ID Supplier"]) != -1)
            {
                snapshotSoftSuppliers.Rows.Add(new object[] { 
                        e.Row["ID Supplier"], 
                        e.Row["Supplier"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshotSoftSuppliers[row_index]);
                    row["Supplier"] = e.Row["Supplier"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshotSuppliers_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "softSupplier":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования поставщика ПО не может превышать 400 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование поставщика ПО не может быть пустым";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftSuppliersViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idSoftSupplier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.softSupplier = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.idSoftSupplier,
            this.softSupplier});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(654, 345);
            this.dataGridView.TabIndex = 8;
            // 
            // idSoftSupplier
            // 
            this.idSoftSupplier.Frozen = true;
            this.idSoftSupplier.HeaderText = "Идентификатор";
            this.idSoftSupplier.Name = "idSoftSupplier";
            this.idSoftSupplier.ReadOnly = true;
            this.idSoftSupplier.Visible = false;
            // 
            // softSupplier
            // 
            this.softSupplier.HeaderText = "Наименование";
            this.softSupplier.MinimumWidth = 100;
            this.softSupplier.Name = "softSupplier";
            // 
            // SoftSuppliersViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(660, 351);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftSuppliersViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Поставщики ПО";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
