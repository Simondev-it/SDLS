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
        public double PassRate { get; set; }
        // Phân tích chi tiết theo danh mục (Ví dụ: Biển báo, Sa hình...)
        public List<CategoryAnalysisDTO> CategoryAnalysis { get; set; } = new();
    }

    public class CategoryAnalysisDTO
    {
        public string CategoryName { get; set; } = null!;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double CorrectRate { get; set; } // % Đúng
        public double WrongRate { get; set; }   // % Sai
    }
}
