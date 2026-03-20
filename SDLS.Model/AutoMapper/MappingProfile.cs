using AutoMapper;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Answer.ExamQuestion;
using SDLS.Model.DTOs.Exam;
using SDLS.Model.DTOs.LessonImage;
using SDLS.Model.DTOs.Question;
using SDLS.Model.DTOs.QuestionTag;
using SDLS.Model.Models;

namespace SDLS.Model.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<QuestionDTO, Question>().ReverseMap();
            CreateMap<QuestionCreateDTO, Question>().ReverseMap();
            CreateMap<QuestionUpdateDTO, Question>().ReverseMap();

            CreateMap<AnswerDTO, Answer>().ReverseMap();
            CreateMap<AnswerCreateDTO, Answer>().ReverseMap();

            CreateMap<QuestionTagDTO, QuestionTag>().ReverseMap();
            CreateMap<QuestionTagCreateDTO, QuestionTag>().ReverseMap();
            CreateMap<QuestionTagUpdateDTO, QuestionTag>().ReverseMap();

            CreateMap<LessonImageDTO, LessonImage>().ReverseMap();
            CreateMap<LessonImageCreateDTO, LessonImage>().ReverseMap();

            CreateMap<ExamDTO, Exam>().ReverseMap();
            CreateMap<ExamCreateDTO, Exam>().ReverseMap();
            CreateMap<ExamUpdateDTO, Exam>().ReverseMap();

            CreateMap<ExamQuestionDTO, ExamQuestion>().ReverseMap();
            CreateMap<ExamQuestionCreateDTO, ExamQuestion>().ReverseMap();
            CreateMap<ExamQuestionUpdateDTO, ExamQuestion>().ReverseMap();
        }
    }
}
