using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using Security;
using System.Globalization;
using System.Linq;

namespace LicenseSoftware.Viewport
{
    internal sealed class SoftLicKeysViewport : Viewport
    {
        #region Components
        private DataGridView dataGridView;
        #endregion Components

        #region Models

        private SoftLicKeysDataModel softLicKeys;
        private DataTable snapshotsoftLicKeys = new DataTable("snapshotSoftLicKeys");
        #endregion Models

        #region Views

        private BindingSource v_softLicKeys;
        private BindingSource v_snapshotSoftLicKeys;
        #endregion Views
        private DataGridViewTextBoxColumn idLicenseKey;
        private DataGridViewTextBoxColumn idLicense;
        private DataGridViewTextBoxColumn LicKey;


        //Флаг разрешения синхронизации snapshot и original моделей
        private bool _syncViews = true;

        private SoftLicKeysViewport()
            : this(null)
        {
        }

        public SoftLicKeysViewport(IMenuCallback menuCallback)
            : base(menuCallback)
        {
            InitializeComponent();
            snapshotsoftLicKeys.Locale = CultureInfo.InvariantCulture;
        }

        public SoftLicKeysViewport(SoftLicKeysViewport softLicKeysViewport, IMenuCallback menuCallback)
            : this(menuCallback)
        {
            DynamicFilter = softLicKeysViewport.DynamicFilter;
            StaticFilter = softLicKeysViewport.StaticFilter;
            ParentRow = softLicKeysViewport.ParentRow;
            ParentType = softLicKeysViewport.ParentType;
        }

        private bool SnapshotHasChanges()
        {
            var listFromView = SoftLicKeysFromView();
            var listFromViewport = SoftLicKeysFromViewport();
            if (listFromView.Count != listFromViewport.Count)
                return true;
            bool founded;
            for (var i = 0; i < listFromView.Count; i++)
            {
                founded = false;
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
                dataRowView["ID LicenseKey"], 
                dataRowView["ID License"], 
                dataRowView["LicKey"]
            };
        }

