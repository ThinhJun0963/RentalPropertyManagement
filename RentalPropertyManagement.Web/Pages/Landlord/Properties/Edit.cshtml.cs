using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalPropertyManagement.BLL.DTOs;
using RentalPropertyManagement.BLL.Interfaces;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Web.Pages.Landlord.Properties
{
    [Authorize(Roles = "Landlord")]
    public class EditModel : PageModel
    {
        private readonly IPropertyService _propertyService;

        public EditModel(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [BindProperty]
        public PropertyDTO Property { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fetch existing data to populate the form
            Property = await _propertyService.GetPropertyByIdAsync(id);

            if (Property == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update the property record
            await _propertyService.UpdatePropertyAsync(Property);

            return RedirectToPage("./Index");
        }
    }
}