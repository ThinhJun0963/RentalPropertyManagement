using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalPropertyManagement.BLL.DTOs
{
    public class PropertyDTO
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public bool IsOccupied { get; set; } // Dùng để lọc các Property chưa có hợp đồng
    }
}
