using Newtonsoft.Json.Linq;
using WinUI.Facebook.Core.Contracts.Services;
using WinUI.Facebook.Core.Models;
using HtmlAgilityPack;
using WinUI.Facebook.Core.Static;
using WinUI.Facebook.Core.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Collections.ObjectModel;

namespace WinUI.Facebook.Core.Services;
public class Facebook_SearchByKeywordService : IFacebook_SearchByKeywordService
{
    public async Task<string> preloadGetDocid_scanPage(string link)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, link);
                request.Headers.Add("sec-ch-ua", SendUnifiedRequestStatic.SecChUa);
                request.Headers.Add("Referer", "https://www.facebook.com/");
                request.Headers.Add("Origin", "https://www.facebook.com");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("user-agent", SendUnifiedRequestStatic.UserAgent);
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var resultString = await response.Content.ReadAsStringAsync();
                if (resultString.Contains("__d(\"SearchCometResultsPaginatedResultsQuery_facebookRelayOperation\",[],(function(a,b,c,d,e,f){e.exports"))
                {
                    return StringHandleHelper.GetSubString(resultString, "__d(\"SearchCometResultsPaginatedResultsQuery_facebookRelayOperation\",[],(function(a,b,c,d,e,f){e.exports=\"", "\"})");
                }
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<(List<Facebook_PageScan>, string, string, string, string, string, string, string, string, string, string, string, string)>
        SearchPageFacebookAsync(string cookie, string keyword)
    {
        List<Facebook_PageScan> pageFBs = new List<Facebook_PageScan>();
        string end_cursor = string.Empty;
        string tokenDTSGInitialData = string.Empty;
        string tokenLSD = string.Empty;
        string userID = string.Empty;
        string haste_session = string.Empty;
        string rev = string.Empty;
        string hsi = string.Empty;
        string spin_r = string.Empty;
        string spin_b = string.Empty;
        string spin_t = string.Empty;
        string bsid = string.Empty;
        string docid = string.Empty;
        try
        {
            string stringRes;
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://www.facebook.com/search/pages/?q=" + keyword);
                request.Headers.Add("authority", "www.facebook.com");
                request.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                request.Headers.Add("accept-language", SendUnifiedRequestStatic.AcceptLanguage);
                request.Headers.Add("cache-control", "max-age=0");
                request.Headers.Add("cookie", cookie);
                request.Headers.Add("dpr", "1");
                request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                request.Headers.Add("sec-ch-ua", SendUnifiedRequestStatic.SecChUa);
                request.Headers.Add("sec-ch-ua-full-version-list", SendUnifiedRequestStatic.SecChUaFullVersionList);
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-model", "\"\"");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-ch-ua-platform-version", "\"15.0.0\"");
                request.Headers.Add("sec-fetch-dest", "document");
                request.Headers.Add("sec-fetch-mode", "navigate");
                request.Headers.Add("sec-fetch-site", "same-origin");
                request.Headers.Add("sec-fetch-user", "?1");
                request.Headers.Add("upgrade-insecure-requests", "1");
                request.Headers.Add("user-agent", SendUnifiedRequestStatic.UserAgent);
                request.Headers.Add("viewport-width", "1021");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                stringRes = await response.Content.ReadAsStringAsync();
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(stringRes);

            var getpreload = htmlDocument.DocumentNode.SelectNodes("//link[@rel='preload']");

            if (getpreload is HtmlNodeCollection preloadLinks)
            {
                foreach (var link in preloadLinks)
                {
                    // Lấy giá trị của thuộc tính href
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(hrefValue))
                    {
                        docid = await preloadGetDocid_scanPage(hrefValue);
                        if (!string.IsNullOrEmpty(docid))
                        {
                            break;
                        }
                    }
                }
            }

            var scriptTags = htmlDocument.DocumentNode.Descendants("script");
            foreach (var scriptTag in scriptTags)
            {
                string scriptContent = scriptTag.InnerText;
                if (string.IsNullOrEmpty(end_cursor) && scriptContent.Contains("serpResponse"))
                {
                    JObject json = JObject.Parse(scriptContent);
                    JArray edgesArray = (JArray)json["require"][0][3][0]["__bbox"]["require"][5][3][1]["__bbox"]["result"]["data"]["serpResponse"]["results"]["edges"];
                    bsid = (string)json["require"][0][3][0]["__bbox"]["require"][5][3][1]["__bbox"]["result"]["data"]["serpResponse"]["results"]["edges"][0]["relay_rendering_strategy"]["view_model"]["chaining_action_view_model"]["chaining_params"]["bsid"];
                    foreach (JObject edge in edgesArray)
                    {
                        JObject profile = (JObject)edge["relay_rendering_strategy"]["view_model"]["profile"];
                        Facebook_PageScan pageFB = new();
                        pageFB.id = (string)profile["id"];
                        pageFB.name = (string)profile["name"];
                        pageFBs.Add(pageFB);
                    }
                    end_cursor = (string)json["require"][0][3][0]["__bbox"]["require"][5][3][1]["__bbox"]["result"]["data"]["serpResponse"]["results"]["page_info"]["end_cursor"];
                }
                else if (string.IsNullOrEmpty(tokenDTSGInitialData) && scriptContent.Contains("\"DTSGInitialData\":{\"token\":\""))
                {
                    tokenDTSGInitialData = StringHandleHelper.GetSubString(scriptContent, "\"DTSGInitialData\":{\"token\":\"", "\"}");
                    tokenLSD = StringHandleHelper.GetSubString(scriptContent, "\"LSD\":{\"token\":\"", "\"}");
                    haste_session = StringHandleHelper.GetSubString(scriptContent, "\"haste_session\":\"", "\"");
                    userID = StringHandleHelper.GetSubString(scriptContent, "\"USER_ID\":\"", "\"");
                    rev = StringHandleHelper.GetSubString(scriptContent, "\"server_revision\":", ",");
                    hsi = StringHandleHelper.GetSubString(scriptContent, "\"hsi\":\"", "\",");
                    spin_r = StringHandleHelper.GetSubString(scriptContent, "\"__spin_r\":", ",");
                    spin_b = StringHandleHelper.GetSubString(scriptContent, "\"__spin_b\":", ",");
                    spin_t = StringHandleHelper.GetSubString(scriptContent, "\"__spin_t\":", ",");
                }

                if(!string.IsNullOrEmpty(end_cursor) && !string.IsNullOrEmpty(tokenDTSGInitialData))
                {
                    break;
                }
            }
        }
        catch{}
        return (pageFBs, end_cursor, tokenDTSGInitialData, tokenLSD, userID, haste_session, rev, hsi, spin_r, spin_b, spin_t, bsid, docid);
    }

    private async Task<(List<Facebook_PageScan>, string)> SearchPageFacebookAsync(string cookie, string keyword,
            string cursor, string tokenDTSGInitialData, string tokenLSD, string userID, string haste_session, string rev, string hsi,
            string spin_r, string spin_b, string spin_t, string bsid, string docid)
    {
        List<Facebook_PageScan> pageFBs = new();
        string end_cursor = "";

        try
        {
            string script;
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://www.facebook.com/api/graphql/");
                request.Headers.Add("authority", "www.facebook.com");
                request.Headers.Add("accept", "*/*");
                request.Headers.Add("accept-language", SendUnifiedRequestStatic.AcceptLanguage);
                request.Headers.Add("cookie", cookie);
                request.Headers.Add("dpr", "1");
                request.Headers.Add("origin", "https://www.facebook.com");
                request.Headers.Add("referer", "https://www.facebook.com/search/pages/?q=" + keyword);
                request.Headers.Add("sec-ch-prefers-color-scheme", "light");
                request.Headers.Add("sec-ch-ua", SendUnifiedRequestStatic.SecChUa);
                request.Headers.Add("sec-ch-ua-full-version-list", SendUnifiedRequestStatic.SecChUaFullVersionList);
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-model", "\"\"");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-ch-ua-platform-version", "\"15.0.0\"");
                request.Headers.Add("sec-fetch-dest", "empty");
                request.Headers.Add("sec-fetch-mode", "cors");
                request.Headers.Add("sec-fetch-site", "same-origin");
                request.Headers.Add("user-agent", SendUnifiedRequestStatic.UserAgent);
                request.Headers.Add("viewport-width", "1021");
                request.Headers.Add("x-asbd-id", "129477");
                request.Headers.Add("x-fb-friendly-name", "SearchCometResultsPaginatedResultsQuery");
                request.Headers.Add("x-fb-lsd", tokenLSD);
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new KeyValuePair<string, string>("av", userID));
                collection.Add(new KeyValuePair<string, string>("__user", userID));
                collection.Add(new KeyValuePair<string, string>("__a", "1"));
                collection.Add(new KeyValuePair<string, string>("__hs", haste_session));
                collection.Add(new KeyValuePair<string, string>("dpr", "1"));
                collection.Add(new KeyValuePair<string, string>("__ccg", "GOOD"));
                collection.Add(new KeyValuePair<string, string>("__rev", rev));
                collection.Add(new KeyValuePair<string, string>("__s", ""));
                collection.Add(new KeyValuePair<string, string>("__hsi", hsi));
                collection.Add(new KeyValuePair<string, string>("__dyn", ""));
                collection.Add(new KeyValuePair<string, string>("__csr", ""));
                collection.Add(new KeyValuePair<string, string>("__comet_req", "15"));
                collection.Add(new KeyValuePair<string, string>("fb_dtsg", tokenDTSGInitialData));
                collection.Add(new KeyValuePair<string, string>("jazoest", "25640"));
                collection.Add(new KeyValuePair<string, string>("lsd", tokenLSD));
                collection.Add(new KeyValuePair<string, string>("__spin_r", spin_r));
                collection.Add(new KeyValuePair<string, string>("__spin_b", spin_b));
                collection.Add(new KeyValuePair<string, string>("__spin_t", spin_t));
                collection.Add(new KeyValuePair<string, string>("fb_api_caller_class", "RelayModern"));
                collection.Add(new KeyValuePair<string, string>("fb_api_req_friendly_name", "SearchCometResultsPaginatedResultsQuery"));
                collection.Add(new KeyValuePair<string, string>("variables", "{\"UFI2CommentsProvider_commentsKey\":\"SearchCometResultsInitialResultsQuery\",\"allow_streaming\":false,\"args\":{\"callsite\":\"COMET_GLOBAL_SEARCH\",\"config\":{\"exact_match\":false,\"high_confidence_config\":null,\"intercept_config\":null,\"sts_disambiguation\":null,\"watch_config\":null},\"context\":{\"bsid\":\"" + bsid + "\",\"tsid\":null},\"experience\":{\"encoded_server_defined_params\":null,\"fbid\":null,\"type\":\"PAGES_TAB\"},\"filters\":[],\"text\":\"cooking\"},\"count\":5,\"cursor\":\"" + cursor + "\",\"displayCommentsContextEnableComment\":false,\"displayCommentsContextIsAdPreview\":false,\"displayCommentsContextIsAggregatedShare\":false,\"displayCommentsContextIsStorySet\":false,\"displayCommentsFeedbackContext\":null,\"feedLocation\":\"SEARCH\",\"feedbackSource\":23,\"fetch_filters\":true,\"focusCommentID\":null,\"locale\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"search_results_page\",\"scale\":1,\"stream_initial_count\":0,\"useDefaultActor\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__CometUFIIsRTAEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":false,\"__relay_internal__pv__StoriesRingrelayprovider\":false}"));
                collection.Add(new KeyValuePair<string, string>("server_timestamps", "true"));
                collection.Add(new KeyValuePair<string, string>("doc_id", docid));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                script = await response.Content.ReadAsStringAsync();
            }

            JObject json = JObject.Parse(script);
            JArray edgesArray = (JArray)json["data"]["serpResponse"]["results"]["edges"];
            foreach (JObject edge in edgesArray)
            {
                try
                {
                    JObject profile = (JObject)edge["relay_rendering_strategy"]["view_model"]["profile"];
                    Facebook_PageScan pageFB = new();
                    pageFB.id = (string)profile["id"];
                    pageFB.name = (string)profile["name"];
                    pageFBs.Add(pageFB);
                }
                catch{ }
            }
            end_cursor = (string)json["data"]["serpResponse"]["results"]["page_info"]["end_cursor"];
        }
        catch{}
        return (pageFBs, end_cursor);
    }

    public async Task SearchByKeywordAsync(Facebook_Account account, string keyword, IEnumerable<Facebook_PageScan> pages)
    {
        (var pageFBs, var end_cursor, var tokenDTSGInitialData,
            var tokenLSD, var userID, var haste_session,
            var rev, var hsi, var spin_r, var spin_b,
            var spin_t, var bsid, var docid) = await SearchPageFacebookAsync(account.Cookies, keyword);

        foreach(var pageFB in pageFBs)
        {
            pages = pages.Append(pageFB);
        }

        while(!string.IsNullOrEmpty(end_cursor))
        {
            (pageFBs, end_cursor) = await SearchPageFacebookAsync(account.Cookies, keyword, end_cursor, tokenDTSGInitialData, tokenLSD, userID, haste_session, rev, hsi, spin_r, spin_b, spin_t, bsid, docid);
            foreach (var pageFB in pageFBs)
            {
                pages = pages.Append(pageFB);
            }
        }
    }
}
