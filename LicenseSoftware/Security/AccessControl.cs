using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LicenseSoftware.DataModels;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace Security
{
    public static class AccessControl
    {
        private static uint priveleges;
        private static string query = @"SELECT dbo.f_user_privileges()";

        public static void LoadPriveleges()
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = query;
                try
                {
                    DataTable table = connection.SqlSelectTable("privileges", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул привелегии для данного пользователя", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        priveleges = 0;
                    }
                    priveleges = Convert.ToUInt32(table.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    priveleges = 0;
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Не удалось загрузить привелегии пользователя. Подробная ошибка: {0}", 
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }

        public static bool HasPrivelege(Priveleges privelege)
        {
            return ((uint)priveleges & (uint)privelege) == (uint)privelege;
        }

        public static bool HasNoPriveleges()
        {
            return (uint)priveleges == 0;
        }
    }
}
