using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace RaceDay.MVC.Services;

public class RaceDayApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string ApiCookieKey = "RaceDay.ApiSessionCookie";

    public RaceDayApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        return SendAsync(HttpMethod.Get, endpoint);
    }

    public Task<HttpResponseMessage> PostAsync(
        string endpoint,
        object data)
    {
        return SendAsync(HttpMethod.Post, endpoint, data);
    }

    public Task<HttpResponseMessage> PutAsync(
        string endpoint,
        object data)
    {
        return SendAsync(HttpMethod.Put, endpoint, data);
    }

    public Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        return SendAsync(HttpMethod.Delete, endpoint);
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string endpoint,
        object? data = null)
    {
        using var request = new HttpRequestMessage(method, endpoint);

        // Add request data when supplied.
        if (data != null)
        {
            request.Content = JsonContent.Create(data);
        }

        var context = _httpContextAccessor.HttpContext;

        // Forward this browser's API session cookie.
        var apiCookie = context?.Session.GetString(ApiCookieKey);

        if (!string.IsNullOrWhiteSpace(apiCookie))
        {
            request.Headers.TryAddWithoutValidation(
                "Cookie",
                apiCookie);
        }

        var response = await _httpClient.SendAsync(request);

        // Save the API session cookie after login or another
        // response that refreshes the session cookie.
        SaveApiSessionCookie(response);

        return response;
    }

    private void SaveApiSessionCookie(HttpResponseMessage response)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context == null)
        {
            return;
        }

        if (response.Headers.TryGetValues(
            "Set-Cookie",
            out var cookies))
        {
            var sessionCookie = cookies.FirstOrDefault(cookie =>
                cookie.StartsWith(
                    ".AspNetCore.Session=",
                    StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(sessionCookie))
            {
                var cookieValue = sessionCookie.Split(';', 2)[0];

                context.Session.SetString(
                    ApiCookieKey,
                    cookieValue);
            }
        }

    }

    public void ClearApiSession()
    {
        _httpContextAccessor.HttpContext?
            .Session.Remove(ApiCookieKey);
    }


    public async Task<HttpResponseMessage> UploadEventBannerAsync(
    int eventId,
    IFormFile bannerImage)
    {
        using var content = new MultipartFormDataContent();

        await using var stream = bannerImage.OpenReadStream();

        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(bannerImage.ContentType);

        content.Add(fileContent, "file", bannerImage.FileName);

        return await _httpClient.PostAsync(
            $"api/Events/{eventId}/banner",
            content);
    }
}