using System.Threading.Tasks;
using Translate.Models;

namespace Translate.Services;

public interface ITranslateService
{
    /// <summary>
    /// 自动执行翻译
    /// </summary>
    /// <param name="value">翻译内容</param>
    /// <returns></returns>
    Task<TranslateDto> ExecuteAsync(string value);

}