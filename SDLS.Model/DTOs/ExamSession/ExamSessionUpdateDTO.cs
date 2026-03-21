using SDLS.Model.DTOs.ExamDetail;
using SDLS.Model.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SDLS.Model.DTOs.ExamSession
{
    public class ExamSessionUpdateDTO
    {
        [NotEmptyGuid]
        public Guid ExamId { get; set; }

        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [Range(0, 100, ErrorMessage = "Giá trị không hợp lệ.")]
        public int? Score { get; set; }

        public bool IsPassed { get; set; }

        public List<ExamDetailUpdateDTO> ExamDetails { get; set; } = new();
    }
}