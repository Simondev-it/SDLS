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
        public static string Build(UserLicenseDTO? dto)
        {
            string profileSection = dto == null
                ? BuildAnonymousSection()
                : BuildProfileSection(dto);

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
                - Hỏi "lộ trình" → chia 4 giai đoạn:
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

        private static string BuildProfileSection(UserLicenseDTO dto)
        {
            string licenseName = dto.DrivingLicense?.Name ?? "Chưa xác định";

                        return $"""
            - Hạng bằng mục tiêu : {licenseName}
            - Trạng thái         : {dto.Status}
            - Ngày tạo           : {dto.CreateAt}
            """;
        }

        private static string BuildAnonymousSection()
        {
            return """
            - Hạng bằng mục tiêu : chưa xác định

            📌 Học viên chưa đăng nhập → tư vấn chung
            """;
        }
    }
}
