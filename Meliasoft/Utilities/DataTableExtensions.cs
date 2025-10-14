using Dynamitey;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Meliasoft.Utilities
{
    public static class DataTableExtensions
    {
        public static List<dynamic> ToDynamic(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject(); 
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    //dic[column.ColumnName] = row[column];
                    var value = row[column.ColumnName];
                    dic[column.ColumnName] = Convert.IsDBNull(value) ? "" : value;

                    var type = value.GetType();
                    if (type.Name == "Decimal")
                    {
                        if (((decimal)value) == 0)
                        {
                            Dynamic.InvokeSet(dic, column.ColumnName, "");
                        }
                    }
                    else if (type.Name == "DateTime")
                    {
                        if (((DateTime)value) == new DateTime(1900, 1, 1))
                        {
                            Dynamic.InvokeSet(dic, column.ColumnName, "");
                        }
                        else
                        {
                            Dynamic.InvokeSet(dic, column.ColumnName, ((DateTime)value).ToString("dd/MM/yyyy"));
                        }
                    }
                }
            }
            return dynamicDt;
        }
    }
}