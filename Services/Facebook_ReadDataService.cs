using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI.Facebook.Core.Contracts.Services;
using WinUI.Facebook.Core.Models;

namespace WinUI.Facebook.Core.Services;
public class Facebook_ReadDataService : IFacebook_ReadDataService
{
    public async Task<IEnumerable<Facebook_Account>> ReadRawDataAsync(string pathFile)
    {
        IEnumerable<Facebook_Account> accounts = new ObservableCollection<Facebook_Account>();
        if (File.Exists(pathFile))
        {
            var datas = File.ReadLines(pathFile);
            foreach (var data in datas)
            {
                var arrDataSplit = data.Split('|');
                if (arrDataSplit.Length < 2)
                {
                    continue;
                }
                accounts.Append(new Facebook_Account(arrDataSplit));
            }
            return accounts;
        }
        await Task.CompletedTask;
        return default;
    }
}
