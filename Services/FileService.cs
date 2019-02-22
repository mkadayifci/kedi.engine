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
        const string THIS_PC = "[ThisPC]";
        private string ConvertSpecialFolder(string path)
        {

            switch (path)
            {
                case "[Desktop]":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case "~":
                    return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                case "[Documents]":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                default:
                    return path;
            }
        }

        public dynamic GetList(string path)
        {
            List<dynamic> returnValue = new List<dynamic>();

            path = this.ConvertSpecialFolder(path);
            if (path == FileService.THIS_PC)
            {
                AddDriverItems(returnValue);
            }
            else
            {
                AddDirectoryItems(path, returnValue);
                AddFileItems(path, returnValue);
            }

            return new
            {
                PathBreadCrumb = this.GetPathBreadCrumbData(path),
                DirectorySeperator = Path.DirectorySeparatorChar,
                WorkingDirectory = path,
                LevelUpDirectory = this.GetParentPath(path),
                Entries = returnValue
                                    .OrderBy(item => item.IsFile) //First folders
                                    .ThenByDescending(item => item.IsFile ? item.ItLooksLikeDump : true) //Then dump files in the first seats
            };
        }

        private string GetParentPath(string path)
        {

            if (path == THIS_PC)
            {
                return path;
            }

            var parent = Directory.GetParent(path);
            if (parent != null)
            {
                return parent.FullName;
            }
            else
            {
                return FileService.THIS_PC;
            }
        }

        private void AddDriverItems(List<dynamic> returnValue)
        {
            //

            foreach (var drive in Directory.GetLogicalDrives())
            {
                returnValue.Add(new
                {
                    Name = drive,
                    Path = drive,
                    IsFile = false,
                    IconBase64 = "data:image/png;base64, " + this.GetIconBase64(Resource.disk)
                });
            }

        }

        private void AddFileItems(string path, List<dynamic> returnValue)
        {
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
        }

        private void AddDirectoryItems(string path, List<dynamic> returnValue)
        {
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
                if (folder == FileService.THIS_PC)
                {
                    currentPath = FileService.THIS_PC;
                }

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
