using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Vues.Converters
{
    class StringToImage : IValueConverter
    {
        private string pathToFolderExec = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//pour avoir le chemin jusqu'a l'executable
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            string imagePath = value as string;
            if (imagePath[0] == '.')//si c'est un chemin relatif
            {
                imagePath = imagePath[2..];
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    return new Uri(Path.Combine(pathToFolderExec, imagePath), UriKind.RelativeOrAbsolute);
                }
                return null;
            }
            else
            {
                return imagePath;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
