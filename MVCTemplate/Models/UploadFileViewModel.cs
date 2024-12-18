using System.ComponentModel.DataAnnotations;

namespace MVCTemplate.Models
{
    public class UploadFileViewModel
    {
        [Required(ErrorMessage = "file is required.")]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }
}
