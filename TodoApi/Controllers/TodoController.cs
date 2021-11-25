using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TodoApi.Factory;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITodoFactory _todoFactory;

        public TodoController(ILogger<TodoController> logger, IConfiguration configuration, ITodoFactory todoFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _todoFactory = todoFactory;
        }

        [Route("GetAllTodos")]
        [HttpGet]
        public ActionResult<IEnumerable<TodoModel>> GetAllTodos()
        {
            var result = GetTodos();

            if (result == null)
            {
                _logger.LogInformation("The GetAllTodos returned null");
                return StatusCode(500);
            }

            return new ActionResult<IEnumerable<TodoModel>>(result);
        }

        [Route("SetTodoComplete")]
        [HttpPut]
        public ActionResult TodoComplete([FromBody] TodoModel model)
        {
            SetCompleted(model);

            return StatusCode(204);

        }

        [Route("CreateTodo")]
        [HttpPost]
        public ActionResult CreateTodo([FromBody] TodoModel todo)
        {
            try
            {
                Create(todo);

                return StatusCode(204);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return StatusCode(500);
            }

        }

        #region Methods

        private void Create(TodoModel todoTask)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("TodoDatabase")))
                {
                    connection.Open();
                    var sql = $"INSERT Todo (TodoName, IsComplete) VALUES (@TodoName, @IsComplete)";


                    using SqlCommand command = new SqlCommand(sql, connection);

                    command.Parameters.Add("@TodoName", SqlDbType.NChar).Value = todoTask.TodoName;
                    command.Parameters.Add("@IsComplete", SqlDbType.Bit).Value = 0;

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                StatusCode(500);
            }
        }

        private void SetCompleted(TodoModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("TodoDatabase")))
                {
                    connection.Open();
                    var IsCompleteValue = CalculateIsCompleted(model.IsComplete);

                    var sql = $"UPDATE Todo Set IsComplete = {IsCompleteValue} WHERE Id = '{model.Id}'";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    connection.Close();
                }

            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                StatusCode(500);
            }
        }

        private int CalculateIsCompleted(bool isCompleted) {


            if(isCompleted)
              return 0;

            return 1;
        }

        private List<TodoModel> GetTodos()
        {
            try
            {
                var todos = new List<TodoModel>();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("TodoDatabase")))
                {
                    connection.Open();
                    var sql = "SELECT Id, TodoName, IsComplete FROM Todo";

                    using SqlCommand command = new SqlCommand(sql, connection);
                    using SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var todo =_todoFactory.Create((int)reader["Id"], (string)reader["TodoName"], (bool)reader["IsComplete"]);
                        todos.Add(todo);
                    }
                    connection.Close();
                }
                return todos;
            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                StatusCode(500);
                return new List<TodoModel>();
            }
        }
    }
    #endregion
}
