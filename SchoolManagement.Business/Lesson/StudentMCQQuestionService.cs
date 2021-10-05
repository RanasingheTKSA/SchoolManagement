using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using SchoolManagement.Business.Interfaces.LessonData;
using SchoolManagement.Data.Data;
using SchoolManagement.Master.Data.Data;
using SchoolManagement.Model;
using SchoolManagement.ViewModel;
using SchoolManagement.ViewModel.Common;
using SchoolManagement.ViewModel.Lesson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Business
{
    public class StudentMCQQuestionService : IStudentMCQQuestionService
    {
        private readonly MasterDbContext masterDb;
        private readonly SchoolManagementContext schoolDb;
        private readonly IConfiguration config;
        private readonly ICurrentUserService currentUserService;

        public StudentMCQQuestionService(MasterDbContext masterDb, SchoolManagementContext schoolDb, IConfiguration config, ICurrentUserService currentUserService)
        {
            this.masterDb = masterDb;
            this.schoolDb = schoolDb;
            this.config = config;
            this.currentUserService = currentUserService;
        }


        public List<StudentMCQQuestionViewModel> GetAllStudentMCQQuestions()
        {
            var response = new List<StudentMCQQuestionViewModel>();
            var query = schoolDb.StudentMCQQuestions.Where(u => u.Question.Id != null);
            var StudentMCQQuestionList = query.ToList();

            foreach (var item in StudentMCQQuestionList)
            {
                var vm = new StudentMCQQuestionViewModel
                {
                    QuestionId = item.QuestionId,
                    QuestionName = item.Question.QuestionText,
                    //StudentAnswerText = 
                    StudentId = item.StudentId,
                    StudentName = item.Student.User.FullName,
                    TeacherComments = item.TeacherComments,
                    Marks = item.Marks,
                    IsCorrectAnswer = item.IsCorrectAnswer
                };
                response.Add(vm);
            }
            return response;
        }

        public async Task<ResponseViewModel> SaveStudentMCQQuestion(StudentMCQQuestionViewModel vm, string userName)
        {
            var respone = new ResponseViewModel();
            try
            {
                var currentuser = schoolDb.Users.FirstOrDefault(x => x.Username.ToUpper() == userName.ToUpper());
                var StudentMCQQuestions = schoolDb.StudentMCQQuestions.FirstOrDefault(x => x.QuestionId == vm.QuestionId);
                var loggedInUser = currentUserService.GetUserByUsername(userName);

                if (StudentMCQQuestions == null)
                {
                    StudentMCQQuestions = new StudentMCQQuestion()
                    {
                        QuestionId = vm.QuestionId,
                        StudentId = vm.StudentId,
                        TeacherComments = vm.TeacherComments,
                        Marks = vm.Marks,
                        IsCorrectAnswer = vm.IsCorrectAnswer
                    };

                    schoolDb.StudentMCQQuestions.Add(StudentMCQQuestions);
                    respone.IsSuccess = true;
                    respone.Message = " Student MCQ Question is added susccesfully.";
                }

                else
                {
                    StudentMCQQuestions.TeacherComments = vm.TeacherComments;
                    StudentMCQQuestions.Marks = vm.Marks;
                    StudentMCQQuestions.IsCorrectAnswer = vm.IsCorrectAnswer;

                    schoolDb.StudentMCQQuestions.Update(StudentMCQQuestions);
                    respone.IsSuccess = true;
                    respone.Message = " Student MCQ Question is updated susccesfully.";
                }

                await schoolDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                respone.IsSuccess = false;
                respone.Message = ex.ToString();
            }

            return respone;
        }

        public List<DropDownViewModel> GetAllQuestions()
        {
            var questions = schoolDb.Questions
              .Where(x => x.IsActive == true)
              .Select(q => new DropDownViewModel() { Id = q.Id, Name = string.Format("{0}", q.QuestionText) })
              .Distinct().ToList();

            return questions;
        }

        public List<DropDownViewModel> GetAllStudentNames()
        {
            var students = schoolDb.Students
              .Where(x => x.IsActive == true)
              .Select(s => new DropDownViewModel() { Id = s.Id, Name = string.Format("{0}", s.User.FullName) })
              .Distinct().ToList();

            return students;
        }

        public List<DropDownViewModel> GetAllStudentAnswerTexts()
        {
            var studentanswertext = schoolDb.MCQQuestionStudentAnswers
               .Where(x => x.QuestionId != null)
               .Select(sqt => new DropDownViewModel() { Id = sqt.QuestionId, Name = string.Format("{0}", sqt.AnswerText) })
               .Distinct().ToList();

            return studentanswertext;
        }

        public PaginatedItemsViewModel<BasicStudentMCQQuestionViewModel> GetStudentNameList(string searchText, int currentPage, int pageSize, int studentNameId)
        {
            int totalRecordCount = 0;
            double totalPages = 0;
            int totalPageCount = 0;

            var vmu = new List<BasicStudentMCQQuestionViewModel>();

            var student = schoolDb.StudentMCQQuestions.OrderBy(x => x.StudentId);

            if (!string.IsNullOrEmpty(searchText))
            {
                student = student.Where(x => x.Student.User.FullName.Contains(searchText)).OrderBy(x => x.StudentId);
            }

            if (studentNameId > 0)
            {
                student = student.Where(x => x.StudentId == studentNameId).OrderBy(x => x.StudentId);
            }


            totalRecordCount = student.Count();
            totalPages = (double)totalRecordCount / pageSize;
            totalPageCount = (int)Math.Ceiling(totalPages);

            var studentNameList = student.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            studentNameList.ForEach(student =>
            {
                var vm = new BasicStudentMCQQuestionViewModel()
                {
                    QuestionId = student.QuestionId,
                    QuestionName = student.Question.QuestionText,
                    StudentId = student.StudentId,
                    StudentName = student.Student.User.FullName,
                    TeacherComments = student.TeacherComments,
                    Marks = student.Marks,
                    IsCorrectAnswer = student.IsCorrectAnswer

                };
                vmu.Add(vm);
            });

            var container = new PaginatedItemsViewModel<BasicStudentMCQQuestionViewModel>(currentPage, pageSize, totalPageCount, totalRecordCount, vmu);

            return container;
        }

        public DownloadFileModel downloadStudentListReport()
        {
            var studentMCQMarksReport = new StudentMCQMarksReport();

            byte[] abytes = studentMCQMarksReport.PrepareReport(GetAllStudentMCQQuestions());

            var response = new DownloadFileModel();

            response.FileData = abytes;
            response.FileType = "application/pdf";


            return response;
        }
    }



    public class StudentMCQMarksReport
    {
        #region Declaration
        int _totalColumn = 4;
        Document _document;
        iTextSharp.text.Font _fontStyle;
        iTextSharp.text.pdf.PdfPTable _pdfPTable = new PdfPTable(4);
        iTextSharp.text.pdf.PdfPCell _pdfPCell;
        MemoryStream _memoryStream = new MemoryStream();
        List<StudentMCQQuestionViewModel> _students = new List<StudentMCQQuestionViewModel>();
        #endregion

        public byte[] PrepareReport(List<StudentMCQQuestionViewModel> response)
        {
            _students = response;

            #region
            _document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            _document.SetPageSize(PageSize.A4);
            _document.SetMargins(20f, 20f, 20f, 20f);
            _pdfPTable.WidthPercentage = 100;
            _pdfPTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _fontStyle = FontFactory.GetFont("TimesNewRoman", 8f, 1);

            iTextSharp.text.pdf.PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();
            _pdfPTable.SetWidths(new float[] { 80f, 150f, 100f, 100f });
            #endregion

            this.ReportHeader();
            this.ReportBody();
            _pdfPTable.HeaderRows = 4;
            _document.Add(_pdfPTable);
            _document.Close();
            return _memoryStream.ToArray();

        }

        private void ReportHeader()
        {
            _fontStyle = FontFactory.GetFont("TimesNewRoman", 18f, 1);
            _pdfPCell = new PdfPCell(new Phrase("STUDENT MARKS REPORT", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 7;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();

            _fontStyle = FontFactory.GetFont("TimesNewRoman", 12f, 1);
            _pdfPCell = new PdfPCell(new Phrase("STUDENT MCQ MARKS LIST", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 7;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();
        }

        private void ReportBody()
        {
            #region Table header
            _fontStyle = FontFactory.GetFont("TimesNewRoman", 10f, 1);
            _pdfPCell = new PdfPCell(new Phrase("STUDENT ID", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfPTable.AddCell(_pdfPCell);


            _pdfPCell = new PdfPCell(new Phrase("STUDENT NAME", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfPTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("TEACHER COMMENTS", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfPTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("MARKS", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfPTable.AddCell(_pdfPCell);
            _pdfPTable.CompleteRow();
            #endregion

            #region Table Body
            _fontStyle = FontFactory.GetFont("TimesNewRoman", 10f, 0);
            foreach (StudentMCQQuestionViewModel vm in _students)
            {
                _pdfPCell = new PdfPCell(new Phrase(vm.StudentId.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(vm.StudentName, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(vm.TeacherComments, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(vm.Marks.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPTable.AddCell(_pdfPCell);
                _pdfPTable.CompleteRow();
            }
            #endregion

        }

    }
}
      
