using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI.Facebook.Core.Helpers;
public class StringHandleHelper
{
    public static string GetSubString(string source, string start, string end)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
        {
            return string.Empty;
        }
        var startIndex = source.IndexOf(start);
        if (startIndex == -1)
        {
            return string.Empty;
        }
        var endIndex = source.IndexOf(end, startIndex + start.Length);
        if (endIndex == -1)
        {
            return string.Empty;
        }
        return source.Substring(startIndex + start.Length, endIndex - startIndex - start.Length);
    }
}
