using System;
using System.Globalization;
using System.Windows.Data;

namespace Yahtzee;
public class WidthToFontSizeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is double width)
		{
			Console.WriteLine("good");
			return width / 1;
		}

		return 12;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}