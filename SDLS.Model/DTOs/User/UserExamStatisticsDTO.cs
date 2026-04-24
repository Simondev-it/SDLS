using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.DTOs.User
{
    public class UserExamStatisticsDTO
    {
        public ExamStats TheoryStats { get; set; } = new();
        public ExamStats SimulationStats { get; set; } = new();
    }

    public class ExamStats
    {
        public int TotalAttempts { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public double PassRate { get; set; }
        public double FailRate { get; set; }
    }
}
