using System.Diagnostics;
using AkilliYemekTarifOneriSistemi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AkilliYemekTarifOneriSistemi.Controllers
{
    // bu controller mvc tarafýndaki klasik home controller
    // react tarafýndaki api controllerlardan farklý
    // burada razor view dönen sayfalar var index privacy error gibi
    public class HomeController : Controller
    {
        // loglama için injected gelen logger
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // burasý uygulamanýn giriþ sayfasý
        // Views/Home/Index.cshtml dosyasýný döner
        public IActionResult Index()
        {
            return View();
        }

        // gizlilik sözleþmesi gibi statik sayfalar için kullanýlan action
        public IActionResult Privacy()
        {
            return View();
        }

        // hata sayfasý
        // eðer bir exception yakalanýrsa buraya düþer
        // ResponseCache attribute ile cache kullanma diyoruz
        // ErrorViewModel içine requestId set edip view'ýna gönderiyoruz
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
