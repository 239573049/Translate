using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Translate.Exceptions;
using Translate.Models;
using Translate.Options;
using Translate.Services.Dto;

namespace Translate.Services;

public class MicrosoftTranslateService
    (IHttpClientFactory httpClientFactory, SystemOptions systemOptions) : ITranslateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(MicrosoftTranslateService));

    public async Task<TranslateDto> ExecuteAsync(string value)
    {
        UpdateHttpClient();

        var uri = systemOptions.MicrosoftEndpoint.TrimEnd('/') + "/translate?api-version=3.0&to=" +
                  systemOptions.TargetLanguage;
        if (!systemOptions.AutomaticDetection && !string.IsNullOrWhiteSpace(systemOptions.Language))
        {
            uri += "from=" + systemOptions.Language;
        }

        var responseMessage = await _httpClient.PostAsJsonAsync(uri, new object[] { new { Text = value} });

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
            Language = result.detectedLanguage.language,
            TargetLanguage = result.translations.FirstOrDefault()?.to,
            Result = result.translations.FirstOrDefault()?.text
        };

        return dto;
    }

    private void UpdateHttpClient()
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