using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaLib
{
    public class MediaManager
    {
        private static MediaLibrary _media = null;
        public static MediaLibrary Media
        {
            get
            {
                if (_media == null)
                    return _media = new MediaLibrary();
                else
                    return _media;
            }
        }

        #region Song

        private static ObservableCollection<MSong> _mSongList = null;
        public static ObservableCollection<MSong> GetMusicInfos()
        {
            if (_mSongList != null)
                return _mSongList;
            
            var songs = Media.Songs;
            if (songs.Count > 0)
            {
                _mSongList = new ObservableCollection<MSong>();
                foreach (var song in songs)
                {
                    MSong mSong = new MSong { Name = song.Name, PlayCount = song.PlayCount, Singer = song.Artist.Name };
                    mSong.Time = string.Format("{0}分{1}秒", song.Duration.Minutes, song.Duration.Seconds);
                    mSong.Song = song;
                    _mSongList.Add(mSong);
                }
            }
            return _mSongList;
        }

        public static void DeleteSong(MSong msong)
        {
            try
            {
                Media.Delete(msong.Song);
                _media = null;
                if(!Media.Songs.Contains(msong.Song))
                {
                    _mSongList.Remove(msong);
                }
            }
            catch{}
        }        

        #endregion

        #region Picture

        private static ObservableCollection<MPicture> _mPictures = null;
        public static ObservableCollection<MPicture> GetPictureInfos()
        {
            if (_mPictures != null)
                return _mPictures;

            _mPictures = new ObservableCollection<MPicture>();
            var pictures = Media.Pictures;
            if (pictures.Count > 0)
            {
                foreach (var pic in pictures)
                {
                    MPicture mPicture = new MPicture { Date = pic.Date, Name = pic.Name, Width = pic.Width, Height = pic.Height };
                    mPicture.Path = pic.GetPath();
                    mPicture.Picture = pic;
                    _mPictures.Add(mPicture);
                }
            }
            return _mPictures;
        }

        public static ObservableCollection<MPicture> GetPicsFromAlbum(MpictureAlbum album)
        {
            ObservableCollection<MPicture>  _mPicsFromAlbum = new ObservableCollection<MPicture>();

            var pictures = album.PictureAlbum.Pictures;
            if (pictures.Count > 0)
            {
                foreach (var pic in pictures)
                {
                    MPicture mPicture = new MPicture { Date = pic.Date, Name = pic.Name, Width = pic.Width, Height = pic.Height };
                    mPicture.Path = pic.GetPath();
                    mPicture.Picture = pic;
                    _mPicsFromAlbum.Add(mPicture);

                }
            }

            var subAlbums = album.PictureAlbum.Albums;
            foreach (var subAlbum in subAlbums)
            {
                if (subAlbum.Pictures.Count > 0)
                {
                    foreach (var pic in subAlbum.Pictures)
                    {
                        MPicture mPicture = new MPicture { Date = pic.Date, Name = pic.Name, Width = pic.Width, Height = pic.Height };
                        mPicture.Path = pic.GetPath();
                        mPicture.Picture = pic;
                        _mPicsFromAlbum.Add(mPicture);

                    }
                }
            }

            return _mPicsFromAlbum;
        }

        private static ObservableCollection<MpictureAlbum> _mPicAlbums = null;
        public static ObservableCollection<MpictureAlbum> GetPicAlbums()
        {
            if (_mPicAlbums != null)
                return _mPicAlbums;
            _mPicAlbums = new ObservableCollection<MpictureAlbum>();
            var albums = Media.RootPictureAlbum.Albums;

            foreach (var album in albums)
            {
                MpictureAlbum mPicAlbum = new MpictureAlbum() { Name = album.Name, Count = album.Pictures.Count, PictureAlbum = album };
                if (mPicAlbum.Count > 0)
                {
                    mPicAlbum.LastPic = album.Pictures.LastOrDefault();
                }
                _mPicAlbums.Add(mPicAlbum);

                var subAlbums = album.Albums;
                foreach (var subAlbum in subAlbums)
                {
                    if (subAlbum.Pictures.Count > 0)
                    {
                        mPicAlbum.Count += subAlbum.Pictures.Count;
                        mPicAlbum.LastPic = subAlbum.Pictures.LastOrDefault();
                    }
                }
            }
            return _mPicAlbums;
        }

        public static void SavaPicture()
        {
            Media.SavePicture("", new byte[1]);
        }

        #endregion

    }
}
