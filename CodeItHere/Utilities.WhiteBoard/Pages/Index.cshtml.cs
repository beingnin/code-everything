using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Utilities.WhiteBoard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string Board = string.Empty;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            this.Board = this.Request.Query["board"];

            if (!Guid.TryParse(this.Board,out _))
            {
                Board = Guid.NewGuid().ToString();
                return RedirectToPage("index", new { board = Board });
            }
            return null;
        }

    }
}
