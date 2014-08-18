using System;

namespace myclouddisk
{
    /// <summary>
    /// 操作类型：新建、重命名、修改、删除
    /// </summary>
    public enum OperationType
    {
        CREATE,
        RENAME,
        MODIFY,
        DELETE
    }
    /// <summary>
    /// 文件类型：目录、文件
    /// </summary>
    public enum FileType
    {
        FILE,
        DIRECTORY,
        OBJECT
    }
    /// <summary>
    /// 文件操作事件类
    /// </summary>
    class FileEvent
    {

        private FileType fileType = FileType.OBJECT;

        public FileType FileType
        {
            get { return fileType; }
            set { fileType = value; }
        }
        private OperationType opertionType;

        public OperationType OpertionType
        {
            get { return opertionType; }
            set { opertionType = value; }
        }
        private string oldName;

        public string OldName
        {
            get { return oldName; }
            set { oldName = value; }
        }
        private string newName;

        public string NewName
        {
            get { return newName; }
            set { newName = value; }
        }
        private string oldFullPath;

        public string OldFullPath
        {
            get { return oldFullPath; }
            set { oldFullPath = value; }
        }
        private string newFullPath;

        public string NewFullPath
        {
            get { return newFullPath; }
            set { newFullPath = value; }
        }
        private DateTime generateTime;

        public DateTime GenerateTime
        {
            get { return generateTime; }
            set { generateTime = value; }
        }
        public FileEvent()
        {

        }
        public FileEvent(FileType fileType, OperationType opertionType, string oldName, string newName, string oldFullPath, string newFullPath, DateTime generateTime)
        {
            this.fileType = fileType;
            this.opertionType = opertionType;
            this.oldName = oldName;
            this.newName = newName;
            this.oldFullPath = oldFullPath;
            this.newFullPath = newFullPath;
            this.generateTime = generateTime;
        }
        public bool syncOperation()
        {
            return true;
        }

    }
}
