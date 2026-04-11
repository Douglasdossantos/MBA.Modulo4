namespace MBA.WebApp.MVC.Models;

public class ResponseResult
{
	public string Title { get; set; } = string.Empty;
	public int Status { get; set; }
	public ResponseErrorMessages Errors { get; set; } = new();
}
