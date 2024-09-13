using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WinUI.Facebook.Core.Contracts.Services;
using WinUI.Facebook.Core.Helpers;
using WinUI.Facebook.Core.Models;

namespace WinUI.Facebook.Core.Services;
public class Facebook_LoginService: IFacebook_LoginService
{
    private async Task<string> GetTokenAccess1Async(Facebook_Account account)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://business.facebook.com/content_management");
                request.Headers.Add("accept", "*/*");
                request.Headers.Add("accept-language", "en-US,en;q=0.9");
                request.Headers.Add("cookie", account.cookies);
                request.Headers.Add("priority", "u=1, i");
                request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"129\", \"Not=A?Brand\";v=\"8\", \"Chromium\";v=\"129\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-fetch-dest", "empty");
                request.Headers.Add("sec-fetch-mode", "cors");
                request.Headers.Add("sec-fetch-site", "none");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var token = StringHandleHelper.GetSubString(content, "[{\"accessToken\":\"", "\"");
                return token;
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<string> GetTokenAccess2Async(Facebook_Account account)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://adsmanager.facebook.com/adsmanager/manage/campaigns");
                request.Headers.Add("accept", "*/*");
                request.Headers.Add("accept-language", "en-US,en;q=0.9");
                request.Headers.Add("cookie", account.cookies);
                request.Headers.Add("priority", "u=1, i");
                request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"129\", \"Not=A?Brand\";v=\"8\", \"Chromium\";v=\"129\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-fetch-dest", "empty");
                request.Headers.Add("sec-fetch-mode", "cors");
                request.Headers.Add("sec-fetch-site", "none");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var linkRedirect = StringHandleHelper.GetSubString(content, "window.location.replace(\"", "\")").Replace("\\","");


                var request2 = new HttpRequestMessage(HttpMethod.Get, linkRedirect);
                request2.Headers.Add("accept", "*/*");
                request2.Headers.Add("accept-language", "en-US,en;q=0.9");
                request2.Headers.Add("cookie", account.cookies);
                request2.Headers.Add("priority", "u=1, i");
                request2.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"129\", \"Not=A?Brand\";v=\"8\", \"Chromium\";v=\"129\"");
                request2.Headers.Add("sec-ch-ua-mobile", "?0");
                request2.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request2.Headers.Add("sec-fetch-dest", "empty");
                request2.Headers.Add("sec-fetch-mode", "cors");
                request2.Headers.Add("sec-fetch-site", "none");
                request2.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36");
                response = await client.SendAsync(request2);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
                var token = StringHandleHelper.GetSubString(content, "window.__accessToken=\"", "\"");
                return token;
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task GetInfomationAccountAsync(Facebook_Account account)
    {
        if (string.IsNullOrEmpty(account.token1) || string.IsNullOrEmpty(account.cookies))
        {
            return;
        }

        try
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://graph.facebook.com/me?access_token={account.token1}");
                request.Headers.Add("accept", "*/*");
                request.Headers.Add("accept-language", "en-US,en;q=0.9");
                request.Headers.Add("cookie", account.cookies);
                request.Headers.Add("priority", "u=1, i");
                request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"129\", \"Not=A?Brand\";v=\"8\", \"Chromium\";v=\"129\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-fetch-dest", "empty");
                request.Headers.Add("sec-fetch-mode", "cors");
                request.Headers.Add("sec-fetch-site", "none");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                JsonObject json = JsonNode.Parse(content) as JsonObject;

                account.name = json["name"]?.GetValue<string>() ?? "";
                account.uid = json["id"]?.GetValue<string>() ?? "";
                account.email = json["email"]?.GetValue<string>() ?? "";
                account.birthday = json["birthday"]?.GetValue<string>() ?? "";
                account.locale = json["locale"]?.GetValue<string>() ?? "";
            }
        }
        catch{}
    }

    public async Task<(string, string)> GetNormalTokenAccessAsync(Facebook_Account account)
    {
        var token1 = await GetTokenAccess1Async(account);
        if(string.IsNullOrEmpty(token1))
        {
            return (string.Empty, string.Empty);
        }
        var token2 = await GetTokenAccess2Async(account);
        return (token1, token2);
    }
}
