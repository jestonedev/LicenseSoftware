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
using DataModels.DataModels;

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
        SoftTypesDataModel softTypes = null;
        DataTable snapshotSoftTypes = new DataTable("snapshotSoftTypes");
        #endregion Models

        #region Views
        BindingSource v_softTypes = null;
        BindingSource v_snapshotSoftTypes = null;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private SoftTypesViewport()
            : this(null)
        {
        }

        public SoftTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotSoftTypes.Locale = CultureInfo.InvariantCulture;
        }

        public SoftTypesViewport(SoftTypesViewport softTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = softTypesViewport.DynamicFilter;
            this.StaticFilter = softTypesViewport.StaticFilter;
            this.ParentRow = softTypesViewport.ParentRow;
            this.ParentType = softTypesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<SoftType> list_from_view = SoftTypesFromView();
            List<SoftType> list_from_viewport = SoftTypesFromViewport();
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
                dataRowView["ID SoftType"], 
                dataRowView["SoftType"]
            };
        }

        private static bool ValidateViewportData(List<SoftType> list)
        {
            foreach (SoftType softType in list)
            {
                if (softType.SoftTypeName == null)
                {
                    MessageBox.Show("Наименование вида программного обеспечения не может быть пустым",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softType.SoftTypeName != null && softType.SoftTypeName.Length > 400)
                {
                    MessageBox.Show("Длина вида программного обеспечения не может превышать 400 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftType RowToSoftType(DataRow row)
        {
            SoftType softType = new SoftType();
            softType.IdSoftType = ViewportHelper.ValueOrNull<int>(row, "ID SoftType");
            softType.SoftTypeName = ViewportHelper.ValueOrNull(row, "SoftType");
            return softType;
        }

        private List<SoftType> SoftTypesFromViewport()
        {
            List<SoftType> list = new List<SoftType>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SoftType st = new SoftType();
                    DataGridViewRow row = dataGridView.Rows[i];
                    st.IdSoftType = ViewportHelper.ValueOrNull<int>(row, "idSoftType");
                    st.SoftTypeName = ViewportHelper.ValueOrNull(row, "softType");
                    list.Add(st);
                }
            }
            return list;
        }

        private List<SoftType> SoftTypesFromView()
        {
            List<SoftType> list = new List<SoftType>();
            for (int i = 0; i < v_softTypes.Count; i++)
            {
                SoftType st = new SoftType();
                DataRowView row = ((DataRowView)v_softTypes[i]);
                st.IdSoftType = ViewportHelper.ValueOrNull<int>(row, "ID SoftType");
                st.SoftTypeName = ViewportHelper.ValueOrNull(row, "SoftType");
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotSoftTypes.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softTypes = SoftTypesDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            softTypes.Select();

            v_softTypes = new BindingSource();
            v_softTypes.DataMember = "SoftTypes";
            v_softTypes.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < softTypes.Select().Columns.Count; i++)
                snapshotSoftTypes.Columns.Add(new DataColumn(
                    softTypes.Select().Columns[i].ColumnName, softTypes.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_softTypes.Count; i++)
                snapshotSoftTypes.Rows.Add(DataRowViewToArray(((DataRowView)v_softTypes[i])));
            v_snapshotSoftTypes = new BindingSource();
            v_snapshotSoftTypes.DataSource = snapshotSoftTypes;
            v_snapshotSoftTypes.CurrentItemChanged += new EventHandler(v_snapshotSoftTypes_CurrentItemChanged);

            dataGridView.DataSource = v_snapshotSoftTypes;
            idSoftType.DataPropertyName = "ID SoftType";
            softType.DataPropertyName = "SoftType";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            softTypes.Select().RowChanged += new DataRowChangeEventHandler(SoftTypesViewport_RowChanged);
            softTypes.Select().RowDeleting += new DataRowChangeEventHandler(SoftTypesViewport_RowDeleting);
            softTypes.Select().RowDeleted += SoftTypesViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            v_snapshotSoftTypes.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotSoftTypes.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotSoftTypes.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotSoftTypes.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotSoftTypes.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotSoftTypes.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotSoftTypes.Position > -1) && (v_snapshotSoftTypes.Position < (v_snapshotSoftTypes.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotSoftTypes.Position > -1) && (v_snapshotSoftTypes.Position < (v_snapshotSoftTypes.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshotSoftTypes.AddNew();
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
            softTypes.Select().RowChanged -= new DataRowChangeEventHandler(SoftTypesViewport_RowChanged);
            softTypes.Select().RowDeleting -= new DataRowChangeEventHandler(SoftTypesViewport_RowDeleting);
            softTypes.Select().RowDeleted -= new DataRowChangeEventHandler(SoftTypesViewport_RowDeleted);
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotSoftTypes.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotSoftTypes[v_snapshotSoftTypes.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotSoftTypes.Clear();
            for (int i = 0; i < v_softTypes.Count; i++)
                snapshotSoftTypes.Rows.Add(DataRowViewToArray(((DataRowView)v_softTypes[i])));
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
            List<SoftType> list = SoftTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = softTypes.Select().Rows.Find(((SoftType)list[i]).IdSoftType);
                if (row == null)
                {
                    int idSoftType = SoftTypesDataModel.Insert(list[i]);
                    if (idSoftType == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshotSoftTypes[i])["ID SoftType"] = idSoftType;
                    softTypes.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotSoftTypes[i]));
                }
                else
                {
                    if (RowToSoftType(row) == list[i])
                        continue;
                    if (SoftTypesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["SoftType"] = list[i].SoftTypeName == null ? DBNull.Value : (object)list[i].SoftTypeName;
                }
            }
            list = SoftTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftType"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftType"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftType"].Value == list[i].IdSoftType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (SoftTypesDataModel.Delete(list[i].IdSoftType.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    softTypes.Select().Rows.Find(((SoftType)list[i]).IdSoftType).Delete();
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
            SoftTypesViewport viewport = new SoftTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        void SoftTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void SoftTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshotSoftTypes.Find("ID SoftType", e.Row["ID SoftType"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshotSoftTypes[row_index]).Delete();
            }
        }

        void SoftTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshotSoftTypes.Find("ID SoftType", e.Row["ID SoftType"]);
            if (row_index == -1 && v_softTypes.Find("ID SoftType", e.Row["ID SoftType"]) != -1)
            {
                snapshotSoftTypes.Rows.Add(new object[] { 
                        e.Row["ID SoftType"], 
                        e.Row["SoftType"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshotSoftTypes[row_index]);
                    row["SoftType"] = e.Row["SoftType"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshotSoftTypes_CurrentItemChanged(object sender, EventArgs e)
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
                case "softType":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования вида программного обеспечения не может превышать 400 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование вида программного обеспечения не может быть пустым";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftTypesViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idSoftType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.softType = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.idSoftType,
            this.softType});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(654, 345);
            this.dataGridView.TabIndex = 8;
            // 
            // idSoftType
            // 
            this.idSoftType.Frozen = true;
            this.idSoftType.HeaderText = "Идентификатор";
            this.idSoftType.Name = "idSoftType";
            this.idSoftType.ReadOnly = true;
            this.idSoftType.Visible = false;
            // 
            // softType
            // 
            this.softType.HeaderText = "Наименование";
            this.softType.MinimumWidth = 100;
            this.softType.Name = "softType";
            // 
            // SoftTypesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(660, 351);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftTypesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Виды ПО";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
