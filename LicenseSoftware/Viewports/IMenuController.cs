using LicenseSoftware.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LicenseSoftware.SearchForms;

namespace LicenseSoftware.Viewport
{
    public interface IMenuController
    {
        Viewport Duplicate();
        void LoadData();
        void MoveFirst();
        void MovePrev();
        void MoveNext();
        void MoveLast();
        void SaveRecord();
        void CancelRecord();
        void CopyRecord();
        void InsertRecord();
        void DeleteRecord();
        void OpenDetails();
        void SearchRecord();
        void ClearSearch();
        void Close();
        void ForceClose();

        bool CanDuplicate();
        bool CanLoadData();
        bool CanMoveFirst();
        bool CanMovePrev();
        bool CanMoveNext();
        bool CanMoveLast();
        bool CanSaveRecord();
        bool CanCancelRecord();
        bool CanCopyRecord();
        bool CanInsertRecord();
        bool CanDeleteRecord();
        bool CanOpenDetails();
        bool CanSearchRecord();
        bool SearchedRecords();
        bool ViewportDetached();

        int GetRecordCount();

        bool Selected { get; set; }

        bool HasAssocLicenses();

        bool HasAssocInstallations();

        bool HasAssocLicKeys();

        void ShowAssocLicenses();

        void ShowAssocInstallations();

        void ShowAssocLicKeys();
    }
}
