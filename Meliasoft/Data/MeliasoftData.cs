using Dapper;
using Meliasoft.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.PeerToPeer;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Meliasoft.Data
{
  public class MeliasoftData : IMeliasoftData
  {
    private string _userName = string.Empty;
    public string UserName
    {
      get { return _userName; }
      set { _userName = value; }
    }

    private string _openKey = string.Empty;
    public string OpenKey
    {
      get { return _openKey; }
      set { _openKey = value; }
    }

    private string _savedConnString = string.Empty;
    public string SavedConnString
    {
      get { return _savedConnString; }
      set { _savedConnString = value; }
    }

    //private string GetConnectionString
    //{
    //  get
    //  {
    //    if (!string.IsNullOrEmpty(SavedConnString))
    //    {
    //      return SavedConnString;
    //    }

    //    string connectionString = ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;

    //    if (!string.IsNullOrEmpty(UserName))
    //    {
    //      dynamic result = null;
    //      using (SqlConnection connection = new SqlConnection(connectionString))
    //      {
    //        try
    //        {
    //          connection.Open();
    //          var parameters = new DynamicParameters();
    //          parameters.Add("@Email", UserName);
    //          result = connection.Query<dynamic>("SELECT CAST(ValueBin AS VARCHAR(MAX)) AS ConnectionString FROM Sys_UserProperty A INNER JOIN Sys_User B ON A.UserId = B.Id WHERE Email = @Email", parameters, commandType: CommandType.Text).FirstOrDefault();
    //        }
    //        catch (Exception ex)
    //        {
    //          throw ex;
    //        }
    //        finally
    //        {
    //          connection.Close();
    //        }
    //      }

    //      if (result != null)
    //      {
    //        return result.ConnectionString;
    //      }
    //    }

    //    return connectionString;
    //  }
    //}

    //--------------------------------------

    //private Dictionary<string, string> _userConnStringCache = new Dictionary<string, string>();

    //private string GetConnectionString
    //{
    //  get
    //  {
    //    if (!string.IsNullOrEmpty(SavedConnString))
    //      return SavedConnString;

    //    if (!string.IsNullOrEmpty(UserName))
    //    {
    //      if (_userConnStringCache.ContainsKey(UserName))
    //        return _userConnStringCache[UserName];

    //      string connectionString = ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;
    //      using (SqlConnection connection = new SqlConnection(connectionString))
    //      {
    //        connection.Open();
    //        var parameters = new DynamicParameters();
    //        parameters.Add("@Email", UserName);
    //        var result = connection.Query<dynamic>(
    //            "SELECT CAST(ValueBin AS VARCHAR(MAX)) AS ConnectionString FROM Sys_UserProperty A INNER JOIN Sys_User B ON A.UserId = B.Id WHERE Email = @Email",
    //            parameters, commandType: CommandType.Text).FirstOrDefault();

    //        if (result != null)
    //        {
    //          _userConnStringCache[UserName] = result.ConnectionString;
    //          return result.ConnectionString;
    //        }
    //      }
    //    }

    //    return ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;
    //  }
    //}



    // Property GetConnectionString chỉ trả về SavedConnString nếu có:
    //private string GetConnectionString
    //{
    //  get
    //  {

    //    if (!string.IsNullOrEmpty(SavedConnString))
    //      return SavedConnString;

    //    // Thử lấy từ Session
    //    if (HttpContext.Current != null && HttpContext.Current.Session != null)
    //    {
    //      var sessionConnString = HttpContext.Current.Session["UserConnString"] as string;
    //      if (!string.IsNullOrEmpty(sessionConnString))
    //        return sessionConnString;
    //    }

    //    return ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;
    //  }
    //}

    private string GetConnectionString
    {
      get
      {
        if (!string.IsNullOrEmpty(SavedConnString))
          return SavedConnString;

        if (!string.IsNullOrEmpty(UserName))
        {
     
          string connectionString = ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;
          using (SqlConnection connection = new SqlConnection(connectionString))
          {
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.Add("@Email", UserName);
            var result = connection.Query<dynamic>(
                "SELECT CAST(ValueBin AS VARCHAR(MAX)) AS ConnectionString FROM Sys_UserProperty A INNER JOIN Sys_User B ON A.UserId = B.Id WHERE Email = @Email",
                parameters, commandType: CommandType.Text).FirstOrDefault();

            if (result != null)
            {
              return result.ConnectionString;
            }
          }
        }

        return ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;
      }
    }

    // Hàm lấy connection string động:
    public static string GetUserConnectionString(string userName)
    {
      string connectionString = ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        var parameters = new DynamicParameters();
        parameters.Add("@Email", userName);
        var result = connection.Query<dynamic>(
            "SELECT CAST(ValueBin AS VARCHAR(MAX)) AS ConnectionString FROM Sys_UserProperty A INNER JOIN Sys_User B ON A.UserId = B.Id WHERE Email = @Email",
            parameters, commandType: CommandType.Text).FirstOrDefault();

        if (result != null)
          return result.ConnectionString;
      }
      return connectionString;
    }



    public List<dynamic> Query(string cmdText, CommandType commandType = CommandType.StoredProcedure, string[] paramNames = null, object[] paramValues = null)
    {
      List<dynamic> result = null;
      using (SqlConnection connection = new SqlConnection(GetConnectionString))
      {
        try
        {
          connection.Open();
          var parameters = new DynamicParameters();
          if (paramNames != null && paramValues != null)
          {
            for (int i = 0; i < paramNames.Length; i++)
            {
              if (paramNames[i].ToUpper().Contains("OPEN_KEY"))
                parameters.Add(paramNames[i], this.OpenKey);
              else
              {
                //var paramValue = paramValues[i];
                //if (paramValues[i] != null && paramValues[i] is bool)
                //{
                //    if (Convert.ToBoolean(paramValues[i]) == true)
                //    {
                //        paramValue = "1";
                //    }
                //    else
                //    {
                //        paramValue = "0";
                //    }
                //}

                string inputString = Convert.ToString(paramValues[i]);
                string paramValue = "";
                DateTime parsedDateTime;

                if (DateTime.TryParseExact(inputString, "dd/MM/yyyy",
                        new CultureInfo("vi-VN"), //en-US
                        DateTimeStyles.None,
                        out parsedDateTime)) //if (DateTime.TryParse(inputString, out dateValue))
                {
                  //String.Format("{0:d/MM/yyyy}", dDate);
                  paramValue = parsedDateTime.ToString("dd/MM/yyyy");
                }
                else
                {
                  //DateTime parsedDateTime;
                  if (DateTime.TryParseExact(inputString, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", null, System.Globalization.DateTimeStyles.None, out parsedDateTime))
                  {
                    paramValue = parsedDateTime.ToString("dd/MM/yyyy");
                  }
                  else
                  {
                    if (paramValues[i] != null && paramValues[i] is bool)
                    {
                      if (Convert.ToBoolean(paramValues[i]) == true)
                      {
                        paramValue = "1";
                      }
                      else
                      {
                        paramValue = "0";
                      }
                    }
                    else
                    {
                      if (inputString.ToLower().Equals("true"))
                      {
                        paramValue = "1";
                      }
                      else if (inputString.ToLower().Equals("false"))
                      {
                        paramValue = "0";
                      }
                      else
                      {
                        paramValue = inputString;
                      }
                    }
                  }
                }

                parameters.Add(paramNames[i], paramValue); //paramValues[i]
              }

            }
          }
          //TEST Voice Msg:
          //cmdText += "; SELECT N'Xin chào các bạn, tôi đang nói những lời đơn giản nhất, các bạn có nghe rõ không?' As VoiceMsg; ";
          result = connection.Query<dynamic>(cmdText, parameters, commandType: commandType).ToList();

        }
        catch (Exception ex)
        {
          throw ex;
        }
        finally
        {
          connection.Close();
        }
      }

      return result;
    }


    public DataSet GetData(string cmdText, CommandType commandType = CommandType.StoredProcedure, string[] paramNames = null, object[] paramValues = null)
    {
      // create the DataSet 
      DataSet dataSet = new DataSet();

      List<dynamic> result = null;
      using (SqlConnection connection = new SqlConnection(GetConnectionString))
      {
        try
        {
          connection.Open();
          //                    var parameters = new DynamicParameters();
          List<SqlParameter> parameters = new List<SqlParameter>();

          if (paramNames != null && paramValues != null)
          {
            for (int i = 0; i < paramNames.Length; i++)
            {
              if (paramNames[i].ToUpper().Contains("OPEN_KEY"))
                parameters.Add(new SqlParameter(paramNames[i], this.OpenKey));
              else
              {
                //var paramValue = paramValues[i];
                //if (paramValues[i] != null && paramValues[i] is bool)
                //{
                //    if (Convert.ToBoolean(paramValues[i]) == true)
                //    {
                //        paramValue = "1";
                //    }
                //    else
                //    {
                //        paramValue = "0";
                //    }
                //}

                string inputString = Convert.ToString(paramValues[i]);
                string paramValue = "";
                DateTime parsedDateTime;

                if (DateTime.TryParseExact(inputString, "dd/MM/yyyy",
                        new CultureInfo("vi-VN"), //en-US
                        DateTimeStyles.None,
                        out parsedDateTime)) //if (DateTime.TryParse(inputString, out dateValue))
                {
                  //String.Format("{0:d/MM/yyyy}", dDate);
                  paramValue = parsedDateTime.ToString("dd/MM/yyyy");
                }
                else
                {
                  //DateTime parsedDateTime;
                  if (DateTime.TryParseExact(inputString, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", null, System.Globalization.DateTimeStyles.None, out parsedDateTime))
                  {
                    paramValue = parsedDateTime.ToString("dd/MM/yyyy");
                  }
                  else
                  {
                    if (paramValues[i] != null && paramValues[i] is bool)
                    {
                      if (Convert.ToBoolean(paramValues[i]) == true)
                      {
                        paramValue = "1";
                      }
                      else
                      {
                        paramValue = "0";
                      }
                    }
                    else
                    {
                      if (inputString.ToLower().Equals("true"))
                      {
                        paramValue = "1";
                      }
                      else if (inputString.ToLower().Equals("false"))
                      {
                        paramValue = "0";
                      }
                      else
                      {
                        paramValue = inputString;
                      }
                    }
                  }
                }

                parameters.Add(new SqlParameter(paramNames[i], paramValue)); //paramValues[i]
              }

            }
          }
          //TEST Voice Msg:
          cmdText += "; SELECT N'Nội dung báo cáo chưa có, vui lòng liên hệ với Meliasoft để được hỗ trợ. Xin cảm ơn!' As VoiceMsg; ";
          //result = connection.Query<dynamic>(cmdText, parameters, commandType: commandType).ToList();

          //dataSet= (DataSet)connection.Query<dynamic>(cmdText, parameters, commandType: commandType);

          //SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdText, connection);
          //// fill the DataSet using our DataAdapter 
          //dataAdapter.Fill(dataSet);

          using (SqlDataAdapter da = new SqlDataAdapter())
          {
            da.SelectCommand = new SqlCommand(cmdText, connection);
            da.SelectCommand.CommandType = commandType;// CommandType.StoredProcedure;
            da.SelectCommand.CommandTimeout = 900;

            //da.SelectCommand.Parameters.Add("@Par1", SqlDbType.Int).Value = par1;
            //da.SelectCommand.Parameters.Add("@Par2", SqlDbType.Int).Value = (object)par2 ?? DBNull.Value;
            da.SelectCommand.Parameters.AddRange(parameters.ToArray());

            da.Fill(dataSet);

            //DataTable dt = ds.Tables["SourceTable_Name"];

            ////foreach (DataRow row in dt.Rows)
            ////{
            ////You can even manipulate your data here
            ////}
            //return dt;
          }

        }
        catch (Exception ex)
        {
          throw ex;
        }
        finally
        {
          connection.Close();
        }
      }

      return dataSet;
    }

    private List<SqlParameter> BuildParametersFromNamesValues(string[] paramNames, object[] paramValues)
    {
      var parameters = new List<SqlParameter>();

      if (paramNames == null || paramValues == null) return parameters;

      for (int i = 0; i < paramNames.Length; i++)
      {
        string keyRaw = paramNames[i] ?? "";
        string pName = keyRaw.StartsWith("@") ? keyRaw : "@" + keyRaw;  // <-- CHỐT Ở ĐÂY

        object pval = paramValues[i];

        if (keyRaw.ToUpper().Contains("OPEN_KEY"))
        {
          parameters.Add(new SqlParameter(pName, this.OpenKey ?? string.Empty));
          continue;
        }

        // SUY LUẬN KIỂU (bám sát khối switch trong ExecuteNonQuery)
        string sqlType = "NVARCHAR(256)";
        object v = pval;

        if (v is string s)
        {
          s = s.Trim();

          // date?
          DateTime dt;
          if (DateTime.TryParseExact(
                  s,
                  new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-dd'T'HH:mm:ss.fff'Z'" },
                  CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
          {
            sqlType = "DATETIME";
            v = dt; // convert sang DateTime thật
          }
          else
          {
            // decimal?
            decimal dec;
            var sn = s.Replace(",", "");
            if (decimal.TryParse(sn, NumberStyles.Any, CultureInfo.InvariantCulture, out dec))
            {
              sqlType = "NUMERIC(18,2)";
              v = dec;
            }
            else if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(s, "1", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(s, "on", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(s, "0", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(s, "off", StringComparison.OrdinalIgnoreCase))
            {
              sqlType = "BIT";
              v = (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(s, "1", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(s, "on", StringComparison.OrdinalIgnoreCase));
            }
            else
            {
              sqlType = "NVARCHAR(256)";
              v = s;
            }
          }
        }
        else if (v is DateTime) { sqlType = "DATETIME"; }
        else if (v is bool) { sqlType = "BIT"; }
        else if (v is int || v is long || v is short) { sqlType = "INT"; }
        else if (v is decimal || v is double || v is float) { sqlType = "NUMERIC(18,2)"; }

        // Ánh xạ sang DbType & add parameter (giữ đúng cách bạn đang làm)
        // Ánh xạ sang DbType & add parameter
        switch (sqlType)
        {
          case "INT":
            parameters.Add(new SqlParameter(pName, Toolkit.ToInt(v)) { DbType = DbType.Int32 });
            break;

          case "NUMERIC(18,2)":
            parameters.Add(new SqlParameter(pName, Toolkit.ToDecimal(v)) { DbType = DbType.Decimal });
            break;

          case "DATETIME":
            var dtn = Toolkit.ToDateOrNull(v);
            parameters.Add(new SqlParameter(pName, dtn.HasValue ? (object)dtn.Value : DBNull.Value) { DbType = DbType.DateTime });
            break;

          case "BIT":
            parameters.Add(new SqlParameter(pName, Toolkit.ToBool(v)) { DbType = DbType.Boolean });
            break;

          default:
            parameters.Add(new SqlParameter(pName, v?.ToString() ?? string.Empty) { DbType = DbType.String });
            break;
        }

      }

      return parameters;
    }

    public DataTable ExecuteDataTable(string cmdText, CommandType commandType = CommandType.StoredProcedure,
                                  string[] paramNames = null, object[] paramValues = null)
    {
      var dt = new DataTable();

      using (var connection = new SqlConnection(GetConnectionString))
      using (var da = new SqlDataAdapter())
      {
        try
        {
          connection.Open();

          var cmd = new SqlCommand(cmdText, connection)
          {
            CommandType = commandType,
            CommandTimeout = 900
          };

          // build params theo đúng logic của ExecuteNonQuery
          var parameters = BuildParametersFromNamesValues(paramNames, paramValues);
          if (parameters.Count > 0)
            cmd.Parameters.AddRange(parameters.ToArray());

          da.SelectCommand = cmd;

          // Fill nhiều resultset
          var ds = new DataSet();
          da.Fill(ds);

          if (ds != null && ds.Tables.Count > 0)
          {
            // lấy bảng đầu tiên
            var last = ds.Tables[0];
            dt = last?.Copy() ?? new DataTable(); // Copy để tách khỏi ds đã dispose
          }
        }
        catch (Exception ex)
        {
          throw; // giữ nguyên pattern của codebase
        }
        finally
        {
          connection.Close();
        }
      }

      return dt;
    }


    public int ExecuteNonQuery(string cmdText, CommandType commandType = CommandType.StoredProcedure, string[] paramNames = null, object[] paramValues = null, List<Dictionary<string, object>> param = null, string table = "")
    {
      int ret = 0;
      using (SqlConnection connection = new SqlConnection(GetConnectionString))
      {
        try
        {
          connection.Open();

          ///
          if (param != null)
          {
            connection.Execute(string.Format("IF OBJECT_ID(N'TEMPDB..{0}') IS NOT NULL DROP TABLE {0}", table), commandType: CommandType.Text);

            bool first = true;
            StringBuilder columns = new StringBuilder();
            StringBuilder names = new StringBuilder();
            var sqlTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in param)
            {
              if (first)
              {
                first = false;

                StringBuilder sb = new StringBuilder();
                //foreach (var column in item)
                //{
                //  string type = string.Empty;

                //  if (column.Value is string)
                //  {
                //    type = "NVARCHAR(256)";
                //  }
                //  else if (column.Value is DateTime)
                //  {
                //    type = "SMALLDATETIME";
                //  }
                //  else if (column.Value is decimal)
                //  {
                //    type = "NUMERIC(18,2)";
                //  }
                //  else if (column.Value is int)
                //  {
                //    type = "INT";
                //  }
                //  else if (column.Value is bool)
                //  {
                //    type = "BIT";
                //  }
                //  else if (column.Value == null)
                //  {
                //    var value = param.Where(s => s[column.Key] != null).Select(s => s[column.Key]).FirstOrDefault();
                //    if (value != null)
                //    {
                //      if (value is string)
                //      {
                //        type = "NVARCHAR(256)";
                //      }
                //      else if (value is DateTime)
                //      {
                //        type = "SMALLDATETIME";
                //      }
                //      else if (value is decimal)
                //      {
                //        type = "NUMERIC(18,2)";
                //      }
                //      else if (value is int)
                //      {
                //        type = "INT";
                //      }
                //      else if (value is bool)
                //      {
                //        type = "BIT";
                //      }
                //    }
                //  }

                //  sb.AppendFormat("{1}[{0}] {2}", column.Key, sb.Length == 0 ? string.Empty : ", ", type);
                //  columns.AppendFormat("{1}[{0}]", column.Key, columns.Length == 0 ? string.Empty : ", ", type);
                //  names.AppendFormat("{1}@{0}", column.Key, names.Length == 0 ? string.Empty : ", ");
                //}

                

                foreach (var column in item)
                {
                  string type = string.Empty;
                  object val = column.Value;

                  if (val == null || (val is string s0 && string.IsNullOrWhiteSpace(s0)))
                  {
                    val = param.Where(r => r.ContainsKey(column.Key))
                               .Select(r => r[column.Key])
                               .FirstOrDefault(v => v != null && !(v is string ss && string.IsNullOrWhiteSpace(ss)));
                  }

                  if (val is string s)
                  {
                    s = s.Trim();
                    DateTime dt;
                    if (DateTime.TryParseExact(s,
                        new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-dd'T'HH:mm:ss.fff'Z'" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                      type = "DATETIME";
                    }
                    else
                    {
                      decimal dec;
                      var sn = s.Replace(",", ""); // bỏ dấu ngăn cách nghìn
                      type = decimal.TryParse(sn, NumberStyles.Any, CultureInfo.InvariantCulture, out dec)
                             ? "NUMERIC(18,2)"
                             : "NVARCHAR(256)";
                    }
                  }
                  else if (val is DateTime) type = "DATETIME"; //else if (val is DateTime) type = "SMALLDATETIME";
                  else if (val is bool) type = "BIT";
                  else if (val is byte || val is short || val is int || val is long)
                    type = "INT";
                  else if (val is float || val is double || val is decimal)
                    type = "NUMERIC(18,2)";

                  if (string.IsNullOrEmpty(type)) type = "NVARCHAR(256)";

                  sqlTypeMap[column.Key] = type; // <-- LƯU LẠI KIỂU

                  sb.AppendFormat("{1}[{0}] {2}", column.Key, sb.Length == 0 ? string.Empty : ", ", type);
                  columns.AppendFormat("{1}[{0}]", column.Key, columns.Length == 0 ? string.Empty : ", ");
                  names.AppendFormat("{1}@{0}", column.Key, names.Length == 0 ? string.Empty : ", ");
                }

                connection.Execute(
                    $"CREATE TABLE {table} ({sb}, UserCheck VARCHAR(16), Modified SMALLDATETIME)",
                    commandType: CommandType.Text);

              }

              {
                //var parameters = new DynamicParameters();
                //foreach (var column in item)
                //{
                //  //var paramValue = column.Value;
                //  //if (column.Value != null && column.Value is bool)
                //  //{
                //  //    if (Convert.ToBoolean(column.Value) == true)
                //  //    {
                //  //        paramValue = "1";
                //  //    } else
                //  //    {
                //  //        paramValue = "0";
                //  //    }
                //  //}

                //  string inputString = Convert.ToString(column.Value);
                //  string paramValue = "";
                //  DateTime parsedDateTime;

                //  if (DateTime.TryParseExact(inputString, "dd/MM/yyyy",
                //          new CultureInfo("vi-VN"), //en-US
                //          DateTimeStyles.None,
                //          out parsedDateTime)) //if (DateTime.TryParse(inputString, out dateValue))
                //  {
                //    //String.Format("{0:d/MM/yyyy}", dDate);
                //    paramValue = parsedDateTime.ToString("dd/MM/yyyy");
                //  }
                //  else
                //  {
                //    //DateTime parsedDateTime;
                //    if (DateTime.TryParseExact(inputString, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", null, System.Globalization.DateTimeStyles.None, out parsedDateTime))
                //    {
                //      paramValue = parsedDateTime.ToString("dd/MM/yyyy");
                //    }
                //    else
                //    {
                //      if (column.Value != null && column.Value is bool)
                //      {
                //        if (Convert.ToBoolean(column.Value) == true)
                //        {
                //          paramValue = "1";
                //        }
                //        else
                //        {
                //          paramValue = "0";
                //        }
                //      }
                //      else
                //      {
                //        if (inputString.ToLower().Equals("true"))
                //        {
                //          paramValue = "1";
                //        }
                //        else if (inputString.ToLower().Equals("false"))
                //        {
                //          paramValue = "0";
                //        }
                //        else
                //        {
                //          paramValue = inputString;
                //        }
                //      }
                //    }
                //  }


                //  parameters.Add(string.Format("@{0}", column.Key), paramValue); //column.Value
                //}
                //connection.Execute(string.Format("INSERT INTO {1} ({2}, UserCheck, Modified) VALUES ({0}, '{3}', GETDATE())", names.ToString(), table, columns.ToString(), UserName), parameters, commandType: CommandType.Text);

                //---------------------------
                var parameters = new DynamicParameters();

                foreach (var column in item)
                {
                  var key = column.Key;
                  var sqlType = sqlTypeMap.ContainsKey(key) ? sqlTypeMap[key] : "NVARCHAR(256)";
                  object pval = column.Value;

                  //switch (sqlType)
                  //{
                  //  case "INT":
                  //    pval = Toolkit.ToInt(pval);
                  //    parameters.Add("@" + key, pval, DbType.Int32);
                  //    break;

                  //  case "NUMERIC(18,2)":
                  //    pval = Toolkit.ToDecimal(pval);
                  //    parameters.Add("@" + key, pval, DbType.Decimal);
                  //    break;

                  //  case "SMALLDATETIME":
                  //    pval = Toolkit.ToDate(pval);
                  //    parameters.Add("@" + key, pval, DbType.DateTime);
                  //    break;

                  //  case "DATETIME":
                  //    var d = Toolkit.ToDateOrNull(pval);
                  //    if (d.HasValue) parameters.Add("@" + key, d.Value, DbType.DateTime);
                  //    else parameters.Add("@" + key, DBNull.Value, DbType.DateTime);
                  //    break;

                  //  case "BIT":
                  //    pval = Toolkit.ToBool(pval);
                  //    parameters.Add("@" + key, pval, DbType.Boolean);
                  //    break;

                  //  default:
                  //    parameters.Add("@" + key, pval?.ToString() ?? string.Empty, DbType.String);
                  //    break;
                  //}

                  switch (sqlType)
                  {
                    case "INT":
                      var iv = Toolkit.ToInt(pval);                    // ""/null -> 0 hoặc null tuỳ bạn
                      parameters.Add("@" + key, iv, DbType.Int32);
                      break;

                    case "NUMERIC(18,2)":
                      var dv = Toolkit.ToDecimal(pval);                // "1.234,56"/"1,234.56" -> 1234.56
                      parameters.Add("@" + key, dv, DbType.Decimal);
                      break;

                    case "DATETIME":                           // dùng DATETIME thay SMALLDATETIME
                      var dt = Toolkit.ToDateOrNull(pval);             // ""/invalid -> null
                      parameters.Add("@" + key, dt.HasValue ? (object)dt.Value : DBNull.Value, DbType.DateTime);
                      break;

                    case "BIT":
                      var bv = Toolkit.ToBool(pval);                   // "true"/"1"/"on" -> true
                      parameters.Add("@" + key, bv, DbType.Boolean);
                      break;

                    default:
                      parameters.Add("@" + key, pval?.ToString() ?? string.Empty, DbType.String);
                      break;
                  }

                }

                connection.Execute(
                    $"INSERT INTO {table} ({columns}, UserCheck, Modified) VALUES ({names}, '{UserName}', GETDATE())",
                    parameters, commandType: CommandType.Text);


              }
            }
          }
          ///

          {
            var parameters = new DynamicParameters();
            if (paramNames != null && paramValues != null)
            {
              for (int i = 0; i < paramNames.Length; i++)
              {
                parameters.Add(paramNames[i], paramValues[i]);
              }
            }

            ret = connection.Execute(cmdText, parameters, commandType: commandType);
          }
        }
        catch (Exception ex)
        {
          throw ex;
        }
        finally
        {
          connection.Close();
        }
      }

      return ret;
    }
  }
}
