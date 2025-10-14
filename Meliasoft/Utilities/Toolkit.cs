using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Meliasoft.Utilities
{
  public class Toolkit
  {
    static string strASCII = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
    static string strTCVN3 = "";
    static string strUNICODE = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";

    static byte[] byteTCVN3 = new byte[] { 184, 181, 182, 183, 185, 169, 202, 199, 200, 201, 203, 168, 190, 187, 188, 189, 198, 174, 208, 204, 206, 207, 209, 170, 213, 210, 211, 212, 214, 221, 215, 216, 220, 222, 227, 223, 225, 226, 228, 171, 232, 229, 230, 231, 233, 172, 237, 234, 235, 236, 238, 243, 239, 241, 242, 244, 173, 248, 245, 246, 247, 249, 253, 250, 251, 252, 254, 184, 181, 182, 183, 185, 162, 202, 199, 200, 201, 203, 161, 190, 187, 188, 189, 198, 167, 208, 204, 206, 207, 209, 163, 213, 210, 211, 212, 214, 221, 215, 216, 220, 222, 227, 223, 225, 226, 228, 164, 232, 229, 230, 231, 233, 165, 237, 234, 235, 236, 238, 243, 239, 241, 242, 244, 166, 248, 245, 246, 247, 249, 253, 250, 251, 252, 254 };

    public static string ConvertFontToUNICODE(string strValue)
    {
      if (string.IsNullOrEmpty(strTCVN3))
      {
        strTCVN3 = "";
        foreach (byte b in byteTCVN3)
        {
          strTCVN3 += (char)b;
        }
      }

      string strRet = "";
      bool bolChange = false;
      for (int i = 0; i < strValue.Length; i++)
      {
        char chrChar = strValue[i];
        int intIndex = strTCVN3.IndexOf(chrChar);
        if (intIndex > -1)
        {
          bolChange = true;
          chrChar = strUNICODE[intIndex];
        }
        strRet += chrChar;
      }
      if (bolChange)
      {
        return strRet;
      }

      return strValue;
    }

    public static string ConvertFontToTCVN3(string strValue)
    {
      if (string.IsNullOrEmpty(strTCVN3))
      {
        strTCVN3 = "";
        foreach (byte b in byteTCVN3)
        {
          strTCVN3 += (char)b;
        }
      }

      string strRet = "";
      bool bolChange = false;
      for (int i = 0; i < strValue.Length; i++)
      {
        char chrChar = strValue[i];
        int intIndex = strUNICODE.IndexOf(chrChar);
        if (intIndex > -1)
        {
          bolChange = true;
          chrChar = strTCVN3[intIndex];
        }
        strRet += chrChar;
      }
      if (bolChange)
      {
        return strRet;
      }

      return strValue;
    }

    public static string Encrypt(string user, string pass)
    {
      user = user.ToUpper().PadRight(10).Substring(0, 10);
      pass = pass.ToUpper().PadRight(10).Substring(0, 10);

      string passEncrypt = string.Empty;

      for (int i = 0; i < user.Length; i++)
      {
        var val1 = (i + 1) % user.Length;
        if (val1 == 0)
        {
          val1 = user.Length;
        }

        var val2 = (i + 1) % pass.Length;
        if (val2 == 0)
        {
          val2 = pass.Length;
        }

        var val3 = 2 * (int)user[val1 - 1] + 3 * (int)pass[val2 - 1];
        val3 = 65 + (val3 % 26);

        passEncrypt = passEncrypt + (char)(val3);
      }

      return pass.Aggregate(passEncrypt, (current, t) => current + (char)((int)t + 10));
    }

    public static string GetUserLogInfo(string type)
    {
      var now = DateTime.Now;

      switch (type)
      {
        case "M":
          break;
        case "S":
          break;
        default:
          break;
      }

      return string.Empty;
    }

    public static string HashString(string password)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(string.Format("eten{0}", password));
      return Convert.ToBase64String(SHA256.Create().ComputeHash(bytes));
    }


    public static int ToInt(object v)
    {
      if (v == null) return 0;
      if (v is int i) return i;
      if (v is long l) return (int)l;
      if (v is double d) return (int)Math.Round(d);
      if (v is decimal m) return (int)Math.Round(m);
      var s = v.ToString().Trim().Replace(",", "");
      int x; return int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out x) ? x : 0;
    }

    public static decimal ToDecimal(object v)
    {
      if (v == null) return 0m;
      if (v is decimal m) return m;
      if (v is double d) return Convert.ToDecimal(d);
      if (v is float f) return Convert.ToDecimal(f);
      if (v is int i) return i;
      if (v is long l) return l;
      var s = v.ToString().Trim().Replace(" ", "");
      s = s.Replace(",", ""); // bỏ nghìn
      decimal x; return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out x) ? x : 0m;
    }

    public static DateTime ToDate(object v)
    {
      if (v is DateTime dt) return dt;
      var s = v?.ToString().Trim();
      DateTime x;
      if (DateTime.TryParseExact(s, new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-dd'T'HH:mm:ss.fff'Z'" },
          CultureInfo.InvariantCulture, DateTimeStyles.None, out x)) return x;
      DateTime.TryParse(s, out x);
      return x;
    }

    public static bool ToBool(object v)
    {
      if (v is bool b) return b;
      var s = (v?.ToString() ?? "").Trim().ToLowerInvariant();
      return s == "1" || s == "true" || s == "y" || s == "on";
    }

    public static DateTime? ToDateOrNull(object v)
    {
      if (v == null) return null;
      if (v is DateTime dt) return dt;

      var s = v.ToString().Trim();
      if (string.IsNullOrEmpty(s)) return null;

      DateTime x;
      // thử các định dạng hay gặp
      if (DateTime.TryParseExact(s,
          new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-dd'T'HH:mm:ss.fff'Z'" },
          CultureInfo.InvariantCulture, DateTimeStyles.None, out x)) return x;

      if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out x)) return x;

      return null; // KHÔNG trả MinValue
    }


  }
}