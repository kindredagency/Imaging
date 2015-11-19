using System.Web.Mvc;

namespace Framework.Prototyping.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
    }
}