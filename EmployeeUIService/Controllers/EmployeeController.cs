using Microsoft.AspNetCore.Mvc;
using EmployeeUIService.Models;
using System.Net.Http.Headers;

namespace EmployeeUIService.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly HttpClient httpClient;

        string? existingPhotoBase64 = null;

        public EmployeeController()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7085"); // Update the base address with your server address
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> Index()
        {
            var response = await httpClient.GetAsync("/Employee/GetAll");
            response.EnsureSuccessStatusCode();

            var employees = await response.Content.ReadAsAsync<List<Employee>>();

            return View(employees);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            var response = await httpClient.PostAsJsonAsync("/Employee/Create", employee);
            response.EnsureSuccessStatusCode();

            var createdEmployeeId = await response.Content.ReadAsAsync<int>();

            return Json(new { success = true, message = "Employee added successfully." });
        }


        public async Task<IActionResult> Edit(int id)
        {
            var response = await httpClient.GetAsync($"/Employee/GetById/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var employee = await response.Content.ReadAsAsync<Employee>();

            existingPhotoBase64 = employee.PhotoBase64;

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                //return View(employee);
                return Json(new { success = false, message = "An error occurred while updating the employee: "});
            }

            employee.PhotoBase64 = employee.PhotoBase64 ?? existingPhotoBase64;

            var response = await httpClient.PostAsJsonAsync($"/Employee/Update/{id}", employee);
            response.EnsureSuccessStatusCode();

            return Json(new { success = true, message = "Employee updated successfully." });
        }

        public async Task<IActionResult> Remove(int id)
        {
            var response = await httpClient.GetAsync($"/Employee/GetById/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var employee = await response.Content.ReadAsAsync<Employee>();

            return View(employee);
        }

        [HttpPost]
        [ActionName("Remove")]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var response = await httpClient.PostAsJsonAsync($"/Employee/Delete/{id}", id);
            response.EnsureSuccessStatusCode();

            return Json(new { success = true, message = "Employee deleted successfully." });
        }

        public async Task<IActionResult> Read(int id)
        {
            var response = await httpClient.GetAsync($"/Employee/GetById/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var employee = await response.Content.ReadAsAsync<Employee>();

            return View(employee);
        }
    }
}
