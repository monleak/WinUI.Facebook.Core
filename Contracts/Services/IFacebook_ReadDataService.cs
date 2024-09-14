using WinUI.Facebook.Core.Models;

namespace WinUI.Facebook.Core.Contracts.Services;
public interface IFacebook_ReadDataService
{
    Task<IEnumerable<Facebook_Account>> ReadRawDataAsync(string pathFile);
    Facebook_Account InitObjFacebookAccountFromLine(string line);
    void SaveDataAccount(IEnumerable<Facebook_Account> accounts);
    IEnumerable<Facebook_Account> GetDataAccount();
}
