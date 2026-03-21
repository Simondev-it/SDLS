using SDLS.Model.DTOs.ExamDetail;
using System;
using System.Collections.Generic;

namespace SDLS.Model.DTOs.ExamSession
{
    public class ExamSessionDTO
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid UserId { get; set; }
        public int? Score { get; set; }
        public bool IsPassed { get; set; }
        public int? Status { get; set; }

        public List<ExamDetailDTO> ExamDetails { get; set; } = new();
    }
}