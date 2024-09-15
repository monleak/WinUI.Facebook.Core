using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI.Facebook.Core.Models;

public enum Facebook_PageScanSource
{
    SearchBar,
    AdsLib,
    Default
}

public enum Facebook_TypePrintPageScan
{
    id,
    idname,
    idnamefollowers
}

public class Facebook_PageScan
{
    public Facebook_PageScanSource Source { get; set; } = Facebook_PageScanSource.Default;
    public string id { get; set; }
    public string name { get; set; }
    public string followers { get; set; }
    public static Facebook_TypePrintPageScan TypePrint { get; set; } = Facebook_TypePrintPageScan.id;

    public override string ToString()
    {
        switch (Facebook_PageScan.TypePrint)
        {
            case Facebook_TypePrintPageScan.id:
                return id.Replace(" ", "").Replace("\n", "");
            case Facebook_TypePrintPageScan.idname:
                return id.Replace(" ", "").Replace("\n", "") + "|" + name.Trim().Replace("\n", "");
            case Facebook_TypePrintPageScan.idnamefollowers:
                return id.Replace(" ", "").Replace("\n", "") + "|" + name.Trim().Replace("\n", "") + "|" + followers.Trim().Replace("\n", "");
            default:
                return id.Replace(" ", "").Replace("\n", "");
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Facebook_PageScan otherPage)
        {
            return id == otherPage.id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + id.GetHashCode();
            return hash;
        }
    }
}
