using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI.Facebook.Core.Contracts.Services;
using WinUI.Facebook.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinUI.Facebook.Core.Services;
public class Facebook_ReadDataService : IFacebook_ReadDataService
{
    private List<Facebook_Account> _accountsStorage = new();

    public IEnumerable<Facebook_Account> GetDataAccount()
    {
        return _accountsStorage;
    }

    public void SaveDataAccount(IEnumerable<Facebook_Account> accounts)
    {
        _accountsStorage.Clear();
        _accountsStorage.AddRange(accounts);
    }

    public async Task<IEnumerable<Facebook_Account>> ReadRawDataAsync(string pathFile)
    {
        IEnumerable<Facebook_Account> accounts = new ObservableCollection<Facebook_Account>();
        if (File.Exists(pathFile))
        {
            var datas = File.ReadLines(pathFile);
            foreach (var data in datas)
            {
                if(InitObjFacebookAccountFromLine(data) is Facebook_Account account)
                {
                    accounts = accounts.Append(account);
                }
            }
            return accounts;
        }
        await Task.CompletedTask;
        return default;
    }

    public Facebook_Account InitObjFacebookAccountFromLine(string line)
    {
        var arrDataSplit = line.Split('|');
        if (arrDataSplit.Length < 2)
        {
            return default;
        }
        return new Facebook_Account(arrDataSplit);
    }
}
