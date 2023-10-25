using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Translate.Converters;

public class VisibleConverter : BooleanConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}