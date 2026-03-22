USE master
GO
IF DB_ID('GREENLIGHT') IS NOT NULL
BEGIN
    DROP DATABASE GREENLIGHT
END
GO
CREATE DATABASE GREENLIGHT
GO
USE GREENLIGHT
GO

IF OBJECT_ID('dbo.[UserNotification]', 'U') IS NOT NULL
    DROP TABLE dbo.[UserNotification];
    GO
IF OBJECT_ID('dbo.[Notification]', 'U') IS NOT NULL
    DROP TABLE dbo.[Notification];
    GO
IF OBJECT_ID('dbo.[Resolve]', 'U') IS NOT NULL
    DROP TABLE dbo.[Resolve];
    GO
IF OBJECT_ID('dbo.[Report]', 'U') IS NOT NULL
    DROP TABLE dbo.[Report];
    GO
IF OBJECT_ID('dbo.[ReportCategory]', 'U') IS NOT NULL
    DROP TABLE dbo.[ReportCategory];
    GO
IF OBJECT_ID('dbo.[SavedTrafficSign]', 'U') IS NOT NULL
    DROP TABLE dbo.[SavedTrafficSign];
    GO
IF OBJECT_ID('dbo.[TrafficSign]', 'U') IS NOT NULL
    DROP TABLE dbo.[TrafficSign];
    GO
IF OBJECT_ID('dbo.[SignCategory]', 'U') IS NOT NULL
    DROP TABLE dbo.[SignCategory];
    GO
IF OBJECT_ID('dbo.[CommentVote]', 'U') IS NOT NULL
    DROP TABLE dbo.[CommentVote];
    GO
IF OBJECT_ID('dbo.[ForumComment]', 'U') IS NOT NULL
    DROP TABLE dbo.[ForumComment];
    GO
IF OBJECT_ID('dbo.[PostReact]', 'U') IS NOT NULL
    DROP TABLE dbo.[PostReact];
    GO
IF OBJECT_ID('dbo.[PostImage]', 'U') IS NOT NULL
    DROP TABLE dbo.[PostImage];
    GO
IF OBJECT_ID('dbo.[ForumPost]', 'U') IS NOT NULL
    DROP TABLE dbo.[ForumPost];
    GO
IF OBJECT_ID('dbo.[ForumTopic]', 'U') IS NOT NULL
    DROP TABLE dbo.[ForumTopic];
    GO
IF OBJECT_ID('dbo.[ExamDetail]', 'U') IS NOT NULL
    DROP TABLE dbo.[ExamDetail];
    GO
IF OBJECT_ID('dbo.[ExamSession]', 'U') IS NOT NULL
    DROP TABLE dbo.[ExamSession];
    GO
IF OBJECT_ID('dbo.[ExamQuestion]', 'U') IS NOT NULL
    DROP TABLE dbo.[ExamQuestion];
    GO
IF OBJECT_ID('dbo.[Exam]', 'U') IS NOT NULL
    DROP TABLE dbo.[Exam];
    GO
IF OBJECT_ID('dbo.[Answer]', 'U') IS NOT NULL
    DROP TABLE dbo.[Answer];
    GO
IF OBJECT_ID('dbo.[LearningProgress]', 'U') IS NOT NULL
    DROP TABLE dbo.[LearningProgress];
    GO
IF OBJECT_ID('dbo.[SavedQuestion]', 'U') IS NOT NULL
    DROP TABLE dbo.[SavedQuestion];
    GO
IF OBJECT_ID('dbo.[QuestionTag]', 'U') IS NOT NULL
    DROP TABLE dbo.[QuestionTag];
    GO
IF OBJECT_ID('dbo.[Tag]', 'U') IS NOT NULL
    DROP TABLE dbo.[Tag];
    GO
IF OBJECT_ID('dbo.[Question]', 'U') IS NOT NULL
    DROP TABLE dbo.[Question];
    GO
IF OBJECT_ID('dbo.[QuestionCategory]', 'U') IS NOT NULL
    DROP TABLE dbo.[QuestionCategory];
    GO
IF OBJECT_ID('dbo.[QuestionTopic]', 'U') IS NOT NULL
    DROP TABLE dbo.[QuestionTopic];
    GO
IF OBJECT_ID('dbo.[LessonProgress]', 'U') IS NOT NULL
    DROP TABLE dbo.[LessonProgress];
    GO
