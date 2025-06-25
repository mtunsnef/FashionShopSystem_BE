using System.ComponentModel.DataAnnotations;

namespace FashionShopSystem.Service.DTOs.UserDto
{
	public class RegisterUserDto
	{
		[Required]
		public string FullName { get; set; } = null!;
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		public string Password { get; set; } = null!;
	}
}
