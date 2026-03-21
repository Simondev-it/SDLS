using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        public NotEmptyGuidAttribute()
        {
            ErrorMessage = "Trường này là bắt buộc.";
        }

        public override bool IsValid(object? value)
        {
            if (value is Guid guid)
            {
                return guid != Guid.Empty;
            }

            return false;
        }
    }
}