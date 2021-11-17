using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Helpers;
using Domain.Responses;
using WebApi.Interfaces;

namespace WebApi.Services
{
    public class GameService : IGameService
    {
        //private readonly IStudentRepository _studentRepository;

        public GameService(/*IStudentRepository studentRepository*/)
        {
            //_studentRepository = studentRepository;
        }


        public IEnumerable<Game> GetGames()
        {
            //IEnumerable<StudentDto> studentsDto = await _studentRepository.GetStudentsAsync();
            //return studentsDto.Select(studentDto => MapStudentDtoToDomain(studentDto)).ToList();
            return Sys.GetGames();
        }

        public PaginatedResponse<Game> GetGames(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }

            //int totalStudents = await _studentRepository.GetTotalStudentsAsync();
            int totalGames = Sys.Games.Count;
            if (totalGames > 0)
            {
                //IEnumerable<StudentDto> repoStudents = await _studentRepository.GetStudentsAsync(page, pageSize);
                //if (repoStudents == null || !repoStudents.Any())
                //{
                //    return null;
                //}

                //var students = new List<Student>();
                //foreach (var studentDto in repoStudents)
                //{
                //    students.Add(MapStudentDtoToDomain(studentDto));
                //}

                //return PaginationHelper<Student>.GeneratePaginatedResponse(pageSize, totalGames, students);
                List<Game> games = Sys.GetGames(page, pageSize);
                return PaginationHelper<Game>.GeneratePaginatedResponse(pageSize, totalGames, games);
            }

            return null;
        }

        public Game GetGameById(int id)
        {
            return Sys.GetGame(id);
        }

        //public async Task<Student> GetStudentByIdAsync(int id)
        //{
        //    StudentDto studentDto = await _studentRepository.GetStudentByIdAsync(id);
        //    if (studentDto != null)
        //    {
        //        return MapStudentDtoToDomain(studentDto);
        //    }

        //    return null;
        //}

        //public async Task<Student> SaveStudentAsync(Student student)
        //{
        //    StudentDto studentDto = MapStudentDomainToDto(student);
        //    var responseStudentDto = await _studentRepository.SaveStudentAsync(studentDto);
        //    return MapStudentDtoToDomain(responseStudentDto);
        //}

        //public async Task<Student> UpdateStudentAsync(Student student)
        //{
        //    StudentDto studentDto = MapStudentDomainToDto(student);
        //    var responseStudentDto = await _studentRepository.UpdateStudentAsync(studentDto);
        //    return MapStudentDtoToDomain(responseStudentDto);
        //}

        //public async Task DeleteStudentAsync(Student student)
        //{
        //    StudentDto studentDto = MapStudentDomainToDto(student);
        //    await _studentRepository.DeleteStudentAsync(studentDto);
        //}

        //private StudentDto MapStudentDomainToDto(Student student)
        //{
        //    return new StudentDto
        //    {
        //        Email = student.Email,
        //        Id = student.Id,
        //        Name = student.Name
        //    };
        //}

        //private Student MapStudentDtoToDomain(StudentDto studentDto)
        //{
        //    return new Student
        //    {
        //        Email = studentDto.Email,
        //        Id = studentDto.Id,
        //        Name = studentDto.Name
        //    };
        //}
    }
}