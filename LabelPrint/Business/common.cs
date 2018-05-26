using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyHttp.Http;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using System.Drawing;
using DevExpress.Utils;
using System.IO;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Collections;

namespace LabelPrint.Business
{
    public static class Common
    {
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static int GetWeekOfYear()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            return cal.GetWeekOfYear(DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        public static DataTable ToDataTableClass<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ToDataTable(List<ExpandoObject> list, string tableName = "myTable", int? _length = null)
        {
            if (list == null || list.Count == 0) return null;
            //build columns
            var props = _length != null ? (IDictionary<string, object>)list[Convert.ToInt32(_length)] : (IDictionary<string, object>)list[0];
            var t = new DataTable(tableName);
            Type type;
            foreach (var prop in props)
            {
                type = typeof(string);
                if (prop.Value != null)
                {
                    type = prop.Value.GetType();
                }

                t.Columns.Add(new DataColumn(prop.Key, type));
            }
            //add rows
            foreach (var row in list)
            {
                var data = t.NewRow();
                foreach (var prop in (IDictionary<string, object>)row)
                {
                    try { data[prop.Key] = prop.Value; } catch { }
                }
                t.Rows.Add(data);
            }
            return t;
        }

        public static DataTable ToDataTable_ColumnString(List<ExpandoObject> list, string tableName = "myTable")
        {
            if (list == null || list.Count == 0) return null;
            //build columns
            var props = (IDictionary<string, object>)list[0];
            var t = new DataTable(tableName);
            Type type;
            foreach (var prop in props)
            {
                type = typeof(string);
                t.Columns.Add(new DataColumn(prop.Key, type));
            }
            //add rows
            foreach (var row in list)
            {
                var data = t.NewRow();
                foreach (var prop in (IDictionary<string, object>)row)
                {
                    data[prop.Key] = prop.Value;
                }
                t.Rows.Add(data);
            }
            return t;
        }

        public static Dictionary<string, string> DataRowToList(DataRow r, DataTable t)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();


            foreach (DataColumn col in t.Columns)
            {
                list.Add(col.ColumnName, r[col.ColumnName].ToString());
            }

            return list;
        }

        public static List<Dictionary<string, string>> DataTableToList(DataTable t)
        {

            List<Dictionary<string, string>> _list = new List<Dictionary<string, string>>();

            foreach (DataRow myR in t.Rows)
            {
                _list.Add(DataRowToList(myR, t));
            }

            return _list;

        }

