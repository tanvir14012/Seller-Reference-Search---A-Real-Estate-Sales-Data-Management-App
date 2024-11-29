namespace Seller_Reference_Search.Models
{
    public class FileUploadResultDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string UploadPath { get; set; }
        public TimeSpan? UploadDuration { get; set; }
        public string UploadedBy { get; set; }
        public string CreatedAt { get; set; }

        public int RowsFound { get; set; }
        public int RowsInserted { get; set; }
        public int RowsUpdated { get; set; }

        public int RowsErrored  => RowsFound - RowsInserted - RowsUpdated;
    }
}