IF OBJECT_ID('dbo.[LessonImage]', 'U') IS NOT NULL
    DROP TABLE dbo.[LessonImage];
    GO
IF OBJECT_ID('dbo.[QuestionLesson]', 'U') IS NOT NULL
    DROP TABLE dbo.[QuestionLesson];
    GO
IF OBJECT_ID('dbo.[QuestionChapter]', 'U') IS NOT NULL
    DROP TABLE dbo.[QuestionChapter];
    GO
IF OBJECT_ID('dbo.[UserLicense]', 'U') IS NOT NULL
    DROP TABLE dbo.[UserLicense];
    GO
IF OBJECT_ID('dbo.[Vehicle]', 'U') IS NOT NULL
    DROP TABLE dbo.[Vehicle];
    GO
IF OBJECT_ID('dbo.[DrivingLicense]', 'U') IS NOT NULL
    DROP TABLE dbo.[DrivingLicense];
    GO
IF OBJECT_ID('dbo.[SimulationSessionDetail]', 'U') IS NOT NULL
    DROP TABLE dbo.[SimulationSessionDetail];
    GO
IF OBJECT_ID('dbo.[SimulationSession]', 'U') IS NOT NULL
    DROP TABLE dbo.[SimulationSession];
    GO
IF OBJECT_ID('dbo.[SimulationExam]', 'U') IS NOT NULL
    DROP TABLE dbo.[SimulationExam];
    GO
IF OBJECT_ID('dbo.[SituationExam]', 'U') IS NOT NULL
    DROP TABLE dbo.[SituationExam];
    GO
IF OBJECT_ID('dbo.[SimulationScenario]', 'U') IS NOT NULL
    DROP TABLE dbo.[SimulationScenario];
    GO
IF OBJECT_ID('dbo.[SimulationDifficultyLevel]', 'U') IS NOT NULL
    DROP TABLE dbo.[SimulationDifficultyLevel];
    GO
IF OBJECT_ID('dbo.[SimulationCategory]', 'U') IS NOT NULL
    DROP TABLE dbo.[SimulationCategory];
    GO
IF OBJECT_ID('dbo.[SimulationChapter]', 'U') IS NOT NULL
    DROP TABLE dbo.[SimulationChapter];
    GO
IF OBJECT_ID('dbo.[Payment]', 'U') IS NOT NULL
    DROP TABLE dbo.[Payment];
    GO
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL
    DROP TABLE dbo.[User];
    GO
IF OBJECT_ID('dbo.[Role]', 'U') IS NOT NULL
    DROP TABLE dbo.[Role];
    GO


-- =====================================================
-- AUTH MODULE
-- =====================================================

-- 1.Role OK
CREATE TABLE [Role] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 2.User AF
CREATE TABLE [User] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    roleId      UNIQUEIDENTIFIER NOT NULL,
    email       NVARCHAR(255) UNIQUE,
    password    NVARCHAR(255) NOT NULL,
    name        NVARCHAR(255),
    avatar      NVARCHAR(255),
    phone       NVARCHAR(20),
    gender      NVARCHAR(20),
    description NVARCHAR(255),
    dateOfBirth DATE,
    licenseType NVARCHAR(20),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (roleId) REFERENCES [Role](id),
);

-- 3.Payment AF
CREATE TABLE [Payment] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    userId      UNIQUEIDENTIFIER NOT NULL,
    method      NVARCHAR(255),
    amount      INT,
    note        NVARCHAR(255),
    response    NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (userId) REFERENCES [User](id),
);

-- =====================================================
-- SIMULATION MODULE
-- =====================================================

-- 4.SimulationChapter OK
CREATE TABLE [SimulationChapter] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 5.SimulationCategory OK
CREATE TABLE [SimulationCategory] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 6.SimulationDifficultyLevel OK
CREATE TABLE [SimulationDifficultyLevel] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 7.SimulationScenario AF
CREATE TABLE [SimulationScenario] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    simulationChapterId             UNIQUEIDENTIFIER NOT NULL,
    simulationCategoryId            UNIQUEIDENTIFIER NOT NULL,
    simulationDifficultyLevelId     UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    video       NVARCHAR(255),
    totalTime   INT NOT NULL,
    startPoint  INT NOT NULL,
    endPoint    INT NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (simulationChapterId) REFERENCES [SimulationChapter](id),
    FOREIGN KEY (simulationCategoryId) REFERENCES [SimulationCategory](id),
    FOREIGN KEY (simulationDifficultyLevelId) REFERENCES [SimulationDifficultyLevel](id),
);

