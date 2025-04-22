namespace Capstone.ECommerceApp.Infra.Common;
public class ResponseDto
{
    public object? Result { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
    public List<string> Errors { get; set; }
}
