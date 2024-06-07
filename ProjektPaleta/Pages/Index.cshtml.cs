using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class IndexModel : PageModel
{
    [BindProperty]
    public string ReceivedValue { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPostProcessValue([FromBody] string value)
    {
        ReceivedValue = value;
        // Process the value as needed
        return new JsonResult(new { success = true, receivedValue = ReceivedValue });
    }
}