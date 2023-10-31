namespace Token.Translate.Helper;

public class StringHelper
{
    /// <summary>
    /// 判断是否为英文
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public static bool IsEnglish(string input)
    {
        foreach (char c in input)
        {
            // 判断字符是否在英文字符的编码范围内
            if ((c >= '\u0041' && c <= '\u005A') || (c >= '\u0061' && c <= '\u007A'))
            {
                return true;
            }
        }

        return false;
    }
}