using System;
using System.Globalization;
using Microsoft.Maui.Graphics;

namespace CafeApp.Converters
{
    public class DurumRenkConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte durum)
            {
                return durum switch
                {
                    1 => Color.FromArgb("#27AE60"), // Boş -> Yeşil
                    2 => Color.FromArgb("#E94560"), // Dolu -> Kırmızı
                    3 => Color.FromArgb("#F39C12"), // Rezerve -> Turuncu
                    _ => Color.FromArgb("#533483")  // Bilinmiyor -> Mor
                };
            }
            return Color.FromArgb("#533483");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}