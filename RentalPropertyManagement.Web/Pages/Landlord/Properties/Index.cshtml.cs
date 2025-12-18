using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Landlord.Properties
{
    // Restrict access to Landlords only
    [Authorize(Roles = "Landlord")]
    public class IndexModel : PageModel
    {
        private readonly IPropertyService _propertyService;

        public IndexModel(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public IEnumerable<PropertyDTO> Properties { get; set; }

        public async Task OnGetAsync()
        {
            // Retrieve all properties from the service to display in the table
            Properties = await _propertyService.GetAllPropertiesAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Call the service to remove the property record
            await _propertyService.DeletePropertyAsync(id);

            // Reload the page to show the updated list
            return RedirectToPage();
        }
    }
}