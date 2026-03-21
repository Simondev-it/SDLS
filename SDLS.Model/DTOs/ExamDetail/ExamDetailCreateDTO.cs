using System;
using SDLS.Model.Validations;

namespace SDLS.Model.DTOs.ExamDetail
{
    public class ExamDetailCreateDTO
    {
        [NotEmptyGuid]
        public Guid AnswerId { get; set; }
    }
}