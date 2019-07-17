using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stromzaehler.Models;

namespace Stromzaehler.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(BlinkDataContext database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public BlinkDataContext Database { get; }

        public void OnGet()
        {

        }
    }
}
