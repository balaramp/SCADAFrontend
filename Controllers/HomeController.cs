using Microsoft.AspNetCore.Mvc;
using SCADAFrontend.Services;
using SCADAFrontend.Models;
using System.Threading.Tasks;

namespace SCADAFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataAcquisitionService _dataService;
        private readonly HistoricalDataService _historicalDataService;

        public HomeController(DataAcquisitionService dataService, HistoricalDataService historicalDataService)
        {
            _dataService = dataService;
            _historicalDataService = historicalDataService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _dataService.GetLatestDataAsync("172.16.0.39", 502);
            return View(data);
        }

        [HttpGet("api/historicalData")]
        public async Task<IActionResult> GetHistoricalData(DateTime startTime, DateTime endTime)
        {
            var retrievedData = await _dataService.GetHistoricalDataAsync(startTime, endTime);
            await _historicalDataService.StoreHistoricalDataAsync(retrievedData);
            var storedData = await _historicalDataService.GetHistoricalDataAsync(startTime, endTime);
            return Ok(storedData);
        }
    }
}
