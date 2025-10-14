using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meliasoft_Data
{
    public interface IMeliasoftData
    {
        string UserName { get; set; }
        string OpenKey { get; set; }
        List<dynamic> Query(string cmdText, System.Data.CommandType commandType = CommandType.StoredProcedure, string[] paramNames = null, object[] paramValues = null);
        DataSet GetData(string cmdText, System.Data.CommandType commandType = CommandType.StoredProcedure, string[] paramNames = null, object[] paramValues = null);
        int ExecuteNonQuery(string cmdText, CommandType commandType = CommandType.StoredProcedure, string[] paramNames = null, object[] paramValues = null, List<Dictionary<string, object>> param = null, string table = "");
    }
}
