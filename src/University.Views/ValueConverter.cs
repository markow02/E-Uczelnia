using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;

namespace University.Views
{
    public class ValidationErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errors = value as System.Collections.ObjectModel.ObservableCollection<ValidationError>;
            if (errors != null && errors.Count > 0)
            {
                return errors[0].ErrorContent; // Return the first error content
            }
            return null; // No error
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null; // Not needed for this use case
        }
    }
}
