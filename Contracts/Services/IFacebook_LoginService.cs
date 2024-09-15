using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI.Facebook.Core.Models;

namespace WinUI.Facebook.Core.Contracts.Services;
public interface IFacebook_LoginService
{
    Task<bool> GetNormalTokenAccessAsync(Facebook_Account account);
    Task GetInfomationAccountAsync(Facebook_Account account);
}
