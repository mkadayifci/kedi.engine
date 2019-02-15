using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace kedi.engine.Services
{
    public class FileService
    {
        public dynamic GetList(string path)
        {
            List<dynamic> returnValue = new List<dynamic>();

            if (path == "~")
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            foreach (var folderPath in Directory.GetDirectories(path))
            {
                returnValue.Add(new
                {
                    Name = Path.GetFileName(folderPath),
                    Path = folderPath,
                    IsFile = false,
                    IconBase64 = "data:image/png;base64, " + this.GetIconBase64(Resource.folder_ico)
                });
            }

            foreach (var filePath in Directory.GetFiles(path))
            {
                returnValue.Add(new
                {
                    Name = Path.GetFileName(filePath),
                    Path = filePath,
                    ItLooksLikeDump = this.IsItLookLikeDump(filePath),
                    IsFile = true,
                    IconBase64 = "data:image/png;base64, " + this.GetIconBase64(filePath)
                });
            }

            return new
            {
                PathBreadCrumb = this.GetPathBreadCrumbData(path),
                DirectorySeperator = Path.DirectorySeparatorChar,
                WorkingDirectory = path,
                Entries = returnValue
            };
        }

        private bool IsItLookLikeDump(string filePath)
        {
            string[] extensionListForDumps = new string[] { ".dmp" };
            string extension = Path.GetExtension(filePath);
            return extensionListForDumps.Where(item => item.ToLowerInvariant() == extension.ToLowerInvariant()).Count() > 0;
        }

        private List<dynamic> GetPathBreadCrumbData(string path)
        {
            var returnValue = new List<dynamic>();
            string[] folders = path.Split(Path.DirectorySeparatorChar);
            var currentPath = string.Empty;

            foreach (var folder in folders)
            {
                currentPath += folder + Path.DirectorySeparatorChar;
                returnValue.Add(new
                {
                    FullPath = currentPath,
                    Name = folder
                });
            }
            return returnValue;
        }

        private string GetIconBase64(string filePath)
        {
            Icon icon = Icon.ExtractAssociatedIcon(filePath);
            return this.GetIconBase64(icon);
        }

        private string GetIconBase64(Icon icon)
        {
            return GetIconBase64(icon.ToBitmap());
        }

        private string GetIconBase64(Bitmap bitmap)
        {
            try
            {
                if (bitmap.Height != 32)
                {
                    Bitmap resized = new Bitmap(bitmap, new Size(32, 32));
                    bitmap = resized;
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    return Convert.ToBase64String(byteImage);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
