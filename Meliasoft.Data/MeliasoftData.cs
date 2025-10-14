using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Meliasoft_Data
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

        private string GetConnectionString
        {
            get
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Meliasoft"].ConnectionString;

                if (!string.IsNullOrEmpty(UserName))
                {
                    dynamic result = null;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            var parameters = new DynamicParameters();
                            parameters.Add("@Email", UserName);
                            result = connection.Query<dynamic>("SELECT CAST(ValueBin AS VARCHAR(MAX)) AS ConnectionString FROM Sys_UserProperty A INNER JOIN Sys_User B ON A.UserId = B.Id WHERE Email = @Email", parameters, commandType: CommandType.Text).FirstOrDefault();
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

                    if (result != null)
                    {
                        return result.ConnectionString;
                    }
                }

                return connectionString;
            }
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
                        foreach (var item in param)
                        {
                            if (first)
                            {
                                first = false;

                                StringBuilder sb = new StringBuilder();
                                foreach (var column in item)
                                {
                                    string type = string.Empty;

                                    if (column.Value is string)
                                    {
                                        type = "NVARCHAR(256)";
                                    }
                                    else if (column.Value is DateTime)
                                    {
                                        type = "SMALLDATETIME";
                                    }
                                    else if (column.Value is decimal)
                                    {
                                        type = "NUMERIC(18,2)";
                                    }
                                    else if (column.Value is int)
                                    {
                                        type = "INT";
                                    }
                                    else if (column.Value is bool)
                                    {
                                        type = "BIT";
                                    }
                                    else if (column.Value == null)
                                    {
                                        var value = param.Where(s => s[column.Key] != null).Select(s => s[column.Key]).FirstOrDefault();
                                        if (value != null)
                                        {
                                            if (value is string)
                                            {
                                                type = "NVARCHAR(256)";
                                            }
                                            else if (value is DateTime)
                                            {
                                                type = "SMALLDATETIME";
                                            }
                                            else if (value is decimal)
                                            {
                                                type = "NUMERIC(18,2)";
                                            }
                                            else if (value is int)
                                            {
                                                type = "INT";
                                            }
                                            else if (value is bool)
                                            {
                                                type = "BIT";
                                            }
                                        }
                                    }

                                    sb.AppendFormat("{1}[{0}] {2}", column.Key, sb.Length == 0 ? string.Empty : ", ", type);
                                    columns.AppendFormat("{1}[{0}]", column.Key, columns.Length == 0 ? string.Empty : ", ", type);
                                    names.AppendFormat("{1}@{0}", column.Key, names.Length == 0 ? string.Empty : ", ");
                                }

                                connection.Execute(string.Format("CREATE TABLE {1} ({0}, UserCheck VARCHAR(16), Modified SMALLDATETIME)", sb.ToString(), table), commandType: CommandType.Text);
                            }

                            {
                                var parameters = new DynamicParameters();
                                foreach (var column in item)
                                {
                                    //var paramValue = column.Value;
                                    //if (column.Value != null && column.Value is bool)
                                    //{
                                    //    if (Convert.ToBoolean(column.Value) == true)
                                    //    {
                                    //        paramValue = "1";
                                    //    } else
                                    //    {
                                    //        paramValue = "0";
                                    //    }
                                    //}

                                    string inputString = Convert.ToString(column.Value);
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
                                            if (column.Value != null && column.Value is bool)
                                            {
                                                if (Convert.ToBoolean(column.Value) == true)
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
                                    

                                    parameters.Add(string.Format("@{0}", column.Key), paramValue); //column.Value
                                }
                                connection.Execute(string.Format("INSERT INTO {1} ({2}, UserCheck, Modified) VALUES ({0}, '{3}', GETDATE())", names.ToString(), table, columns.ToString(), UserName), parameters, commandType: CommandType.Text);
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
