using System.ComponentModel.DataAnnotations;

namespace Rnwood.Smtp4dev.API.DTO
{
    public class ServerUpdate
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Port { get; set; }
    }
}