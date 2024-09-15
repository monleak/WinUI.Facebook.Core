using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI.Facebook.Core.Models;

namespace WinUI.Facebook.Core.Contracts.Services;
public interface IFacebook_SearchByKeywordService
{
    Task SearchByKeywordAsync(Facebook_Account account, string keyword, IEnumerable<Facebook_PageScan> pages);
}
