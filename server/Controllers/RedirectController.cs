using AI_Assisted_URL_Shortener.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI_Assisted_URL_Shortener.Controllers
{
    [ApiController]
    [Route("{code}")]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public RedirectController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet]
        public IActionResult RedirectToOriginal(string code)
        {
            var record = _urlService.GetForRedirect(code);
            if (record is null)
            {
                return NotFound();
            }

            _urlService.LogRedirect(record, Request.Headers["User-Agent"].FirstOrDefault());
            return Redirect(record.OriginalUrl);
        }
    }
}
