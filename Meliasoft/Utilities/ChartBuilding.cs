using Meliasoft.Data;
using Meliasoft.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Meliasoft.Utilities
{
  public class ChartBuilding
  {

    public List<dynamic> getReportCharts(IMeliasoftData meliasoftData, int chart_id = 0, int report_id = 0, bool isFromAIWeb = false)
    {
      //      var commandText = @"          
      //            SELECT ROW_NUMBER() OVER (Order by RC.Id) AS RowNumber, RC.*, 
      //CT.TypeName, CT.StyleName, CT.ChartConfig, CT.ChartDataSample,
      //WR.StoredProcedureName, '0' AS SortNo 
      //              FROM Web_ReportCharts RC
      //                  LEFT JOIN Web_ReportChartTypes CT ON CT.TypeId = RC.ChartTypeId
      //   LEFT JOIN Web_Report WR ON WR.Id = RC.ReportId
      //              WHERE RC.Visible = 1 ";

      //--------------------------------------------------------
      var commandText = @"          
                        SELECT TOP 10 ROW_NUMBER() OVER (Order by RC.Id DESC) AS RowNumber, 
                            RC.Id, RC.ReportId, RC.ChartTypeId, RC.SelectedColumns, RC.SelectedRows, RC.AutoSpeech, 
                            CT.TypeName, CT.StyleName, CT.ChartConfig, CT.ChartDataSample,
                            WR.StoredProcedureName, WR.Title, '0' AS SortNo, '' AS VoiceMsg, '' AS ElementCount   
                        FROM Web_ReportCharts RC
                            LEFT JOIN Web_ReportChartTypes CT ON CT.TypeId = RC.ChartTypeId
                            LEFT JOIN Web_Report WR ON WR.Id = RC.ReportId
                        WHERE (WR.Visible = 1 AND RC.Visible = 1) ";

      if (isFromAIWeb)
        commandText = @"          
                        SELECT TOP 10 ROW_NUMBER() OVER (Order by RC.Id DESC) AS RowNumber, 
                            RC.Id, RC.ReportId, RC.ChartTypeId, RC.SelectedColumns, RC.SelectedRows, RC.AutoSpeech, 
                            CT.TypeName, CT.StyleName, CT.ChartConfig, CT.ChartDataSample,
                            WR.StoredProcedureName, WR.Title_VN AS Title, '0' AS SortNo, '' AS VoiceMsg, '' AS ElementCount   
                        FROM Web_ReportCharts RC
                            LEFT JOIN Web_ReportChartTypes CT ON CT.TypeId = RC.ChartTypeId
                            LEFT JOIN Web_Report WR ON WR.Id = RC.ReportId
                        WHERE (WR.Visible = 1 AND RC.Visible = 1) ";

      ////--------------------------------------------------------
      //var commandText = "SELECT TOP 10 ROW_NUMBER() OVER (Order by RC.Id DESC) AS RowNumber, 	";
      //commandText += "    ISNULL(RC.Id, 0) AS Id, WR.Id AS ReportId, ISNULL(RC.ChartTypeId, (SELECT TOP (1) [TypeId] FROM [Web_ReportChartTypes]  ORDER BY [ShowingOrder])) AS ChartTypeId, ";
      //commandText += "    ISNULL(RC.SelectedColumns, '') AS SelectedColumns, ISNULL(RC.SelectedRows, '') AS SelectedRows,  ";
      //commandText += "    ISNULL(CT.TypeName, 'bar') AS TypeName, ISNULL(CT.StyleName, 'standard') AS StyleName, ";
      //commandText += "    ISNULL(CT.ChartConfig, '{\"type\": \"bar\", \"title\": {\"text\": \"[chart_title]\",\"font-color\": \"#605F3D\",\"backgroundColor\": \"none\",\"font-size\": \"16px\",\"align\": \"left\",\"alpha\": 1,\"adjust-layout\":true}, \"plot\":{\"value-box\":{ \"short\":true  }, \"tooltip\": { \"text\": \"%t: %v \", \"padding\": \"5 10\"}, \"styles\":[\"#FFD700\",\"orange\",\"yellow\",\"#DEB887\", \"green\",\"blue\", \"#D2691E\",\"purple\", \"#B8860B\", \"brown\",\"#9370DB\", \"#C71585\"] }, \"plotarea\": {\"adjust-layout\": true}, \"scale-y\":{ \"short\":true }, \"scale-x\": {\"label\": { \"text\": \"[chart_subtitle]\"}, \"labels\": [chart_x_captions]}, \"series\": [chart_data]}') AS ChartConfig, ";
      //commandText += "    ISNULL(CT.ChartDataSample, '[{\"values\": [1, 1, 1, 1, 1, 1, 1, 1]}]') AS ChartDataSample,";
      //commandText += "    WR.StoredProcedureName, WR.Title, '0' AS SortNo, '' AS VoiceMsg  "; 
      //commandText += " FROM Web_Report WR ";
      //commandText += "    LEFT JOIN Web_ReportCharts RC ON WR.Id = RC.ReportId ";
      //commandText += "    LEFT JOIN Web_ReportChartTypes CT ON CT.TypeId = RC.ChartTypeId ";
      //commandText += "    WHERE WR.Visible = 1   ";

      if (report_id > 0)
      {
        //if (isFromAIWeb)
        //  commandText += @" OR WR.Report_Code = " + report_id; //AND
        //else
        commandText += @" OR WR.Id = " + report_id; //AND
      }


      if (chart_id > 0)
        commandText += @" OR RC.Id = " + chart_id; //AND

      commandText += @" ORDER BY RowNumber";

      var reportCharts = meliasoftData.Query(commandText, CommandType.Text);
      //List<dynamic> reportCharts_Sorted ;

      if (reportCharts != null)
      {
        foreach (var oneChart in reportCharts)
        {
          oneChart.SortNo = Convert.ToString(oneChart.RowNumber);
        }

        commandText = @"          
                  SELECT *
                    FROM Web_ReportChart_SavedInfo 
                    WHERE AccountName = '" + meliasoftData.UserName + "'";

        var reportCharts_SavedInfo = meliasoftData.Query(commandText, CommandType.Text);

        string[] reportIdList = new string[] { };

        if (reportCharts_SavedInfo != null)
        {
          foreach (var oneChartInfo in reportCharts_SavedInfo)
          {
            string savedInfo = oneChartInfo.SavedInfo;
            //string[] reportIdList = savedInfo.Split(",",);
            string[] stringSeparators = new string[] { "," };
            reportIdList = savedInfo.Split(stringSeparators, StringSplitOptions.None);
            string hitIDs = ",";

            for (int i = 0; i < reportIdList.Length; i++)
            {
              foreach (var oneChart in reportCharts)
              {
                string chartId = Convert.ToString(oneChart.Id);
                if (reportIdList[i].Equals(chartId) && !hitIDs.Contains("," + chartId + ","))
                {
                  hitIDs += oneChart.Id + ",";
                  oneChart.SortNo = Convert.ToString(i + 1); //SortNo
                                                             //oneChart.RowNumber = i;
                  break;
                }
              }
            }
            break;
          }
          reportCharts.Sort((x, y) => x.SortNo.CompareTo(y.SortNo));
          //reportCharts.Sort((x, y) => x.RowNumber.CompareTo(y.RowNumber));
          for (int i = 0; i < reportCharts.Count; i++)
          {
            var oneChart = reportCharts[i];
            oneChart.SortNo = Convert.ToString(i + 1);
          }
        }

        string lastestViewedChartIds = "";
        int chartCounter = 0;

        foreach (var rowChart in reportCharts)
        {
          if (chartCounter < 10)
          {
            // Set chart's id for save
            chartCounter++;
            if (string.IsNullOrEmpty(lastestViewedChartIds))
            {
              lastestViewedChartIds = Convert.ToString(rowChart.Id);
            }
            else
            {
              lastestViewedChartIds += "," + Convert.ToString(rowChart.Id);
            }
          }

          //if (chartCounter == 1)
          //{
          string chartTitle = ""; // rowChart.Title;
          string chartSubTitle = ""; //rowChart.Subtitle;
          string selectedRowIndexes = "";

          if (!string.IsNullOrEmpty(rowChart.SelectedRows))
            selectedRowIndexes = "," + rowChart.SelectedRows + ",";

          //string selectedColumnIndexes = "," + rowChart.SelectedColumns + ",";
          string selectedColumnNames = "";
          if (!string.IsNullOrEmpty(rowChart.SelectedColumns))
            selectedColumnNames = "," + rowChart.SelectedColumns + ",";

          string chartConfig = rowChart.ChartConfig;
          string chartData = "";
          string chartXCaptions = "";
          string xOneCaption = "";
          string chartXLabels = "";
          string xOneLabel = "";


          int countSelectedRows = selectedRowIndexes.Split(',').Length - 1;  // OR: int count = source.Count(f => f == '/'); 
          int countSelectedCols = selectedColumnNames.Split(',').Length - 1;

          List<string> listSelectedColumns = selectedColumnNames.Split(',').ToList();

          // -------------- Get Report Comluns -------------- 
          string cmdReportColumnConfig = @"SELECT  [Id]
                                                    ,[ReportId]
                                                    ,[HeaderText]
                                                    ,[DataFieldName]
                                                    ,[Format]
                                                    ,[Bold]
                                                FROM [dbo].[Web_ReportColumnConfig]
                                                WHERE [Visible] = 1 AND [ReportId] = {0}
                                                ORDER BY [ReportId], [Order]";

          //if (isFromAIWeb)
          //  cmdReportColumnConfig = @"SELECT  *
          //                                      FROM [dbo].[Web_ReportColumnConfig]
          //                                      WHERE [Visible] = 1 AND [Id] = {0}
          //                                      ORDER BY [Order]";
          //cmdReportColumnConfig = @"SELECT  [Id]
          //                                        ,[Report_Code] AS ReportId
          //                                        ,[HeaderText]
          //                                        ,[DataFieldName]
          //                                        ,[Format]
          //                                        ,[Bold]
          //                                    FROM [dbo].[Web_ReportColumnConfig]
          //                                    WHERE [Visible] = 1 AND [Report_Code] = {0}
          //                                    ORDER BY [Report_Code], [Order]";


          StringBuilder cmdText_ReportColumnConfig = new StringBuilder();
          cmdText_ReportColumnConfig.AppendFormat(cmdReportColumnConfig, rowChart.ReportId);

          var resultReportColumns = meliasoftData.Query(cmdText_ReportColumnConfig.ToString(), CommandType.Text);

          //var columns = resultReportColumns.Select(s => s);

          // -------------- Get Report Data -------------- 
          StringBuilder cmdText = new StringBuilder();
          bool testingFlg = false;
          if (chartCounter == 1 && testingFlg)
          {
            cmdText.AppendFormat("SET DATEFORMAT DMY; SELECT [TaskName], [PlanStart], [PlanEnd],[ActualStart],[ActualEnd],[PercentCompleted] FROM [Web_ReportChart_ProgressSample]");
            selectedColumnNames = "";
            selectedRowIndexes = "";
            rowChart.TypeName = Const.CHART_TYPE_H_BAR;
            rowChart.StyleName = Const.CHART_STYLE_NAME_OVERLAPPING_PROGRESS;
            chartConfig = "{\"type\": \"hbar\",  \"plot\":{ \"bars-overlap\": \"100%\", \"hover-state\": {  \"visible\": false }, \"animation\": {  \"delay\": 300,  \"sequence\": 1 } }, \"labels\": [chart_x_captions],  \"scale-x\": { \"labels\": [chart_x_labels]   }, \"series\": [chart_data]}";
          }
          else
          {
            cmdText.AppendFormat("SET DATEFORMAT DMY; EXEC [{0}] ", rowChart.StoredProcedureName);
          }


          StringBuilder errorText = new StringBuilder();
          errorText.Append(cmdText);

          try
          {
            //var resultReport = meliasoftData.Query(cmdText.ToString(), CommandType.Text);
            var resultDs = meliasoftData.GetData(cmdText.ToString(), CommandType.Text);

            DataTable resultTbl = resultDs.Tables[0];

            List<dynamic> resultReport = resultTbl.ToDynamic();

            //List<KeyValuePair<string, object>> lKVP = new List<KeyValuePair<string, object>>();
            List<JsonItem> lKVP = new List<JsonItem>(); //JsonItem

            int rowIndex = 0;
            int usedRowIndex = 0;


            ////List<KeyValuePair<string, object>> listValues = new List<KeyValuePair<string, object>>();
            //Dictionary<string, JsonItem> listValues = new Dictionary<string, JsonItem>();
            //JsonItem oneValues;

            string offsetValues = "";
            string offsetValues1 = "";
            string offsetValues2 = "";

            string text1 = "";
            string text2 = "";
            string text3 = "";
            string text4 = "";

            string values1 = "";
            string values2 = "";
            string values3 = "";
            string values4 = "";
            //string values5 = "";

            foreach (var row in resultReport)
            {
              rowIndex++;

              //if (selectedColumnNames.Length > 1)
              //{
              string text = "";
              string values = "";

              int colIndex = 0;


              foreach (KeyValuePair<string, object> item in row)
              {

                if (selectedColumnNames.Contains("," + item.Key + ",") || (selectedColumnNames.Length == 0) || testingFlg)
                {
                  colIndex++;

                  // Columns are series, rows are values, but first column are captions

                  string boldFlg = "K";

                  try
                  {
                    if (row.Bold != null)
                      boldFlg = row.Bold;
                  }
                  catch (Exception ex)
                  {
                    boldFlg = "K";
                  }

                  bool foundFlg = false;
                  if (selectedRowIndexes.Length > 1)
                  {
                    if (selectedRowIndexes.Contains("," + rowIndex + ","))
                    {
                      foundFlg = true;
                    }
                  }
                  else
                  {
                    if (boldFlg != "C")
                      foundFlg = true;
                  }

                  if (foundFlg || testingFlg)
                  {

                    if ((countSelectedRows > 1 && countSelectedRows >= countSelectedCols) || (countSelectedRows <= 1) || (selectedColumnNames.Length == 0)
                        || (rowChart.TypeName == Const.CHART_TYPE_H_BAR && rowChart.StyleName == Const.CHART_STYLE_NAME_OVERLAPPING_PROGRESS))
                    {

                      if (colIndex == 1)
                      {
                        text = Convert.ToString(item.Value);
                        text = text.Trim().Replace("[+]", "").Replace("[-]", "");
                        text = text.Trim().Replace("[", "(").Replace("]", ")");
                        text = text.Trim();
                        if (text.IndexOf('+') == 0)
                        {
                          text = text.Remove(0, 1);
                          text = text.Trim();
                        }
                        if (text.IndexOf('-') == 0)
                        {
                          text = text.Remove(0, 1);
                          text = text.Trim();
                        }
                        text = FormatLabel(text);

                        xOneCaption = text;


                        if ((rowChart.TypeName == Const.CHART_TYPE_H_BAR
                            && rowChart.StyleName == Const.CHART_STYLE_NAME_OVERLAPPING_PROGRESS)
                            || testingFlg)
                        {
                          xOneCaption = "\"text\": \"" + xOneCaption + ": [PercentValue" + usedRowIndex + "]%\", \"background-color\": \"transparent\", ";
                          xOneCaption += "\"font-size\": \"12px\",\"font-family\": \"arial\", \"font-weight\": \"normal\",";
                          xOneCaption += "\"font-color\": \"#000000\", \"padding\": \"6%\", \"border-radius\": \"3px\",";
                          xOneCaption += "\"offset-x\": -100, \"offset-y\": 0, \"shadow\": false, \"callout\": false,";
                          xOneCaption += "\"hook\": \"node:plot=1;index=" + usedRowIndex + "\" ";

                          if (string.IsNullOrEmpty(chartXCaptions))
                          {
                            chartXCaptions = "{" + xOneCaption + "}";
                          }
                          else
                          {
                            chartXCaptions += ",{" + xOneCaption + "}";
                          }

                          xOneLabel = "Task " + rowIndex.ToString();
                          if (string.IsNullOrEmpty(chartXLabels))
                          {
                            chartXLabels = "\"" + xOneLabel + "\"";
                          }
                          else
                          {
                            chartXLabels += ",\"" + xOneLabel + "\"";
                          }

                        }
                        else
                        {
                          if (string.IsNullOrEmpty(chartXCaptions))
                          {
                            chartXCaptions = "\"" + xOneCaption + "\"";
                          }
                          else
                          {
                            chartXCaptions += ",\"" + xOneCaption + "\"";
                          }

                          xOneLabel = text;
                          if (string.IsNullOrEmpty(chartXLabels))
                          {
                            chartXLabels = "\"" + xOneLabel + "\"";
                          }
                          else
                          {
                            chartXLabels += ",\"" + xOneLabel + "\"";
                          }
                        }

                      }
                      else
                      {
                        if ((rowChart.TypeName == Const.CHART_TYPE_H_BAR && rowChart.StyleName == Const.CHART_STYLE_NAME_OVERLAPPING_PROGRESS)
                            || (rowChart.TypeName == Const.CHART_TYPE_BAR && rowChart.StyleName == Const.CHART_STYLE_NAME_LAYOUT_3X))
                        {
                          if (colIndex == 2)
                          {
                            text1 = GetText(item.Key, resultReportColumns);
                            double resultValue = 0;
                            bool checkNumber = Double.TryParse(Convert.ToString(item.Value), out resultValue);
                            if (string.IsNullOrEmpty(values1))
                              values1 = Convert.ToString(resultValue); //item.Value
                            else
                              values1 += "," + Convert.ToString(resultValue); //item.Value
                          }
                          else if (colIndex == 3)
                          {
                            text2 = GetText(item.Key, resultReportColumns);
                            double resultValue = 0;
                            bool checkNumber = Double.TryParse(Convert.ToString(item.Value), out resultValue);
                            if (string.IsNullOrEmpty(values2))
                              values2 = Convert.ToString(resultValue); //item.Value
                            else
                              values2 += "," + Convert.ToString(resultValue); //item.Value
                          }
                          else if (colIndex == 4)
                          {
                            text3 = GetText(item.Key, resultReportColumns);
                            double resultValue = 0;
                            bool checkNumber = Double.TryParse(Convert.ToString(item.Value), out resultValue);
                            if (string.IsNullOrEmpty(values3))
                              values3 = Convert.ToString(resultValue); //item.Value
                            else
                              values3 += "," + Convert.ToString(resultValue); //item.Value
                          }
                          else if (colIndex == 5)
                          {
                            text4 = GetText(item.Key, resultReportColumns);
                            double resultValue = 0;
                            bool checkNumber = Double.TryParse(Convert.ToString(item.Value), out resultValue);
                            if (string.IsNullOrEmpty(values4))
                              values4 = Convert.ToString(resultValue); //item.Value
                            else
                              values4 += "," + Convert.ToString(resultValue); //item.Value
                          }
                          else if (colIndex == 6)
                          {
                            double resultValue = 0;
                            bool checkNumber = Double.TryParse(Convert.ToString(item.Value), out resultValue);
                            chartXCaptions = chartXCaptions.Replace("[PercentValue" + usedRowIndex + "]", resultValue.ToString());

                            usedRowIndex++;
                          }
                        }
                        else
                        {
                          bool validValueFlg = false;

                          if (selectedColumnNames.Length == 0)
                          {
                            if (colIndex == 2)
                              validValueFlg = true;
                          }
                          else
                          {
                            validValueFlg = true;
                          }
                          if (validValueFlg)
                          {
                            double resultValue = 0;
                            bool checkNumber = Double.TryParse(Convert.ToString(item.Value), out resultValue);
                            if (string.IsNullOrEmpty(values))
                              values = Convert.ToString(resultValue); //item.Value
                            else
                              values += "," + Convert.ToString(resultValue); //item.Value
                          }
                          else
                          {
                            break;
                          }
                        }
                      }
                    }
                    else
                    {
                      //============================================
                      //  Columns are captions, rows are values   //
                      //============================================


                      string xCaption = Convert.ToString(item.Key);


                      text = GetText(xCaption, resultReportColumns);

                      //if (string.IsNullOrEmpty(chartXCaptions))
                      //{
                      //    chartXCaptions = "\"" + xCaption + "\"";
                      //}
                      //else
                      //{
                      //    chartXCaptions += ",\"" + xCaption + "\"";
                      //}

                      xOneLabel = text;
                      if (string.IsNullOrEmpty(chartXLabels))
                      {
                        chartXLabels = "\"" + xOneLabel + "\"";
                      }
                      else
                      {
                        chartXLabels += ",\"" + xOneLabel + "\"";
                      }


                      double resultValue = 0;
                      bool checkNumber = Double.TryParse(Convert.ToString(item.Value), out resultValue);
                      //values = Convert.ToString(item.Value);                                                    
                      if (string.IsNullOrEmpty(values))
                        values = Convert.ToString(resultValue); //item.Value
                      else
                        values += "," + Convert.ToString(resultValue); //item.Value

                      ////Add to key-value array
                      if (rowChart.TypeName != Const.CHART_TYPE_BAR)
                      {
                        AddToKVArray(text, values, rowChart, lKVP); //
                        text = "";
                        values = "";
                      }


                    }
                  }

                }

              } // End FOR loop of one ROW ----------------------------------

              if (!string.IsNullOrEmpty(values)) //!string.IsNullOrEmpty(text) &&
              {
                if (rowChart.TypeName == Const.CHART_TYPE_BAR)
                {
                  text = "";
                }
                //Add to key-value array
                AddToKVArray(text, values, rowChart, lKVP); //text
              }


              //}
            }
            // END of one Report-Chart

            if (!string.IsNullOrEmpty(values1) || !string.IsNullOrEmpty(values2)) //!string.IsNullOrEmpty(text) &&
            {
              // Add to multi key-value array
              AddToMultiKVArray(text1, text2, text3, text4, values1, values2, values3, values4, rowChart, lKVP, testingFlg);

              //if (rowChart.TypeName == Const.CHART_TYPE_BAR && rowChart.StyleName == Const.CHART_STYLE_NAME_LAYOUT_3X)
              //{
              //    chartConfig = chartConfig.Replace("\"text\": \"%v\",", "\"text\": \"%t: %v\",");
              //}
            }


            if (lKVP.Count > 0)
            {
              SetBackColorForList(lKVP);

              string json = JsonConvert.SerializeObject(lKVP);
              chartData = json.Replace("\"[", "[").Replace("]\"", "]");
              //[chart_data]
              chartConfig = chartConfig.Replace("[chart_data]", chartData);

              if (!string.IsNullOrEmpty(chartXCaptions))
              {
                chartXCaptions = "[" + chartXCaptions + "]";

              }
              else
              {
                chartXCaptions = "[ ]";
              }
              chartConfig = chartConfig.Replace("[chart_x_captions]", chartXCaptions);

              if (!string.IsNullOrEmpty(chartXLabels))
              {
                chartXLabels = this.ConvertXLabels(chartXLabels);
                chartXLabels = "[" + chartXLabels + "]";

              }
              else
              {
                chartXLabels = "[ ]";
              }
              //chartXLabels = "[ ]"; //tmp

              chartConfig = chartConfig.Replace("[chart_x_labels]", chartXLabels);
              chartConfig = chartConfig.Replace("[chart_x_values]", chartXLabels);

              //[]
            }
            else
            {
              chartXCaptions = "[ ]";
              chartConfig = chartConfig.Replace("[chart_x_captions]", chartXCaptions);

              chartXLabels = "[ ]";
              chartConfig = chartConfig.Replace("[chart_x_labels]", chartXLabels);
              chartConfig = chartConfig.Replace("[chart_x_values]", chartXLabels);

              //rowChart.ChartConfig = rowChart.ChartDataSample;
              chartConfig = chartConfig.Replace("[chart_data]", rowChart.ChartDataSample);
            }

            chartConfig = chartConfig.Replace("[chart_title]", chartTitle);
            chartConfig = chartConfig.Replace("[chart_subtitle]", ""); //chartSubTitle

            rowChart.ChartConfig = chartConfig;

            //------------- Get and Set VoiceMsg --------------
            string voiceMsg = "";
            if (resultDs.Tables.Count > 1)
            {
              DataTable voiceTbl = resultDs.Tables[1];
              if (voiceTbl != null && voiceTbl.Rows.Count > 0)
              {
                voiceMsg = Convert.ToString(voiceTbl.Rows[0][0]);
              }
            }
            rowChart.VoiceMsg = voiceMsg;
            rowChart.ElementCount = Convert.ToString(countSelectedRows);

          }
          catch (Exception ex)
          {
            rowChart.ChartConfig = "{}";
            rowChart.VoiceMsg = "";
            //return Json(new { Success = false, Error = string.Format("Error: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
          }
          //} else
          //{
          //    rowChart.ChartConfig = "";
          //    rowChart.VoiceMsg = "";
          //}


        }

        //Set viewed charts' is for save
        if (chartCounter < 10 && reportIdList != null)
        {
          for (int i = 0; i < reportIdList.Length; i++)
          {
            string tmpChartIds = "," + lastestViewedChartIds + ",";
            if (!tmpChartIds.Contains("," + reportIdList[i] + ","))
            {
              lastestViewedChartIds += "," + reportIdList[i];
              chartCounter++;
              if (chartCounter >= 10)
                break;
            }

          }
        }

        // Save viewed charts:
        try
        {
          string sqlUpdate = "UPDATE Web_ReportChart_SavedInfo SET ";
          sqlUpdate += "      SavedInfo = '" + lastestViewedChartIds + "' ";
          sqlUpdate += " WHERE ";
          sqlUpdate += "      [AccountName] = '" + meliasoftData.UserName + "'";

          int updResult = meliasoftData.ExecuteNonQuery(sqlUpdate, CommandType.Text);

          if (updResult == 0)
          {
            string sqlInsert = "INSERT INTO Web_ReportChart_SavedInfo ([AccountName], [SavedInfo] ) VALUES ( ";
            sqlInsert += " '" + meliasoftData.UserName + "', ";
            sqlInsert += " '" + lastestViewedChartIds + "' ";
            sqlInsert += ") ";

            int istResult = meliasoftData.ExecuteNonQuery(sqlInsert, CommandType.Text);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine("ERROR UPDATE/INSERT INTO Web_ReportChart_SavedInfo: " + ex.Message);
        }
      }

      return reportCharts;
    }

    private string ConvertXLabels(string chartXLabels)
    {
      string cuttedXLabels = "";
      List<string> stringList = chartXLabels.Split(',').ToList();
      foreach (string str in stringList)
      {
        string oneLabel = str.Replace("\"", "");
        if (oneLabel.Contains(" "))
        {
          string cuttedOneLabel = "";
          if (oneLabel.Contains("(") || oneLabel.Contains("<") || oneLabel.Contains("{") || oneLabel.Contains("[") || oneLabel.Contains("/") || oneLabel.Contains(":"))
          {
            cuttedOneLabel = oneLabel;
            if (oneLabel.Length > 18)
            {
              cuttedOneLabel = oneLabel.Substring(0, 18) + "..";
            }
          }
          else
          {
            if (oneLabel.Length > 8)
            {
              List<string> oneList = oneLabel.Split(' ').ToList();
              if (oneList.Count > 1)
              {
                for (int i = 0; i < oneList.Count; i++)
                {
                  string oneWord = oneList[i];
                  if (i == oneList.Count - 1)
                  {
                    cuttedOneLabel += "." + oneWord;
                  }
                  else
                  {
                    if (oneLabel.Length > 12)
                    {
                      cuttedOneLabel += oneWord.Substring(0, 1).ToUpper();
                    }
                    else
                    {
                      if (i == 0)
                      {
                        cuttedOneLabel = oneWord;
                      }
                      else
                      {
                        cuttedOneLabel += oneWord.Substring(0, 1).ToUpper();
                      }
                    }

                  }
                }
              }

            }
          }
          if (!String.IsNullOrEmpty(cuttedOneLabel))
            oneLabel = cuttedOneLabel;

        }
        else if (oneLabel.Length > 7)
        {
          oneLabel = oneLabel.Substring(0, 7);
        }

        if (cuttedXLabels.Length > 0)
          cuttedXLabels += ",";
        cuttedXLabels += "\"" + oneLabel + "\"";
      }

      return cuttedXLabels;
    }

    string GetText(string xCaption, List<dynamic> resultReportColumns)
    {
      string text = "";
      //int rowIndex = 0;
      foreach (var itemCaption in resultReportColumns)
      {
        //rowIndex++;

        if (xCaption.Equals(itemCaption.DataFieldName))
        {
          //selectedColumnNames += item.DataFieldName + ",";
          //selectedColumnNames = selectedColumnNames.Replace("", "");
          xCaption = itemCaption.HeaderText;
          break;
        }
      }

      xCaption = xCaption.Trim().Replace("[+]", "").Replace("[-]", "");
      xCaption = xCaption.Trim().Replace("[", "(").Replace("]", ")");

      text = xCaption;
      text = text.Trim();
      if (text.IndexOf('+') == 0)
      {
        text = text.Remove(0, 1);
        text = text.Trim();
      }
      if (text.IndexOf('-') == 0)
      {
        text = text.Remove(0, 1);
        text = text.Trim();
      }
      text = FormatLabel(text);

      return text;
    }

    string FormatLabel(string inputText)
    {
      string outputText = inputText;
      int maxLen = 100;

      if (inputText.Length > maxLen)
      {
        outputText = inputText.Substring(0, maxLen) + "...";
      }
      else
      {
        //for (int i = inputText.Length; i < maxLen; i++)
        //{
        //    if (i % 3 == 0)
        //    {
        //        outputText += "&nbsp;"; // "";
        //    }
        //}
        //outputText += ".";
      }

      int spaceCount = 0;
      for (int i = 0; i < outputText.Length - 3; i++)
      {
        string oneChar = outputText.Substring(i, 1);
        if (oneChar == " " || oneChar == ":" || oneChar == "." || oneChar == "," || oneChar == "-" || oneChar == ")" || oneChar == "/" || oneChar == "+" || oneChar == "=" || oneChar == ";" || oneChar == "&" || oneChar == "!")
        {
          spaceCount++;
          if (spaceCount == 4 || spaceCount == 8 || spaceCount == 12 || spaceCount == 16 || spaceCount == 20 || spaceCount == 24)
            outputText = outputText.Substring(0, i + 1) + "<br/>" + outputText.Substring(i + 1);

        }
      }

      //for (int i = inputText.Length; i < maxLen; i++)
      //{
      //    if (i % 2 == 0)
      //    {
      //        outputText += "&nbsp;"; // "";
      //    }
      //}
      //outputText += ".";

      return outputText;

    }

    // = new List<JsonItem>();
    void AddToKVArray(string text, string values, dynamic rowChart, List<JsonItem> lKVP)
    {
      string offsetValues = "";

      if (rowChart.TypeName == Const.CHART_TYPE_PIE
                          || rowChart.TypeName == Const.CHART_TYPE_PIE_3D
                          || rowChart.TypeName == Const.CHART_TYPE_RING
                          || rowChart.TypeName == Const.CHART_TYPE_RING_3D
                          || rowChart.TypeName == Const.CHART_TYPE_BAR
                          || rowChart.TypeName == Const.CHART_TYPE_BAR_3D
                          || rowChart.TypeName == Const.CHART_TYPE_H_BAR
                          || rowChart.TypeName == Const.CHART_TYPE_H_BAR_3D
                          || rowChart.TypeName == Const.CHART_TYPE_AREA
                          || rowChart.TypeName == Const.CHART_TYPE_AREA_3D
                          || rowChart.TypeName == Const.CHART_TYPE_V_AREA
                          || rowChart.TypeName == Const.CHART_TYPE_LINE
                          || rowChart.TypeName == Const.CHART_TYPE_V_LINE
                          || rowChart.TypeName == Const.CHART_TYPE_RADAR
                          || rowChart.TypeName == Const.CHART_TYPE_NESTED_PIE)
      {
        if (rowChart.StyleName == Const.CHART_STYLE_NAME_STANDARD
            || rowChart.StyleName == Const.CHART_STYLE_NAME_STANDARD_DEFAULT_COLOR
            || rowChart.StyleName == Const.CHART_STYLE_NAME_LAYOUT_3X
            || rowChart.StyleName == Const.CHART_STYLE_NAME_STACKED
            || rowChart.StyleName == Const.CHART_STYLE_NAME_100P_STACKED
            || rowChart.StyleName == Const.CHART_STYLE_NAME_OFFSET_VALUES
            || rowChart.StyleName == Const.CHART_STYLE_NAME_CYLINDER
            || rowChart.StyleName == Const.CHART_STYLE_NAME_PYRAMID
            || rowChart.StyleName == Const.CHART_STYLE_NAME_SPLINE
            || rowChart.StyleName == Const.CHART_STYLE_NAME_CATEGORY_SCALE
            || rowChart.StyleName == Const.CHART_STYLE_NAME_AREA)
        {
          //normal
          values = "[" + values + "]";
          lKVP.Add(new JsonItem(text, values));

        }
        else if (rowChart.StyleName == Const.CHART_STYLE_NAME_OVERLAPPING)
        {
          //Sample data: [{"values": [70,50,75,51,70,73,67,46], "background-color": "#2B2836", "alpha": 0.5, "hover-state": {   "visible": false }, "max-trackers": 0  },  
          //              { "values": [73,77,91,86,67,76,88,96], "background-color": "#C42E53", "alpha": 0.9, "bar-width": "40%"  }]

          values = "[" + values + "]";
          if (lKVP.Count == 0)
            lKVP.Add(new JsonItem(text, values, 0.5, "#2B2836", "", false, 0, ""));
          else if (lKVP.Count == 1)
            lKVP.Add(new JsonItem(text, values, 0.9, "#C42E53", "", true, 0, "40%"));

        }
        else if (rowChart.StyleName == Const.CHART_STYLE_NAME_OFFSET_VALUES)
        {
          //[{"values":[20,40,25,50,15,45,33,34], "offset-values":[0,3,5,15,28,30,4,9]}]
          values = "[" + values + "]";
          if (lKVP.Count == 0)
            lKVP.Add(new JsonItem(text, values));
          else if (lKVP.Count == 1)
          {
            offsetValues = values;
            values = ((JsonItem)lKVP[0]).values;
            lKVP.RemoveAt(0);
            lKVP.Add(new JsonItem(text, values, offsetValues, 1)); // offset-values = values
          }


        }
        else if (rowChart.StyleName == Const.CHART_STYLE_NAME_OVERLAPPING_PROGRESS)
        {
          // "series": [
          //  {
          //    "values": [70,50,75,51,70,73,67,46],
          // "offset-values":[0,3,5,15,28,30,4,9] 
          //    "background-color": "#2B2836",
          //    "alpha": 0.5,
          //    "hover-state": {
          //      "visible": false
          //    },
          //    "max-trackers": 0
          //  },
          //  {
          //    "values": [73,77,91,86,67,76,88,96],
          // "offset-values":[0,3,5,15,28,30,4,9] 
          //    "background-color": "#C42E53",
          //    "alpha": 0.9,
          //    "bar-width": "40%"
          //  }
          //]

          values = "[" + values + "]";
          if (string.IsNullOrEmpty(offsetValues))
          {
            offsetValues = values;
          }
          else
          {
            //lKVP.Add(new JsonItem(text, values, offsetValues, 1));
            if (lKVP.Count == 0)
              //JsonItem(string _text, string _values, string _offsetValues, double _alpha = 1.0, string _backgroundColor = "", string _goals = "", bool _hoverState = true, int _maxTrackers = 0, string _barWidth = "")
              lKVP.Add(new JsonItem(text, values, offsetValues, 0.5, "#2B2836", "", false, 0, "100%"));
            else if (lKVP.Count == 1)
            {
              lKVP.Add(new JsonItem(text, values, offsetValues, 0.9, "#C42E53", "", false, -1, "40%"));
            }
          }
        }
        else
        {
          //other --> normal style
          values = "[" + values + "]";
          lKVP.Add(new JsonItem(text, values));
        }

      }
      else if (rowChart.TypeName == Const.CHART_TYPE_BULLET
          || rowChart.TypeName == Const.CHART_TYPE_H_BULLET)
      {

        values = "[" + values + "]";
        if (lKVP.Count == 0)
          lKVP.Add(new JsonItem(text, values));
        else if (lKVP.Count == 1)
          lKVP.Add(new JsonItem(text, "", values)); //goals = values

      }
      else if (rowChart.TypeName == Const.CHART_TYPE_MIXED
          || rowChart.TypeName == Const.CHART_TYPE_H_MIXED)
      {
        // Sample data:
        //[{"type": "area","values": [34, 70, 40, 75, 33, 50, 65],"aspect": "stepped","contour-on-top": false,"text": "Area Chart"},
        //{ "type": "bar","values": [49, 30, 21, 15, 59, 51, 69],"bar-width": "50%","text": "Bar Chart"},
        //{ "type": "line","values": [5, 9, 3, 19, 7, 15, 14],"aspect": "spline","text": "Line Chart"}]

        //JsonMixedItem(string _text, string _values, string _type, string _aspect, string _contourOnTop = "", string _barWidth = "")
        values = "[" + values + "]";
        if (lKVP.Count == 0)
          lKVP.Add(new JsonItem(text, values, "area", "stepped", false));
        else if (lKVP.Count == 1)
          lKVP.Add(new JsonItem(text, values, "bar", "", true, "50%"));
        else if (lKVP.Count == 2)
          lKVP.Add(new JsonItem(text, values, "line", "spline"));

      }
      else if (rowChart.TypeName == Const.CHART_TYPE_POP_PYRAMID)
      {
        // Data sample: [{ "data-side": 1, "values": [1656154, 1787564 ]},{ "data-side": 2, "values": [1656154, 1787564 ]} ]
        values = "[" + values + "]";
        if (lKVP.Count == 0)
          //JsonItem(string _dataSide, string _values, bool isPopPyramid, double _alpha = 1.0, string _backgroundColor = "")
          lKVP.Add(new JsonItem(1, values));
        else if (lKVP.Count == 1)
          lKVP.Add(new JsonItem(2, values));
      }
      else if (rowChart.TypeName == Const.CHART_TYPE_RANGE)
      {
        // Data sample: [{"values":[ [15,30], [20,40], [16,35], [21,30], [25,45], [18,27], [23,35], [29,39], [27,30], [19,34] ]} ]
        values = "[" + values + "]";
        lKVP.Add(new JsonItem(text, values));
      }
      else if (rowChart.TypeName == Const.CHART_TYPE_WATERFALL
          || rowChart.TypeName == Const.CHART_TYPE_H_WATERFALL
          || rowChart.TypeName == Const.CHART_TYPE_WATERFALL_3D
          || rowChart.TypeName == Const.CHART_TYPE_H_WATERFALL_3D)
      {
        // Data sample: [{ "values": [420,210,-170,-140,"SUM"] } ]
        values = "[" + values + ",\"SUM\"]";
        lKVP.Add(new JsonItem(text, values));
      }
      else
      {
        // Else: default
        values = "[" + values + "]";
        lKVP.Add(new JsonItem(text, values));
      }
    }

    void AddToMultiKVArray(string text1, string text2, string text3, string text4, string values1, string values2, string values3, string values4, dynamic rowChart, List<JsonItem> lKVP, bool testingFlg = false)
    {
      string offsetValues = "";
      string offsetValues1 = "";
      string offsetValues2 = "";

      if (rowChart.TypeName == Const.CHART_TYPE_PIE
                      || rowChart.TypeName == Const.CHART_TYPE_PIE_3D
                      || rowChart.TypeName == Const.CHART_TYPE_RING
                      || rowChart.TypeName == Const.CHART_TYPE_RING_3D
                      || rowChart.TypeName == Const.CHART_TYPE_BAR
                      || rowChart.TypeName == Const.CHART_TYPE_BAR_3D
                      || rowChart.TypeName == Const.CHART_TYPE_H_BAR
                      || rowChart.TypeName == Const.CHART_TYPE_H_BAR_3D
                      || rowChart.TypeName == Const.CHART_TYPE_AREA
                      || rowChart.TypeName == Const.CHART_TYPE_AREA_3D
                      || rowChart.TypeName == Const.CHART_TYPE_V_AREA
                      || rowChart.TypeName == Const.CHART_TYPE_LINE
                      || rowChart.TypeName == Const.CHART_TYPE_V_LINE
                      || rowChart.TypeName == Const.CHART_TYPE_RADAR
                      || rowChart.TypeName == Const.CHART_TYPE_NESTED_PIE
                      || testingFlg)
      {
        if (rowChart.StyleName == Const.CHART_STYLE_NAME_STANDARD
            || rowChart.StyleName == Const.CHART_STYLE_NAME_STANDARD_DEFAULT_COLOR
            || rowChart.StyleName == Const.CHART_STYLE_NAME_LAYOUT_3X
            || rowChart.StyleName == Const.CHART_STYLE_NAME_STACKED
            || rowChart.StyleName == Const.CHART_STYLE_NAME_100P_STACKED
            || rowChart.StyleName == Const.CHART_STYLE_NAME_OFFSET_VALUES
            || rowChart.StyleName == Const.CHART_STYLE_NAME_CYLINDER
            || rowChart.StyleName == Const.CHART_STYLE_NAME_PYRAMID
            || rowChart.StyleName == Const.CHART_STYLE_NAME_SPLINE
            || rowChart.StyleName == Const.CHART_STYLE_NAME_CATEGORY_SCALE
            || rowChart.StyleName == Const.CHART_STYLE_NAME_AREA)
        {
          //normal
          if (!string.IsNullOrEmpty(values1))
          {
            values1 = "[" + values1 + "]";
            lKVP.Add(new JsonItem(text1, values1));
          }
          if (!string.IsNullOrEmpty(values2))
          {
            values2 = "[" + values2 + "]";
            lKVP.Add(new JsonItem(text2, values2));
          }
          if (!string.IsNullOrEmpty(values3))
          {
            values3 = "[" + values3 + "]";
            lKVP.Add(new JsonItem(text3, values3));
          }
          if (!string.IsNullOrEmpty(values4))
          {
            values4 = "[" + values4 + "]";
            lKVP.Add(new JsonItem(text4, values4));
          }

        }
        else if (rowChart.StyleName == Const.CHART_STYLE_NAME_OVERLAPPING)
        {
          //Sample data: [{"values": [70,50,75,51,70,73,67,46], "background-color": "#2B2836", "alpha": 0.5, "hover-state": {   "visible": false }, "max-trackers": 0  },  
          //              { "values": [73,77,91,86,67,76,88,96], "background-color": "#C42E53", "alpha": 0.9, "bar-width": "40%"  }]

          if (!string.IsNullOrEmpty(values1))
          {
            values1 = "[" + values1 + "]";
            lKVP.Add(new JsonItem(text1, values1, 0.5, "#2B2836", "", false, 0, ""));
          }
          if (!string.IsNullOrEmpty(values2))
          {
            values2 = "[" + values2 + "]";
            lKVP.Add(new JsonItem(text2, values2, 0.9, "#C42E53", "", true, 0, "40%"));
          }
        }
        else if (rowChart.StyleName == Const.CHART_STYLE_NAME_OFFSET_VALUES)
        {
          if (!string.IsNullOrEmpty(values1) && !string.IsNullOrEmpty(values2))
          {
            values1 = "[" + values1 + "]";
            values2 = "[" + values2 + "]";
            offsetValues = values1;
            lKVP.Add(new JsonItem("", values2, offsetValues, 1));
          }
        }
        else if (rowChart.StyleName == Const.CHART_STYLE_NAME_OVERLAPPING_PROGRESS || testingFlg)
        {
          // "series": [
          //  {
          //    "values": [70,50,75,51,70,73,67,46],
          // "offset-values":[0,3,5,15,28,30,4,9] 
          //    "background-color": "#2B2836",
          //    "alpha": 0.5,
          //    "hover-state": {
          //      "visible": false
          //    },
          //    "max-trackers": 0
          //  },
          //  {
          //    "values": [73,77,91,86,67,76,88,96],
          // "offset-values":[0,3,5,15,28,30,4,9] 
          //    "background-color": "#C42E53",
          //    "alpha": 0.9,
          //    "bar-width": "40%"
          //  }
          //]

          offsetValues1 = "[" + values1 + "]";
          values2 = "[" + values2 + "]";
          offsetValues2 = "[" + values3 + "]";
          values4 = "[" + values4 + "]";

          lKVP.Add(new JsonItem("", values2, offsetValues1, 0.6, "gray", "", false, 0, "100%"));
          lKVP.Add(new JsonItem("", values4, offsetValues2, 0.8, "#4ee44e", "", false, -1, "60%")); //008000

        }

      }
      else if (rowChart.TypeName == Const.CHART_TYPE_BULLET
          || rowChart.TypeName == Const.CHART_TYPE_H_BULLET)
      {

        values1 = "[" + values1 + "]";
        values2 = "[" + values2 + "]";

        lKVP.Add(new JsonItem(text1, values1));
        lKVP.Add(new JsonItem(text2, "", values2)); //goals = values

      }
      else if (rowChart.TypeName == Const.CHART_TYPE_MIXED
          || rowChart.TypeName == Const.CHART_TYPE_H_MIXED)
      {
        // Sample data:
        //[{"type": "area","values": [34, 70, 40, 75, 33, 50, 65],"aspect": "stepped","contour-on-top": false,"text": "Area Chart"},
        //{ "type": "bar","values": [49, 30, 21, 15, 59, 51, 69],"bar-width": "50%","text": "Bar Chart"},
        //{ "type": "line","values": [5, 9, 3, 19, 7, 15, 14],"aspect": "spline","text": "Line Chart"}]

        //JsonMixedItem(string _text, string _values, string _type, string _aspect, string _contourOnTop = "", string _barWidth = "")
        values1 = "[" + values1 + "]";
        values2 = "[" + values2 + "]";
        values3 = "[" + values3 + "]";

        lKVP.Add(new JsonItem(text1, values1, "area", "stepped", false));
        lKVP.Add(new JsonItem(text2, values2, "bar", "", true, "50%"));
        lKVP.Add(new JsonItem(text3, values3, "line", "spline"));

      }
      else if (rowChart.TypeName == Const.CHART_TYPE_POP_PYRAMID)
      {
        // Data sample: [{ "data-side": 1, "values": [1656154, 1787564 ]},{ "data-side": 2, "values": [1656154, 1787564 ]} ]
        values1 = "[" + values1 + "]";
        values2 = "[" + values2 + "]";

        lKVP.Add(new JsonItem(1, values1));
        lKVP.Add(new JsonItem(2, values2));
      }
      else if (rowChart.TypeName == Const.CHART_TYPE_RANGE)
      {
        // Data sample: [{"values":[ [15,30], [20,40], [16,35], [21,30], [25,45], [18,27], [23,35], [29,39], [27,30], [19,34] ]} ]
        values1 = "[" + values1 + "]";
        lKVP.Add(new JsonItem("", values1));
      }
      else if (rowChart.TypeName == Const.CHART_TYPE_WATERFALL
          || rowChart.TypeName == Const.CHART_TYPE_H_WATERFALL
          || rowChart.TypeName == Const.CHART_TYPE_WATERFALL_3D
          || rowChart.TypeName == Const.CHART_TYPE_H_WATERFALL_3D)
      {
        // Data sample: [{ "values": [420,210,-170,-140,"SUM"] } ]
        values1 = "[" + values1 + ",\"SUM\"]";
        lKVP.Add(new JsonItem("", values1));
      }
      else
      {
        // Else: default
        values1 = "[" + values1 + "]";
        lKVP.Add(new JsonItem("", values1));
      }
    }

    void SetBackColorForList(List<JsonItem> lKVP)
    {
      string strValue = "";
      double itemValue = 0;
      double minValue = 0;
      double maxValue = 0;
      double avgValue = 0;
      double tolalValue = 0;

      bool isValidFlg = true;

      for (int i = 0; i < lKVP.Count; i++)
      {
        JsonItem jsonItem = lKVP[i];
        strValue = jsonItem.values.Replace("[", "").Replace("]", "");
        if (strValue.Contains(","))
        {
          isValidFlg = false;
          break;
        }
        else
        {
          itemValue = Convert.ToDouble(strValue);
          if (i == 0)
          {
            minValue = itemValue;
            maxValue = itemValue;
          }
          else
          {
            if (itemValue > 0)
            {
              if (minValue > itemValue)
              {
                minValue = itemValue;
              }

              if (maxValue < itemValue)
              {
                maxValue = itemValue;
              }
            }

          }
          tolalValue += itemValue;
        }
      }

      if (isValidFlg)
      {
        avgValue = tolalValue / lKVP.Count;

        for (int i = 0; i < lKVP.Count; i++)
        {
          JsonItem jsonItem = lKVP[i];
          strValue = jsonItem.values.Replace("[", "").Replace("]", "");
          if (!strValue.Contains(","))
          {
            itemValue = Convert.ToDouble(strValue);
            string backColor = "#29a2cc"; // 7F8C8D, 5D6D7E, B0C4DE, 29a2cc, GetBackColor(itemValue, minValue, avgValue, maxValue);
            jsonItem.SetBackColor(backColor);
          }
        }
      }

    }

    string GetBackColor(double itemValue, double minValue, double avgValue, double maxValue)
    {

      string colorCode = "#FF0000"; //red
      if (itemValue <= avgValue)
      {
        double colorRate = (itemValue - minValue) / (avgValue - minValue);
        if (colorRate < 0.1)
        {
          colorCode = "#FF0000"; //red
        }
        else if (colorRate >= 0.1 && colorRate < 0.2)
        {
          colorCode = "#FF3000";
        }
        else if (colorRate >= 0.2 && colorRate < 0.3)
        {
          colorCode = "#FF6000";
        }
        else if (colorRate >= 0.3 && colorRate < 0.4)
        {
          colorCode = "#FF9000";
        }
        else if (colorRate >= 0.4 && colorRate < 0.5)
        {
          colorCode = "#FFC100";
        }
        else if (colorRate >= 0.5 && colorRate < 0.6)
        {
          colorCode = "#FFE800";
        }
        else if (colorRate >= 0.6 && colorRate < 0.7)
        {
          colorCode = "#D1FF00";
        }
        else if (colorRate >= 0.7 && colorRate < 0.8)
        {
          colorCode = "#8FFF00";
        }
        else if (colorRate >= 0.8 && colorRate < 0.9)
        {
          colorCode = "#32FF00";
        }
        else if (colorRate >= 0.9)
        {
          colorCode = "#00FF00";  //green
        }
      }
      else
      {
        double colorRate = (itemValue - avgValue) / (maxValue - avgValue);
        if (colorRate < 0.1)
        {
          colorCode = "#00FF50";
        }
        else if (colorRate >= 0.1 && colorRate < 0.2)
        {
          colorCode = "#00D8FF";
        }
        else if (colorRate >= 0.2 && colorRate < 0.3)
        {
          colorCode = "#00B2FF";
        }
        else if (colorRate >= 0.3 && colorRate < 0.4)
        {
          colorCode = "#008FFF";
        }
        else if (colorRate >= 0.4 && colorRate < 0.5)
        {
          colorCode = "#0070FF";
        }
        else if (colorRate >= 0.5 && colorRate < 0.6)
        {
          colorCode = "#0051FF";
        }
        else if (colorRate >= 0.6 && colorRate < 0.7)
        {
          colorCode = "#0036FF";
        }
        else if (colorRate >= 0.8 && colorRate < 0.9)
        {
          colorCode = "#001CFF";
        }
        else if (colorRate >= 0.9)
        {
          colorCode = "#0000FF"; //blue
        }
      }

      return colorCode;
    }
  }
}