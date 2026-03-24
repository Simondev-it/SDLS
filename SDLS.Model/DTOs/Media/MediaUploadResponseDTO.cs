namespace SDLS.Model.DTOs.Media;

public class MediaUploadResponseDTO
{
    public string Url { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? Name { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? MimeType { get; set; }
    public long? FileSize { get; set; }
}
