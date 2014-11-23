using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdLib
{
    public class SdFolder : SdFModel
    {
        public ExternalStorageFolder ESFolder { get; set; }

        public SdFolder Parent { get; set; }

        private List<SdFile> _files = null;
        public List<SdFile> Files 
        {
            get 
            {
                if (_files == null)
                    _files = new List<SdFile>();
                return _files; 
            }
        }
        private List<SdFolder> _folders = null;
        public List<SdFolder> Folders 
        {
            get
            {
                if (_folders == null)
                    _folders = new List<SdFolder>();
                return _folders;
            }
        }

        public void AddFolder(SdFolder subFolder)
        {
            if (subFolder != null)
            {
                subFolder.Parent = this;
                Folders.Add(subFolder);
            }
        }

        public void AddFile(SdFile file)
        {
            if (file != null)
            {
                Files.Add(file);
            }
        }
    }
}
