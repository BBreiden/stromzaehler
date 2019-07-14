using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stromzaehler.Models;

namespace Stromzaehler.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(CounterModel counter)
        {
            Counter = counter ?? throw new ArgumentNullException(nameof(counter));
        }

        public CounterModel Counter { get; }

        public void OnGet()
        {

        }
    }
}