-- =====================================================
-- SITUATION MODULE
-- =====================================================

-- 8.SituationExam OK
CREATE TABLE [SituationExam] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    title       NVARCHAR(255) NOT NULL,
    description NVARCHAR(255),
    duration    INT,
    passScore   INT,
    isRandom    BIT NOT NULL DEFAULT 0,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 9.SimulationExam AF
CREATE TABLE [SimulationExam] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    situationExamId     UNIQUEIDENTIFIER NOT NULL,
    simulationId        UNIQUEIDENTIFIER NOT NULL,
    baseScore   INT,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (situationExamId) REFERENCES [SituationExam](id) ON DELETE CASCADE,
    FOREIGN KEY (simulationId) REFERENCES [SimulationScenario](id) ON DELETE CASCADE,
    UNIQUE (situationExamId, simulationId),
);

-- 10.SimulationSession AF
CREATE TABLE [SimulationSession] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    situationExamId UNIQUEIDENTIFIER NOT NULL,
    userId          UNIQUEIDENTIFIER NOT NULL,
    totalScore      INT,
    totalDuration   INT,
    isPassed        BIT NOT NULL DEFAULT 1,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (situationExamId) REFERENCES [SituationExam](id),
    FOREIGN KEY (userId) REFERENCES [User](id),
);

-- 11.SimulationSessionDetail AF
CREATE TABLE [SimulationSessionDetail] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    simulationExamId        UNIQUEIDENTIFIER NOT NULL,
    simulationSessionId     UNIQUEIDENTIFIER NOT NULL,
    durationSecond  INT,
    score           INT,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (simulationExamId) REFERENCES [SimulationExam](id) ON DELETE CASCADE,
    FOREIGN KEY (simulationSessionId) REFERENCES [SimulationSession](id) ON DELETE CASCADE,
    UNIQUE (simulationExamId, simulationSessionId),
);

-- =====================================================
-- LICENSE MODULE
-- =====================================================

-- 12.DrivingLicense OK
CREATE TABLE [DrivingLicense] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 13.Vehicle AF
CREATE TABLE [Vehicle] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    drivingLicenseId    UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (drivingLicenseId) REFERENCES [DrivingLicense](id),
);

-- 14.UserLicense OF
CREATE TABLE [UserLicense] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    userId              UNIQUEIDENTIFIER NOT NULL,
    drivingLicenseId    UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
    FOREIGN KEY (drivingLicenseId) REFERENCES [DrivingLicense](id) ON DELETE CASCADE,
    UNIQUE (userId, drivingLicenseId),
);

-- =====================================================
-- QUESTION MODULE
-- =====================================================

-- 15.QuestionChapter AF
CREATE TABLE [QuestionChapter] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    drivingLicenseId    UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (drivingLicenseId) REFERENCES [DrivingLicense](id),
);

-- 16.QuestionLesson AF
CREATE TABLE [QuestionLesson] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionChapterId   UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    content     NVARCHAR(MAX),
    FOREIGN KEY (questionChapterId) REFERENCES [QuestionChapter](id),
);

-- 17.LessonImage AF
CREATE TABLE [LessonImage] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionLessonId    UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255),
    url         NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (questionLessonId) REFERENCES [QuestionLesson](id),
);

-- 18.LessonProgress OF
CREATE TABLE [LessonProgress] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    userId              UNIQUEIDENTIFIER NOT NULL,
    questionLessonId    UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
    FOREIGN KEY (questionLessonId) REFERENCES [QuestionLesson](id) ON DELETE CASCADE,
    UNIQUE (userId, questionLessonId),
);

-- 19.QuestionTopic OK
CREATE TABLE [QuestionTopic] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 20.QuestionCategory OK
CREATE TABLE [QuestionCategory] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 21.Question AF
CREATE TABLE [Question] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionLessonId    UNIQUEIDENTIFIER NOT NULL,
    questionTopicId     UNIQUEIDENTIFIER NOT NULL,
    questionCategoryId  UNIQUEIDENTIFIER NOT NULL,
    parentId            UNIQUEIDENTIFIER NULL,
    content     NVARCHAR(255) NOT NULL,
    image       NVARCHAR(255),
    explanation NVARCHAR(255),
    type        NVARCHAR(20),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (questionLessonId) REFERENCES [QuestionLesson](id),
    FOREIGN KEY (questionTopicId) REFERENCES [QuestionTopic](id),
    FOREIGN KEY (questionCategoryId) REFERENCES [QuestionCategory](id),
    FOREIGN KEY (parentId) REFERENCES [Question](id),
);

