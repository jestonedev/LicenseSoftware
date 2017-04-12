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
    internal sealed class SoftLicDocTypesViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftLicDocType;
        private DataGridViewTextBoxColumn softLicDocType;
        #endregion Components

        #region Models
        SoftLicDocTypesDataModel softLicDocTypes = null;
        DataTable snapshotSoftLicDocTypes = new DataTable("snapshotSoftLicDocTypes");
        #endregion Models

        #region Views
        BindingSource v_softLicDocTypes = null;
        BindingSource v_snapshotSoftLicDocTypes = null;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private SoftLicDocTypesViewport()
            : this(null)
        {
        }

        public SoftLicDocTypesViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotSoftLicDocTypes.Locale = CultureInfo.InvariantCulture;
        }

        public SoftLicDocTypesViewport(SoftLicDocTypesViewport softLicDocTypesViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = softLicDocTypesViewport.DynamicFilter;
            this.StaticFilter = softLicDocTypesViewport.StaticFilter;
            this.ParentRow = softLicDocTypesViewport.ParentRow;
            this.ParentType = softLicDocTypesViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<SoftLicDocType> list_from_view = SoftLicTypesFromView();
            List<SoftLicDocType> list_from_viewport = SoftLicTypesFromViewport();
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
                dataRowView["ID DocType"], 
                dataRowView["DocType"]
            };
        }

        private static bool ValidateViewportData(List<SoftLicDocType> list)
        {
            foreach (SoftLicDocType softLicDocType in list)
            {
                if (softLicDocType.DocType == null)
                {
                    MessageBox.Show("Наименование вида документа-основания на приобретение лицензии не может быть пустым",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softLicDocType.DocType != null && softLicDocType.DocType.Length > 400)
                {
                    MessageBox.Show("Длина наименования вида документа-основания на приобретение лицензии не может превышать 400 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftLicDocType RowToSoftLicDocType(DataRow row)
        {
            SoftLicDocType softLicDocType = new SoftLicDocType();
            softLicDocType.IdDocType = ViewportHelper.ValueOrNull<int>(row, "ID DocType");
            softLicDocType.DocType = ViewportHelper.ValueOrNull(row, "DocType");
            return softLicDocType;
        }

        private List<SoftLicDocType> SoftLicTypesFromViewport()
        {
            List<SoftLicDocType> list = new List<SoftLicDocType>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SoftLicDocType st = new SoftLicDocType();
                    DataGridViewRow row = dataGridView.Rows[i];
                    st.IdDocType = ViewportHelper.ValueOrNull<int>(row, "idSoftLicDocType");
                    st.DocType = ViewportHelper.ValueOrNull(row, "softLicDocType");
                    list.Add(st);
                }
            }
            return list;
        }

        private List<SoftLicDocType> SoftLicTypesFromView()
        {
            List<SoftLicDocType> list = new List<SoftLicDocType>();
            for (int i = 0; i < v_softLicDocTypes.Count; i++)
            {
                SoftLicDocType st = new SoftLicDocType();
                DataRowView row = ((DataRowView)v_softLicDocTypes[i]);
                st.IdDocType = ViewportHelper.ValueOrNull<int>(row, "ID DocType");
                st.DocType = ViewportHelper.ValueOrNull(row, "DocType");
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotSoftLicDocTypes.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softLicDocTypes = SoftLicDocTypesDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            softLicDocTypes.Select();

            v_softLicDocTypes = new BindingSource();
            v_softLicDocTypes.DataMember = "SoftLicDocTypes";
            v_softLicDocTypes.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < softLicDocTypes.Select().Columns.Count; i++)
                snapshotSoftLicDocTypes.Columns.Add(new DataColumn(
                    softLicDocTypes.Select().Columns[i].ColumnName, softLicDocTypes.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_softLicDocTypes.Count; i++)
                snapshotSoftLicDocTypes.Rows.Add(DataRowViewToArray(((DataRowView)v_softLicDocTypes[i])));
            v_snapshotSoftLicDocTypes = new BindingSource();
            v_snapshotSoftLicDocTypes.DataSource = snapshotSoftLicDocTypes;
            v_snapshotSoftLicDocTypes.CurrentItemChanged += new EventHandler(v_snapshotSoftLicTypes_CurrentItemChanged);

            dataGridView.DataSource = v_snapshotSoftLicDocTypes;
            idSoftLicDocType.DataPropertyName = "ID DocType";
            softLicDocType.DataPropertyName = "DocType";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += new DataGridViewCellEventHandler(dataGridView_CellValidated);
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            //Синхронизация данных исходные->текущие
            softLicDocTypes.Select().RowChanged += new DataRowChangeEventHandler(SoftLicDocTypesViewport_RowChanged);
            softLicDocTypes.Select().RowDeleting += new DataRowChangeEventHandler(SoftLicDocTypesViewport_RowDeleting);
            softLicDocTypes.Select().RowDeleted += SoftLicDocTypesViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            v_snapshotSoftLicDocTypes.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotSoftLicDocTypes.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotSoftLicDocTypes.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotSoftLicDocTypes.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotSoftLicDocTypes.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotSoftLicDocTypes.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotSoftLicDocTypes.Position > -1) && (v_snapshotSoftLicDocTypes.Position < (v_snapshotSoftLicDocTypes.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotSoftLicDocTypes.Position > -1) && (v_snapshotSoftLicDocTypes.Position < (v_snapshotSoftLicDocTypes.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshotSoftLicDocTypes.AddNew();
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
            softLicDocTypes.Select().RowChanged -= new DataRowChangeEventHandler(SoftLicDocTypesViewport_RowChanged);
            softLicDocTypes.Select().RowDeleting -= new DataRowChangeEventHandler(SoftLicDocTypesViewport_RowDeleting);
            softLicDocTypes.Select().RowDeleted -= new DataRowChangeEventHandler(SoftLicDocTypesViewport_RowDeleted);
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotSoftLicDocTypes.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotSoftLicDocTypes[v_snapshotSoftLicDocTypes.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotSoftLicDocTypes.Clear();
            for (int i = 0; i < v_softLicDocTypes.Count; i++)
                snapshotSoftLicDocTypes.Rows.Add(DataRowViewToArray(((DataRowView)v_softLicDocTypes[i])));
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
            List<SoftLicDocType> list = SoftLicTypesFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = softLicDocTypes.Select().Rows.Find(((SoftLicDocType)list[i]).IdDocType);
                if (row == null)
                {
                    int idSoftLicDocType = SoftLicDocTypesDataModel.Insert(list[i]);
                    if (idSoftLicDocType == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshotSoftLicDocTypes[i])["ID DocType"] = idSoftLicDocType;
                    softLicDocTypes.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotSoftLicDocTypes[i]));
                }
                else
                {

                    if (RowToSoftLicDocType(row) == list[i])
                        continue;
                    if (SoftLicDocTypesDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["DocType"] = list[i].DocType == null ? DBNull.Value : (object)list[i].DocType;
                }
            }
            list = SoftLicTypesFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftLicDocType"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftLicDocType"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftLicDocType"].Value == list[i].IdDocType))
                        row_index = j;
                if (row_index == -1)
                {
                    if (SoftLicDocTypesDataModel.Delete(list[i].IdDocType.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    softLicDocTypes.Select().Rows.Find(((SoftLicDocType)list[i]).IdDocType).Delete();
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
            SoftLicDocTypesViewport viewport = new SoftLicDocTypesViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        void SoftLicDocTypesViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void SoftLicDocTypesViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshotSoftLicDocTypes.Find("ID DocType", e.Row["ID DocType"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshotSoftLicDocTypes[row_index]).Delete();
            }
        }

        void SoftLicDocTypesViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshotSoftLicDocTypes.Find("ID DocType", e.Row["ID DocType"]);
            if (row_index == -1 && v_softLicDocTypes.Find("ID DocType", e.Row["ID DocType"]) != -1)
            {
                snapshotSoftLicDocTypes.Rows.Add(new object[] { 
                        e.Row["ID DocType"], 
                        e.Row["DocType"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshotSoftLicDocTypes[row_index]);
                    row["DocType"] = e.Row["DocType"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshotSoftLicTypes_CurrentItemChanged(object sender, EventArgs e)
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
                case "softLicDocType":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования вида документа-основания на приобретение лицензии не может превышать 400 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование вида документа-основания на приобретение лицензии не может быть пустым";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftLicDocTypesViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idSoftLicDocType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.softLicDocType = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.idSoftLicDocType,
            this.softLicDocType});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(654, 345);
            this.dataGridView.TabIndex = 8;
            // 
            // idSoftLicDocType
            // 
            this.idSoftLicDocType.Frozen = true;
            this.idSoftLicDocType.HeaderText = "Идентификатор";
            this.idSoftLicDocType.Name = "idSoftLicDocType";
            this.idSoftLicDocType.ReadOnly = true;
            this.idSoftLicDocType.Visible = false;
            // 
            // softLicDocType
            // 
            this.softLicDocType.HeaderText = "Наименование";
            this.softLicDocType.MinimumWidth = 100;
            this.softLicDocType.Name = "softLicDocType";
            // 
            // SoftLicDocTypesViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(660, 351);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftLicDocTypesViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Документы-основания";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
