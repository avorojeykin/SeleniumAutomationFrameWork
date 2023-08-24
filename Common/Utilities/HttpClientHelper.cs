using System;
using System.Net.Http;

namespace Common.Utilities
{
    public class HttpClientHelper
    {
        public static bool IsLinkAlive(string url, out string error)
        {
            bool result = false;
            error = string.Empty;
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(30);
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                { result = true; }
            }
            catch (HttpRequestException ex)
            {
                error = $"HttpRequestException: {ex.Message}";
            }
            catch (ArgumentNullException ex)
            {
                error = $"Null exception: {ex.Message}";
            }
            catch (Exception ex)
            {
                error = CommonUtilities.GetExceptionString(ref ex);
            }
            return result;
        }
    }
}
