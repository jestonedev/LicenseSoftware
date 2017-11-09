using System;
using LicenseSoftware.DataModels;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace Security
{
    public static class AccessControl
    {
        private static uint _priveleges;
        private const string QueryUser = @"SELECT TOP 1 *
                                        FROM
                                            dbo.ACLUsers
                                        WHERE
                                            UserName = suser_sname()";

        public static void LoadPriveleges()
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                try
                {
                    command.CommandText = QueryUser;
                    var table = connection.SqlSelectTable("privileges", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул привелегии для данного пользователя", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        _priveleges = 0;
                        UserId = 0;
                        return;
                    }
                    _priveleges = Convert.ToUInt32(table.Rows[0][2], CultureInfo.InvariantCulture);
                    UserId = Convert.ToInt32(table.Rows[0][0], CultureInfo.InvariantCulture);                    
                }
                catch (SqlException e)
                {
                    _priveleges = 0;
                    UserId = 0;
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Не удалось загрузить привелегии пользователя. Подробная ошибка: {0}", 
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }

        public static int UserId { get; private set; }

        public static bool HasPrivelege(Priveleges privelege)
        {
            return (_priveleges & (uint)privelege) == (uint)privelege;
        }

        public static bool HasNoPriveleges()
        {
            return _priveleges == 0;
        }
    }
}
