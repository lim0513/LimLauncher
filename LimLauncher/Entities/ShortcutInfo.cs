using LimLauncher.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LimLauncher.Entities
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class ShortcutInfo
    {

        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string GroupID { get; set; }
        /// <summary>
        /// 文件完整路径
        /// </summary>
        public string FileFullPath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get { return System.IO.Path.GetFileNameWithoutExtension(FileFullPath); } }

        [PropertyChanged.AlsoNotifyFor("FileRenameDisp")]
        public string FileRename { get; set; }

        public string FileRenameDisp { get { return string.IsNullOrWhiteSpace(FileRename) ? FileName : FileRename; } }

        private ImageSource FileIconSave { get; set; }
        /// <summary>
        /// 文件图标
        /// </summary>
        public ImageSource FileIcon
        {
            get
            {
                if (FileIconSave == null)
                    FileIconSave = IconManager.FindIconForFilename(FileFullPath, true);
                return FileIconSave;
            }
        }

        public string FileSize { get { return System.IO.File.Exists(FileFullPath) ? Common.GetString(new System.IO.FileInfo(FileFullPath).Length) : ""; } }

        public string FileTypeDescription { get { return Common.GetFileTypeDescription(FileFullPath); } }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="FileName"></param>
        public void StartFile()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    System.Diagnostics.Process.Start(FileFullPath);
                }
                catch { }
            }).Start();
        }

        internal void AddNewShortcutToDB()
        {
            SqliteHelper.Instance.ExecuteNonQuery(
                "insert into ShortcutInfo (GroupID,ID,FileFullPath,FileRename) values (@GroupID,@ID,@FileFullPath,@FileRename)",
                new Dictionary<string, object>()
                {
                    {"GroupID",this.GroupID },
                    {"ID",this.ID },
                    {"FileFullPath",this.FileFullPath },
                    {"FileRename",this.FileName},
                });
        }

        internal void RemoveShortcutFromDB()
        {
            SqliteHelper.Instance.ExecuteNonQuery(
                "Delete from ShortcutInfo where ID=@Id",
                new Dictionary<string, object>()
                {
                    {"Id", this.ID}
                });
        }

        internal void UpdateShortcutToDB()
        {
            SqliteHelper.Instance.ExecuteNonQuery(
                "update ShortcutInfo set FileRename=@FileRename where ID=@Id",
                new Dictionary<string, object>()
                {
                    {"FileRename", this.FileRename},
                    {"Id", this.ID}
                });
        }
    }
}
