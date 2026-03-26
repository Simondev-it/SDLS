using SDLS.Model.Validations;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.SavedTrafficSign
{
    public class SavedTrafficSignUpdateDTO
    {
        [NotEmptyGuid]
        public Guid TrafficSignId { get; set; }

        [Range(0, 1, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Status { get; set; }
    }
}