using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
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

public partial class Facebook_Account : ObservableRecipient
{
    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string twoFA;

    [ObservableProperty]
    private string cookies;

    [ObservableProperty]
    private Facebook_AccountStatus status_account;

    [ObservableProperty]
    private Facebook_CookieStatus status_cookie;

    [ObservableProperty]
    private string token1;

    [ObservableProperty]
    private string token2;

    //Infomation
    [ObservableProperty]
    private string name = "N/A";

    [ObservableProperty]
    private string uid = "N/A";

    [ObservableProperty]
    public string email = "N/A";

    [ObservableProperty]
    private string birthday = "N/A";

    [ObservableProperty]
    private string locale = "N/A";

    public Facebook_Account(string[] rawDatas)
    {
        status_account = Facebook_AccountStatus.Default;
        status_cookie = Facebook_CookieStatus.Default;
        username = rawDatas[0];
        password = rawDatas[1];
        for (int i = 0; i < rawDatas.Length; i++)
        {
            if (rawDatas[i].Contains("c_user"))
            {
                cookies = rawDatas[i];
                break;
            }
        }
        if (rawDatas.Length > 2)
        {
            if (!rawDatas[2].Contains("c_user"))
            {
                twoFA = rawDatas[2];
            }
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Facebook_Account otherPerson)
        {
            return Username == otherPerson.Username;
        }
        return false;
    }
}
