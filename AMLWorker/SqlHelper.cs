using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Logger;
using Microsoft.Data.Sqlite;

namespace AMLWorker
{
    public class PrimaryKeyAttribute : Attribute
    {
        
    }
    public class SqlHelper
    {
        public static bool TableExists(SqliteConnection conn,String tableName)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}';";
                using (var rdr = cmd.ExecuteReader())
                {
                    return rdr.HasRows;
                }
            }
        }

        static String textColumns(IEnumerable<String> g)
        {
            var z = g.Aggregate("", (x, y) => x + y + ",");
            return z.TrimEnd(',');
        }

        public static int ExecuteCommandLog(SqliteConnection conn, String cmdText)
        {
            L.Trace($"Executing - {cmdText}");
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                return cmd.ExecuteNonQuery();
            }
        }

        public static int CreateTextSearchTable(SqliteConnection conn, string tableName,IEnumerable<String> columns)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"create virtual table {tableName} using fts4({textColumns(columns)});";
                return cmd.ExecuteNonQuery();
            }
        }

        public static int CreateTable<T>(SqliteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = CreateTableCommand<T>();
                return cmd.ExecuteNonQuery();
            }
        }

        public static void CreateAnyNewProperties<T>(SqliteConnection conn)
        {
            List<String> columnNames = new List<string>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"pragma table_info({typeof(T).Name};";
                using (var rdr = cmd.ExecuteReader())
                {
                    columnNames.Add(rdr.GetString(1));
                }
            }

            foreach (var t in typeof(T).GetProperties(BindingFlags.Public))
            {
                if (columnNames.Contains(t.Name) == false)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"alter table {typeof(T).Name} add column {GetCreateFieldString(t)}";
                        var ret = cmd.ExecuteNonQuery();
                        L.Trace($"cmd - {cmd.CommandText} returned {ret}");
                    }
                }
            }
        }

        static String CreateTableCommand<T>()
        {
            var cmd = $"create table {typeof(T).Name} (";

            foreach (var c in typeof(T).GetProperties(BindingFlags.Public))
            {
                cmd += GetCreateFieldString(c);
                cmd += ",";
            }
            cmd = cmd.TrimEnd(',');

            cmd += ");";

            return cmd;

        }

        static String GetCreateFieldString(PropertyInfo pi)
        {
            var ret = "";
            if (pi.PropertyType == typeof(String))
            {
                ret = $"{pi.Name} TEXT ";
            }
            else if (pi.PropertyType == typeof(Int32) || pi.PropertyType == typeof(Int64) || pi.PropertyType == typeof(DateTime))
            {
                ret = $"{pi.Name} NUMERIC ";
            }
            else
            {
                throw new Exception($"Need to finish adding data-types - {pi.PropertyType.Name} not implemented");
            }

            if (pi.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                ret += "primary key ";
            return ret;
        }
    }
}