-- 22.Tag OK
CREATE TABLE [Tag] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    colorCode   NVARCHAR(255) NOT NULL UNIQUE,
);

-- 23.QuestionTag OF
CREATE TABLE [QuestionTag] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionId  UNIQUEIDENTIFIER NOT NULL,
    tagId       UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (questionId) REFERENCES [Question](id) ON DELETE CASCADE,
    FOREIGN KEY (tagId) REFERENCES [Tag](id) ON DELETE CASCADE,
    UNIQUE (questionId, tagId),
);

-- 24.SavedQuestion OF
CREATE TABLE [SavedQuestion] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionId  UNIQUEIDENTIFIER NOT NULL,
    userId      UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (questionId) REFERENCES [Question](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
    UNIQUE (questionId, userId),
);

-- 25.LearningProgress OF
CREATE TABLE [LearningProgress] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionId  UNIQUEIDENTIFIER NOT NULL,
    userId      UNIQUEIDENTIFIER NOT NULL UNIQUE,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (questionId) REFERENCES [Question](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
    UNIQUE (questionId, userId),
);

-- 26.Answer AF
CREATE TABLE [Answer] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionId  UNIQUEIDENTIFIER NOT NULL,
    content     NVARCHAR(255) NOT NULL,
    isCorrect   BIT NOT NULL DEFAULT 1,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (questionId) REFERENCES [Question](id) ON DELETE CASCADE,
);

-- =====================================================
-- EXAM MODULE
-- =====================================================

-- 27.Exam AF
CREATE TABLE [Exam] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    userId      UNIQUEIDENTIFIER NOT NULL,
    title       NVARCHAR(255) NOT NULL,
    description NVARCHAR(255),
    duration    INT,
    passScore   INT,
    isRandom    BIT NOT NULL DEFAULT 0,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (userId) REFERENCES [User](id),
);

-- 28.ExamQuestion OF
CREATE TABLE [ExamQuestion] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    questionId  UNIQUEIDENTIFIER NOT NULL,
    examId      UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (questionId) REFERENCES [Question](id) ON DELETE CASCADE,
    FOREIGN KEY (examId) REFERENCES [Exam](id) ON DELETE CASCADE,
    UNIQUE (questionId, examId),
);

-- 29.ExamSession AF
CREATE TABLE [ExamSession] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    examId      UNIQUEIDENTIFIER NOT NULL,
    userId      UNIQUEIDENTIFIER NOT NULL,
    score       INT,
    totalDuration   INT,
    isPassed    BIT NOT NULL DEFAULT 1,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (examId) REFERENCES [Exam](id),
    FOREIGN KEY (userId) REFERENCES [User](id),
);

-- 30.ExamDetail OF
CREATE TABLE [ExamDetail] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    answerId        UNIQUEIDENTIFIER NOT NULL,
    examSessionId   UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (answerId) REFERENCES [Answer](id) ON DELETE CASCADE,
    FOREIGN KEY (examSessionId) REFERENCES [ExamSession](id) ON DELETE CASCADE,
    UNIQUE (answerId, examSessionId),
);

-- =====================================================
-- FORUM MODULE
-- =====================================================

-- 31.ForumTopic OK
CREATE TABLE [ForumTopic] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 32.ForumPost AF
CREATE TABLE [ForumPost] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    forumTopicId    UNIQUEIDENTIFIER NOT NULL,
    userId          UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255),
    title       NVARCHAR(255) NOT NULL,
    content     NVARCHAR(255) NOT NULL,
    viewCount   INT DEFAULT 0,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (forumTopicId) REFERENCES [ForumTopic](id),
    FOREIGN KEY (userId) REFERENCES [User](id),
);

-- 33.PostImage AF
CREATE TABLE [PostImage] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    forumPostId UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255),
    url         NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (forumPostId) REFERENCES [ForumPost](id),
);

