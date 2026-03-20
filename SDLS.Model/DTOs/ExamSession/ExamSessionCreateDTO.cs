using SDLS.Model.DTOs.ExamDetail;
using System;
using System.Collections.Generic;

namespace SDLS.Model.DTOs.ExamSession
{
    public class ExamSessionCreateDTO
    {
        public Guid ExamId { get; set; }
        public Guid UserId { get; set; }
        public int? Score { get; set; }
        public bool IsPassed { get; set; }

        public List<ExamDetailCreateDTO> ExamDetails { get; set; } = new();
    }
}