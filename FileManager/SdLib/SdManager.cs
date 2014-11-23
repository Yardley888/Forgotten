using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdLib
{
    public class SdManager
    {
        //private static List<SdFile> allSdFiles = null;
        //public static List<SdFile> AllSdFiles
        //{
        //    get
        //    {
        //        return allSdFiles;
        //    }
        //}

        public async static Task<ExternalStorageDevice> GetSDCard()
        {
            var sds = await ExternalStorage.GetExternalStorageDevicesAsync();
            if (sds != null && sds.Count() > 0)
            {
                return sds.FirstOrDefault();
            }
            return null;
        }

        public async static Task<List<SdFile>> GetAllFilesOnSd()
        {
            var sd = await GetSDCard();
            if (sd != null)
            {
                var allSdFiles = await LoopFromFolder(sd.RootFolder);

                return allSdFiles;
            }

            return null;
        }

        public async static Task<List<SdFile>> LoopFromFolder(ExternalStorageFolder folder)
        {
            List<SdFile> sdFiles = new List<SdFile>();
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                sdFiles.Add(new SdFile { Storage = file, Name = file.Name, Path = file.Path, ModifyTime = file.DateModified.DateTime });
            }
            var folders = await folder.GetFoldersAsync();
            foreach (var innerFolder in folders)
            {
                var list = await LoopFromFolder(innerFolder);
                sdFiles.AddRange(list);
            }

            return sdFiles;
        }

        public async static Task<SdFolder> LoopToGetTopSDFolder(ExternalStorageFolder folder)
        {
            SdFolder sdFolder = new SdFolder { Name = folder.Name, Path = folder.Path };
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                sdFolder.AddFile(new SdFile { Storage = file, Name = file.Name, Path = file.Path, ModifyTime = file.DateModified.DateTime });
            }
            var folders = await folder.GetFoldersAsync();
            foreach (var innerFolder in folders)
            {
                sdFolder.AddFolder(new SdFolder { Name = innerFolder.Name, Path = innerFolder.Path });
                var sdInnerFolder = await LoopToGetTopSDFolder(innerFolder);
            }
            return sdFolder;
        }

        public async static Task<SdFolder> SerachFoldersAndFiles(ExternalStorageFolder folder)
        {
            SdFolder _sdFolder = new SdFolder { ESFolder = folder, Name = folder.Name, Path = folder.Path };
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                _sdFolder.AddFile(new SdFile { Storage = file, Name = file.Name, Path = file.Path, ModifyTime = file.DateModified.DateTime });
            }

            var folders = await folder.GetFoldersAsync();
            foreach (var innerFolder in folders)
            {
                _sdFolder.AddFolder(new SdFolder { ESFolder = innerFolder, Name = innerFolder.Name, Path = innerFolder.Path });
            }
            return _sdFolder;
        }

        public async static Task<SdFolder> SerachFoldersAndFiles(SdFolder _sdFolder)
        {
            var result = await SerachFoldersAndFiles(_sdFolder.ESFolder);
            return result;
        }

        public static List<SdFModel> GetSdFModelsFromSdFolder(SdFolder _sdFolder)
        {
            List<SdFModel> sdFModelList = new List<SdFModel>();
            foreach (var folder in _sdFolder.Folders)
            {
                sdFModelList.Add(folder);
            }
            foreach (var file in _sdFolder.Files)
            {
                sdFModelList.Add(file);
            }
            return sdFModelList;
        }
    }
}
