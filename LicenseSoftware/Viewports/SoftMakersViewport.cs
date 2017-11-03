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
    internal sealed class SoftMakersViewport: Viewport
    {
        #region Components
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn idSoftMaker;
        private DataGridViewTextBoxColumn softMaker;
        #endregion Components

        #region Models
        SoftMakersDataModel softMakers = null;
        DataTable snapshotSoftMakers = new DataTable("snapshotSoftMakers");
        #endregion Models

        #region Views
        BindingSource v_softMakers = null;
        BindingSource v_snapshotSoftMakers = null;
        #endregion Models

        //Флаг разрешения синхронизации snapshot и original моделей
        bool sync_views = true;

        private SoftMakersViewport()
            : this(null)
        {
        }

        public SoftMakersViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotSoftMakers.Locale = CultureInfo.InvariantCulture;
        }

        public SoftMakersViewport(SoftMakersViewport softMakersViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            this.DynamicFilter = softMakersViewport.DynamicFilter;
            this.StaticFilter = softMakersViewport.StaticFilter;
            this.ParentRow = softMakersViewport.ParentRow;
            this.ParentType = softMakersViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            List<SoftMaker> list_from_view = SoftMakersFromView();
            List<SoftMaker> list_from_viewport = SoftMakersFromViewport();
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
                dataRowView["ID SoftMaker"], 
                dataRowView["SoftMaker"]
            };
        }

        private static bool ValidateViewportData(List<SoftMaker> list)
        {
            foreach (SoftMaker softMaker in list)
            {
                if (softMaker.SoftMakerName == null)
                {
                    MessageBox.Show("Наименование разработчика ПО не может быть пустым",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softMaker.SoftMakerName != null && softMaker.SoftMakerName.Length > 400)
                {
                    MessageBox.Show("Длина наименования разработчика ПО не может превышать 400 символов",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            return true;
        }

        private static SoftMaker RowToSoftType(DataRow row)
        {
            SoftMaker softMaker = new SoftMaker();
            softMaker.IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "ID SoftMaker");
            softMaker.SoftMakerName = ViewportHelper.ValueOrNull(row, "SoftMaker");
            return softMaker;
        }

        private List<SoftMaker> SoftMakersFromViewport()
        {
            List<SoftMaker> list = new List<SoftMaker>();
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    SoftMaker st = new SoftMaker();
                    DataGridViewRow row = dataGridView.Rows[i];
                    st.IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "idSoftMaker");
                    st.SoftMakerName = ViewportHelper.ValueOrNull(row, "softMaker");
                    list.Add(st);
                }
            }
            return list;
        }

        private List<SoftMaker> SoftMakersFromView()
        {
            List<SoftMaker> list = new List<SoftMaker>();
            for (int i = 0; i < v_softMakers.Count; i++)
            {
                SoftMaker st = new SoftMaker();
                DataRowView row = ((DataRowView)v_softMakers[i]);
                st.IdSoftMaker = ViewportHelper.ValueOrNull<int>(row, "ID SoftMaker");
                st.SoftMakerName = ViewportHelper.ValueOrNull(row, "SoftMaker");
                list.Add(st);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotSoftMakers.Count;
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softMakers = SoftMakersDataModel.GetInstance();

            //Ожидаем дозагрузки данных, если это необходимо
            softMakers.Select();

            v_softMakers = new BindingSource();
            v_softMakers.DataMember = "SoftMakers";
            v_softMakers.DataSource = DataSetManager.DataSet;

            //Инициируем колонки snapshot-модели
            for (int i = 0; i < softMakers.Select().Columns.Count; i++)
                snapshotSoftMakers.Columns.Add(new DataColumn(
                    softMakers.Select().Columns[i].ColumnName, softMakers.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (int i = 0; i < v_softMakers.Count; i++)
                snapshotSoftMakers.Rows.Add(DataRowViewToArray(((DataRowView)v_softMakers[i])));
            v_snapshotSoftMakers = new BindingSource();
            v_snapshotSoftMakers.DataSource = snapshotSoftMakers;
            v_snapshotSoftMakers.CurrentItemChanged += v_snapshotSoftMakers_CurrentItemChanged;

            dataGridView.DataSource = v_snapshotSoftMakers;
            idSoftMaker.DataPropertyName = "ID SoftMaker";
            softMaker.DataPropertyName = "SoftMaker";

            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            softMakers.Select().RowChanged += SoftMakersViewport_RowChanged;
            softMakers.Select().RowDeleting += SoftMakersViewport_RowDeleting;
            softMakers.Select().RowDeleted += SoftMakersViewport_RowDeleted;
        }

        public override void MoveFirst()
        {
            v_snapshotSoftMakers.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotSoftMakers.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotSoftMakers.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotSoftMakers.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotSoftMakers.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotSoftMakers.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotSoftMakers.Position > -1) && (v_snapshotSoftMakers.Position < (v_snapshotSoftMakers.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotSoftMakers.Position > -1) && (v_snapshotSoftMakers.Position < (v_snapshotSoftMakers.Count - 1));
        }

        public override bool CanInsertRecord()
        {
            return  AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void InsertRecord()
        {
            DataRowView row = (DataRowView)v_snapshotSoftMakers.AddNew();
            row.EndEdit();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
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
            softMakers.Select().RowChanged -= SoftMakersViewport_RowChanged;
            softMakers.Select().RowDeleting -= SoftMakersViewport_RowDeleting;
            softMakers.Select().RowDeleted -= SoftMakersViewport_RowDeleted;
            base.OnClosing(e);
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotSoftMakers.Position != -1) && AccessControl.HasPrivelege(Priveleges.DirectoriesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotSoftMakers[v_snapshotSoftMakers.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotSoftMakers.Clear();
            for (int i = 0; i < v_softMakers.Count; i++)
                snapshotSoftMakers.Rows.Add(DataRowViewToArray(((DataRowView)v_softMakers[i])));
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
            List<SoftMaker> list = SoftMakersFromViewport();
            if (!ValidateViewportData(list))
            {
                sync_views = true;
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataRow row = softMakers.Select().Rows.Find(((SoftMaker)list[i]).IdSoftMaker);
                if (row == null)
                {
                    int idSoftMaker = SoftMakersDataModel.Insert(list[i]);
                    if (idSoftMaker == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    ((DataRowView)v_snapshotSoftMakers[i])["ID SoftMaker"] = idSoftMaker;
                    softMakers.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotSoftMakers[i]));
                }
                else
                {

                    if (RowToSoftType(row) == list[i])
                        continue;
                    if (SoftMakersDataModel.Update(list[i]) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    row["SoftMaker"] = list[i].SoftMakerName == null ? DBNull.Value : (object)list[i].SoftMakerName;
                }
            }
            list = SoftMakersFromView();
            for (int i = 0; i < list.Count; i++)
            {
                int row_index = -1;
                for (int j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idSoftMaker"].Value != null) &&
                        !String.IsNullOrEmpty(dataGridView.Rows[j].Cells["idSoftMaker"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idSoftMaker"].Value == list[i].IdSoftMaker))
                        row_index = j;
                if (row_index == -1)
                {
                    if (SoftMakersDataModel.Delete(list[i].IdSoftMaker.Value) == -1)
                    {
                        sync_views = true;
                        return;
                    }
                    softMakers.Select().Rows.Find(((SoftMaker)list[i]).IdSoftMaker).Delete();
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
            SoftMakersViewport viewport = new SoftMakersViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        void SoftMakersViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (Selected)
            {
                MenuCallback.EditingStateUpdate();
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
            }
        }

        void SoftMakersViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            if (e.Action == DataRowAction.Delete)
            {
                int row_index = v_snapshotSoftMakers.Find("ID SoftMaker", e.Row["ID SoftMaker"]);
                if (row_index != -1)
                    ((DataRowView)v_snapshotSoftMakers[row_index]).Delete();
            }
        }

        void SoftMakersViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!sync_views)
                return;
            int row_index = v_snapshotSoftMakers.Find("ID SoftMaker", e.Row["ID SoftMaker"]);
            if (row_index == -1 && v_softMakers.Find("ID SoftMaker", e.Row["ID SoftMaker"]) != -1)
            {
                snapshotSoftMakers.Rows.Add(new object[] { 
                        e.Row["ID SoftMaker"], 
                        e.Row["SoftMaker"]
                    });
            }
            else
                if (row_index != -1)
                {
                    DataRowView row = ((DataRowView)v_snapshotSoftMakers[row_index]);
                    row["SoftMaker"] = e.Row["SoftMaker"];
                }
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
            }
        }

        void v_snapshotSoftMakers_CurrentItemChanged(object sender, EventArgs e)
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
                case "softMaker":
                    if (cell.Value.ToString().Trim().Length > 400)
                        cell.ErrorText = "Длина наименования разработчика ПО не может превышать 400 символов";
                    else
                        if (String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                            cell.ErrorText = "Наименование разработчика ПО не может быть пустым";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftMakersViewport));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.idSoftMaker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.softMaker = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.idSoftMaker,
            this.softMaker});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(654, 345);
            this.dataGridView.TabIndex = 8;
            // 
            // idSoftMaker
            // 
            this.idSoftMaker.Frozen = true;
            this.idSoftMaker.HeaderText = "Идентификатор";
            this.idSoftMaker.Name = "idSoftMaker";
            this.idSoftMaker.ReadOnly = true;
            this.idSoftMaker.Visible = false;
            // 
            // softMaker
            // 
            this.softMaker.HeaderText = "Наименование";
            this.softMaker.MinimumWidth = 100;
            this.softMaker.Name = "softMaker";
            // 
            // SoftMakersViewport
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(660, 351);
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SoftMakersViewport";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "Разработчики ПО";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
