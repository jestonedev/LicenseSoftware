using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;
using Settings;

namespace LicenseSoftware.DataModels
{
    public sealed class DBConnection: IDisposable
    {
        private static string _providerName = "SQLClient";
        private static readonly DbProviderFactory Factory = DbProviderFactories.GetFactory(ParseProviderName(_providerName));

        private DbTransaction _transaction;
        private readonly DbConnection _connection;

        public DBConnection()
        {
            _connection = Factory.CreateConnection();
            if (_connection == null) return;
            _connection.ConnectionString = LicenseSoftwareSettings.ConnectionString;
            if (_connection.State != ConnectionState.Closed) return;
            try
            {
                _connection.Open();
            }
            catch(SqlException e)
            {
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                    "Произошла ошибка при установке соединения с базой данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Application.Exit();
            }
        }

        public static DbCommand CreateCommand()
        {
            return Factory.CreateCommand();
        }

        public static DbParameter CreateParameter<T>(string name, T value) 
        {
            var parameter = Factory.CreateParameter();
            if (parameter == null) return null;
            parameter.ParameterName = name;
            parameter.Value = value == null ? DBNull.Value : (object)value;
            return parameter;
        }

        private static string ParseProviderName(string name)
        {
            var dt = DbProviderFactories.GetFactoryClasses();
            var providers = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                providers.Add(row["InvariantName"].ToString());
            }
            foreach (var provider in providers)
            {
                if (Regex.IsMatch(provider, name, RegexOptions.IgnoreCase))
                    return provider;
            }
            throw new DataModelException(string.Format(CultureInfo.InvariantCulture, "Провайдер {0} не найден", name));
        }

        public DataTable SqlSelectTable(string resultTableName, DbCommand command)
        {
            if (command == null)
                throw new DataModelException("Не передана ссылка на исполняемую команду SQL");
            command.Connection = _connection;
            if (_transaction != null)
                command.Transaction = _transaction;
            if (_connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение с базой данных прервано по неизвестным причинам");
            var adapter = Factory.CreateDataAdapter();
            if (adapter == null) return null;
            adapter.SelectCommand = command;
            var dt = new DataTable(resultTableName) {Locale = CultureInfo.InvariantCulture};
            adapter.Fill(dt);
            return dt;
        }

        public int SqlExecuteNonQuery(DbCommand command)
        {
            if (command == null)
                throw new DataModelException("Не передана ссылка на исполняемую команду SQL");
            command.Connection = _connection;
            if (_transaction != null)
                command.Transaction = _transaction;
            if (_connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение прервано по неизвестным причинам");
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (SqlException)
            {
                SqlRollbackTransaction();
                throw;
            }
        }

        public object SqlExecuteScalar(DbCommand command)
        {
            if (command == null)
                throw new DataModelException("Не передана ссылка на исполняемую команду SQL");
            command.Connection = _connection;
            if (_transaction != null)
                command.Transaction = _transaction;
            if (_connection.State == ConnectionState.Closed)
                throw new DataModelException("Соединение прервано по неизвестным причинам");
            try
            {
                return command.ExecuteScalar();
            }
            catch (SqlException)
            {
                SqlRollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// Начать выполнение транзакции
        /// </summary>
        public void SqlBeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// Подтверждение транзакции
        /// </summary>
        public void SqlCommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        /// <summary>
        /// Откат транзакции
        /// </summary>
        public void SqlRollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
