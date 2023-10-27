using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Token.Translate.Helper;
using Token.Translate.Options;
using Translate;
using Translate.Models;
using Translate.Services;

namespace Token.Translate.Services;

public class AiTranslateService(IHttpClientFactory httpClientFactory) : ITranslateService
{
    private HttpClient _client;

    public async Task<TranslateDto> ExecuteAsync(string value)
    {
        var systemOptions = TranslateContext.GetService<SystemOptions>();

        UpdateHttpClient(systemOptions);


        string targe;
        if (systemOptions.TranslationChineseAndEnglish)
        {

            if (StringHelper.IsEnglish(value))
            {
                targe = "zh-Hans";
            }
            else
            {
                targe = "en";
            }
        }
        else
        {
            targe = systemOptions.TargetLanguage;
        }

        var option = new
        {
            model = systemOptions.AiModel,
            stream = false,
            temperature = 0.5,
            max_tokens = 2000,
            frequency_penalty = 2,
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = $"下面我发送的所有内容请直接翻译成{targe},并且不要用如何的回复，直接返回内容返回翻译的结果就可以了。"
                },
                new
                {
                    role = "user",
                    content = value
                }
            }
        };

        var responseMessage = await _client.PostAsJsonAsync(systemOptions.AiEndpoint, option);

        if (!responseMessage.IsSuccessStatusCode)
            return new TranslateDto()
            {
                Result = "翻译似乎失败了！",
                Language = systemOptions.Language,
                TargetLanguage = systemOptions.TargetLanguage,
                Value = value
            };

        var result = await responseMessage.Content.ReadFromJsonAsync<ChatResponseDto>();
        return new TranslateDto()
        {
            Result = result.choices.First().message.content,
            Language = systemOptions.Language,
            TargetLanguage = systemOptions.TargetLanguage,
            Value = value
        };

    }


    private void UpdateHttpClient(SystemOptions systemOptions)
    {
        if (_client == null)
        {
            // 不使用代理则默认从工厂创建
            if (!systemOptions.UseProxy)
            {
                _client = httpClientFactory.CreateClient(nameof(AiTranslateService));
            }
            else
            {
                // 请注意这里将只创建一次，所以代理不能实时更新！
                var proxy = new WebProxy(systemOptions.ProxyServer);

                if (!string.IsNullOrWhiteSpace(systemOptions.ProxyUsername))
                {
                    proxy.Credentials = new NetworkCredential(systemOptions.ProxyUsername, systemOptions.ProxyPassword);
                }

                _client = new HttpClient(new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = proxy,
                });
            }
        }

        _client.DefaultRequestHeaders.Remove("X-Token");
        _client.DefaultRequestHeaders.Add("X-Token", "token");
        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + systemOptions.AiKey);
    }
}