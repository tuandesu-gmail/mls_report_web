using Dynamitey;
using FastMember;
using Meliasoft.Controllers;
using Meliasoft.Cores;
using Meliasoft.Data;
using Meliasoft.Models;
using Meliasoft.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace Meliasoft.Controllers
{
  [Authorize]
  public class ReportController : BaseController
  {
    public static int pageIndex = 0;
    public ReportController(IMeliasoftData meliasoftData)
        : base(meliasoftData)
    {
    }

    public ActionResult Home(int report_id = 0)
    {
      ViewBag.ReportId = report_id;
      ViewBag.ErrMsg = "";
      return View();
    }

    public ActionResult Comments(string user_name, int report_id = 0, string report_row_code = "")
    {
      string rootUrlReportRowComment = ConfigurationManager.AppSettings["URL_REPORT_ROW_COMMENT"];
      string allowUploadPhotoFlg = ConfigurationManager.AppSettings["ALLOW_UPLOAD_PHOTO_WHEN_COMMENT"];
      ViewBag.UserName = user_name;
      ViewBag.ReportId = report_id;
      ViewBag.ReportRowCode = report_row_code;
      ViewBag.RootUrlReportRowComment = rootUrlReportRowComment;
      ViewBag.AllowUploadPhotoFlg = allowUploadPhotoFlg;
      ViewBag.ErrMsg = "";
      return View();
    }

    public ActionResult Chart(int id = 0)
    {
      ViewBag.Id = id;
      ViewBag.ErrMsg = "";

      return View();
    }

    //private string GetCommentCount(string rootUrlReportRowComment, string subFolderCmt, string report_id = "", string key_detail = "")
    //{
    //    string commentCount = "";
    //    if (string.IsNullOrEmpty(key_detail))
    //    {
    //        key_detail = report_id;
    //    }
    //    // Create a request using a URL that can receive a post.
    //    WebRequest request = WebRequest.Create(rootUrlReportRowComment + subFolderCmt + "/CommentCount?report_row_code=" + key_detail);
    //    // Set the Method property of the request to POST.
    //    request.Method = "POST";

    //    // Create POST data and convert it to a byte array.
    //    string postData = "This is a test that posts this string to a Web server.";
    //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

    //    // Set the ContentType property of the WebRequest.
    //    request.ContentType = "application/x-www-form-urlencoded";
    //    // Set the ContentLength property of the WebRequest.
    //    request.ContentLength = byteArray.Length;

    //    // Get the request stream.
    //    Stream dataStream = request.GetRequestStream();
    //    // Write the data to the request stream.
    //    dataStream.Write(byteArray, 0, byteArray.Length);
    //    // Close the Stream object.
    //    dataStream.Close();

    //    // Get the response.
    //    WebResponse response = request.GetResponse();

    //    // Display the status.
    //    //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

    //    // Get the stream containing content returned by the server.
    //    // The using block ensures the stream is automatically closed.
    //    using (dataStream = response.GetResponseStream())
    //    {
    //        // Open the stream using a StreamReader for easy access.
    //        StreamReader reader = new StreamReader(dataStream);
    //        // Read the content.
    //        commentCount = reader.ReadToEnd();
    //        if (!string.IsNullOrEmpty(commentCount))
    //        {
    //            commentCount = commentCount.Replace("<html>", "").Replace("</html>", "").Replace("<head></head>", "").Replace("<body>", "").Replace("</body>", "");
    //        }
    //        // Display the content.
    //        //Console.WriteLine(responseFromServer);
    //    }

    //    // Close the response.
    //    response.Close();

    //    return commentCount;
    //}

    public ActionResult Index(string id = "0", string key_detail = "")
    {
      try
      {
        string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
        string masterDetailReports = ConfigurationManager.AppSettings["MASTER_DETAIL_REPORTS"];
        string rootUrlReportRowComment = ConfigurationManager.AppSettings["URL_REPORT_ROW_COMMENT"];
        string allowUploadPhotoFlg = ConfigurationManager.AppSettings["ALLOW_UPLOAD_PHOTO_WHEN_COMMENT"];
        string subFolder = ConfigurationManager.AppSettings["SUB_FOLDER"];
        string subFolderCmt = ConfigurationManager.AppSettings["SUB_FOLDER_CMT"];

        string commentCount = "0";
        bool isMasterDetailRpFlg = true; // false;
        Boolean showCommentFlg = false;
        Boolean hasSttFieldFlg = false;
        Boolean hasNgayCtFieldFlg = false;
        bool hasIsKeyDetailColFlg = false;

        int IsHasLockedColumnFlg = 0;

        if (!string.IsNullOrEmpty(masterDetailReports))
          masterDetailReports = masterDetailReports.ToLower();

        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();

        if (id == "0")
        {
          return View("Home");
        }

        string sqlCmd = "SELECT * FROM Web_Report WHERE Id = @ReportId ";
        //if (IsFromAIweb())
        //  sqlCmd = "SELECT * FROM Web_Report WHERE Report_Code = @ReportId";
        if (IsFromAIweb())
          sqlCmd = "SELECT Web_Report.Title_VN AS Name, Web_Report.* FROM Web_Report WHERE  Id = @ReportId";

        var model = _meliasoftData.Query(sqlCmd,
                CommandType.Text,
                new string[] { "@ReportId" },
                new object[] { id })
            .FirstOrDefault();

        if (model != null && model.Note != null)
        {
          string note = model.Note;
          note = note.Replace("<<CHR(13)>>", "<br>");
          note = "<b>Ghi chú:</b><br/>" + note;
          model.Note = note;
        }


        //string reportStoredProcedureName = model.StoredProcedureName;
        //if (!string.IsNullOrEmpty(reportStoredProcedureName) && !reportStoredProcedureName.Contains("["))
        //  reportStoredProcedureName = "[" + reportStoredProcedureName.ToLower() + "]";

        ////if (!string.IsNullOrEmpty(masterDetailReports) && masterDetailReports.Contains(reportStoredProcedureName) )
        ////{                    
        ////    isMasterDetailRpFlg = true;
        ////    if (!string.IsNullOrEmpty(key_detail))
        //        commentCount = this.GetCommentCount(rootUrlReportRowComment, subFolderCmt, Convert.ToString(id), key_detail);
        ////}


        //AND Visible = 1

        string sqlCmd2 = "SELECT * FROM Web_ReportColumnConfig WHERE ReportId = @ReportId ORDER BY [Order]";
        //if (IsFromAIweb())
        //  sqlCmd2 = "SELECT * FROM Web_ReportColumnConfig WHERE Report_Code = @ReportId ORDER BY [Order]";


        var query = _meliasoftData.Query(sqlCmd2,
            CommandType.Text,
            new string[] { "@ReportId" },
            new object[] { id });

        var width = query.Sum(s => s.Width);
        var columns = query.Select(s => s);

        StringBuilder colgroup = new StringBuilder();
        StringBuilder thead = new StringBuilder();
        StringBuilder tbody = new StringBuilder();

        StringBuilder dummy = new StringBuilder();
        StringBuilder fields = new StringBuilder();


        //bool hasIsCheckColFlg = false;
        //foreach (var item in columns)
        //{
        //    if (string.Compare(item.DataFieldName, "IsCheck", true) == 0) {
        //        hasIsCheckColFlg = true;
        //        break;
        //    }
        //}


        foreach (var item in columns)
        {
          string fieldName = item.DataFieldName;
          fieldName = fieldName.ToLower();
          if (string.Compare(fieldName, "stt", true) == 0
              || string.Compare(fieldName, "column_key_detail", true) == 0)
          {
            hasIsKeyDetailColFlg = true;
            break;
          }
        }

        // ----- Add comment column -----
        if (isMasterDetailRpFlg && string.IsNullOrEmpty(key_detail) && (hasIsKeyDetailColFlg))
        {
          //string displayField = "<img class=\"logo\" src=\"/Images/comment_counter.png\" >";
          string keyField = "#=Stt#";

          string displayField = "<div class=\"comment_container\">";
          displayField += "   <img src=\"/Images/#=COUNT_COMMENT_BG_IMG#\" width=\"25\" style=\"margin-left:2px; margin-top: 2px;\">"; //comment_counter_green.png
          displayField += "   <div class=\"comment_counter_centered\"><span style=\"font-size: 63%; letter-spacing: -0.05em; color: grey;\">#=COUNT_COMMENT#</span></div>"; //#=COUNT_COMMENT#
          displayField += "</div> ";

          //string commentUrl = rootUrlReportRowComment + subFolderCmt + "/Default?user_name=" + userName + "&report_id=" + id
          ////+ "&report_row_code=" + keyDetail + "&allow_upload=" + allowUploadPhotoFlg + "&from_root_url=" + rootUrl + subFolder;
          //ViewBag.UserName = _meliasoftData.UserName;
          //ViewBag.Id = id;
          //ViewBag.KeyDetail = key_detail;


          //dummy.Append(", ");
          dummy.Append("[");
          dummy.AppendFormat("{{template: '<a href=\"{0}/ReportComment?user_name={1}&report_id={2}&report_row_code={3}&allow_upload={4}&from_root_url={5}\">{6}</a>', title: \"{7}\", width: \"{8}px\", locked: {9}{10}{11}}}",
          rootUrlReportRowComment + subFolderCmt,
          _meliasoftData.UserName,
          id,
          keyField,
          "YES",
          baseUrl + subFolder,
          displayField,
          "Comment",
          80,
          "false",
          "",
          "");
        }
        //-------------------

        foreach (var item in columns)
        {
          string fieldName = item.DataFieldName;
          fieldName = fieldName.ToLower();

          if (item.Visible)
          {
            bool isNumber = false;
            if (item.Format == null || string.IsNullOrEmpty(item.Format.ToString()))
            {
              //item.Alignment = "left";
            }
            else if (item.Format.ToString() == "{0:#,###}")
            {
              //item.Alignment = "right";
              isNumber = true;
              //item.Format = "number:0";
              //item.Format = "[>0]#,###;[=0]'';[<0]#,###";
              if (fields.Length == 0)
              {
                fields.Append("[");
              }
              else
              {
                fields.Append(", ");
              }

              fields.AppendFormat("{{field: \"{0}\"}}", item.DataFieldName);
            }
            else if (item.Format.ToString() == "{0:dd/MM/yyyy}")
            {
              //item.Alignment = "center";
              //item.Format = "date:\"dd/MM/yyyy\"";
              if (fields.Length == 0)
              {
                fields.Append("[");
              }
              else
              {
                fields.Append(", ");
              }

              fields.AppendFormat("{{field: \"{0}\"}}", item.DataFieldName);
            }
            else
            {
              //item.Alignment = "left";
            }

            if (item.Source != null && !string.IsNullOrEmpty(item.Source.ToString()))
            {
              item.InputType = "Lookup";
            }


            // Tính lại width hợp lý dựa vào độ dài tiêu đề
            var header = (item.HeaderText ?? string.Empty).Trim();
            int widthCol = item.Width; // width từ DB (px)

            // Header NGẮN (<= 40 ký tự) -> đảm bảo đủ chỗ để không bị "..."
            if (!string.IsNullOrEmpty(header) && header.Length <= 40)
            {
              // Ước lượng bề rộng chữ: ~9px/ký tự + 24px padding
              int minWidth = Math.Max(66, Math.Min(220, header.Length * 9 + 24));
              if (widthCol <= 0 || widthCol < minWidth)
                widthCol = minWidth;
            }

            // Nếu width không hợp lệ thì để auto
            if (widthCol <= 0)
              colgroup.Append("<col/>");
            else
              colgroup.AppendFormat("<col width=\"{0}px\"/>", widthCol);

            //colgroup.AppendFormat("<col width=\"{0}px\"/>", item.Width);

            //thead.AppendFormat("<th class=\"text-center\">{0}</th>", item.HeaderText);
            // inline-style thắng mọi CSS của theme/Kendo
            var thStyle = $"min-width:{widthCol}px;white-space:nowrap;overflow:visible;text-overflow:clip;";
            thead.AppendFormat("<th class=\"text-center\" style=\"{1}\">{0}</th>", item.HeaderText, thStyle);

            if (isNumber)
            {
              tbody.AppendFormat("<td class=\"text-{1}\" ng-class-odd=\"'odd'\" ng-class-even=\"'even'\" ng-class=\"{{bold: row.Bold === 'C'{3}}}\">{{{{row.{0} ? (row.{0}{2}) : ''}}}}</td>", item.DataFieldName, item.Alignment, string.IsNullOrEmpty(item.Format) ? string.Empty : string.Format(" | {0}", item.Format), !item.Bold ? string.Empty : " || 0 === 0");
            }
            else
            {
              tbody.AppendFormat("<td class=\"text-{1}\" ng-class-odd=\"'odd'\" ng-class-even=\"'even'\" ng-class=\"{{bold: row.Bold === 'C'{3}}}\">{{{{row.{0}{2}}}}}</td>", item.DataFieldName, item.Alignment, string.IsNullOrEmpty(item.Format) ? string.Empty : string.Format(" | {0}", item.Format), !item.Bold ? string.Empty : " || 0 === 0");
            }

            if (dummy.Length == 0)
            {
              dummy.Append("[");
            }
            else
            {
              dummy.Append(", ");
            }

            var isMainApproveCol = string.Equals(item.DataFieldName, "IsCheck", StringComparison.OrdinalIgnoreCase);
            var isSuffixApproveCol = fieldName.EndsWith("_duyet") || fieldName.EndsWith("_approve");
            var isIsCheckLikeCol = fieldName.Contains("ischeck");

            if (isMainApproveCol || isSuffixApproveCol || isIsCheckLikeCol)
            {
              var fieldNameForTemplate = item.DataFieldName;
              var onChangeAttr = isMainApproveCol ? " onchange=\\\"doalert(this)\\\"" : " onchange=\\\"approveCheckboxChanged(this)\\\"";

              dummy.AppendFormat(
                  "{{locked: {2}, template: '<input type=\"checkbox\" #= {3} ? \\'checked=\"checked\"\\' : \"\" # class=\"chkbx\" data-field=\"{3}\"{4} />', title: \"{0}\", width: \"66px\"}}",
                  item.HeaderText,
                  widthCol,
                  item.Locked ? "true" : "false",
                  fieldNameForTemplate,
                  onChangeAttr);
            }
            else if (string.Compare(item.Format, "{0:dd/MM/yyyy}", true) == 0)
            {
              //dummy.AppendFormat("{{field: \"{0}\", title: \"{1}\", width: \"{2}px\", locked: {3}, template: '#= kendo.toString(kendo.parseDate({0}, \"yyyy-MM-dd\"), \"dd/MM/yyyy\") #'}}", item.DataFieldName, item.HeaderText, item.Width, item.Locked ? "true" : "false");
              dummy.AppendFormat("{{field: \"{0}\", title: \"{1}\", width: \"{2}px\", locked: {3}{4}}}", item.DataFieldName, item.HeaderText, widthCol, item.Locked ? "true" : "false",
                  string.IsNullOrEmpty(item.Alignment) ? "" : string.Format(", attributes: {{ class: \"text-{0}\"  }}", item.Alignment));
            }
            else
            {


              if (isMasterDetailRpFlg && string.IsNullOrEmpty(key_detail)
                  && ("column_key_detail".Equals(fieldName) || "so_ct".Equals(fieldName)
                      || "dien_giai".Equals(fieldName) || "ten_dt".Equals(fieldName))
                      || "file_name".Equals(fieldName)) //So_Ct, Dien_Giai
              {
                //Column_Key_Detail

                if (hasIsKeyDetailColFlg)
                {
                  string displayField = "Chi tiết...";
                  if ("so_ct".Equals(fieldName))
                  {
                    displayField = "#=So_Ct#";
                  }
                  else if ("dien_giai".Equals(fieldName))
                  {
                    displayField = "#=Dien_Giai#";
                  }
                  else if ("ten_dt".Equals(fieldName))
                  {
                    displayField = "#=Ten_Dt#";
                  }
                  else if ("file_name".Equals(fieldName))
                  {
                    displayField = "#=File_Name#";
                  }

                  string keyField = "#=Stt#";
                  if ("stt".Equals(fieldName))
                  {
                    keyField = "#=Stt#";
                  }
                  else if ("column_key_detail".Equals(fieldName))
                  {
                    keyField = "#=Column_Key_Detail#";
                  }

                  dummy.AppendFormat("{{template: '<a href=\"{0}/Report?id={1}&key_detail={2}\">{3}</a>', title: \"{4}\", width: \"{5}px\", locked: {6}{7}{8}}}",
                  subFolder,
                  id,
                  keyField,
                  displayField,
                  item.HeaderText,
                  widthCol, //item.Width,
                  item.Locked ? "true" : "false",
                  string.IsNullOrEmpty(item.Format) ? "" : string.Format(", format: \"{0}\"", item.Format),
                  string.IsNullOrEmpty(item.Alignment) ? "" : string.Format(", attributes: {{ class: \"text-{0}\"  }}", item.Alignment));
                }
                else
                {
                  dummy.AppendFormat("{{field: \"{0}\", title: \"{1}\", width: \"{2}px\", locked: {3}{4}{5}}}",
                  item.DataFieldName,
                  item.HeaderText,
                  widthCol, //item.Width,
                  item.Locked ? "true" : "false",
                  string.IsNullOrEmpty(item.Format) ? "" : string.Format(", format: \"{0}\"", item.Format),
                  string.IsNullOrEmpty(item.Alignment) ? "" : string.Format(", attributes: {{ class: \"text-{0}\"  }}", item.Alignment));
                }

              }
              else
              {
                dummy.AppendFormat("{{field: \"{0}\", title: \"{1}\", width: \"{2}px\", locked: {3}{4}{5}}}",
                    item.DataFieldName,
                    item.HeaderText,
                    widthCol, //item.Width,
                    item.Locked ? "true" : "false",
                    string.IsNullOrEmpty(item.Format) ? "" : string.Format(", format: \"{0}\"", item.Format),
                    string.IsNullOrEmpty(item.Alignment) ? "" : string.Format(", attributes: {{ class: \"text-{0}\"  }}", item.Alignment));
              }

            }

            if (item.Locked)
            {
              IsHasLockedColumnFlg = 1;
            }
          }


          //showCommentFlg
          if ("so_ct".Equals(fieldName) || ("stt".Equals(fieldName) || "column_key_detail".Equals(fieldName)))
          {
            hasSttFieldFlg = true;
          }
          if ("ngay_ct".Equals(fieldName))
          {
            hasNgayCtFieldFlg = true;
          }

        }



        if (hasSttFieldFlg && hasNgayCtFieldFlg)
          showCommentFlg = true;

        //if (dummy.Length == 0)
        //{
        //    dummy.Append("[");
        //}
        //else
        //{
        //    dummy.Append(", ");
        //}

        //dummy.AppendFormat("{{template: '<input type=\"checkbox\" #= IsCheck ? \\'checked=\"checked\"\\' : \"\" # class=\"chkbx\" />', title: \"...\", width: \"80px\"}}");


        if (dummy.Length == 0)
        {
          dummy.Append("[]");
        }
        else
        {
          dummy.Append("]");
        }
        if (fields.Length == 0)
        {
          fields.Append("[]");
        }
        else
        {
          fields.Append("]");
        }

        colgroup.AppendFormat("<col width=\"80px\"  min-width=\"80px\"/>");
        thead.AppendFormat("<th class=\"text-center\"><input type=\"checkbox\" ng-model=\"check\" ng-change=\"checkAll()\"/></th>");
        tbody.AppendFormat("<td class=\"text-center\" ng-class-odd=\"'odd'\" ng-class-even=\"'even'\"><input type=\"checkbox\" ng-model=\"row.IsCheck\" /></td>");



        ViewBag.RootUrlReportRowComment = rootUrlReportRowComment;
        ViewBag.AllowUploadPhotoFlg = allowUploadPhotoFlg;
        ViewBag.SubFolder = subFolder;
        ViewBag.SubFolderCmt = subFolderCmt;
        ViewBag.UserName = _meliasoftData.UserName;
        ViewBag.Id = id;
        ViewBag.KeyDetail = key_detail;
        ViewBag.ShowCommentFlg = showCommentFlg;
        ViewBag.CommentCount = 0; // commentCount; 
        ViewBag.Width = string.Format("{0}px", width);
        ViewBag.ColGroup = colgroup;
        ViewBag.THead = thead;
        ViewBag.TBody = tbody;

        ViewBag.Dummy = dummy;
        ViewBag.Fields = fields;

        ViewBag.HasLockedColumn = IsHasLockedColumnFlg;

        string sqlCmd3 = "SELECT * FROM Web_SpParameterConfig WHERE ReportId = @ReportId ORDER BY [_Order]";
        //if (IsFromAIweb())
        //  sqlCmd3 = "SELECT * FROM Web_SpParameterConfig WHERE Report_Code = @ReportId ORDER BY [_Order]";


        query = _meliasoftData.Query(sqlCmd3,
                CommandType.Text,
                new string[] { "@ReportId" },
                new object[] { id });

        StringBuilder param = new StringBuilder();
        List<ParameterModel> paramModel = new List<ParameterModel>();
        List<bool> popupModel = new List<bool>();

        int index = 0;
        int indexPopup = 0;
        foreach (var item in query)
        {
          string name = item.Name.Replace("@", "");
          object value = null;


          //string cookieKey = "meliasoft_param_" + id + "_" + name;
          //string cookieValue = "";
          //if (Request.Cookies[cookieKey] != null)
          //{
          //    cookieValue = Request.Cookies[cookieKey].Value;
          //}

          if (name.ToUpper().Contains("OPEN_KEY"))
          {
            //paramModel.Add(new ParameterModel { Name = name, Value = this.GetOpenKey() });
            if (IsFromAIweb())
              param.AppendFormat(@"<input type=""hidden"" ng-model=""param[{2}].Value"">", item.Title_VN, name, index);
            else
              param.AppendFormat(@"<input type=""hidden"" ng-model=""param[{2}].Value"">", item.Title, name, index);

            if (name.ToUpper().Contains("OPEN_KEY"))
              value = this.GetOpenKey();
            else
              value = string.Empty;
          }
          else
          {

            string inputType = item.InputType.ToString();
            string itemTitle = item.Title;
            //if (IsFromAIweb())
            //  itemTitle = item.Title_VN;

            switch (inputType)
            {
              case "DateTimePicker":
                param.AppendFormat(@"<div class=""col-lg-6 col-md-6 col-sm-6 col-xs-12 form-group"">
                            <div class=""col-lg-3 col-md-3 col-sm-4 col-xs-12"">{0}</div>
                            <div class=""col-lg-9 col-md-9 col-sm-8 col-xs-12"">
                                <input class=""form-control datepicker"" type=""text"" ng-model=""param[{2}].Value"" uib-datepicker-popup = ""{{{{format}}}}"" is-open = ""popup[{3}]"" datepicker-options = ""dateOptions"" ng-required = ""true"" close-text = ""Close"" alt-input-formats = ""altInputFormats"">
                                <span class=""btn btn-default date-picker"" ng-click=""openPopup({3})"">
                                    <i class=""glyphicon glyphicon-calendar""></i>
                                </span>
                            </div>
                        </div>", itemTitle, name, index, indexPopup++);

                popupModel.Add(false);

                //DateTime now = DateTime.Now;
                //switch (name)
                //{
                //    case "_Ngay_Ct1":
                //        value = now.AddDays(1 - now.Day);
                //        break;
                //    case "_Ngay_Ct2":
                //        value = now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1);
                //        break;
                //    default:
                //        value = now;
                //        break;
                //}
                value = "";
                break;
              case "TextBox":
                param.AppendFormat(@"<div class=""col-lg-6 col-md-6 col-sm-6 col-xs-12 form-group"">
                            <div class=""col-lg-3 col-md-3 col-sm-4 col-xs-12"">{0}</div>
                            <div class=""col-lg-9 col-md-9 col-sm-8 col-xs-12"">
                                <input class=""form-control"" type=""text"" ng-model=""param[{2}].Value"">
                            </div>
                        </div>", itemTitle, name, index);

                value = string.Empty; ; // cookieValue; // string.Empty;
                break;
              case "Lookup":
                param.AppendFormat(@"<div class=""col-lg-6 col-md-6 col-sm-6 col-xs-12 form-group"">
                            <div class=""col-lg-3 col-md-3 col-sm-4 col-xs-12"">{0}</div>
                            <div class=""col-lg-9 col-md-9 col-sm-8 col-xs-12"">
                                <angucomplete-alt id=""lookup{3}"" pause=""100""
                                    selected-object=""param[{2}].Value""
                                    remote-api-handler=""searchAPI""
                                    search-fields=""Code,Name""
                                    title-field=""Code""
						            description-field=""Name""
                                    minlength=""1""
                                    input-class=""form-control form-control-small"" />
                            </div>
                        </div>", itemTitle, name, index, item.Id);

                value = string.Empty;
                break;
              case "CheckBox":
                var guid = Guid.NewGuid().ToString();
                //string checkedValue = "";
                //string cssClass = "form-control";
                //if (cookieValue == "true" || cookieValue == "1")
                //{
                //    checkedValue = "checked"; //checkedValue = "checked= 'checked'";
                //    //cssClass = "form-control ng-valid ng-touched ng-dirty ng-valid-parse ng-not-empty";
                //}

                //param.AppendFormat(@"<div class=""col-lg-6 col-md-6 col-sm-6 col-xs-12 form-group"">
                //    <div class=""col-lg-3 col-md-3 col-sm-4 col-xs-12""></div>
                //    <div class=""col-lg-9 col-md-9 col-sm-8 col-xs-12"">
                //        <input class=""{5}"" type=""checkbox"" ng-model=""param[{2}].Value"" id=""{3}"" style=""float: left; width: 34px;"" {4}>
                //        <label for=""{3}"" style=""height: 34px; margin: 8px 10px; font-weight: normal;"">{0}</label>
                //    </div>
                //</div>", item.Title, name, index, guid, checkedValue, cssClass);

                param.AppendFormat(@"<div class=""col-lg-6 col-md-6 col-sm-6 col-xs-12 form-group"">
                                <div class=""col-lg-3 col-md-3 col-sm-4 col-xs-12""></div>
                                <div class=""col-lg-9 col-md-9 col-sm-8 col-xs-12"">
                                    <input class=""form-control"" type=""checkbox"" ng-model=""param[{2}].Value"" id=""{3}"" style=""float: left; width: 34px;"">
                                    <label for=""{3}"" style=""height: 34px; margin: 8px 10px; font-weight: normal;"">{0}</label>
                                </div>
                            </div>", itemTitle, name, index, guid);


                value = false; // cookieValue; // 
                break;
              //                    case "ComboBox":
              //                        param.AppendFormat(@"<div class=""col-lg-6 col-md-6 col-sm-6 col-xs-12 form-group"">
              //                            <div class=""col-lg-3 col-md-3 col-sm-4 col-xs-12"">{0}</div>
              //                            <div class=""col-lg-9 col-md-9 col-sm-8 col-xs-12"">
              //                                <input class=""form-control"" type=""checkbox"" ng-model=""param[{2}].Value"">
              //                            </div>
              //                        </div>", item.Title, name, index);

              //                        value = string.Empty;
              //                        break;

              case "None":
              case "Hidden":
                param.AppendFormat(@"<input type=""hidden"" ng-model=""param[{2}].Value"">", itemTitle, name, index);
                if (name.ToUpper().Contains("OPEN_KEY"))
                  value = this.GetOpenKey();
                else
                  value = string.Empty;
                break;
            }
          }
          paramModel.Add(new ParameterModel { Name = name, Value = value });

          index++;

        }

        ViewBag.Param = param;
        ViewBag.ParamModel = paramModel;
        ViewBag.PopupModel = popupModel;

        ViewBag.ErrMsg = "";

        //return View(model);
        return View((object)model);
      }
      //catch (Exception ex)
      //{
      //  ViewBag.ErrMsg = "System Error: " + ex.Message;
      //  //return View("ErrMsg");
      //  return View();
      //}
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
      }
    }



    //public async Task<DataSet> GetReportRowCommentCountersAsync(String reportId)
    //{
    //    string HOST_URL = ConfigurationManager.AppSettings["HOST_API_URL"];
    //    string API_NAME = "api/WebService/DataTransfer";
    //    string TOKEN_KEY_VALUE = ConfigurationManager.AppSettings["API_ADMIN_TOKEN_KEY"];
    //    //string rowCode = reportRowCode;
    //    //if (string.IsNullOrEmpty(rowCode))
    //    //    rowCode = reportId;

    //    string jsonData = "{";
    //    jsonData += "   \"value\": {";
    //    jsonData += "       \"TokenKey\":\"" + TOKEN_KEY_VALUE + "\", "; //Convert.ToString(Session["TokenKey"])
    //    jsonData += "       \"procedureName\":\"[MLS_API_SP_COUNT_GROUP_BY_REPORT_ROW_CODE]\",";
    //    jsonData += "       \"postData\": {";
    //    jsonData += "           \"REPORT_ID\":\"" + reportId + "\" ";
    //    jsonData += "       }";
    //    jsonData += "    }";
    //    jsonData += "}";

    //    DataSet ds = null;

    //    JsonResult<DataSet> jsonResult;
    //    string url = HOST_URL + API_NAME; // "api/WebService/DataTransfer";

    //    jsonResult = await ApiCaller.PostJsonString<JsonResult<DataSet>>(url, jsonData).ConfigureAwait(false);
    //    //dynamic myListObject = null;

    //    if (jsonResult != null && jsonResult.ResultCode == 0)
    //    {
    //        ds = jsonResult.JsonData;

    //        //// get data
    //        //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
    //        //{
    //        //    //myListObject = FileUtility.ConvertToReportCommentList(ds.Tables[0]);
    //        //}
    //    }

    //    return ds;
    //}


    [HttpPost]
    public JsonResult GetData(int id, string key_detail, List<ParameterModel> param)
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      Boolean showCommentFlg = false;
      Boolean hasSttFieldFlg = false;
      Boolean hasNgayCtFieldFlg = false;

      string sqlCmd = "SELECT * FROM Web_Report WHERE Id = @ReportId";
      if (IsFromAIweb())
        sqlCmd = "SELECT Web_Report.Title_VN AS Name, Web_Report.* FROM Web_Report WHERE Id = @ReportId";

      //sqlCmd = "SELECT Web_Report.Report_Code AS Id, Web_Report.Title_VN AS Name, Web_Report.* FROM Web_Report WHERE  Report_Code = @ReportId"; 

      var report = _meliasoftData.Query(sqlCmd,
              CommandType.Text,
              new string[] { "@ReportId" },
              new object[] { id })
          .FirstOrDefault();

      if (report != null)
      {
        StringBuilder cmdText = new StringBuilder();
        cmdText.AppendFormat("SET DATEFORMAT DMY; EXEC [{0}] ", report.StoredProcedureName);

        List<string> paramNames = new List<string>();
        List<object> paramValues = new List<object>();
        StringBuilder errorText = new StringBuilder();
        errorText.Append(cmdText);

        if (param != null)
        {
          bool first = true;
          foreach (var item in param)
          {
            if (item.Value == null)
            {
              //continue;
              item.Value = "";
            }

            if (!first)
            {
              cmdText.Append(", ");
              errorText.Append(", ");
            }

            cmdText.AppendFormat("@{0} = @{0}", item.Name);

            paramNames.Add(string.Format("@{0}", item.Name));

            ////if (item.Value is DateTime)
            ////    paramValues.Add(((DateTime)item.Value).ToString("dd/MM/yyyy")); //item.Value
            ////else
            ////    paramValues.Add(item.Value); 
            //string inputString = Convert.ToString(item.Value);
            //string paramValue = "";
            //DateTime parsedDateTime;

            //if (DateTime.TryParseExact(inputString, "dd/MM/yyyy",
            //        new CultureInfo("vi-VN"), //en-US
            //        DateTimeStyles.None,
            //        out parsedDateTime)) //if (DateTime.TryParse(inputString, out dateValue))
            //{
            //    //String.Format("{0:d/MM/yyyy}", dDate);
            //    paramValue = parsedDateTime.ToString("dd/MM/yyyy");                            
            //}
            //else
            //{
            //    //DateTime parsedDateTime;
            //    if (DateTime.TryParseExact(inputString, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", null, System.Globalization.DateTimeStyles.None, out parsedDateTime))
            //    {
            //        paramValue = parsedDateTime.ToString("dd/MM/yyyy");
            //    } else
            //    {
            //        if (item.Value != null && item.Value is bool)
            //        {
            //            if (Convert.ToBoolean(item.Value) == true)
            //            {
            //                paramValue = "1";
            //            }
            //            else
            //            {
            //                paramValue = "0";
            //            }
            //        }
            //        else
            //        {
            //            if (inputString.ToLower().Equals("true"))
            //            {
            //                paramValue = "1";
            //            }
            //            else if (inputString.ToLower().Equals("false"))
            //            {
            //                paramValue = "0";
            //            }
            //            else
            //            {
            //                paramValue = inputString;
            //            }
            //        }

            //    }                            
            //}
            //paramValues.Add(paramValue);

            paramValues.Add(item.Value);

            if (item.Value != null)
            {
              var type = item.Value.GetType();
              if (type.Name == "DateTime")
              {
                errorText.AppendFormat("@{0} = '{1}'", item.Name, ((DateTime)item.Value).ToString("dd/MM/yyyy")); //yyyyMMdd
              }
              else
              {
                errorText.AppendFormat("@{0} = '{1}'", item.Name, item.Value);
              }
            }

            first = false;
          }
        }

        try
        {
          //var result = _meliasoftData.Query(cmdText.ToString(),
          //    CommandType.Text,
          //    paramNames.ToArray(),
          //    paramValues.ToArray());
          var resultDs = _meliasoftData.GetData(cmdText.ToString(),
              CommandType.Text,
              paramNames.ToArray(),
              paramValues.ToArray());

          string masterDetailReports = ConfigurationManager.AppSettings["MASTER_DETAIL_REPORTS"];
          if (!string.IsNullOrEmpty(masterDetailReports))
            masterDetailReports = masterDetailReports.ToLower();

          string reportStoredProcedureName = report.StoredProcedureName;
          if (!string.IsNullOrEmpty(reportStoredProcedureName) && !reportStoredProcedureName.Contains("["))
            reportStoredProcedureName = "[" + reportStoredProcedureName.ToLower() + "]";

          int dataTableIdx = 0;
          //if (!string.IsNullOrEmpty(masterDetailReports) && masterDetailReports.Contains(reportStoredProcedureName) && !string.IsNullOrEmpty(key_detail))
          if (!string.IsNullOrEmpty(key_detail))
            if (resultDs.Tables.Count > 1)
              dataTableIdx = 1;

          //key_detail
          DataTable resultTbl = resultDs.Tables[dataTableIdx];
          if (resultTbl.Columns.Contains("Column_Key_Detail"))
          {
            if (dataTableIdx == 1)
            {
              //foreach (DataRow dataRow in resultTbl.Rows)
              //{
              //    string keyDetail = Convert.ToString(dataRow["Column_Key_Detail"]);
              //    if (!key_detail.Equals(keyDetail))
              //    {
              //        resultTbl.Rows.Remove(dataRow);
              //        resultTbl.AcceptChanges();
              //    }
              //}
              foreach (DataRow dataRow in resultTbl.Select())
              {
                string keyDetail = Convert.ToString(dataRow["Column_Key_Detail"]);
                if (!key_detail.Equals(keyDetail))
                {
                  resultTbl.Rows.Remove(dataRow);
                }
              }
            }
            else
            {
              //add column "COUNT_COMMENT"
              resultTbl.Columns.Add("COUNT_COMMENT", typeof(String));
              resultTbl.Columns.Add("COUNT_COMMENT_BG_IMG", typeof(String));

              //// Get comment count
              //using (DataSet ds = GetReportRowCommentCountersAsync(Convert.ToString(id)).Result)
              //{
              //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
              //    {
              //        foreach (DataRow dataRow in resultTbl.Select())
              //        {
              //            bool foundFlg = false;
              //            string keyDetail = Convert.ToString(dataRow["Column_Key_Detail"]);
              //            foreach (DataRow dataRowCnt in ds.Tables[0].Select())
              //            {
              //                string keyDetailCnt = Convert.ToString(dataRowCnt["Column_Key_Detail"]);
              //                if (keyDetail.Equals(keyDetailCnt))
              //                {
              //                    string commentCnt = Convert.ToString(dataRowCnt["COUNT_COMMENT"]);
              //                    // Set Comment Counter
              //                    dataRow["COUNT_COMMENT"] = commentCnt;
              //                    dataRow["COUNT_COMMENT_BG_IMG"] = "comment_counter_green.png";
              //                    foundFlg = true;
              //                    break;
              //                }
              //            }
              //            if (!foundFlg)
              //            {
              //                dataRow["COUNT_COMMENT"] = "0";
              //                dataRow["COUNT_COMMENT_BG_IMG"] = "comment_counter.png";
              //            }
              //        }
              //    } else
              //    {
              //        foreach (DataRow dataRow in resultTbl.Select())
              //        { 
              //            // Set default Comment Counter
              //            dataRow["COUNT_COMMENT"] = "0";
              //        }
              //    }
              //}
              foreach (DataRow dataRow in resultTbl.Select())
              {
                // Set default Comment Counter
                dataRow["COUNT_COMMENT"] = "0";
                dataRow["COUNT_COMMENT_BG_IMG"] = "comment_counter.png";
              }
            }
          }


          //showCommentFlg
          if (resultTbl.Columns.Contains("Column_Key_Detail") || resultTbl.Columns.Contains("stt"))
          {
            hasSttFieldFlg = true;
          }
          if (resultTbl.Columns.Contains("Ngay_CT"))
          {
            hasNgayCtFieldFlg = true;
          }

          if (hasSttFieldFlg && hasNgayCtFieldFlg)
            showCommentFlg = true;

          ViewBag.ShowCommentFlg = showCommentFlg;


          // dt là DataTable dữ liệu chính đã chọn từ resultDs
          var hasStt = resultTbl.Columns.Contains("Stt");
          var hasKeyDetail = resultTbl.Columns.Contains("Column_Key_Detail");

          // Nếu không có Stt nhưng có Column_Key_Detail -> tạo alias Stt từ Column_Key_Detail
          if (!hasStt && hasKeyDetail)
          {
            resultTbl.Columns.Add("Stt", typeof(string));
            foreach (DataRow row in resultTbl.Rows)
            {
              var keyVal = row["Column_Key_Detail"]?.ToString();
              row["Stt"] = keyVal ?? string.Empty;
            }
          }


          List<dynamic> result = resultTbl.ToDynamic();


          //foreach (var row in result)
          //{
          //    //foreach (KeyValuePair<string, object> item in row)
          //    //{
          //    //    if (item.Value != null)
          //    //    {
          //    //        var type = item.Value.GetType();
          //    //        if (type.Name == "Decimal")
          //    //        {
          //    //            if (((decimal)item.Value) == 0)
          //    //            {
          //    //                Dynamic.InvokeSet(row, item.Key, "");
          //    //            }
          //    //        }
          //    //        else if (type.Name == "DateTime")
          //    //        {
          //    //            if (((DateTime)item.Value) == new DateTime(1900, 1, 1))
          //    //            {
          //    //                Dynamic.InvokeSet(row, item.Key, "");
          //    //            }
          //    //            else
          //    //            {
          //    //                Dynamic.InvokeSet(row, item.Key, ((DateTime)item.Value).ToString("dd/MM/yyyy"));
          //    //            }
          //    //        }
          //    //    }
          //    //}

          //    //for (int i = 0; i < row.Count; i++)
          //    //{
          //    //    KeyValuePair<string, object> item = row[i];
          //    //    if (item.Value != null)
          //    //    {
          //    //        var type = item.Value.GetType();
          //    //        if (type.Name == "Decimal")
          //    //        {
          //    //            if (((decimal)item.Value) == 0)
          //    //            {
          //    //                Dynamic.InvokeSet(row, item.Key, "");
          //    //            }
          //    //        }
          //    //        else if (type.Name == "DateTime")
          //    //        {
          //    //            if (((DateTime)item.Value) == new DateTime(1900, 1, 1))
          //    //            {
          //    //                Dynamic.InvokeSet(row, item.Key, "");
          //    //            }
          //    //            else
          //    //            {
          //    //                Dynamic.InvokeSet(row, item.Key, ((DateTime)item.Value).ToString("dd/MM/yyyy"));
          //    //            }
          //    //        }
          //    //    }
          //    //}
          //}

          string voiceMsg = "";
          if (resultDs.Tables.Count > dataTableIdx + 1)
          {
            DataTable voiceTbl = resultDs.Tables[dataTableIdx + 1];
            if (voiceTbl != null && voiceTbl.Rows.Count > 0)
            {
              voiceMsg = Convert.ToString(voiceTbl.Rows[0][0]);
            }
          }
          dynamic dyn = new ExpandoObject();
          var dic = (IDictionary<string, object>)dyn;
          //dic["voiceMsg_Name"] = "voiceMsg";
          dic["VoiceMsgValue"] = voiceMsg;
          result.Add(dyn);

          //var data = result.Skip(pageIndex).Take(10);
          pageIndex++;
          return new JsonNetResult
          {
            ContentType = "application/json",
            Data = result,// data, //result,
                          //VoiceMsg = voiceMsg,
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

    //[HttpPost]
    //[ValidateInput(false)]               // cho phép JSON có ký tự “nhạy cảm”
    //public JsonResult SetData(int id, List<Dictionary<string, object>> param, bool changed)
    //{
    //  _meliasoftData.UserName = GetUserName();
    //  _meliasoftData.OpenKey = GetOpenKey();
    //  _meliasoftData.SavedConnString = GetSavedConnectionString();

    //  try
    //  {
    //    _meliasoftData.ExecuteNonQuery("SET DATEFORMAT DMY; EXEC [Web_Update] @ReportId = @ReportId, @TableName = @TableName", CommandType.Text,
    //        new string[] { "@ReportId", "@TableName" },
    //        new object[] { id, "#TMelia" },
    //        param, "#TMelia");

    //    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
    //  }
    //  catch (Exception ex)
    //  {
    //    return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
    //  }

    //  //return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
    //}

    [HttpPost]
    [ValidateInput(false)]
    public JsonResult SetData() // KHÔNG có tham số ở đây, tránh binder phải deserialize JSON lớn
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();
      string errMsg = "";
      var json = "";
      //string addr = GetAddress();
      string ma_DVCS = GetMaDVCS();

      try
      {
        // Đọc raw body
        Request.InputStream.Position = 0;
        string body;
        using (var reader = new StreamReader(Request.InputStream))
          body = reader.ReadToEnd();

        // Parse JSON
        var root = JObject.Parse(body);
        var id = root.Value<int>("id");
        var changed = root.Value<bool?>("changed") ?? false;
        var paramArray = (JArray)(root["param"] ?? new JArray());

        var rows = paramArray.ToObject<List<Dictionary<string, object>>>();

        // địa chỉ nhận từ client (có thể null)
        var clientAddress = (root["clientAddress"] ?? "").ToString();

        // fallback: IP/machine nếu không có địa chỉ
        var ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrWhiteSpace(ip)) ip = Request.UserHostAddress;
        var logComputer = !string.IsNullOrWhiteSpace(clientAddress)
                            ? $"{clientAddress} ({ip})"
                            : (Environment.MachineName ?? ip ?? "");

        //---------------------

        bool savedOK = false;


        //try
        //{
        //  // rows: List<Dictionary<string, object>> – chỉ gồm các dòng cần update
        //  // 1) Serialize sang JSON (bỏ null cho gọn, định dạng ngày dd/MM/yyyy nếu bạn muốn)
        //  json = JsonConvert.SerializeObject(
        //      rows,
        //      new JsonSerializerSettings
        //      {
        //        NullValueHandling = NullValueHandling.Ignore,
        //        DateFormatString = "dd/MM/yyyy"
        //      });

        //  // 2) Gọi SP đúng kiểu StoredProcedure, truyền JSON vào @_j_Value
        //  // Nếu bạn chỉ muốn truyền đúng @_j_Value và để các tham số khác dùng default trong SP:
        //  _meliasoftData.ExecuteNonQuery(
        //      "[dbo].[MLS_REPORT_APPROVE]",
        //      CommandType.StoredProcedure,
        //      new[] { "@_j_Value", "@M_Name", "@M_Language", "@M_Ma_Dvcs", "@M_Log_Computer" },
        //      new object[] {
        //          json,                               // @_j_Value
        //          _meliasoftData.UserName ?? "MLS",   // @M_Name – dùng username đang đăng nhập
        //          "VN",                               // @M_Language
        //          "A01",                              // @M_Ma_Dvcs (đặt theo thực tế môi trường)
        //          logComputer ?? ""       // @M_Log_Computer (tuỳ chọn)
        //      }
        //  );

        //  errMsg = " - Username: " + _meliasoftData.UserName + " - LogComputer: " + logComputer + " - JSON: " + json;
        //  savedOK = true;
        //}
        //catch (Exception ex) {
        //  savedOK = false;
        //  errMsg = ex.Message + " - Username: " + _meliasoftData.UserName + " - LogComputer: " + logComputer + " - JSON: " + json;
        //}

        try
        {
          // 1) Serialize JSON như bạn đang làm
          json = JsonConvert.SerializeObject(rows, new JsonSerializerSettings
          {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "dd/MM/yyyy"
          });

          // 2) Gọi SP và LẤY KẾT QUẢ (recordset) thay vì ExecuteNonQuery
          var dt = _meliasoftData.ExecuteDataTable(
              "[dbo].[MLS_REPORT_APPROVE]",
              CommandType.StoredProcedure,
              new[] { "@_j_Value", "@M_Name", "@M_Language", "@M_Ma_Dvcs", "@M_Log_Computer" },
              new object[] {
            json,                              // @_j_Value
            _meliasoftData.UserName ?? "MLS",  // @M_Name
            "VN",                               // @M_Language
            ma_DVCS ?? "A01",                              // @M_Ma_Dvcs (tùy môi trường)
            logComputer ?? ""                   // @M_Log_Computer
              }
          );

          // 3) Mặc định coi là thành công nếu SP không trả bảng/cột lỗi
          savedOK = true;
          errMsg = null;

          if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("_Is_Error"))
          {
            var isErr = 0;
            var val = dt.Rows[0]["_Is_Error"];
            if (val != null && val != DBNull.Value) isErr = Convert.ToInt32(val);

            if (isErr >= 0)
            {
              // Ưu tiên VN; nếu thiếu thì rơi về EN/col khác
              string msg =
                  (dt.Columns.Contains("ReMessVN") ? Convert.ToString(dt.Rows[0]["ReMessVN"]) : null) ??
                  (dt.Columns.Contains("ReMess") ? Convert.ToString(dt.Rows[0]["ReMess"]) : null) ??
                  (dt.Columns.Contains("ReMessEN") ? Convert.ToString(dt.Rows[0]["ReMessEN"]) : null);

              savedOK = false;
              errMsg = string.IsNullOrWhiteSpace(msg)
                          ? "Có lỗi từ thủ tục MLS_REPORT_APPROVE nhưng không có thông điệp chi tiết."
                          : msg;
            }
          }
        }
        catch (Exception ex)
        {
          savedOK = false;
          // Giữ lại thông tin debug khi có exception thực sự
          errMsg = ex.Message + " - Username: " + (_meliasoftData.UserName ?? "MLS")
                 + " - LogComputer: " + (logComputer ?? "")
                 + " - JSON: " + json;
        }

        // Trả JSON về client (report.js đã dùng .Success/.Error)
        //return Json(new { Success = savedOK, Error = errMsg }, JsonRequestBehavior.AllowGet);



        if (!savedOK)
        {
          //---------------------
          //// Gọi logic cũ
          //_meliasoftData.ExecuteNonQuery(
          //    "SET DATEFORMAT DMY; EXEC [Web_Update] @ReportId = @ReportId, @TableName = @TableName",
          //    CommandType.Text,
          //    new[] { "@ReportId", "@TableName" },
          //    new object[] { id, "#TMelia" },
          //    rows, "#TMelia");

          try { 
            // 1) (tùy chọn) set dateformat riêng
            _meliasoftData.ExecuteNonQuery("SET DATEFORMAT DMY;", CommandType.Text);

            // 2) gọi SP như StoredProcedure, không để "EXEC ..." trong cmdText
            _meliasoftData.ExecuteNonQuery(
                "[Web_Update]",
                CommandType.StoredProcedure,
                new[] { "@ReportId", "@TableName" },
                new object[] { id, "#TMelia" },
                rows, "#TMelia");

            //savedOK = true;
          }
          catch (Exception ex)
          {
            savedOK = false;
            errMsg += " - [Web_Update]: " + ex.Message;
          }
        }
        

        return Json(new { Success = savedOK, Error = errMsg }, JsonRequestBehavior.AllowGet);
      }
      catch (SqlException ex)
      {
        return Json(new
        {
          Success = false,
          Error = $"SQL {ex.Number} at line {ex.LineNumber} in {ex.Procedure}: {ex.Message} - {errMsg}"
        },
          JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = "Error: " + ex.Message + " - " + errMsg }, JsonRequestBehavior.AllowGet);
      }
    }


    [HttpPost]
    public JsonResult GetMenu()
    {

      try
      {
        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();

        //var commandText = @"
        //            IF EXISTS(SELECT TOP 1 1 FROM Web_Report_Sec WHERE Email = @Email)
        //                SELECT * FROM Web_Report
        //                    INNER JOIN Web_Report_Sec ON Web_Report.Id = Web_Report_Sec.Id_Report
        //                    WHERE Web_Report_Sec.Email = @Email AND Web_Report.Visible = 1
        //                    ORDER BY Name
        //            ELSE
        //                SELECT * FROM Web_Report
        //                    WHERE Web_Report.Visible = 1
        //                    ORDER BY Name";

        //if (IsFromAIweb())
        //  commandText = @"
        //            IF EXISTS(SELECT TOP 1 1 FROM Web_Report_Sec WHERE Email = @Email)
        //                SELECT Web_Report.Title_VN AS Title, Web_Report.Title_VN AS Name, Web_Report.* FROM Web_Report
        //                    INNER JOIN Web_Report_Sec ON Web_Report.Report_Code = Web_Report_Sec.Report_Code
        //                    WHERE Web_Report_Sec.Email = @Email AND Web_Report.Visible = 1
        //                    ORDER BY Stt 
        //            ELSE
        //                SELECT Web_Report.Title_VN AS Title, Web_Report.Title_VN AS Name, Web_Report.* FROM Web_Report
        //                    WHERE Web_Report.Visible = 1
        //                    ORDER BY Stt ";

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
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
      }
    }

    [HttpPost]
    public JsonResult GetAlerts()
    {
      try
      {

        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();

        //"SET DATEFORMAT DMY; EXEC [Web_Update] @ReportId = @ReportId, @TableName = @TableName"
        //var commandText = @"            
        //        SELECT COUNT(*) AS MsgCount FROM Web_Report_Msg
        //            WHERE IsViewed = 0 
        //                AND UserName = @UserName ";
        var commandText = @"            
                    SET DATEFORMAT DMY; EXEC [Web_Alert_Detail] @_Email = @UserName ";

        var result = _meliasoftData.Query(commandText,
            CommandType.Text,
            new string[] { "@UserName" },
            new object[] { _meliasoftData.UserName });

        return new JsonNetResult
        {
          ContentType = "application/json",
          Data = result,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
      }
    }

    [HttpPost]
    public JsonResult GetAlertCount()
    {
      try
      {

        _meliasoftData.UserName = GetUserName();
        _meliasoftData.OpenKey = GetOpenKey();
        _meliasoftData.SavedConnString = GetSavedConnectionString();

        //"SET DATEFORMAT DMY; EXEC [Web_Update] @ReportId = @ReportId, @TableName = @TableName"
        //var commandText = @"            
        //        SELECT COUNT(*) AS MsgCount FROM Web_Report_Msg
        //            WHERE IsViewed = 0 
        //                AND UserName = @UserName ";
        var commandText = @"            
                    SET DATEFORMAT DMY; EXEC [Web_Alert] @_Email = @UserName ";

        var result = _meliasoftData.Query(commandText,
            CommandType.Text,
            new string[] { "@UserName" },
            new object[] { _meliasoftData.UserName });

        return new JsonNetResult
        {
          ContentType = "application/json",
          Data = result,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
      }



    }

    [HttpPost]
    public JsonResult GetCharts(int chart_id = 0, int report_id = 0)
    {
      try
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
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
      }
    }

    [HttpPost]
    public JsonResult GetLookup(string id, string value)
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      id = id.Replace("lookup", "");

      var config = _meliasoftData.Query("SELECT * FROM Web_SpParameterConfig Where Id = @Id",
              CommandType.Text,
              new string[] { "@Id" },
              new object[] { id })
          .FirstOrDefault();

      if (config != null)
      {
        string sql = string.Format("SELECT * FROM ({0}) A WHERE Code LIKE ('%' + @Value + '%') OR Name LIKE ('%' + @Value + '%') ORDER BY Code", config.Source);
        var result = _meliasoftData.Query(sql,
            CommandType.Text,
            new string[] { "@Value" },
            new object[] { value.Trim().ToUpper() });

        return new JsonNetResult
        {
          ContentType = "application/json",
          Data = result,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }
      else
      {
        var result = _meliasoftData.Query("SELECT '' AS Code, '' AS Name", CommandType.Text);

        return new JsonNetResult
        {
          ContentType = "application/json",
          Data = result,
          JsonRequestBehavior = JsonRequestBehavior.AllowGet
        };
      }
    }

    [HttpPost]
    public JsonResult UpdateDefaultChart(int report_id)
    {
      _meliasoftData.UserName = GetUserName();
      _meliasoftData.OpenKey = GetOpenKey();
      _meliasoftData.SavedConnString = GetSavedConnectionString();

      string selected_fields = "";
      string selected_rows = "";
      int chart_type_idx = 0;
      //int show_at_home_page = 1;
      int auto_speech = 1;

      try
      {
        // Get default ChartTypes
        string sqlCmdText = "SELECT TypeId  ";
        sqlCmdText += "  FROM Web_ReportChartTypes ";
        sqlCmdText += "  WHERE AvailableFlg = 1 ";
        sqlCmdText += "  ORDER BY ShowingOrder, TypeName, StyleName ";

        var query = _meliasoftData.Query(sqlCmdText, CommandType.Text);

        var chartTypes = query.Select(s => s);

        var item = chartTypes.ElementAt(chart_type_idx);
        int chartTypeId = item.TypeId;

        // Check exising (before insert)
        if (IsFromAIweb())
        {
          sqlCmdText = "SELECT [Id]  ";
          sqlCmdText += "  FROM Web_ReportCharts ";
          sqlCmdText += "  WHERE ";
          if (IsFromAIweb())
            //cmdText += "    LEFT JOIN Web_ReportCharts RC ON WR.Report_Code = RC.Report_Code ";
            sqlCmdText += "      Report_Code = " + report_id + " ";

        }  else
        {
          sqlCmdText = "SELECT [Id]  ";
          sqlCmdText += "  FROM Web_ReportCharts ";
          sqlCmdText += "  WHERE ";
          sqlCmdText += "      ReportId = " + report_id + " ";
        }
          

        var queryChart = _meliasoftData.Query(sqlCmdText, CommandType.Text);

        if (queryChart.Count == 0)
        {
          // Insert Web_ReportCharts (when not existing)
          string sqlInsert = "INSERT INTO Web_ReportCharts ([ReportId], [ChartTypeId] ,[SelectedColumns],[SelectedRows], [Visible], [AutoSpeech]) VALUES ( "; //[Title] ,[Subtitle] , ,
          sqlInsert += report_id + ", ";

          if (chartTypeId >= 0)
            sqlInsert += chartTypeId + ", ";
          else
            sqlInsert += "1, ";
          sqlInsert += "'" + selected_fields + "', ";
          sqlInsert += "'" + selected_rows + "', ";
          sqlInsert += "1, "; //'" + show_at_home_page + "'
          sqlInsert += auto_speech.ToString();
          sqlInsert += ") ";

          int istResult = _meliasoftData.ExecuteNonQuery(sqlInsert, CommandType.Text);
        }

        // Check exising (after inserted) and get ChartID                

        var queryChart2 = _meliasoftData.Query(sqlCmdText, CommandType.Text);

        if (queryChart2.Count > 0)
        {
          var charts = queryChart2.Select(s => s);

          var oneChartInfo = charts.ElementAt(0);
          int chart_id = oneChartInfo.Id;

          // Save viewed charts:
          if (chart_id > 0)
          {
            string sqlUpdate = "";
            int updResult = -1;
            int istResult = -1;

            try
            {
              sqlUpdate = "UPDATE Web_ReportChart_SavedInfo SET ";
              sqlUpdate += "      SavedInfo = '" + chart_id + ",' + SavedInfo ";
              //if (show_at_home_page == 1)
              //{
              //    sqlUpdate += "      SavedInfo = '" + chart_id + ",' + SavedInfo ";
              //}
              //else
              //{
              //    sqlUpdate += "      SavedInfo = REPLACE(',' + SavedInfo + ',', '," + chart_id + ",', ',') ";
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

                istResult = _meliasoftData.ExecuteNonQuery(sqlInsert, CommandType.Text);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine("ERROR UPDATE/INSERT INTO Web_ReportChart_SavedInfo: " + ex.Message);
            }
          }
        }


      }
      catch (Exception ex)
      {
        return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
      }

      return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
    }
  }





  //class JsonMixedItem
  //{
  //    public string text;
  //    public string values;
  //    public string type;
  //    public string aspect;
  //    public bool contourOnTop;
  //    public string barWidth;


  //}

  public class JsonResult<T>
  {
    public int ResultCode { get; set; }
    public T JsonData { get; set; }
    public string ErrorMsg { get; set; }

  }
}
