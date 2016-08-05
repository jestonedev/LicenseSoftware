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
using System.Collections.ObjectModel;

namespace Security
{
    public static class AccessControl
    {
        private static uint priveleges;
        private static int idCurrentUser;
        private static string queryUser = @"SELECT TOP 1 *
                                        FROM
                                            dbo.ACLUsers
                                        WHERE
                                            UserName = suser_sname()";        

        public static void LoadPriveleges()
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                try
                {
                    command.CommandText = queryUser;
                    DataTable table = connection.SqlSelectTable("privileges", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул привелегии для данного пользователя", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        priveleges = 0;
                        idCurrentUser = 0;
                        return;
                    }
                    priveleges = Convert.ToUInt32(table.Rows[0][2], CultureInfo.InvariantCulture);
                    idCurrentUser = Convert.ToInt32(table.Rows[0][0], CultureInfo.InvariantCulture);                    
                }
                catch (SqlException e)
                {
                    priveleges = 0;
                    idCurrentUser = 0;
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Не удалось загрузить привелегии пользователя. Подробная ошибка: {0}", 
                        e.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }

        public static int UserID
        {
            get { return idCurrentUser; }
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
