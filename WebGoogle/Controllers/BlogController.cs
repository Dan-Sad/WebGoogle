using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using WebGoogle.Models;

namespace WebGoogle.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/blogs")]
    public class BlogController : ControllerBase
    {
        private readonly string _filePath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string? userId => _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public BlogController(string filePath, IHttpContextAccessor httpContextAccessor)
        {
            _filePath = filePath;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return string.IsNullOrEmpty(userId) ? NotFound() : Get(userId);
        }

        [HttpGet("{id}")]
        private IActionResult Get(string id)
        {
            try
            {
                string json = System.IO.File.ReadAllText(_filePath);
                List<BlogModel>? blogs = JsonConvert.DeserializeObject<List<BlogModel>>(json)?
                    .FindAll(p => p.Id == id);

                return blogs?.Any() == true ? Ok(blogs) : NoContent();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string requestBody = await reader.ReadToEndAsync();
                BlogModel? blog = JsonConvert.DeserializeObject<BlogModel>(requestBody);

                if (blog == null)
                {
                    return BadRequest();
                }

                string json = System.IO.File.ReadAllText(_filePath);
                List<BlogModel>? blogs = JsonConvert.DeserializeObject<List<BlogModel>>(json);

                if (blogs == null)
                {
                    blogs = new List<BlogModel>();
                }

                string? userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId != null)
                    blog.Id = userId;
                else
                    return Unauthorized();

                blogs.Add(blog);

                System.IO.File.WriteAllText(_filePath, JsonConvert.SerializeObject(blogs));

                return Ok(blog);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put()
        {
            return string.IsNullOrEmpty(userId) ? NotFound() : await Put(userId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id)
        {
            try
            {
                string json = System.IO.File.ReadAllText(_filePath);
                List<BlogModel>? blogs = JsonConvert.DeserializeObject<List<BlogModel>>(json);
                BlogModel? blog = blogs?.FirstOrDefault(p => p.Id == id);

                if (blog == null)
                {
                    return NotFound();
                }

                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    BlogModel? updatedPost = JsonConvert.DeserializeObject<BlogModel>(requestBody);

                    if (updatedPost == null)
                    {
                        return BadRequest();
                    }

                    blog.Title = updatedPost.Title;
                    blog.Body = updatedPost.Body;

                    System.IO.File.WriteAllText(_filePath, JsonConvert.SerializeObject(blogs));

                    return Ok(blog);
                }
            }
            catch
            {
                return NotFound();
            }
        }

    }
}