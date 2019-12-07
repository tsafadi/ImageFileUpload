using FileUpload.DTOs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FileUpload
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize Helper
            Helper helper = new Helper();

            Console.WriteLine("Please authenticate!");
            // Benutzername
            Console.Write("Username: ");
            string username = Console.ReadLine();
            // Passwort
            Console.Write("Password: ");
            string password = Console.ReadLine();

            // Get token
            Console.WriteLine("\nRetrieving token...");
            AuthorizationDTO tokenModel = await helper.GetToken(username, password);
            Console.WriteLine("Token received!\n");
            if(tokenModel == null)
            {
                Console.Write("Authorization failed!");
                return;
            }

            // Initialize MultipartFormDataContent
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            // Set data for image 1
            Console.Write("Local path for image 1: ");
            string imgPath1 = Console.ReadLine();
            helper.AppendFormData(multiContent, 0, imgPath1, "Image 1 Test 1", "Image 1 Test 2", "Image 1 Test 3");
            // Set data for image 2
            Console.Write("Local path for image 2: ");
            string imgPath2 = Console.ReadLine();
            helper.AppendFormData(multiContent, 1, imgPath2, "Image 2 Test 1", "Image 2 Test 2", "Image 2 Test 3");

            // Send data
            using (var client = new HttpClient())
            {
                Console.WriteLine("\nUploading images...");

                // Append token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenModel.token);
                var response = await client.PostAsync($"{Constants.WEB_URL}/{Constants.IMAGEUPLOAD_ENDPOINT}/1", multiContent);

                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Images uploaded succesfully!");
                else
                    Console.WriteLine("An error occured while attempting to upload images!");
            }
        }
    }
}
