using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
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
        List<Facebook_Account> accounts = new List<Facebook_Account>();
        if (File.Exists(pathFile))
        {
            var datas = File.ReadLines(pathFile);
            foreach (var data in datas)
            {
                if(InitObjFacebookAccountFromLine(data) is Facebook_Account account)
                {
                    accounts.Add(account);
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

    public async Task<IEnumerable<Facebook_Account>> ReadJsonDataAsync(string pathFile)
    {
        List<Facebook_Account> accounts = new List<Facebook_Account>();
        if (File.Exists(pathFile))
        {
            var datas = File.ReadAllText(pathFile);
            try
            {
                JsonArray jsonDatas = JsonNode.Parse(datas) as JsonArray;
                foreach (var data in jsonDatas)
                {
                    if (data is JsonObject obj)
                    {
                        try
                        {
                            accounts.Add(new Facebook_Account(obj));
                        }
                        catch { }
                    }
                }
                return accounts;
            }
            catch { }
        }
        await Task.CompletedTask;
        return default;
    }
}
