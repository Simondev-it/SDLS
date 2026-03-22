using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.SavedTrafficSign
{
    public class SavedTrafficSignCreateDTO
    {
        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [NotEmptyGuid]
        public Guid TrafficSignId { get; set; }
    }
}