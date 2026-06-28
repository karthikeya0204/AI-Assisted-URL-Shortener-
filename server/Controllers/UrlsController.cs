using AI_Assisted_URL_Shortener.DTOs;
using AI_Assisted_URL_Shortener.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI_Assisted_URL_Shortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlsController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpPost]
        public IActionResult Create(CreateUrlRequest request)
        {
            var response = _urlService.Create(request);
            return CreatedAtAction(nameof(GetByCode), new { code = response.ShortCode }, response);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_urlService.GetAll());
        }

        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var response = _urlService.GetByCode(code);
            return response is null ? NotFound() : Ok(response);
        }

        [HttpPut("{code}")]
        public IActionResult Update(string code, UpdateUrlRequest request)
        {
            var response = _urlService.Update(code, request);
            return Ok(response);
        }

        [HttpGet("{code}/analytics")]
        public IActionResult GetAnalytics(string code)
        {
            var response = _urlService.GetAnalytics(code);
            return Ok(response);
        }
    }
}
