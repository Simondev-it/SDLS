using AutoMapper;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.LessonImage;
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

            CreateMap<LessonImageDTO, LessonImage>().ReverseMap();
            CreateMap<LessonImageCreateDTO, LessonImage>().ReverseMap();
        }
    }
}
