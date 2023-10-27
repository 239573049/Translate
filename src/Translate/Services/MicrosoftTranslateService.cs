using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Token.Translate.Helper;
using Token.Translate.Options;
using Translate;
using Translate.Exceptions;
using Translate.Models;
using Translate.Services;
using Translate.Services.Dto;

namespace Token.Translate.Services;

public class MicrosoftTranslateService
    (IHttpClientFactory httpClientFactory) : ITranslateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(MicrosoftTranslateService));

    public async Task<TranslateDto> ExecuteAsync(string value)
    {
        var systemOptions = TranslateContext.GetService<SystemOptions>();
        UpdateHttpClient(systemOptions);


        var uri = systemOptions.MicrosoftEndpoint.TrimEnd('/') + "/translate?api-version=3.0&to=" +
                  systemOptions.TargetLanguage;

        string targe;
        string language;
        if (systemOptions.TranslationChineseAndEnglish)
        {
            if (StringHelper.IsEnglish(value))
            {
                targe = "zh-Hans";
                language = "en";
                uri += "&to=zh-Hans&from=en";
            }
            else
            {
                language = "zh-Hans";
                targe = "en";
                uri += "&to=en&from=zh-Hans";
            }
        }
        else
        {
            language = systemOptions.Language;
            targe = systemOptions.TargetLanguage;
            uri += "&to=" + systemOptions.TargetLanguage;
            if (!systemOptions.AutomaticDetection && !string.IsNullOrWhiteSpace(systemOptions.Language))
            {
                uri += "from=" + systemOptions.Language;
            }
        }


        var responseMessage = await _httpClient.PostAsJsonAsync(uri, new object[] { new { Text = value } });

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new BusinessException(await responseMessage.Content.ReadAsStringAsync())
            {
                Code = (int)responseMessage.StatusCode
            };
        }

        var result = (await responseMessage.Content.ReadFromJsonAsync<MicrosoftTranslateDto[]>()).FirstOrDefault();

        var dto = new TranslateDto
        {
            Value = value,
            Language = result?.detectedLanguage?.language,
            TargetLanguage = result.translations.FirstOrDefault(x => x.to == targe)?.to,
            Result = result.translations.FirstOrDefault(x => x.to == targe)?.text
        };

        return dto;
    }

    private void UpdateHttpClient(SystemOptions systemOptions)
    {
        _httpClient.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", systemOptions.MicrosoftKey);
        _httpClient.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Region");
        if (!string.IsNullOrEmpty(systemOptions.MicrosoftLocation))
        {
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", systemOptions.MicrosoftLocation);
        }
    }
}