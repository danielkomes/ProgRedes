using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminServer;
using Domain;
using Domain.Helpers;
using Domain.Responses;
using Grpc.Net.Client;
using WebApi.Interfaces;
using static AdminServer.MessageExchanger;

namespace WebApi.Services
{
    public class GameService : IGameService
    {
        //private readonly IStudentRepository _studentRepository;
        private readonly MessageExchangerClient client;

        public GameService(/*IStudentRepository studentRepository*/)
        {
            //_studentRepository = studentRepository;
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001"); //TODO: move to ServerConfig.json
            client = new MessageExchangerClient(channel);
        }


        public IEnumerable<Game> GetGames()
        {
            //IEnumerable<StudentDto> studentsDto = await _studentRepository.GetStudentsAsync();
            //return studentsDto.Select(studentDto => MapStudentDtoToDomain(studentDto)).ToList();
            return Sys.GetGames();
        }

        public async Task<PaginatedResponse<Game>> GetGames(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return null;
            }

            MessageReply reply = await client.ListPagedAsync(
                new PagedListRequest
                {
                    Page = page,
                    PageSize = pageSize
                });
            List<Game> games = Logic.DecodeListGames(reply.Message);
            int totalGames = games.Count;
            return PaginationHelper<Game>.GeneratePaginatedResponse(pageSize, totalGames, games);
        }

        public async Task<Game> GetGameById(int id)
        {
            MessageReply reply = await client.GetGameByIdAsync(
                new MessageRequest
                {
                    Message = id.ToString()
                });
            Game game = null;
            if (!string.IsNullOrEmpty(reply.Message))
            {
                game = Logic.DecodeGame(reply.Message);
            }
            return game;
        }
        public async Task<Game> PublishGameAsync(Game game)
        {
            PublishReply reply = await client.PublishAsync(
                new MessageRequest
                {
                    Message = Logic.EncodeGame(game)
                });
            game.Id = reply.Id;
            return game;
        }

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