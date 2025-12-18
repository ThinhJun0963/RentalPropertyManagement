using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Landlord.Properties
{
    [Authorize(Roles = "Landlord")]
    public class CreateModel : PageModel
    {
        private readonly IPropertyService _propertyService;

        public CreateModel(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [BindProperty]
        public PropertyDTO Property { get; set; } = new PropertyDTO();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create the new property via the service
            await _propertyService.AddPropertyAsync(Property);

            // Redirect back to the list after successful creation
            return RedirectToPage("./Index");
        }
    }
}