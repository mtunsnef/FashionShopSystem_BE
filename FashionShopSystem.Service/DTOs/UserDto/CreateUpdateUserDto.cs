namespace FashionShopSystem.Service.DTOs.UserDto
{
	public class CreateUpdateUserDto
	{
		public string FullName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? PhoneNumber { get; set; }
		public string? Password { get; set; }
		public short RoleId { get; set; }
	}
}
