using System.Drawing;
using Task.Areas.Admin.ViewModels;

namespace Task.Utils
{
    public static class Extension
    {
        public static bool CheckFileSize(this IFormFile file, int size)
        {
            return file.Length / 1024 < size;
        }

        public static bool CheckContentType(this IFormFile file, string type)
        {
            return file.ContentType.Contains($"{type}/") ;
        }
    }
}
