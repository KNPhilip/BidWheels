using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace IdentityService.Pages.Account.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class index : PageModel
    {
        public void OnGet()
        {
        }
    }
}