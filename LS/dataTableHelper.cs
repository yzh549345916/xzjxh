using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
namespace LS
{
    /// <summary> 
    /// DataTable操作 
    /// </summary> 
    public static class DataTableHelper
    {
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item);
                table.Rows.Add(row);
            }
            return table;
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;
            if (rows != null)
            {
                list = new List<T>();
                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }
            return list;
        }

        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
                return null;

            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in table.Rows)
                rows.Add(row);

            return ConvertTo<T>(rows);
        }

        //Convert DataRow into T Object
        public static T CreateItem<T>(DataRow row)
        {
            string columnName;
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    columnName = column.ColumnName;
                    //Get property with same columnName
                    PropertyInfo prop = obj.GetType().GetProperty(columnName);
                    try
                    {
                        //Get value for the column
                        object value = (row[columnName].GetType() == typeof(DBNull))
                        ? null : row[columnName];
                        //Set property value
                        if (prop.CanWrite)    //判断其是否可写
                            prop.SetValue(obj, value, null);
                    }
                    catch
                    {
                        throw;
                        //Catch whatever here
                    }
                }
            }
            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, prop.PropertyType);

            return table;
        }
    }
    public class LinqList<T> : IEnumerable<T>, IEnumerable
    {
        IEnumerable items;
        internal LinqList(IEnumerable items)
        {
            this.items = items;
        }
        #region IEnumerable<DataRow> Members 
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (T item in items) yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<T> ie = this;
            return ie.GetEnumerator();
        }
        #endregion

    }

    public static class Projecttable
    {
        public static DataTable CopyToDataTable<T>(this IEnumerable<T> array)
        {
            var ret = new DataTable();
            foreach (PropertyDescriptor dp in TypeDescriptor.GetProperties(typeof(T)))
                // if (!dp.IsReadOnly) 
                ret.Columns.Add(dp.Name, dp.PropertyType);
            foreach (T item in array)
            {
                var Row = ret.NewRow();
                foreach (PropertyDescriptor dp in TypeDescriptor.GetProperties(typeof(T)))
                    // if (!dp.IsReadOnly) 
                    Row[dp.Name] = dp.GetValue(item);
                ret.Rows.Add(Row);
            }
            return ret;
        }
    }
}