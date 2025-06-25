using System.Net;

namespace FashionShopSystem.Service.DTOs.ApiResponseDto
{
	public class ApiResponseDto<T>
	{
		public bool Success { get; set; }
		public int StatusCode { get; set; }
		public T Data { get; set; }
		public string Message { get; set; } // Added property

		public ApiResponseDto(bool success, T data, int statusCode = (int)HttpStatusCode.OK, string message = "")
		{
			Success = success;
			Data = data;
			StatusCode = statusCode;
			Message = message;
		}

		public static ApiResponseDto<T> SuccessResponse(T data, string message = "")
		{
			return new ApiResponseDto<T>(true, data, (int)HttpStatusCode.OK, message);
		}

		public static ApiResponseDto<T> FailResponse(int statusCode = (int)HttpStatusCode.BadRequest, string message = "")
		{
			return new ApiResponseDto<T>(false, default, statusCode, message);
		}
	}
}
