using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace Librarian_UI.Converters
{
    public class BooleanToTextDecorationConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value into Text Decoration value
        /// </summary>
        /// <param name="value">The value produced by the binding source</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="language">The culture to use in the converter</param>
        /// <returns>TextDecoration object</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value)
                ? TextDecorations.Underline : TextDecorations.None;
        }

        /// <summary>
        /// Converts a Text Definition value, into a boolean value
        /// </summary>
        /// <param name="value">The value produced by the binding source</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="language">The culture to use in the converter</param>
        /// <returns>Boolean value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Does not support reverse conversion");
        }
    }
}
