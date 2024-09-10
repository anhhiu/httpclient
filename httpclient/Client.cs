using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace httpclient
{
    public class Client
    {
        public async Task GetData()
        {
            using var client = new HttpClient();
            var httpMessageRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://postman-echo.com/get?test=123")
            };
            httpMessageRequest.Headers.Add("User-Agent", "Mozilla/5.0");

            // Gửi yêu cầu và nhận phản hồi
            var httpResponseMessage = await client.SendAsync(httpMessageRequest);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                // Đọc nội dung trả về từ phản hồi
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine("Dữ liệu từ server:");
                Console.WriteLine(responseContent);
            }
            else
            {
                Console.WriteLine($"Yêu cầu thất bại với mã trạng thái: {httpResponseMessage.StatusCode}");
            }
        }

        public async Task TestClient2()
        {
            using var client = new HttpClient();
            var httpMessageRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://postman-echo.com/post?test=123")
            };
            httpMessageRequest.Headers.Add("User-Agent", "Mozilla/5.0");

            // Dữ liệu JSON cần gửi
            string data = @"{
                    ""key1"": ""value1"",
                    ""key2"": ""giatri2""
                    }";

            // In ra dữ liệu JSON
            Console.WriteLine("Dữ liệu gửi đi:");
            Console.WriteLine(data);

            // Tạo nội dung JSON
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            // Gán nội dung vào yêu cầu
            httpMessageRequest.Content = content;

            // Gửi yêu cầu và nhận phản hồi
            var httpResponseMessage = await client.SendAsync(httpMessageRequest);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine("Phản hồi từ server:");
                Console.WriteLine(responseContent);
            }
            else
            {
                Console.WriteLine($"Yêu cầu thất bại với mã trạng thái: {httpResponseMessage.StatusCode}");
            }
        }
        public async Task testFile()
        {
            using var client = new HttpClient();
            var httpmessageRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://postman-echo.com/post")
            };
            httpmessageRequest.Headers.Add("User-Agent", "Mozilla/5.0");

            var content = new MultipartFormDataContent();

            Stream file = File.OpenRead("D:\\c#\\c#_can_ban\\httpclient\\httpclient\\1.txt");
            var fileUpload = new StreamContent(file);

            content.Add(fileUpload, "fileupload", "abc.xyz");
            content.Add(new StringContent("value1"), "key3");
            
            // Gán nội dung vào yêu cầu
            httpmessageRequest.Content = content;

            // Gửi yêu cầu và nhận phản hồi
            var httpResponseMessage = await client.SendAsync(httpmessageRequest);
            file.Close();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine("Phản hồi từ server:");
                Console.WriteLine(responseContent);
            }
            else
            {
                Console.WriteLine($"Yêu cầu thất bại với mã trạng thái: {httpResponseMessage.StatusCode}");
            }
        }
        public async Task testSocketHttpHander()
        {

            var url = "https://postman-echo.com/post";
            var cookies = new CookieContainer(); // Sử dụng CookieContainer để quản lý cookies
            using var socket = new SocketsHttpHandler
            {
                AllowAutoRedirect = true, // Cho phép chuyển hướng URL
                AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                CookieContainer = cookies, // Gắn CookieContainer
                UseCookies = true // Bật tính năng sử dụng cookies
            };

            using var client = new HttpClient(socket);
            var httpRequestMesage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
            };
            httpRequestMesage.Headers.Add("User-Agent", "Mozilla/5.0");

            var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("key1", "value1"),
                        new KeyValuePair<string, string>("key2", "value13"),
                        new KeyValuePair<string, string>("key3", "value12")
                    };

            httpRequestMesage.Content = new FormUrlEncodedContent(parameters);

            // Gửi yêu cầu POST và nhận phản hồi
            var response = await client.SendAsync(httpRequestMesage);

            // Đọc nội dung phản hồi
            var html = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Nội dung phản hồi:");
            Console.WriteLine(html);

            // Lấy cookies từ phản hồi
            var responseCookies = cookies.GetCookies(new Uri(url));

            Console.WriteLine("\nCác cookies đã nhận được:");
            foreach (Cookie cookie in responseCookies)
            {
                Console.WriteLine($"Name: {cookie.Name}, Value: {cookie.Value}");
            }
        }

       
    }
}
