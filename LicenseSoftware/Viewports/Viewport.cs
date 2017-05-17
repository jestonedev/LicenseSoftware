using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LicenseSoftware.Entities;
using WeifenLuo.WinFormsUI.Docking;
using LicenseSoftware.Reporting;
using LicenseSoftware.SearchForms;

namespace LicenseSoftware.Viewport
{
    public class Viewport: DockContent, IMenuController
    {
        private IMenuCallback menuCallback;

        protected IMenuCallback MenuCallback { get { return menuCallback; } set { menuCallback = value; } }
        private bool selected_ = false;

        
        public string StaticFilter { get; set; }
        public string DynamicFilter { get; set; }
        public DataRow ParentRow { get; set; }
        public ParentTypeEnum ParentType { get; set; }

        protected Viewport(): this(null)
        {
        }

        protected Viewport(IMenuCallback menuCallback)
        {
            StaticFilter = "";
            DynamicFilter = "";
            ParentRow = null;
            ParentType = ParentTypeEnum.None;
            this.MenuCallback = menuCallback;
        }

        public new virtual void Close()
        {
            MenuCallback.SwitchToPreviousViewport();
            base.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MenuCallback.SwitchToPreviousViewport();
            base.OnClosing(e);
        }

        public virtual int GetRecordCount()
        {
            return 0;
        }

        public virtual Viewport Duplicate()
        { 
            return this;
        }

        public virtual bool CanDuplicate()
        {
            return false;
        }

        public virtual bool CanLoadData()
        {
            return false;
        }

        public virtual void LoadData()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveFirst()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MovePrev()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveNext()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void MoveLast()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool CanMoveFirst()
        {
            return false;
        }

        public virtual bool CanMovePrev()
        {
            return false;
        }

        public virtual bool CanMoveNext()
        {
            return false;
        }

        public virtual bool CanMoveLast()
        {
            return false;
        }

        public virtual void SaveRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void CancelRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void CopyRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void InsertRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void DeleteRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void OpenDetails()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void DataRefresh()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void SearchRecord()
        {
            throw new ViewportException("Не реализовано");
        }

        protected virtual Viewport ShowAssocViewport(IMenuCallback menuCallback, ViewportType viewportType, 
            string staticFilter, DataRow parentRow, ParentTypeEnum parentType)
        {
            if (menuCallback == null)
                throw new ViewportException("Не заданна ссылка на интерфейс menuCallback");
            Viewport viewport = ViewportFactory.CreateViewport(menuCallback, viewportType);
            viewport.StaticFilter = staticFilter;
            viewport.ParentRow = parentRow;
            viewport.ParentType = parentType;
            if ((viewport as IMenuController).CanLoadData())
                (viewport as IMenuController).LoadData();
            menuCallback.AddViewport(viewport);
            return viewport;
        }

        public virtual void ClearSearch()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual bool CanSaveRecord()
        {
            return false;
        }

        public virtual bool CanCancelRecord()
        {
            return false;
        }

        public virtual bool CanCopyRecord()
        {
            return false;
        }

        public virtual bool CanInsertRecord()
        {
            return false;
        }

        public virtual bool CanDeleteRecord()
        {
            return false;
        }

        public virtual bool CanOpenDetails()
        {
            return false;
        }

        public virtual bool CanSearchRecord()
        {
            return false;
        }

        public virtual bool SearchedRecords()
        {
            return false;
        }

        public virtual void ForceClose()
        {
            Dispose();
        }

        public virtual bool ViewportDetached()
        {
            return ((ParentRow != null) && ((ParentRow.RowState == DataRowState.Detached) || (ParentRow.RowState == DataRowState.Deleted)));
        }

        public bool Selected
        {
            get
            {
                return selected_;
            }
            set
            {
                selected_ = value;
            }
        }

        public virtual bool HasAssocLicenses()
        {
            return false;
        }

        public virtual bool HasAssocInstallations()
        {
            return false;
        }

        public virtual bool HasAssocLicKeys()
        {
            return false;
        }

        public virtual bool HasAssocSoftVersions()
        {
            return false;
        }

        public virtual void ShowAssocLicenses()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowAssocInstallations()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowAssocLicKeys()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual void ShowAssocSoftVersions()
        {
            throw new ViewportException("Не реализовано");
        }

        public virtual List<string> GetIdLicenses()
        {
            throw new ViewportException("Не реализовано");
        }
        public virtual List<string> GetIdInstallations()
        {
            throw new ViewportException("Не реализовано");
        }
    }
}
