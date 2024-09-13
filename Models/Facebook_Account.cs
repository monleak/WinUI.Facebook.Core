using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI.Facebook.Core.Models;

public enum Facebook_AccountStatus
{
    Live,
    Banned,
    Verify,
    Default
}

public enum Facebook_CookieStatus
{
    Live,
    Die,
    Default
}

public class Facebook_Account
{
    public string username
    {
        get; set;
    }
    public string password
    {
        get; set;
    }

    public string twoFA
    {
        get; set;
    }

    public string cookies
    {
        get; set;
    }

    public Facebook_AccountStatus status_account
    {
        get; set;
    }
    public Facebook_CookieStatus status_cookie
    {
        get; set;
    }

    public string token1
    {
        get; set;
    }
    public string token2
    {
        get; set;
    }


    //Infomation
    public string name
    {
        get; set;
    }
    public string uid
    {
        get; set;
    }
    public string email
    {
        get; set;
    }
    public string birthday
    {
        get; set;
    }

    public string locale
    {
        get; set;
    }

    public Facebook_Account(string[] rawDatas)
    {
        status_account = Facebook_AccountStatus.Default;
        status_cookie = Facebook_CookieStatus.Default;
        username = rawDatas[0];
        password = rawDatas[1];
        if(rawDatas.Length > 2)
        {
            for (int i = 2; i < rawDatas.Length; i++)
            {
                if (rawDatas[i].Contains("c_user"))
                {
                    cookies = rawDatas[i];
                    break;
                }
            }
            if (!rawDatas[2].Contains("c_user"))
            {
                twoFA = rawDatas[2];
            }
        }
    }
}
