// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "LicenseSoftware.CalcDataModels")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "LicenseSoftware.CalcDataModels.CalcDataModel.#Dispose()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "type", Target = "LicenseSoftware.CalcDataModels.CalcDataModel")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "LicenseSoftware.CalcDataModels.CalcDataModelLicensesConcat.#InitializeTable()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "LicenseSoftware.CalcDataModels.CalcDataModelLicensesConcat.#GetInstance()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "LicenseSoftware.CalcDataModels.CalcDataModelSoftwareConcat.#InitializeTable()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "LicenseSoftware.DataModels.DBConnection.#SqlSelectTable(System.String,System.Data.Common.DbCommand)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "LicenseSoftware.DataModels.DepartmentsDataModel.#SelectVisibleDepartments()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "LicenseSoftware.DataModels.DepartmentsDataModel.#SortResultDepartments(System.Data.DataTable)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "LicenseSoftware.CalcDataModels.CalcDataModelSoftwareConcat.#GetInstance()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "LicenseSoftware.DataModels.CalcDataModelsUpdater.#Run()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.DataModel.#.ctor(System.Windows.Forms.ToolStripProgressBar,System.Int32,System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Select", Scope = "member", Target = "LicenseSoftware.DataModels.DataModel.#Select()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IDs", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelHelper.#GetLicenseIDsByCondition(System.Func`2<System.Data.DataRow,System.Boolean>,LicenseSoftware.Entities.EntityType)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IDs", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelHelper.#GetSoftwareIDsBySoftType(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IDs", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelHelper.#GetSoftwareIDsBySoftMaker(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IDs", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelHelper.#GetComputerIDsByDepartment(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "LicenseSoftware.DataModels.DepartmentsDataModel.#TabulateResultDepartments(System.Data.DataTable)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "LicenseSoftware.DataModels.DepartmentsDataModel.#SortResultDepartments(System.Data.DataTable)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftInstallationsDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftInstallationsDataModel.#Update(LicenseSoftware.Entities.SoftInstallation)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftInstallationsDataModel.#Insert(LicenseSoftware.Entities.SoftInstallation)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftInstallatorsDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftInstallatorsDataModel.#Update(LicenseSoftware.Entities.SoftInstallator)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftInstallatorsDataModel.#Insert(LicenseSoftware.Entities.SoftInstallator)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicDocTypesDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicDocTypesDataModel.#Update(LicenseSoftware.Entities.SoftLicDocType)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicDocTypesDataModel.#Insert(LicenseSoftware.Entities.SoftLicDocType)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicensesDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicensesDataModel.#Update(LicenseSoftware.Entities.SoftLicense)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicensesDataModel.#Insert(LicenseSoftware.Entities.SoftLicense)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicKeysDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicKeysDataModel.#Update(LicenseSoftware.Entities.SoftLicKey)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicKeysDataModel.#Insert(LicenseSoftware.Entities.SoftLicKey)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicTypesDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicTypesDataModel.#Update(LicenseSoftware.Entities.SoftLicType)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftLicTypesDataModel.#Insert(LicenseSoftware.Entities.SoftLicType)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftMakersDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftMakersDataModel.#Update(LicenseSoftware.Entities.SoftMaker)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftMakersDataModel.#Insert(LicenseSoftware.Entities.SoftMaker)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftSuppliersDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftSuppliersDataModel.#Update(LicenseSoftware.Entities.SoftSupplier)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftSuppliersDataModel.#Insert(LicenseSoftware.Entities.SoftSupplier)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftTypesDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftTypesDataModel.#Update(LicenseSoftware.Entities.SoftType)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftTypesDataModel.#Insert(LicenseSoftware.Entities.SoftType)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftwareDataModel.#Delete(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftwareDataModel.#Update(LicenseSoftware.Entities.Software)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.SoftwareDataModel.#Insert(LicenseSoftware.Entities.Software)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelsCallbackUpdater.#Initialize()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelsCallbackUpdater.#Run()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "operation_type", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelsCallbackUpdater.#CalcDataModelsUpdate(System.String,System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "field_name", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelsCallbackUpdater.#CalcDataModelsUpdate(System.String,System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "LicenseSoftware.DataModels.DataModelsCallbackUpdater.#GetInstance()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "LicenseSoftware.DataModels.DepartmentsDataModel.#SelectVisibleDepartments()")]
