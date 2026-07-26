using System;

namespace FreelanceHub.Web.ViewModels
{
    public class ApplicationAttachmentViewModel
    {
        public int AttachmentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string? FileExtension { get; set; }
    }
}