        public static DataTable ToDataTable(dynamic[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                DataTable dt = new DataTable("tmpTable");/*foreach (PropertyInfo pi in t.GetProperties())
                {
                    dt.Columns.Add(new DataColumn(pi.Name));
                }*/
                foreach (KeyValuePair<string, object> di in objects[0])
                {
                    dt.Columns.Add(di.Key);
                }

                foreach (Dictionary<string, object> o in objects)
                {
                    DataRow dr = dt.NewRow();
                    foreach (KeyValuePair<string, object> od in o)
                    {
                        dr[od.Key] = od.Value;
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return null;
        }

        public static string[] getDigiResc(int iNeed)
        {
            string[] jData = null;

            string url = "v2/inv/digital-resources/requestAllocating";

            var param = new
            {
                number = iNeed
            };

            HttpResponse res = HTTP.Instance.Get(url, param);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (res.DynamicBody.rtn.status == "OK")
                    return res.DynamicBody.rtn.data;
                else
                {
                    return null;
                }
            }

            return null;
        }

        public static int MyDiv(int a, int b, out int c)
        {
            if (b <= 0) { c = 0; return c; }
            int _div_result = a / b;
            int _remain_result = a % b;

            if (_remain_result > 0)
                c = _div_result + 1;
            else
            {
                c = _div_result;
            }
            return c;
        }

        public static int? ConvertInt(dynamic value)
        {
            try
            {
                if (value == null) return null;
                return Convert.ToInt32(value);
            }
            catch { return null; }
        }

        public static int? ConvertLong(dynamic value)
        {
            try
            {
                if (value == null) return null;
                return Convert.ToInt64(value);
            }
            catch { return null; }
        }

        public static byte? ConvertByte(dynamic value)
        {
            try
            {
                if (value == null) return null;
                return Convert.ToByte(value);
            }
            catch { return null; }
        }

        public static dynamic ConvertDouble(dynamic value)
        {
            try
            {
                if (value == null) return null;
                return Convert.ToDouble(value);
            }
            catch { return null; }
        }

        public static dynamic ConvertDatetime(dynamic value)
        {
            try
            {
                if (value == null) return null;
                return Convert.ToDateTime(value);
            }
            catch { return null; }
        }

        public static string convertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static dynamic caller(String myclass, String method, object[] _params = null)
        {
            try
            {
                // Get a type from the string 
                Type type = Type.GetType(myclass);
                // Create an instance of that type
                Object obj = Activator.CreateInstance(type);
                // Retrieve the method you are looking for
                MethodInfo methodInfo = type.GetMethod(method);
                // Invoke the method on the instance we created above
                return methodInfo.Invoke(obj, _params);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static string HexToBinary(string hex)
        {
            return String.Join(String.Empty, hex.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
        }

        public static string BinaryToHex(string binary)
        {
            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... Will throw otherwise

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }

        public static string MD5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }

        public static int FindMaxInListObject(List<dynamic> list, string field)
        {
            if (list.Count == 0)
                //throw new InvalidOperationException("Empty list");
                return -1;

            int max = int.MinValue;
            try
            {
                foreach (dynamic element in list)
                {
                    int value = ConvertInt(element[field]);
                    if (value > max)
                    {
                        max = value;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return max;
        }

        public static List<string> ListDynamicToListString(dynamic myList, string key)
        {
            List<string> Result = new List<string>();
            try
            {
                foreach (var data in myList)
                {
                    Result.Add((string)data.GetType().GetProperty(key).GetValue(data, null));
                }
            }
            catch { }
            return Result;
        }

        public static List<string> ListDynamicToListString(dynamic myList)
        {
            List<string> Result = new List<string>();
            try
            {
                foreach (var data in myList)
                {
                    Result.Add((string)data);
                }
            }
            catch { }
            return Result;
        }

        public static List<string> CreateSequential(int nextNumber, int step, int length, int quantity, string prefix = "")
        {
            List<string> result = new List<string>();
            string form = new String('0', length);
            int i = nextNumber;
            while (i < (nextNumber + quantity))
            {
                string replace = form.Substring(0, length - i.ToString().Length);
                result.Add(prefix + replace + i.ToString());
                i++;
            }
            return result;
        }

        public static ExpandoObject ToExpando(this string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IDictionary<string, object> dictionary = serializer.Deserialize<IDictionary<string, object>>(json);
            return Expando(dictionary);
        }

        public static ExpandoObject Expando(this IDictionary<string, object> dictionary)
        {
            ExpandoObject expandoObject = new ExpandoObject();
            IDictionary<string, object> objects = expandoObject;

            foreach (var item in dictionary)
            {
                bool processed = false;

                if (item.Value is IDictionary<string, object>)
                {
                    objects.Add(item.Key, Expando((IDictionary<string, object>)item.Value));
                    processed = true;
                }
                else if (item.Value is ICollection)
                {
                    List<object> itemList = new List<object>();

                    foreach (var item2 in (ICollection)item.Value)

                        if (item2 is IDictionary<string, object>)
                            itemList.Add(Expando((IDictionary<string, object>)item2));
                        else
                            itemList.Add(Expando(new Dictionary<string, object> { { "Unknown", item2 } }));

                    if (itemList.Count > 0)
                    {
                        objects.Add(item.Key, itemList);
                        processed = true;
                    }
                }

                if (!processed)
                    objects.Add(item);
            }

            return expandoObject;
        }

        public static string DictionaryStringToJson(this Dictionary<string, string> dictionary)
        {
            var kvs = dictionary.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, string.Concat(",", kvp.Value)));
            return string.Concat("{", string.Join(",", kvs), "}");
        }

        public static Dictionary<string, string> FromJsonToDictionary(this string json)
        {
            string[] keyValueArray = json.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');
            return keyValueArray.ToDictionary(item => item.Split(':')[0], item => item.Split(':')[1]);
        }

        public static string DictionaryObjectToJson(Dictionary<string, object> dict)
        {
            string entries = "";
            foreach (KeyValuePair<string, object> entry in dict)
            {
                Type type = entry.Value.GetType();
                if ((entry.Value.ToString().Trim().Substring(0, 1) == "[") || (entry.Value.ToString().Trim().Substring(0, 1) == "{"))
                {
                    if (entries.Length > 0)
                        entries = entries + "," + "\"" + entry.Key + "\":" + entry.Value.ToString();
                    else
                        entries = "\"" + entry.Key + "\":" + entry.Value.ToString();
                }
                else if (type.Name.ToLower().Contains("string"))
                {
                    if (entries.Length > 0)
                        entries = entries + "," + "\"" + entry.Key + "\":\"" + entry.Value.ToString() + "\"";
                    else
                        entries = "\"" + entry.Key + "\":\"" + entry.Value.ToString() + "\"";
                }
                else
                {
                    if (entries.Length > 0)
                        entries = entries + "," + "\"" + entry.Key + "\":" + entry.Value.ToString();
                    else
                        entries = "\"" + entry.Key + "\":" + entry.Value.ToString();
                }
            }
            return "{" + entries + "}";
        }

        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
    }

    public static class DataTableExt
    {
        public static IList<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            IList<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        public static IList<T> ToList<T>(this DataTable table, Dictionary<string, string> mappings) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            IList<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties, mappings);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                property.SetValue(item, row[property.Name], null);
            }
            return item;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties, Dictionary<string, string> mappings) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (mappings.ContainsKey(property.Name))
                    property.SetValue(item, row[mappings[property.Name]], null);
            }
            return item;
        }

