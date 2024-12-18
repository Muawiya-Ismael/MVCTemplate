using System.ComponentModel.DataAnnotations;

namespace MVCTemplate.Models
{
    public class UploadedFile
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "File name is required.")]
        public string OriginalFileName { get; set; }

        public string UniqueFileName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

    }
}
