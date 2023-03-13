using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Tittle { get; set; }
    [BindProperty]
    public string Body { get; set; }

    public void OnGet()
    {
        Body = " 2";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return null;
    }
}
