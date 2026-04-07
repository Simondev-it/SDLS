using SDLS.Model.DTOs.TrafficSign;

namespace SDLS.Model.DTOs.SavedTrafficSign
{
    public class SavedTrafficSignDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TrafficSignId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public TrafficSignDTO? TrafficSign { get; set; }
    }
}