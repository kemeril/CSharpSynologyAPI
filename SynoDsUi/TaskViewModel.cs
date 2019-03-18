using StdUtils;
using SynologyRestDAL.Ds;
using System;

namespace SynoDsUi
{
    public class TaskViewModel : Task
    {
        public string Filename => Title;

        public double Progress => long.Parse(Size) == 0 ? 0 : (long.Parse(Additional.Transfer.SizeDownloaded) / long.Parse(Size)) * 100;

        public string DownloadedLabel => FileSizeUtils.FormatBytes(long.Parse(Additional.Transfer.SizeDownloaded) / 1024 / 1024);

        public string UploadedLabel => FileSizeUtils.FormatBytes(long.Parse(Additional.Transfer.SizeUploaded) / 1024 / 1024);

        public string SizeLabel => FileSizeUtils.FormatBytes(long.Parse(Size) / 1024 / 1024);

        public string CreatedLabel => Additional.Detail.CreateTime == null ? "" : $"{DateTime.Parse(Additional.Detail.CreateTime):U}";
    }
}