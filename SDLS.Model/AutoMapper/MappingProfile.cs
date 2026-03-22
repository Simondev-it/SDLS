using AutoMapper;
using SDLS.Model.DTOs.Answer;
using SDLS.Model.DTOs.Answer.ExamQuestion;
using SDLS.Model.DTOs.Exam;
using SDLS.Model.DTOs.ExamDetail;
using SDLS.Model.DTOs.ExamSession;
using SDLS.Model.DTOs.ForumPost;
using SDLS.Model.DTOs.LearningProgress;
using SDLS.Model.DTOs.LessonImage;
using SDLS.Model.DTOs.Question;
using SDLS.Model.DTOs.QuestionChapter;
using SDLS.Model.DTOs.QuestionLesson;
using SDLS.Model.DTOs.QuestionTag;
using SDLS.Model.DTOs.DrivingLicense;
using SDLS.Model.DTOs.Vehicle;
using SDLS.Model.Models;
using SDLS.Model.DTOs.Tag;
using SDLS.Model.DTOs.QuestionCategory;
using SDLS.Model.DTOs.QuestionTopic;
using SDLS.Model.DTOs.SimulationDifficultyLevel;
using SDLS.Model.DTOs.SignCategory;
using SDLS.Model.DTOs.SimulationCategory;
using SDLS.Model.DTOs.ReportCategory;
using SDLS.Model.DTOs.ForumTopic;

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

            CreateMap<ExamSessionDTO, ExamSession>().ReverseMap();
            CreateMap<ExamSessionCreateDTO, ExamSession>().ReverseMap();
            CreateMap<ExamSessionUpdateDTO, ExamSession>().ReverseMap();

            CreateMap<ExamDetailDTO, ExamDetail>().ReverseMap();
            CreateMap<ExamDetailCreateDTO, ExamDetail>().ReverseMap();
            CreateMap<ExamDetailUpdateDTO, ExamDetail>().ReverseMap();

            CreateMap<LearningProgressCreateDTO, LearningProgress>().ReverseMap();
            CreateMap<LearningProgressUpdateDTO, LearningProgress>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<LearningProgress, LearningProgressDTO>().ReverseMap();

            CreateMap<QuestionChapterDTO, QuestionChapter>().ReverseMap();
            CreateMap<QuestionChapterCreateDTO, QuestionChapter>().ReverseMap();
            CreateMap<QuestionChapterUpdateDTO, QuestionChapter>().ReverseMap();

            CreateMap<QuestionLessonDTO, QuestionLesson>().ReverseMap();
            CreateMap<QuestionLessonCreateDTO, QuestionLesson>().ReverseMap();
            CreateMap<QuestionLessonUpdateDTO, QuestionLesson>().ReverseMap();

            CreateMap<QuestionLessonImageDTO, LessonImage>().ReverseMap();
            CreateMap<QuestionLessonImageCreateDTO, LessonImage>().ReverseMap();
            CreateMap<QuestionLessonImageUpdateDTO, LessonImage>().ReverseMap();

            CreateMap<DrivingLicenseDTO, DrivingLicense>().ReverseMap();
            CreateMap<DrivingLicenseCreateDTO, DrivingLicense>().ReverseMap();
            CreateMap<DrivingLicenseUpdateDTO, DrivingLicense>().ReverseMap();

            CreateMap<VehicleDTO, Vehicle>().ReverseMap();
            CreateMap<VehicleCreateDTO, Vehicle>().ReverseMap();
            CreateMap<VehicleUpdateDTO, Vehicle>().ReverseMap();

            CreateMap<ForumPostDTO, ForumPost>().ReverseMap();
            CreateMap<ForumPostCreateDTO, ForumPost>().ReverseMap();
            CreateMap<ForumPostUpdateDTO, ForumPost>().ReverseMap();

            CreateMap<ForumPostImageDTO, PostImage>().ReverseMap();
            CreateMap<TagDTO, Tag>().ReverseMap();
            CreateMap<TagCreateDTO, Tag>().ReverseMap();
            CreateMap<TagUpdateDTO, Tag>().ReverseMap();

            CreateMap<QuestionCategoryDTO, QuestionCategory>().ReverseMap();
            CreateMap<QuestionCategoryCreateDTO, QuestionCategory>().ReverseMap();
            CreateMap<QuestionCategoryUpdateDTO, QuestionCategory>().ReverseMap();

            CreateMap<QuestionTopicDTO, QuestionTopic>().ReverseMap();
            CreateMap<QuestionTopicCreateDTO, QuestionTopic>().ReverseMap();
            CreateMap<QuestionTopicUpdateDTO, QuestionTopic>().ReverseMap();

            CreateMap<SimulationDifficultyLevelDTO, SimulationDifficultyLevel>().ReverseMap();
            CreateMap<SimulationDifficultyLevelCreateDTO, SimulationDifficultyLevel>().ReverseMap();
            CreateMap<SimulationDifficultyLevelUpdateDTO, SimulationDifficultyLevel>().ReverseMap();

            CreateMap<SignCategoryDTO, SignCategory>().ReverseMap();
            CreateMap<SignCategoryCreateDTO, SignCategory>().ReverseMap();
            CreateMap<SignCategoryUpdateDTO, SignCategory>().ReverseMap();

            CreateMap<SimulationCategoryDTO, SimulationCategory>().ReverseMap();
            CreateMap<SimulationCategoryCreateDTO, SimulationCategory>().ReverseMap();
            CreateMap<SimulationCategoryUpdateDTO, SimulationCategory>().ReverseMap();

            CreateMap<ReportCategoryDTO, ReportCategory>().ReverseMap();
            CreateMap<ReportCategoryCreateDTO, ReportCategory>().ReverseMap();
            CreateMap<ReportCategoryUpdateDTO, ReportCategory>().ReverseMap();

            CreateMap<ForumTopicDTO, ForumTopic>().ReverseMap();
            CreateMap<ForumTopicCreateDTO, ForumTopic>().ReverseMap();
            CreateMap<ForumTopicUpdateDTO, ForumTopic>().ReverseMap();
        }
    }
}