        public static DataRow[] SearchExpression(DataTable datatable, string search)
        {
            DataRow[] foundRows = datatable.Select(search);
            return foundRows;
        }

        public static DataRow FindInMultiPKey(DataTable datatable, string valueSearch)
        {
            // Create an array for the key values to find.
            object[] findTheseVals = new object[1];

            // Set the values of the keys to find.
            findTheseVals[0] = valueSearch;

            DataRow foundRow = datatable.Rows.Find(findTheseVals);
            return foundRow;
        }

        public static DataRow FindByKey(DataTable datatable, string key, string value)
        {
            for (int i = 0; i < datatable.Rows.Count; i++)
            {
                if (datatable.Rows[i][key] == value)
                {
                    return datatable.Rows[i];
                }
            }
            return null;
        }
    }

    public class GridView
    {
        public static void AddGridColumn(DevExpress.XtraGrid.Views.Grid.GridView GridView, string ColName, string FieldName, string Caption, bool Visible = true, bool AllowEdit = false)
        {
            // Create an unbound column.
            GridColumn unbColumn = GridView.Columns.AddField(ColName);
            unbColumn.VisibleIndex = GridView.Columns.Count;
            unbColumn.FieldName = FieldName;
            unbColumn.Caption = Caption;
            unbColumn.Visible = Visible;

            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Bound;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = AllowEdit;
            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
            //unbColumn.DisplayFormat.FormatString = "c";
            // Customize the appearance settings.
            unbColumn.AppearanceCell.BackColor = Color.LemonChiffon;
        }

        public static int FindRowIndex(DevExpress.XtraGrid.Views.Grid.GridView GridView, string column, string value)
        {
            int i;
            for (i = 0; i < GridView.DataRowCount; i++)
            {
                if (GridView.GetRowCellValue(i, column).ToString().Contains(value))
                {
                    return i;
                }
            }
            return -1;
        }      

        public static List<string> GetListStringColumnValue(DevExpress.XtraGrid.Views.Grid.GridView view, string fieldName)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < view.DataRowCount; i++)
            {
                result.Add(view.GetRowCellValue(i, fieldName).ToString());
            }

            return result;
        }

        public static object[] GetListColumnValue(DevExpress.XtraGrid.Views.Grid.GridView view, string fieldName)
        {
            int[] selectedRows = view.GetSelectedRows();
            object[] result = new object[selectedRows.Length];
            for (int i = 0; i < selectedRows.Length; i++)
            {
                int rowHandle = selectedRows[i];
                if (!view.IsGroupRow(rowHandle))
                {
                    result[i] = view.GetRowCellValue(rowHandle, fieldName);
                }
                else
                    result[i] = null; // default value
            }
            return result;
        }
    }

    public class DateEdit
    {
        public static void Format(DevExpress.XtraEditors.DateEdit DateControl, string FormatString = "dd/MM/yyyy hh:mm tt")
        {
            DateControl.Properties.DisplayFormat.FormatType = FormatType.DateTime;
            DateControl.Properties.DisplayFormat.FormatString = FormatString;
        }
    }

    public class Paging
    {
        private int _currentPage = 1;
        private int _pageCount;
        private int _numberOfItem;

        public int NumberOfItem
        {
            get { return _numberOfItem; }
            set { _numberOfItem = value; }
        }

        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = value; }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }
    }
}
