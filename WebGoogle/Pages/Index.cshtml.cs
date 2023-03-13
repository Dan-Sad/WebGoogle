using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebGoogle.Models;

namespace WebGoogle.Pages.Shared
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string apiUrl = "https://localhost:7194/api/blogs";

        [BindProperty]
        public Blog Blog { get; set; } = new("Title", "Body");

        public IndexModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            HttpClient httpClient = GetClient();
            HttpResponseMessage? response = httpClient.GetAsync(apiUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string? json = response.Content.ReadAsStringAsync().Result;
                    List<BlogModel>? blogs = JsonConvert.DeserializeObject<List<BlogModel>>(json);
                    BlogModel? blog = blogs?.FirstOrDefault();

                    if (blog != null)
                    {
                        Blog = new(blog.Title, blog.Body);
                    }
                    else
                    {
                        httpClient.PostAsync(apiUrl, JsonContent.Create(Blog));
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
		}

		public void OnPost()
		{
			try
			{
				GetClient().PutAsync(apiUrl, JsonContent.Create(Blog));
			}
			catch
			{
				throw new Exception();
			}
		}

		private HttpClient GetClient()
        {
            var httpClient = new HttpClient();
            var cookies = _httpContextAccessor?.HttpContext?.Request.Cookies;

            if (cookies != null)
            {
                string cookieString = string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));

                if (cookieString.Any())
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookieString);
            }

            return httpClient;
        }
    }

    public record class Blog(string Title, string Body);
}
