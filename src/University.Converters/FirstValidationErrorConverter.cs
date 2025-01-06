using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace University.Converters
{
    public class FirstValidationErrorConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errors = value as ReadOnlyObservableCollection<ValidationError>;
            if (errors != null && errors.Count > 0)
            {
                return errors[0].ErrorContent?.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
