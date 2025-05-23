using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApiApp4119.Models;

namespace WebApiApp4119.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly string _connStr;

        public BooksController(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var list = new List<Books>();

            using var conn = new SqlConnection(_connStr);
            conn.Open();

            string sql = "SELECT * FROM Books";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Books
                {
                    BookID = (int)reader["BookID"],
                    Title = reader["Title"].ToString(),
                    Author = reader["Author"].ToString(),
                    ISBN = reader["ISBN"].ToString(),
                    YearPublished = reader["YearPublished"].ToString()
                });
            }

            return Ok(list);
        }

        [HttpGet("get/{id}")]
        public IActionResult GetById(int id)
        {
            Books book = null;

            using var conn = new SqlConnection(_connStr);
            conn.Open();

            string sql = "SELECT * FROM Books WHERE BookID = @id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                book = new Books
                {
                    BookID = (int)reader["BookID"],
                    Title = reader["Title"].ToString(),
                    Author = reader["Author"].ToString(),
                    ISBN = reader["ISBN"].ToString(),
                    YearPublished = reader["YearPublished"].ToString()
                };
            }

            return book == null ? NotFound("Book not found") : Ok(book);
        }

        [HttpPost("add")]
        public IActionResult AddBook([FromBody] Books book)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            string sql = "INSERT INTO Books (Title, Author, ISBN, YearPublished) " +
                         "VALUES (@Title, @Author, @ISBN, @YearPublished)";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
            cmd.Parameters.AddWithValue("@YearPublished", book.YearPublished);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? Ok("Book added successfully") : StatusCode(500, "Error adding book");
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteBook(int id)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();

            string sql = "DELETE FROM Books WHERE BookID = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? Ok("Book deleted successfully") : NotFound("Book not found");
        }
    }
}
