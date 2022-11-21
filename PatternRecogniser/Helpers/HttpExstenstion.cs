using Microsoft.AspNetCore.Http;

namespace PatternRecogniser.Helpers
{
    public static class HttpExtraOperations
    {
        public static bool IsZip(IFormFile file)
        {
            return file.ContentType == "application/zip";
        }
    }
}
