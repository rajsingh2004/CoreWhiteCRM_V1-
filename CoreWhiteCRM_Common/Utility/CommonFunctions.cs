using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreWhiteCRM_Common.Utility
{
    public class CommonFunctions
    {
        public static List<T> ConvertTo<T>(DataTable datatable) where T : new()
        {
            List<T> lstData = new List<T>();
            try
            {
                List<string> columnnames = datatable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                List<PropertyInfo> Properties = typeof(T).GetProperties().Where(p => p.GetCustomAttribute(typeof(DBHelpAttribute)) != null).ToList();
                
                lstData.AddRange(datatable.AsEnumerable().Select(row =>
                {

                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in Properties)
                    {
                        DBHelpAttribute ObjHelpAttribute = pro.GetCustomAttribute(typeof(DBHelpAttribute)) as DBHelpAttribute;
                        var columnname = columnnames.Find(name => name.ToLower() == ObjHelpAttribute.ParameterName.ToLower());
                        if (!string.IsNullOrEmpty(columnname))
                        {
                            var value = row[columnname].ToString();
                            if (Nullable.GetUnderlyingType(pro.PropertyType) != null)
                            {
                                pro.SetValue(objT, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(pro.PropertyType).ToString())), null);
                            }
                            else
                            {
                                pro.SetValue(objT, Convert.ChangeType(value, Type.GetType(pro.PropertyType.ToString())), null);
                            }
                        }
                    }
                    return objT;
                }).ToList());

                return lstData;

            }
            catch
            {
                return lstData;
            }
        }
    }
}
