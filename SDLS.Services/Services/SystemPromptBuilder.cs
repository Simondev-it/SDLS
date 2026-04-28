using SDLS.Model.DTOs.User;
using SDLS.Model.DTOs.UserLicense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Services.Services
{

    public static class SystemPromptBuilder
    {
        public static string Build(UserAIProfile? user)
        {
            string profileSection = user == null
                ? BuildAnonymousSection()
                : BuildProfileSection(user);

                            return $"""
                Bạn là trợ lý AI đào tạo lái xe thông minh của hệ thống SDLS 🚗

                ## THÔNG TIN HỌC VIÊN
                {profileSection}

                ## QUY TẮC BẮT BUỘC
                - LUÔN sử dụng "Hạng bằng" từ hệ thống
                - KHÔNG hỏi lại user học bằng gì
                - Mỗi câu hỏi là độc lập (không bị ảnh hưởng câu trước)
                - Nếu không chắc → nói rõ

                ## FORMAT TRẢ LỜI (BẮT BUỘC)
                - Có tiêu đề rõ ràng
                - Dùng emoji: ✅ ⚠️ 📌
                - Bullet points
                - Không viết dài dòng

                ## LOGIC TRẢ LỜI
                - Hỏi "lộ trình" → PHẢI:
                  + Nếu thiếu → hỏi 3 thông tin:
                    1. Học mấy buổi/tuần
                    2. Mục tiêu (đậu / chạy rành)
                    3. Thời gian dự kiến

                  + Nếu đủ → chia 4 giai đoạn:
                    1. Lý thuyết
                    2. Sa hình
                    3. Đường trường
                    4. Ôn thi

                - Hỏi "mẹo" → trả lời ngắn, thực tế
                - Hỏi lại câu cũ → trả lời lại từ đầu

                ## CẤM
                - Không lan man
                - Không trả lời ngoài GPLX
                - Không bịa luật giao thông

                ⚠️ Hãy trả lời NGẮN GỌN - ĐÚNG TRỌNG TÂM.
                """;
          }

        // =========================
        // PROFILE USER (FIX CHÍNH)
        // =========================
        private static string BuildProfileSection(UserAIProfile user)
        {
            string license = NormalizeLicense(user.LicenseType);

                    return $"""
        - Tên học viên        : {user.Name ?? "Ẩn danh"}
        - Hạng bằng mục tiêu  : {license}
        """;
        }

        // =========================
        //  USER CHƯA LOGIN
        // =========================
        private static string BuildAnonymousSection()
        {
            return """
            - Hạng bằng mục tiêu : chưa xác định

            📌 Học viên chưa đăng nhập → tư vấn chung
            """;
        }

        // =========================
        // FIX LICENSETYPE 
        // =========================
        private static string NormalizeLicense(string? license)
        {
            if (string.IsNullOrEmpty(license))
                return "chưa xác định";

            license = license.ToUpper().Trim();

            return license switch
            {
                "A1" => "Xe máy dưới 125cc",
                "A" => "Xe máy (luật mới)",
                "B1" => "Ô tô số tự động",
                "B2" => "Ô tô số sàn",
                "B" => "Ô tô",
                "C1" => "Xe tải nhẹ",
                _ => license
            };
        }
    }
}
