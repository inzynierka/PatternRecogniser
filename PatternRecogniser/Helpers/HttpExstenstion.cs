using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
