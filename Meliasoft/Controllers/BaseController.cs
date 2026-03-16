using Meliasoft.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace Meliasoft.Controllers
{
  public class BaseController : Controller
  {
    protected IMeliasoftData _meliasoftData;

    public BaseController(IMeliasoftData meliasoftData)
    {
      _meliasoftData = meliasoftData;
    }

    protected List<Dictionary<string, object>> GetTableRows(DataTable dtData, string[] paramColumns = null)
    {
      var rows = new List<Dictionary<string, object>>();
      foreach (DataRow dataRow in dtData.Rows)
      {
        var row = new Dictionary<string, object>();
        foreach (DataColumn dataColumn in dtData.Columns)
        {
          object value = dataRow[dataColumn];
          if (value is string && !string.IsNullOrEmpty(value as string) && paramColumns != null)
          {
            if (paramColumns.Any(s => s.ToLower() == dataColumn.ColumnName.ToLower()))
            {
              value = Utilities.Toolkit.ConvertFontToUNICODE(value as string);
            }
          }

          row.Add(dataColumn.ColumnName.ToUpper(), value);
        }

        rows.Add(row);
      }

      return rows;
    }

    protected void ApplyModel(DataTable dtData, object model, string[] paramColumns = null)
    {
      if (dtData.Rows.Count > 0)
      {
        var properties = model.GetType().GetProperties();
        foreach (var property in properties)
        {
          if (dtData.Columns.Contains(property.Name))
          {
            object value = dtData.Rows[0][property.Name];
            if (value is string && !string.IsNullOrEmpty(value as string) && paramColumns != null)
            {
              if (paramColumns.Any(s => s.ToLower() == property.Name.ToLower()))
              {
                value = Utilities.Toolkit.ConvertFontToUNICODE(value as string);
              }
            }

            property.SetValue(model, value);
          }
        }
      }
    }

    private ClaimsPrincipal GetCurrentUser()
    {
      var context = HttpContext.GetOwinContext();
      if (context == null)
      {
        return null;
      }

      if (context.Authentication == null || context.Authentication.User == null)
      {
        return null;
      }

      return context.Authentication.User;
    }

    protected string GetUserName()
    {
      var user = GetCurrentUser();
      if (user == null)
      {
        return null;
      }

      var claim = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier);
      if (claim == null)
      {
        return null;
      }

      //if (!String.IsNullOrEmpty(username) && username.ToLower().Contains("_blank_user"))
      return claim.Value.Replace("_blank_user", "");
    }

    protected string GetMaDVCS()
    {
      var user = GetCurrentUser();
      if (user == null)
      {
        return null;
      }

      var claim = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.StateOrProvince); // Ma_DVCS
      if (claim == null)
      {
        return null;
      }

      //if (!String.IsNullOrEmpty(username) && username.ToLower().Contains("_blank_user"))
      return claim.Value;
    }

    protected string GetAddress()
    {
      var user = GetCurrentUser();
      if (user == null)
      {
        return null;
      }

      var claim = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.StreetAddress);
      if (claim == null)
      {
        return null;
      }

      //if (!String.IsNullOrEmpty(username) && username.ToLower().Contains("_blank_user"))
      return claim.Value;
    }

    protected Boolean IsFromAIweb()
    {
      var user = GetCurrentUser();
      if (user == null)
      {
        return false;
      }

      var claim = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier);
      if (claim == null)
      {
        return false;
      }

      if (claim.Value.Contains("_blank_user"))
        return true;
      else 
        return false;

        //if (!String.IsNullOrEmpty(username) && username.ToLower().Contains("_blank_user"))
        //return claim.Value.Replace("_blank_user", "");
    }

    protected string GetOpenKey()
    {
      var user = GetCurrentUser();
      if (user == null)
      {
        return null;
      }

      var claimUserName = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier);
      bool isBlankUser = claimUserName != null && !string.IsNullOrEmpty(claimUserName.Value) && claimUserName.Value.Contains("_blank_user");

      var claim = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.UserData);
      if (claim == null)
      {
        return null;
      }

      string userData = claim.Value ?? "";
      if (string.IsNullOrWhiteSpace(userData))
      {
        return "";
      }

      if (isBlankUser)
      {
        int lastComma = userData.LastIndexOf(',');
        if (lastComma >= 0 && lastComma < userData.Length - 1)
        {
          string tail = userData.Substring(lastComma + 1).Trim();
          if (!string.IsNullOrEmpty(tail) && !tail.Contains(";") && !tail.Contains("="))
          {
            return tail;
          }
        }

        return "";
      }

      string[] parts = userData.Split(new[] { "," }, StringSplitOptions.None);
      if (parts.Length >= 2 && parts[1] != null)
      {
        return parts[1];
      }

      return "";
    }

    protected string GetSavedConnectionString()
    {
      var user = GetCurrentUser();
      string connStr = "";
      string username = "";

      if (user == null)
      {
        return null;
      }

      var claimUserName = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier);
      if (claimUserName == null)
      {
        return null;
      }
      else
      {
        username = claimUserName.Value;
      }

      var claim = user.Claims.FirstOrDefault(o => o.Type == ClaimTypes.UserData);
      if (claim == null)
      {
        return null;
      }
      else
      {
        if (!String.IsNullOrEmpty(username) && username.ToLower().Contains("_blank_user"))
        {
          string userData = claim.Value;
          //if (decryptInfo.ToLower().Contains("user id") && decryptInfo.ToLower().Contains("password"))
          if (!String.IsNullOrEmpty(userData) && userData.ToLower().Contains("user id") && userData.ToLower().Contains("password"))
          {
            connStr = userData;
            int lastComma = userData.LastIndexOf(',');
            if (lastComma > 0 && lastComma < userData.Length - 1)
            {
              string tail = userData.Substring(lastComma + 1).Trim();
              if (!string.IsNullOrEmpty(tail) && !tail.Contains(";") && !tail.Contains("="))
              {
                connStr = userData.Substring(0, lastComma);
              }
            }
          }
        }

      }
      return connStr; // claim.Value;
    }
  }
}