        private bool ValidateSoftLicKeys(List<SoftLicKey> softLicKeysParam)
        {
            foreach (var softLicKey in softLicKeysParam)
            {
                if (softLicKey.LicKey == null)
                {
                    MessageBox.Show(@"Лицензионный ключ является обязательным для заполнения", @"Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (softLicKey.LicKey != null && softLicKey.LicKey.Length > 200)
                {
                    MessageBox.Show(@"Длина лицензионного ключа не может превышать 200 символов", @"Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                var localSoftLicKey = softLicKey;
                if (softLicKeysParam.Count(v => v.LicKey == localSoftLicKey.LicKey) > 1)
                {
                    MessageBox.Show(string.Format("Вы пытаетесь добавить лицензионный ключ {0} дважды", localSoftLicKey.LicKey),
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
                var duplicates = softLicKeys.Select().AsEnumerable().
                    Where(v => v.Field<string>("LicKey") == localSoftLicKey.LicKey && v.Field<int>("ID License") != localSoftLicKey.IdLicense);
                if (duplicates.Any())
                {
                    MessageBox.Show(string.Format("Нельзя добавить лицензионный ключ {0}, т.к. он уже присутствует в другой лицензии", localSoftLicKey.LicKey), 
                        @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            if (ParentRow["InstallationsCount"] != DBNull.Value && v_snapshotSoftLicKeys.Count > (int)ParentRow["InstallationsCount"])
            {
                var result = MessageBox.Show(@"Количество внесенных ключей превышает количество разрешенных установок данного ПО. " +
                    @"Вы уверены, что хотите сохранить изменения в базу данных?", @"Внимание",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (result != DialogResult.Yes)
                    return false;
            }
            return true;
        }

        private static SoftLicKey RowToSoftLicKey(DataRow row)
        {
            var softLicKey = new SoftLicKey
            {
                IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey"),
                IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License"),
                LicKey = ViewportHelper.ValueOrNull(row, "LicKey")
            };
            return softLicKey;
        }

        private List<SoftLicKey> SoftLicKeysFromViewport()
        {
            var list = new List<SoftLicKey>();
            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    var slk = new SoftLicKey();
                    var row = dataGridView.Rows[i];
                    slk.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "idLicenseKey");
                    slk.IdLicense = ViewportHelper.ValueOrNull<int>(row, "idLicense");
                    slk.LicKey = ViewportHelper.ValueOrNull(row, "LicKey");
                    list.Add(slk);
                }
            }
            return list;
        }

        private List<SoftLicKey> SoftLicKeysFromView()
        {
            var list = new List<SoftLicKey>();
            for (var i = 0; i < v_softLicKeys.Count; i++)
            {
                var slk = new SoftLicKey();
                var row = ((DataRowView)v_softLicKeys[i]);
                slk.IdLicenseKey = ViewportHelper.ValueOrNull<int>(row, "ID LicenseKey");
                slk.IdLicense = ViewportHelper.ValueOrNull<int>(row, "ID License");
                slk.LicKey = ViewportHelper.ValueOrNull(row, "LicKey");
                list.Add(slk);
            }
            return list;
        }

        public override int GetRecordCount()
        {
            return v_snapshotSoftLicKeys.Count;
        }

        public override void MoveFirst()
        {
            v_snapshotSoftLicKeys.MoveFirst();
        }

        public override void MoveLast()
        {
            v_snapshotSoftLicKeys.MoveLast();
        }

        public override void MoveNext()
        {
            v_snapshotSoftLicKeys.MoveNext();
        }

        public override void MovePrev()
        {
            v_snapshotSoftLicKeys.MovePrevious();
        }

        public override bool CanMoveFirst()
        {
            return v_snapshotSoftLicKeys.Position > 0;
        }

        public override bool CanMovePrev()
        {
            return v_snapshotSoftLicKeys.Position > 0;
        }

        public override bool CanMoveNext()
        {
            return (v_snapshotSoftLicKeys.Position > -1) && (v_snapshotSoftLicKeys.Position < (v_snapshotSoftLicKeys.Count - 1));
        }

        public override bool CanMoveLast()
        {
            return (v_snapshotSoftLicKeys.Position > -1) && (v_snapshotSoftLicKeys.Position < (v_snapshotSoftLicKeys.Count - 1));
        }

        public override bool CanLoadData()
        {
            return true;
        }

        public override void LoadData()
        {
            dataGridView.AutoGenerateColumns = false;
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            softLicKeys = SoftLicKeysDataModel.GetInstance();
            // Дожидаемся дозагрузки данных, если это необходимо
            softLicKeys.Select();

            v_softLicKeys = new BindingSource
            {
                DataMember = "SoftLicKeys",
                Filter = StaticFilter
            };
            if (!string.IsNullOrEmpty(StaticFilter) && !String.IsNullOrEmpty(DynamicFilter))
                v_softLicKeys.Filter += " AND ";
            v_softLicKeys.Filter += DynamicFilter;
            v_softLicKeys.DataSource = DataSetManager.DataSet;

            if (ParentRow != null && ParentType == ParentTypeEnum.License)
                Text = string.Format(CultureInfo.InvariantCulture, "Лицензионные ключи лицензии №{0}", ParentRow["ID License"]);
            else
                throw new ViewportException("Неизвестный тип родительского объекта");

            //Инициируем колонки snapshot-модели
            for (var i = 0; i < softLicKeys.Select().Columns.Count; i++)
                snapshotsoftLicKeys.Columns.Add(new DataColumn(softLicKeys.Select().Columns[i].ColumnName, softLicKeys.Select().Columns[i].DataType));
            //Загружаем данные snapshot-модели из original-view
            for (var i = 0; i < v_softLicKeys.Count; i++)
                snapshotsoftLicKeys.Rows.Add(DataRowViewToArray(((DataRowView)v_softLicKeys[i])));
            v_snapshotSoftLicKeys = new BindingSource {DataSource = snapshotsoftLicKeys};
            v_snapshotSoftLicKeys.CurrentItemChanged += v_snapshotLicKeys_CurrentItemChanged;

            dataGridView.DataSource = v_snapshotSoftLicKeys;
            idLicenseKey.DataPropertyName = "ID LicenseKey";
            idLicense.DataPropertyName = "ID License";
            LicKey.DataPropertyName = "LicKey";
            dataGridView.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            dataGridView.CellValidated += dataGridView_CellValidated;
            //События изменения данных для проверки соответствия реальным данным в модели
            dataGridView.CellValueChanged += dataGridView_CellValueChanged;
            //Синхронизация данных исходные->текущие
            softLicKeys.Select().RowChanged += SoftLicKeysViewport_RowChanged;
            softLicKeys.Select().RowDeleting += SoftLicKeysViewport_RowDeleting;
            softLicKeys.Select().RowDeleted += SoftLicKeysViewport_RowDeleted;
        }

        public override bool CanInsertRecord()
        {
            return (ParentType == ParentTypeEnum.License) && (ParentRow != null) &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void InsertRecord()
        {
            if ((ParentRow == null) || (ParentType != ParentTypeEnum.License))
                return;
            var row = (DataRowView)v_snapshotSoftLicKeys.AddNew();
            row["ID License"] = ParentRow["ID License"];
            row.EndEdit();
        }

        public override bool CanDeleteRecord()
        {
            return (v_snapshotSoftLicKeys.Position != -1) &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void DeleteRecord()
        {
            ((DataRowView)v_snapshotSoftLicKeys[v_snapshotSoftLicKeys.Position]).Row.Delete();
        }

        public override bool CanCancelRecord()
        {
            return SnapshotHasChanges();
        }

        public override void CancelRecord()
        {
            snapshotsoftLicKeys.Clear();
            for (var i = 0; i < v_softLicKeys.Count; i++)
                snapshotsoftLicKeys.Rows.Add(DataRowViewToArray(((DataRowView)v_softLicKeys[i])));
            MenuCallback.EditingStateUpdate();
        }

        public override bool CanSaveRecord()
        {
            return SnapshotHasChanges() &&
                AccessControl.HasPrivelege(Priveleges.LicensesReadWrite);
        }

        public override void SaveRecord()
        {
            dataGridView.EndEdit();
            _syncViews = false;
            var list = SoftLicKeysFromViewport();
            if (!ValidateSoftLicKeys(list))
            {
                _syncViews = true;
                return;
            }
            for (var i = 0; i < list.Count; i++)
            {
                var row = softLicKeys.Select().Rows.Find(list[i].IdLicenseKey);
                if (row == null)
                {
                    var idLicKey = SoftLicKeysDataModel.Insert(list[i]);
                    if (idLicKey == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    ((DataRowView)v_snapshotSoftLicKeys[i])["ID LicenseKey"] = idLicKey;
                    softLicKeys.Select().Rows.Add(DataRowViewToArray((DataRowView)v_snapshotSoftLicKeys[i]));
                }
                else
                {
                    var softLicKeyFromView = RowToSoftLicKey(row);
                    if (softLicKeyFromView == list[i])
                        continue;
                    if (SoftLicKeysDataModel.Update(list[i]) == -1)
                    {
                        _syncViews = true;
                        return;
                    }
                    row["ID License"] = list[i].IdLicense == null ? DBNull.Value : (object)list[i].IdLicense;
                    row["LicKey"] = list[i].LicKey == null ? DBNull.Value : (object)list[i].LicKey;
                }
            }
            list = SoftLicKeysFromView();
            for (var i = 0; i < list.Count; i++)
            {
                var rowIndex = -1;
                for (var j = 0; j < dataGridView.Rows.Count; j++)
                    if ((dataGridView.Rows[j].Cells["idLicenseKey"].Value != null) &&
                        !string.IsNullOrEmpty(dataGridView.Rows[j].Cells["idLicenseKey"].Value.ToString()) &&
                        ((int)dataGridView.Rows[j].Cells["idLicenseKey"].Value == list[i].IdLicenseKey))
                        rowIndex = j;
                if (rowIndex != -1) continue;
                if (SoftLicKeysDataModel.Delete(list[i].IdLicenseKey.Value) == -1)
                {
                    _syncViews = true;
                    return;
                }
                softLicKeys.Select().Rows.Find(list[i].IdLicenseKey).Delete();
            }
            _syncViews = true;
            MenuCallback.EditingStateUpdate();
            //TODO тут будет вызов обновления вычисляемой модели доступных лицензионных ключей
        }

        public override bool CanDuplicate()
        {
            return true;
        }

        public override Viewport Duplicate()
        {
            var viewport = new SoftLicKeysViewport(this, MenuCallback);
            if (viewport.CanLoadData())
                viewport.LoadData();
            return viewport;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (SnapshotHasChanges())
            {
                var result = MessageBox.Show(@"Сохранить изменения о комнатах в базу данных?", @"Внимание",
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
            softLicKeys.Select().RowChanged -= SoftLicKeysViewport_RowChanged;
            softLicKeys.Select().RowDeleting -= SoftLicKeysViewport_RowDeleting;
            softLicKeys.Select().RowDeleted -= SoftLicKeysViewport_RowDeleted;
        }

        public override void ForceClose()
        {
            softLicKeys.Select().RowChanged -= SoftLicKeysViewport_RowChanged;
            softLicKeys.Select().RowDeleting -= SoftLicKeysViewport_RowDeleting;
            softLicKeys.Select().RowDeleted -= SoftLicKeysViewport_RowDeleted;
            Close();
        }


        private void v_snapshotLicKeys_CurrentItemChanged(object sender, EventArgs e)
        {
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            switch (cell.OwningColumn.Name)
            {
                case "LicKey":
                    if (cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString().Trim()))
                        cell.ErrorText = "Номер лицензионного ключа не может быть пустым";
                    else
                        if (cell.Value.ToString().Trim().Length > 200)
                            cell.ErrorText = "Длина лицензионного ключа не может превышать 200 символов";
                        else
                            cell.ErrorText = "";
                    break;
            }
        }

        private void SoftLicKeysViewport_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            MenuCallback.ForceCloseDetachedViewports();
            if (Selected)
            {
                MenuCallback.NavigationStateUpdate();
                MenuCallback.StatusBarStateUpdate();
                MenuCallback.EditingStateUpdate();
                MenuCallback.RelationsStateUpdate();
            }
        }

        private void SoftLicKeysViewport_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            if (e.Action != DataRowAction.Delete) return;
            var rowIndex = v_snapshotSoftLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]);
            if (rowIndex != -1)
                ((DataRowView)v_snapshotSoftLicKeys[rowIndex]).Delete();
        }

        private void SoftLicKeysViewport_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!_syncViews)
                return;
            var rowIndex = v_snapshotSoftLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]);
            if (rowIndex == -1 && v_softLicKeys.Find("ID LicenseKey", e.Row["ID LicenseKey"]) != -1)
            {
                snapshotsoftLicKeys.Rows.Add(e.Row["ID LicenseKey"], e.Row["ID License"], e.Row["LicKey"]);
            }
            else
                if (rowIndex != -1)
                {
                    var row = ((DataRowView)v_snapshotSoftLicKeys[rowIndex]);
                    row["ID License"] = e.Row["ID License"];
                    row["LicKey"] = e.Row["LicKey"];
                }
            if (!Selected) return;
            MenuCallback.NavigationStateUpdate();
            MenuCallback.StatusBarStateUpdate();
            MenuCallback.EditingStateUpdate();
            MenuCallback.RelationsStateUpdate();
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MenuCallback.EditingStateUpdate();
        }

        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftLicKeysViewport));
            dataGridView = new DataGridView();
            idLicenseKey = new DataGridViewTextBoxColumn();
            idLicense = new DataGridViewTextBoxColumn();
            LicKey = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BackgroundColor = System.Drawing.Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
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
            dataGridView.Columns.AddRange(idLicenseKey, idLicense, LicKey);
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new System.Drawing.Point(3, 3);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.Size = new System.Drawing.Size(795, 333);
            dataGridView.TabIndex = 0;
            // 
            // idLicenseKey
            // 
            idLicenseKey.HeaderText = @"Внутренний номер лицензионного ключа";
            idLicenseKey.Name = "idLicenseKey";
            idLicenseKey.Visible = false;
            // 
            // idLicense
            // 
            idLicense.HeaderText = @"Внутренний номер лицензии";
            idLicense.Name = "idLicense";
            idLicense.Visible = false;
            // 
            // LicKey
            // 
            LicKey.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            LicKey.HeaderText = @"Лицензионный ключ";
            LicKey.MinimumWidth = 150;
            LicKey.Name = "LicKey";
            // 
            // SoftLicKeysViewport
            // 
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(801, 339);
            Controls.Add(dataGridView);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "SoftLicKeysViewport";
            Padding = new Padding(3);
            Text = @"Перечень лицензионных ключей";
            ((System.ComponentModel.ISupportInitialize)(dataGridView)).EndInit();
            ResumeLayout(false);

        }
    }
}
