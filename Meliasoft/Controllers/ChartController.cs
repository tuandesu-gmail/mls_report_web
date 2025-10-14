using Dynamitey;
using FastMember;
using Meliasoft.Controllers;
using Meliasoft.Data;
using Meliasoft.Models;
using Meliasoft.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Meliasoft.Controllers
{
  [Authorize]
  public class ChartController : BaseController
  {
    public static int pageIndex = 0;
    public ChartController(IMeliasoftData meliasoftData)
        : base(meliasoftData)
    {
    }

    //public ActionResult Home()
    //{
    //    return View();
    //}

    public ActionResult Chart(int chart_id = 0, int report_id = 0)
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      ViewBag.ChartId = chart_id;
      ViewBag.ReportId = report_id;
      ViewBag.ErrMsg = "";

      //var result = _meliasoftData.Query("SELECT * FROM Web_Report WHERE Id = @ReportId",
      //            CommandType.Text,
      //            new string[] { "@ReportId" },
      //            new object[] { report_id })
      //        .FirstOrDefault();

      //ViewBag.ReportTitle = "";
      //ViewBag.CompanyName = "";
      //if (result != null )
      //{
      //    if (result.Count > 0)
      //    {
      //        var item = result[0];

      //        ViewBag.ReportTitle = Convert.ToString(item.Title);
      //        ViewBag.CompanyName = Convert.ToString(item.CompanyName);
      //    }
      //}
      return View();
    }

    public ActionResult ChartCustomize(int chart_id = 0, int report_id = 0)
    {
      try
      {
        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();

        //if (chart_id == 0)
        //{
        //    return View("Chart");
        //}


        StringBuilder dummy_Columns = new StringBuilder();
        StringBuilder fields_Columns = new StringBuilder();

        // onchange=\"selectChartColumn(this)\"
        // title : \"Chọn\"
        //dummy_Columns.AppendFormat("[{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"chkbx\"  ng-checked=\"chkBoxChief\" />', headerTemplate: '<input type=\"checkbox\" ng-change=\"triggerAllColumns()\" ng-model=\"chkBoxChief\"/>' , width: \"80px\", attributes: {{ class: \"text-center\"  }}}},");
        dummy_Columns.AppendFormat("[{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"check-box-inner\" />', headerTemplate: '<input id=\"chkAll\" class=\"checkAllCls\" type=\"checkbox\"/>', width: \"80px\", attributes: {{ class: \"text-center\"  }}}},");
        //dummy_Columns.AppendFormat("[{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"chkbx\" ng-checked=\"IsCheck\" />', title : \"Chọn\", width: \"80px\", attributes: {{ class: \"text-center\"  }}}},");
        //title: "<input id='chkAll' class='checkAllCls' type='checkbox'/>", width: "35px", template: "<input type='checkbox' class='check-box-inner' />",

        dummy_Columns.AppendFormat("{{field: \"HeaderText\", title: \"Tên cột\"}}]");
        //dummy.AppendFormat("{{field: \"DataFieldName\", title: \"Tên trường\"}}]");

        fields_Columns.AppendFormat("[{{field: \"DataFieldName\"}},");
        fields_Columns.AppendFormat("{{field: \"HeaderText\"}}]");

        ViewBag.ChartColumns_Dummy = dummy_Columns;
        ViewBag.ChartColumns_Fields = fields_Columns;

        //--------------------------------------------------------------
        //dummy.Clear();
        //fields.Clear();
        StringBuilder dummy_Rows = new StringBuilder();
        StringBuilder fields_Rows = new StringBuilder();

        string sqlCmd = "SELECT * FROM Web_ReportColumnConfig WHERE ReportId = @ReportId AND Visible = 1 ORDER BY [Order]";
        //if (IsFromAIweb())
        //  sqlCmd = "SELECT * FROM Web_ReportColumnConfig WHERE Report_Code = @ReportId AND Visible = 1 ORDER BY [Order]";

        var query = _meliasoftData.Query(sqlCmd,
            CommandType.Text,
            new string[] { "@ReportId" },
            new object[] { report_id });

        var columns = query.Select(s => s);

        int displayedColumnCount = 0;

        foreach (var item in columns)
        {
          displayedColumnCount++;

          if (displayedColumnCount <= 5)
          {

            if (fields_Rows.Length == 0)
            {
              fields_Rows.Append("[");
            }
            else
            {
              fields_Rows.Append(", ");
            }

            fields_Rows.AppendFormat("{{field: \"{0}\"}}", item.DataFieldName);

            if (dummy_Rows.Length == 0)
            {
              //dummy_Rows.Append("[");
              // onchange=\"selectChartRow(this)\"
              //dummy_Columns.AppendFormat("[{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"check-box-inner\" />', headerTemplate: '<input id=\"chkAll\" class=\"checkAllCls\" type=\"checkbox\"/>', width: \"80px\", attributes: {{ class: \"text-center\"  }}}},");
              //dummy_Rows.AppendFormat("[{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"chkbx\"  />', title: \"Chọn\", width: \"80px\", attributes: {{ class: \"text-center\"  }}}},");
              dummy_Rows.AppendFormat("[{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"check-box-inner-rows\"  />', headerTemplate: '<input id=\"chkAllRows\" class=\"checkAllRowsCls\" type=\"checkbox\"/>', width: \"80px\", attributes: {{ class: \"text-center\"  }}}},");
            }
            else
            {
              dummy_Rows.Append(", ");
            }

            //if (displayedColumnCount == 4 || displayedColumnCount == query.Count)
            //{
            //    dummy_Rows.AppendFormat("{{field: \"{0}\", title: \"{1}\" {2}{3}}}", item.DataFieldName, item.HeaderText, 
            //    string.IsNullOrEmpty(item.Format) ? "" : string.Format(", format: \"{0}\"", item.Format),
            //    string.IsNullOrEmpty(item.Alignment) ? "" : string.Format(", attributes: {{ class: \"text-{0}\"  }}", item.Alignment));
            //} else
            //{
            dummy_Rows.AppendFormat("{{field: \"{0}\", title: \"{1}\", width: \"{2}px\" {3}{4}}}", item.DataFieldName, item.HeaderText, item.Width,
            string.IsNullOrEmpty(item.Format) ? "" : string.Format(", format: \"{0}\"", item.Format),
            string.IsNullOrEmpty(item.Alignment) ? "" : string.Format(", attributes: {{ class: \"text-{0}\"  }}", item.Alignment));
            //}


          }
          else
          {
            break;
          }

        }
        dummy_Rows.AppendFormat(",{{template: '<span />', title: \"...\"}},");

        if (dummy_Rows.Length == 0)
        {
          dummy_Rows.Append("[]");
        }
        else
        {
          dummy_Rows.Append("]");
        }
        if (fields_Rows.Length == 0)
        {
          fields_Rows.Append("[]");
        }
        else
        {
          fields_Rows.Append("]");
        }


        ViewBag.ChartRows_Dummy = dummy_Rows;
        ViewBag.ChartRows_Fields = fields_Rows;

        //----------------------------------------------

        string cmdText = " SELECT TOP 1 ";
        cmdText += "    ISNULL(RC.Id, 0) AS Id,  ";
        cmdText += "    WR.Id AS ReportId, ";
        cmdText += "    ISNULL(RC.ChartTypeId, (SELECT TOP (1) [TypeId] FROM [Web_ReportChartTypes]  ORDER BY [ShowingOrder])) AS ChartTypeId, ";
        cmdText += "    ISNULL(RC.SelectedColumns, '') AS SelectedColumns,  ";
        cmdText += "    ISNULL(RC.SelectedRows, '') AS SelectedRows, ";
        cmdText += "    ISNULL(RC.Visible, 1) AS ChartVisible,  ";
        cmdText += "    ISNULL(RC.AutoSpeech, 0) AS AutoSpeech ";
        cmdText += " FROM Web_Report WR ";
        
        //if (IsFromAIweb())
        //  cmdText += "    LEFT JOIN Web_ReportCharts RC ON WR.Report_Code = RC.Report_Code ";
        //else
          cmdText += "    LEFT JOIN Web_ReportCharts RC ON WR.Id = RC.ReportId ";

        cmdText += " WHERE WR.Visible = 1 AND WR.Id = " + Convert.ToString(report_id);
        if (chart_id > 0)
          cmdText += "    AND RC.Id = " + Convert.ToString(chart_id);

        //var result = _meliasoftData.Query("SELECT * FROM [Web_ReportCharts] WHERE [Id] = @ChartId ",
        //    CommandType.Text,
        //    new string[] { "@ChartId" },
        //    new object[] { chart_id });
        var result = _meliasoftData.Query(cmdText, CommandType.Text);

        var dataColumns = result.Select(s => s);

        string chartTitle = "";
        string chartSubTitle = "";
        bool chartVisible = true;
        bool autoSpeech = true;
        int selectedTypeId = 0;
        string selectedColumns = "";
        string selectedRows = "";
        string chartNote = "";

        if (result.Count > 0)
        {
          var item = result[0];

          //if (IsFromAIweb())
          //  chartTitle = Convert.ToString(item.Title_VN);
          //else
            chartTitle = Convert.ToString(item.Title);
          chartSubTitle = Convert.ToString(item.Subtitle);
          chartVisible = Convert.ToBoolean(item.ChartVisible);
          autoSpeech = Convert.ToBoolean(item.AutoSpeech);
          selectedColumns = Convert.ToString(item.SelectedColumns);
          selectedRows = Convert.ToString(item.SelectedRows);
          selectedTypeId = Convert.ToInt32(item.ChartTypeId);
          chartNote = Convert.ToString(item.Note);
        }

        ViewBag.ChartTitle = chartTitle;
        ViewBag.ChartSubTitle = chartSubTitle;
        ViewBag.ChartVisible = chartVisible;
        ViewBag.AutoSpeech = chartVisible;
        ViewBag.SelectedColumns = selectedColumns;
        ViewBag.SelectedRows = selectedRows;
        ViewBag.SelectedTypeId = selectedTypeId;
        ViewBag.ChartNote = chartNote;

        ViewBag.ReportId = report_id;
        ViewBag.ChartId = chart_id;
        ViewBag.ErrMsg = "";

        return View();
      }
      catch (Exception ex)
      {
        ViewBag.ErrMsg = "System Error: " + ex.Message;
        return View(); //return View("ErrMsg");
      }
    }


    //public ActionResult ChartColumns(int chart_id = 0, int report_id = 0)
    //{
    //    try
    //    {
    //        _meliasoftData.UserName = GetUserName();
    //        _meliasoftData.OpenKey = GetOpenKey();



    //        StringBuilder dummy = new StringBuilder();
    //        StringBuilder fields = new StringBuilder();


    //        dummy.AppendFormat("[{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"chkbx\"  onchange=\"doalert(this)\" />', title: \"Chọn\", width: \"80px\", attributes: {{ class: \"text-center\"  }}}},");
    //        dummy.AppendFormat("{{field: \"HeaderText\", title: \"Tên cột\"}}]");
    //        //dummy.AppendFormat("{{field: \"DataFieldName\", title: \"Tên trường\"}}]");

    //        fields.AppendFormat("[{{field: \"DataFieldName\"}},");
    //        fields.AppendFormat("{{field: \"HeaderText\"}}]");

    //        ViewBag.Dummy = dummy;
    //        ViewBag.Fields = fields;

    //        ViewBag.ReportId = report_id;
    //        ViewBag.ChartId = chart_id;
    //        ViewBag.ErrMsg = "";

    //        return View();
    //    }
    //    catch (Exception ex)
    //    {
    //        ViewBag.ErrMsg = "System Error: " + ex.Message;
    //        return View("ErrMsg");
    //    }
    //}

    public ActionResult GetChartColumns(int chart_id = 0, int report_id = 0)
    {
      try
      {

        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();


        //StringBuilder dummy = new StringBuilder();
        //StringBuilder fields = new StringBuilder();

        //ViewBag.ReportId = report_id;
        //ViewBag.ChartId = chart_id;
        //ViewBag.ErrMsg = "";


        string sqlCmdText = "SELECT RCC.HeaderText, RCC.DataFieldName, ";
        //if (IsFromAIweb())
        //  //cmdText += "    LEFT JOIN Web_ReportCharts RC ON WR.Report_Code = RC.Report_Code ";
        //  sqlCmdText += "      SELECTED = ISNULL((SELECT RC.Report_Code FROM[Web_ReportCharts] RC WHERE RC.Report_Code = @ChartId AND ( ',' + RC.[SelectedColumns] + ',' LIKE '%,' + RCC.DataFieldName + ',%')), 0) ";
        //else
          sqlCmdText += "      SELECTED = ISNULL((SELECT RC.ID FROM[Web_ReportCharts] RC WHERE RC.Id = @ChartId AND ( ',' + RC.[SelectedColumns] + ',' LIKE '%,' + RCC.DataFieldName + ',%')), 0) ";

        sqlCmdText += "  FROM Web_ReportColumnConfig RCC ";
        sqlCmdText += "  WHERE RCC.ReportId = @ReportId AND RCC.Visible = 1 ORDER BY RCC.[Order] ";

        //"SELECT HeaderText, DataFieldName FROM Web_ReportColumnConfig WHERE ReportId = @ReportId AND Visible = 1 ORDER BY [Order]"
        var result = _meliasoftData.Query(sqlCmdText,
           CommandType.Text,
           new string[] { "@ChartId", "@ReportId" },
           new object[] { chart_id, report_id });


        ViewBag.ErrMsg = "";

        return new JsonNetResult
        {
          ContentType = "application/json",
          Data = result,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }
      catch (Exception ex)
      {
        ViewBag.ErrMsg = "System Error: " + ex.Message;
        return View(); //return View("ErrMsg");
      }
    }

    public ActionResult GetChartRows(int chart_id = 0, int report_id = 0)
    {
      try
      {
        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();

        string sqlCmd = "SELECT * FROM Web_Report WHERE Id = @ReportId";
        //if (IsFromAIweb())
        //  sqlCmd = "SELECT * FROM Web_Report WHERE Report_Code = @ReportId";

        var report = _meliasoftData.Query(sqlCmd,
                        CommandType.Text,
                        new string[] { "@ReportId" },
                        new object[] { report_id })
                    .FirstOrDefault();

        if (report != null)
        {
          StringBuilder cmdText = new StringBuilder();
          cmdText.AppendFormat("SET DATEFORMAT DMY; EXEC [{0}] ", report.StoredProcedureName);

          List<string> paramNames = new List<string>();
          List<object> paramValues = new List<object>();
          StringBuilder errorText = new StringBuilder();
          errorText.Append(cmdText);


          try
          {
            var result = _meliasoftData.Query(cmdText.ToString(),
                CommandType.Text);

            return new JsonNetResult
            {
              ContentType = "application/json",
              Data = result,
              JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
          }
          catch (Exception ex)
          {
            //return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            return Json(new { Success = false, Error = string.Format("Error: {0}", errorText) }, JsonRequestBehavior.AllowGet);
          }
        }

        return new JsonResult
        {
          ContentType = "application/json",
          Data = new List<object>(),
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }
      catch (Exception ex)
      {
        ViewBag.ErrMsg = "System Error: " + ex.Message;
        return View(); //return View("ErrMsg");
      }

    }


    public ActionResult GetChartTypes(int selected_chart_type_id = 0)
    {
      try
      {

        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();

        string sqlCmdText = "SELECT *, TypeName + ' - ' + StyleName AS TypeFullName  ";
        sqlCmdText += "  FROM Web_ReportChartTypes ";
        sqlCmdText += "  WHERE AvailableFlg = 1 ";
        sqlCmdText += "  ORDER BY ShowingOrder, TypeName, StyleName ";

        var result = _meliasoftData.Query(sqlCmdText, CommandType.Text);

        ViewBag.ErrMsg = "";

        return new JsonNetResult
        {
          ContentType = "application/json",
          Data = result,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }
      catch (Exception ex)
      {
        ViewBag.ErrMsg = "System Error: " + ex.Message;
        return View(); //return View("ErrMsg");
      }
    }


    [HttpPost]
    public JsonResult GetData(int report_id, List<ParameterModel> param)
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      var report = _meliasoftData.Query("SELECT * FROM Web_Report WHERE Id = @ReportId",
                    CommandType.Text,
                    new string[] { "@ReportId" },
                    new object[] { report_id })
                .FirstOrDefault();

      if (report != null)
      {
        StringBuilder cmdText = new StringBuilder();
        cmdText.AppendFormat("SET DATEFORMAT DMY; EXEC [{0}] ", report.StoredProcedureName);

        List<string> paramNames = new List<string>();
        List<object> paramValues = new List<object>();
        StringBuilder errorText = new StringBuilder();
        errorText.Append(cmdText);


        try
        {
          var result = _meliasoftData.Query(cmdText.ToString(),
              CommandType.Text);

          return new JsonNetResult
          {
            ContentType = "application/json",
            Data = result,// data, //result,
            JsonRequestBehavior = JsonRequestBehavior.AllowGet
          };
        }
        catch (Exception ex)
        {
          //return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
          return Json(new { Success = false, Error = string.Format("Error: {0}", errorText) }, JsonRequestBehavior.AllowGet);
        }
      }

      return new JsonResult
      {
        ContentType = "application/json",
        Data = new List<object>(),
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
      };
    }

    //, string title = "", string sub_title = "", int visible = -1, string chart_note = ""
    [HttpPost]
    public JsonResult UpdateChart(int chart_id, int report_id, string selected_fields, string selected_rows = "",
                                int chart_type_idx = 0, int show_at_home_page = 0, int auto_speech = 0)
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      try
      {
        string sqlCmdText = "SELECT TypeId  ";
        sqlCmdText += "  FROM Web_ReportChartTypes ";
        sqlCmdText += "  WHERE AvailableFlg = 1 ";
        sqlCmdText += "  ORDER BY ShowingOrder, TypeName, StyleName ";

        var query = _meliasoftData.Query(sqlCmdText, CommandType.Text);

        var columns = query.Select(s => s);

        var item = columns.ElementAt(chart_type_idx);
        int chartTypeId = item.TypeId;


        string sqlUpdate = "UPDATE Web_ReportCharts SET ";
        sqlUpdate += " SelectedColumns = '" + selected_fields + "' ";

        if (!string.IsNullOrEmpty(selected_rows))
          sqlUpdate += ", SelectedRows = '" + selected_rows + "' ";
        //if (!string.IsNullOrEmpty(title))
        //    sqlUpdate += ", Title = N'" + title + "' ";

        //sqlUpdate += ", Subtitle = N'" + sub_title + "' ";

        if (chartTypeId > 0)
          sqlUpdate += ", ChartTypeId = " + chartTypeId + " ";

        if (show_at_home_page >= 0)
          sqlUpdate += ", Visible = " + show_at_home_page + " ";
        //sqlUpdate += ", Visible = 1 ";

        if (auto_speech >= 0)
          sqlUpdate += ", AutoSpeech = " + auto_speech + " ";

        //sqlUpdate += ", Note = N'" + chart_note + "' ";

        sqlUpdate += " WHERE ";
        sqlUpdate += "      [Id] = " + chart_id + " AND ReportId = " + report_id + " ";

        int updResult = _meliasoftData.ExecuteNonQuery(sqlUpdate, CommandType.Text);

        if (updResult == 0)
        {
          string sqlInsert = "INSERT INTO Web_ReportCharts ([ReportId], [ChartTypeId] ,[SelectedColumns],[SelectedRows], [Visible], [AutoSpeech]) VALUES ( "; //[Title] ,[Subtitle] , ,
          sqlInsert += report_id + ", ";
          // sqlInsert += "N'" + title + "', ";
          // sqlInsert += "N'" + sub_title + "', ";
          //if (visible >= 0)
          //    sqlInsert += visible + ", ";
          //else
          //    sqlInsert += "0, ";
          if (chartTypeId >= 0)
            sqlInsert += chartTypeId + ", ";
          else
            sqlInsert += "1, ";
          sqlInsert += "'" + selected_fields + "', ";
          sqlInsert += "'" + selected_rows + "', ";
          sqlInsert += "'" + show_at_home_page + "', ";
          sqlInsert += "'" + auto_speech + "' ";
          sqlInsert += ") ";

          int istResult = _meliasoftData.ExecuteNonQuery(sqlInsert, CommandType.Text);
        }

        // Save viewed charts:
        if (chart_id > 0)
        {
          try
          {
            sqlUpdate = "UPDATE Web_ReportChart_SavedInfo SET ";
            sqlUpdate += "      SavedInfo = '" + chart_id + ",' + SavedInfo ";
            //if (show_at_home_page == 1)
            //{
            //    sqlUpdate += "      SavedInfo = '" + chart_id + ",' + SavedInfo ";
            //} else
            //{
            //    sqlUpdate += "      SavedInfo = REPLACE(',' + SavedInfo + ',', ','" + chart_id + "',', ',') ";
            //}

            sqlUpdate += " WHERE ";
            sqlUpdate += "      [AccountName] = '" + _meliasoftData.UserName + "'";

            updResult = _meliasoftData.ExecuteNonQuery(sqlUpdate, CommandType.Text);

            if (updResult == 0)
            {
              string sqlInsert = "INSERT INTO Web_ReportChart_SavedInfo ([AccountName], [SavedInfo] ) VALUES ( ";
              sqlInsert += " '" + _meliasoftData.UserName + "', ";
              sqlInsert += " '" + chart_id + "' ";
              sqlInsert += ") ";

              int istResult = _meliasoftData.ExecuteNonQuery(sqlInsert, CommandType.Text);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine("ERROR UPDATE/INSERT INTO Web_ReportChart_SavedInfo: " + ex.Message);
          }
        }

      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
      }

      return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public JsonResult GetMenu()
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      //var commandText = @"
      //      IF EXISTS(SELECT TOP 1 1 FROM Web_Report_Sec WHERE Email = @Email)
      //          SELECT * FROM Web_Report
      //              INNER JOIN Web_Report_Sec ON Web_Report.Id = Web_Report_Sec.Id_Report
      //              WHERE Web_Report_Sec.Email = @Email AND Web_Report.Visible = 1
      //              ORDER BY Name
      //      ELSE
      //          SELECT * FROM Web_Report
      //              WHERE Web_Report.Visible = 1
      //              ORDER BY Name"; //Name

      //if (IsFromAIweb())
      //  commandText = @"
      //              IF EXISTS(SELECT TOP 1 1 FROM Web_Report_Sec WHERE Email = @Email)
      //                  SELECT Web_Report.Title_VN AS Title, Web_Report.Title_VN AS Name, Web_Report.* FROM Web_Report
      //                      INNER JOIN Web_Report_Sec ON Web_Report.Report_Code = Web_Report_Sec.Report_Code
      //                      WHERE Web_Report_Sec.Email = @Email AND Web_Report.Visible = 1
      //                      ORDER BY Stt 
      //              ELSE
      //                  SELECT Web_Report.Title_VN AS Title, Web_Report.Title_VN AS Name, Web_Report.* FROM Web_Report
      //                      WHERE Web_Report.Visible = 1
      //                      ORDER BY Stt ";

      //var result = _meliasoftData.Query(commandText,
      //    CommandType.Text,
      //    new string[] { "@Email" },
      //    new object[] { _meliasoftData.UserName });

      object result;

      if (IsFromAIweb())
      {
        // Gọi stored procedure khi là AI web
        string mLanguage = "VN"; // hoặc lấy động nếu cần
        result = _meliasoftData.Query(
            "Web_Report_Select",
            CommandType.StoredProcedure,
            new string[] { "@M_Language", "@_Email", "@_Open_Key" },
            new object[] { mLanguage, _meliasoftData.UserName, _meliasoftData.OpenKey ?? "" }
        );
      }
      else
      {
        // Giữ nguyên SQL động cũ cho trường hợp khác
        var commandText = @"
                IF EXISTS(SELECT TOP 1 1 FROM Web_Report_Sec WHERE Email = @Email)
                    SELECT * FROM Web_Report
                        INNER JOIN Web_Report_Sec ON Web_Report.Id = Web_Report_Sec.Id_Report
                        WHERE Web_Report_Sec.Email = @Email AND Web_Report.Visible = 1
                        ORDER BY Name
                ELSE
                    SELECT * FROM Web_Report
                        WHERE Web_Report.Visible = 1
                        ORDER BY Name";

        result = _meliasoftData.Query(
            commandText,
            CommandType.Text,
            new string[] { "@Email" },
            new object[] { _meliasoftData.UserName }
        );
      }

      return new JsonNetResult
      {
        ContentType = "application/json",
        Data = result,
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
      };
    }

    [HttpPost]
    public JsonResult GetCharts(int chart_id = 0, int report_id = 0)
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      ChartBuilding chartBuilding = new ChartBuilding();
      var reportCharts = chartBuilding.getReportCharts(_meliasoftData, chart_id, report_id, IsFromAIweb());

      return new JsonNetResult
      {
        ContentType = "application/json",
        Data = reportCharts,
        JsonRequestBehavior = JsonRequestBehavior.AllowGet
      };
    }

  }

}