-- 34.PostReact AF
CREATE TABLE [PostReact] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    forumPostId UNIQUEIDENTIFIER NOT NULL,
    userId      UNIQUEIDENTIFIER NOT NULL,
    reactType   NVARCHAR(20) NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (forumPostId) REFERENCES [ForumPost](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
    UNIQUE (forumPostId, userId),
);

-- 35.ForumComment AF
CREATE TABLE [ForumComment] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    replyId     UNIQUEIDENTIFIER NULL,
    forumPostId UNIQUEIDENTIFIER NOT NULL,
    userId      UNIQUEIDENTIFIER NOT NULL,
    content     NVARCHAR(1000) NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (replyId) REFERENCES [ForumComment](id),
    FOREIGN KEY (forumPostId) REFERENCES [ForumPost](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id),
);

-- 36.CommentVote OF
CREATE TABLE [CommentVote] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    forumCommentId  UNIQUEIDENTIFIER NOT NULL,
    userId          UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (forumCommentId) REFERENCES [ForumComment](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
    UNIQUE (forumCommentId, userId),
);

-- =====================================================
-- TRAFFIC SIGN MODULE
-- =====================================================

-- 37.SignCategory OK
CREATE TABLE [SignCategory] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 38.TrafficSign AF
CREATE TABLE [TrafficSign] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    signCategoryId  UNIQUEIDENTIFIER NOT NULL,
    name        NVARCHAR(255) NOT NULL UNIQUE,
    code        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    vectorData  NVARCHAR(255),
    image       NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (signCategoryId) REFERENCES [SignCategory](id),
);

-- 39.SavedTrafficSign OF
CREATE TABLE [SavedTrafficSign] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    trafficSignId   UNIQUEIDENTIFIER NOT NULL,
    userId          UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (trafficSignId) REFERENCES [TrafficSign](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
    UNIQUE (trafficSignId, userId),
);

-- =====================================================
-- REPORT MODULE
-- =====================================================

-- 40.ReportCategory OK
CREATE TABLE [ReportCategory] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name        NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 41.Report AF
CREATE TABLE [Report] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    simulationId        UNIQUEIDENTIFIER NULL,
    forumPostId         UNIQUEIDENTIFIER NULL,
    forumCommentId      UNIQUEIDENTIFIER NULL,
    questionId          UNIQUEIDENTIFIER NULL,
    reportCategoryId    UNIQUEIDENTIFIER NOT NULL,
    userId              UNIQUEIDENTIFIER NOT NULL,
    title       NVARCHAR(255) NOT NULL,
    content     NVARCHAR(1000) NOT NULL,
    image       NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (simulationId) REFERENCES [SimulationScenario](id) ON DELETE CASCADE,
    FOREIGN KEY (forumPostId) REFERENCES [ForumPost](id) ON DELETE CASCADE,
    FOREIGN KEY (forumCommentId) REFERENCES [ForumComment](id),
    FOREIGN KEY (questionId) REFERENCES [Question](id) ON DELETE CASCADE,
    FOREIGN KEY (reportCategoryId) REFERENCES [ReportCategory](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id),
);

-- 42.Resolve AF
CREATE TABLE [Resolve] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    reportId    UNIQUEIDENTIFIER NOT NULL,
    userId      UNIQUEIDENTIFIER NOT NULL,
    title       NVARCHAR(255) NOT NULL,
    content     NVARCHAR(255) NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (reportId) REFERENCES [Report](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
);

-- =====================================================
-- NOTIFICATION MODULE
-- =====================================================

-- 43.Notification OK
CREATE TABLE [Notification] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    title       NVARCHAR(255) NOT NULL,
    content     NVARCHAR(255) NOT NULL,
    image       NVARCHAR(255),
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
);

-- 44.UserNotification OF
CREATE TABLE [UserNotification] (
    id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    notificationId  UNIQUEIDENTIFIER NOT NULL,
    userId          UNIQUEIDENTIFIER NOT NULL,
    createAt    DATETIME2 DEFAULT GETDATE(),
    updateAt    DATETIME2 DEFAULT GETDATE(),
    status      INT DEFAULT 1,
    FOREIGN KEY (notificationId) REFERENCES [Notification](id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES [User](id) ON DELETE CASCADE,
);

--INSERT INTO [Role] VALUES (NEWID(), N'Admin', N'Admin', GETDATE(), GETDATE(), 1);
--SELECT * FROM [Role]