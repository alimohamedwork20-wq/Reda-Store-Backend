using System.ComponentModel;

namespace Reda.Dtos
{
    public class AddAddressDto
    {
        public string Title { get; set; }
        public string City { get; set; }
        public string Details { get; set; }
        public string Phone { get; set; }
        [DefaultValue(false)]
        public bool IsDefault { get; set; } = false;
    }
}