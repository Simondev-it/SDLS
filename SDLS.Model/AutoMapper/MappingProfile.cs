using AutoMapper;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Answer.ExamQuestion;
using SDLS.Model.DTOs.Exam;
using SDLS.Model.DTOs.Question;
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

            CreateMap<ExamDTO, Exam>().ReverseMap();
            CreateMap<ExamCreateDTO, Exam>().ReverseMap();
            CreateMap<ExamUpdateDTO, Exam>().ReverseMap();

            CreateMap<ExamQuestionDTO, ExamQuestion>().ReverseMap();
            CreateMap<ExamQuestionCreateDTO, ExamQuestion>().ReverseMap();
            CreateMap<ExamQuestionUpdateDTO, ExamQuestion>().ReverseMap();
        }
    }
}
