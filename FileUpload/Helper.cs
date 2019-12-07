using FileUpload.DTOs;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileUpload
{
    public class Helper
    {
        /// <summary>
        /// Authorize user against an Api endpoint.
        /// On successfull authorization, the token and expiration date will be mapped instide the AuthoridationDTO object.
        /// On failure, the returned object will be null.
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns></returns>
        public async Task<AuthorizationDTO> GetToken(string username, string password)
        {
            AuthorizationDTO tokenModel;
            using (var client = new HttpClient())
            {
                // query string parameters
                var query = new Dictionary<string, string>
                {
                    ["Username"] = username,
                    ["Password"] = password,
                };

                // send
                var authorizationResponse = await client.GetAsync(QueryHelpers.AddQueryString($"{Constants.WEB_URL}/{Constants.AUTHORIZATION_ENDPOINT}", query));
                // check if valid
                if (!authorizationResponse.IsSuccessStatusCode)
                    return null;
                else
                {
                    // read response as string
                    var contents = await authorizationResponse.Content.ReadAsStringAsync();
                    // Deserialize token
                    tokenModel = JsonSerializer.Deserialize<AuthorizationDTO>(contents);

                    return tokenModel;
                }
            }
        }

        /// <summary>
        /// Append an image and related satellite data to a MultipartFormDataContent object.
        /// </summary>
        /// <param name="multipartFormDataContent">The object representing the form data</param>
        /// <param name="formDataCount">A count representing the n-th object of the form data</param>
        /// <param name="imagePath">The path of a local image</param>
        /// <param name="categoryName">The category name</param>
        /// <param name="categoryDescription">The category desciption</param>
        /// <param name="detail">The detail</param>
        public void AppendFormData(MultipartFormDataContent multipartFormDataContent, int formDataCount, string imagePath, string categoryName, string categoryDescription, string detail)
        {
            // Get stream from path
            FileStream fs = File.OpenRead(imagePath);
            // Get name of image
            string imageName = Path.GetFileName(imagePath);

            // Image
            HttpContent fileStreamContent = new StreamContent(fs);
            fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = $"appFileDTOs[{formDataCount}].File",
                FileName = imageName
            };
            fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            multipartFormDataContent.Add(fileStreamContent);

            // Data for image
            multipartFormDataContent.Add(new StringContent(categoryName), $"appFileDTOs[{formDataCount}].CategoryName");
            multipartFormDataContent.Add(new StringContent(categoryDescription), $"appFileDTOs[{formDataCount}].CategoryDescription");
            multipartFormDataContent.Add(new StringContent(detail), $"appFileDTOs[{formDataCount}].Detail");
        }
    }
}
