using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Token.Translate.Options;
using Translate.Exceptions;
using Translate.Models;
using Translate.Services.Dto;

namespace Translate.Services;

public class YoudaoTranslateService(IHttpClientFactory httpClientFactory) : ITranslateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(YoudaoTranslateService));

    public async Task<TranslateDto> ExecuteAsync(string value)
    {
        var systemOptions = TranslateContext.GetService<SystemOptions>();

        var param = new Dictionary<string, string[]>()
        {
            { "q", new[] { value } },
            { "from", new[] { systemOptions.AutomaticDetection ? "auto" : systemOptions.Language } },
            { "to", new[] { systemOptions.TargetLanguage } }
        };

        int i = 0;

        AddAuthParams(systemOptions.YoudaoKey, systemOptions.YoudaoAppSecret, @param);

        StringBuilder content = new StringBuilder();

        foreach (var p in param)
        {
            foreach (var v in p.Value)
            {
                if (i > 0)
                {
                    content.Append("&");
                }

                content.AppendFormat("{0}={1}", p.Key, System.Web.HttpUtility.UrlEncode(v));
                i++;
            }
        }

        var para = new StringContent(content.ToString(),Encoding.UTF8,"application/x-www-form-urlencoded");

        var response =
            await _httpClient.PostAsync("https://openapi.youdao.com/api",
                para);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<YoudaoTranslateDto>();
            
            return new TranslateDto()
            {
                Value = value,
                TargetLanguage = systemOptions.TargetLanguage,
                Language = result.l,
                Result = result.translation.FirstOrDefault()
            };
        }

        throw new BusinessException(await response.Content.ReadAsStringAsync())
        {
            Code = (int)response.StatusCode
        };
    }


    public static void AddAuthParams(string appKey, string appSecret, Dictionary<string, string[]> paramsMap)
    {
        string q = "";
        string[] qArray;
        if (paramsMap.ContainsKey("q"))
        {
            qArray = paramsMap["q"];
        }
        else
        {
            qArray = paramsMap["img"];
        }

        foreach (var item in qArray)
        {
            q += item;
        }

        string salt = System.Guid.NewGuid().ToString();
        string curtime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + "";
        string sign = calculateSign(appKey, appSecret, q, salt, curtime);
        paramsMap.Add("appKey", new string[] { appKey });
        paramsMap.Add("salt", new string[] { salt });
        paramsMap.Add("curtime", new string[] { curtime });
        paramsMap.Add("signType", new string[] { "v3" });
        paramsMap.Add("sign", new string[] { sign });
    }

    public static string calculateSign(string appKey, string appSecret, string q, string salt, string curtime)
    {
        string strSrc = appKey + getInput(q) + salt + curtime + appSecret;
        return encrypt(strSrc);
    }

    private static string encrypt(string strSrc)
    {
        Byte[] inputBytes = Encoding.UTF8.GetBytes(strSrc);
        HashAlgorithm algorithm = new SHA256CryptoServiceProvider();
        Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", "");
    }

    private static string getInput(string q)
    {
        if (q == null)
        {
            return "";
        }

        int len = q.Length;
        return len <= 20 ? q : q.Substring(0, 10) + len + q.Substring(len - 10, 10);
    }
}