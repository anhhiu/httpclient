using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace httpclient
{
    // delegatingHandder // tạo ra 1 chuỗi hander
    // 
    // khi truy vấn request phair ddi qua hander
    // khi trả về repone cung phai di qua tung hander 1
    // hander cuoi cung  truc tiep gui truy truy van den hander : sockethttphander

    public class Hander
    {
        public async Task testHander()
        {
            //string url = "https://www.facebook.com/xuanthulab";
              string url = "https://postman-echo.com/post?test=123";
         //   string url = "https://www.google.com/";
            CookieContainer cookies = new CookieContainer();

            // TẠO CHUỖI HANDLER
            var bottomHandler = new MyHttpClientHandler(cookies);              // handler đáy (cuối)
            var changeUriHandler = new ChangeUri(bottomHandler);
            var denyAccessFacebook = new DenyAccessFacebook(changeUriHandler); // handler đỉnh

            // Khởi tạo HttpCliet với hander đỉnh chuỗi hander
            var httpClient = new HttpClient(denyAccessFacebook);

            // Thực hiện truy vấn
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string htmltext = await response.Content.ReadAsStringAsync();

            Console.WriteLine(htmltext);
        }

        //p
        public async Task testHander2()
        {
           
                string url = "https://postman-echo.com/post?test=123";
                CookieContainer cookies = new CookieContainer();

                // Tạo chuỗi handler
                var bottomHandler = new MyHttpClientHandler(cookies);  // handler đáy (cuối)
                var changeUriHandler = new ChangeUri(bottomHandler);
                var denyAccessFacebook = new DenyAccessFacebook(changeUriHandler);  // handler đỉnh

                // Khởi tạo HttpClient với handler đỉnh chuỗi handler
                var httpClient = new HttpClient(denyAccessFacebook);

                // Thiết lập header
                httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                // Tạo nội dung cho yêu cầu POST
                var parameters = new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("key1", "value1"),
                                new KeyValuePair<string, string>("key2", "value2")
                            };
                var content = new FormUrlEncodedContent(parameters);

                // Gửi yêu cầu POST
                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                // Kiểm tra phản hồi có thành công không
                response.EnsureSuccessStatusCode();  // Ném lỗi nếu phản hồi không phải là 2xx

                // Đọc nội dung phản hồi
                string htmltext = await response.Content.ReadAsStringAsync();
                Console.WriteLine(htmltext);
            }
    }
    // TRIEN KHAI Tu httpclienthander
    public class MyHttpClientHandler : HttpClientHandler
    {
        public MyHttpClientHandler(CookieContainer cookie_container)
        {

            CookieContainer = cookie_container;     // Thay thế CookieContainer mặc định
            AllowAutoRedirect = false;                // không cho tự động Redirect
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            UseCookies = true;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     CancellationToken cancellationToken)
        {
            Console.WriteLine("Bất đầu kết nối " + request.RequestUri.ToString());
            // Thực hiện truy vấn đến Server
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine("Hoàn thành tải dữ liệu");
            return response;
        }
    }

    public class ChangeUri : DelegatingHandler
    {
        public ChangeUri(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            var host = request.RequestUri.Host.ToLower();
            Console.WriteLine($"Check in  ChangeUri - {host}");
            if (host.Contains("google.com"))
            {
                // Đổi địa chỉ truy cập từ google.com sang github
                request.RequestUri = new Uri("https://github.com/");
            }
            // Chuyển truy vấn cho base (thi hành InnerHandler)
            return base.SendAsync(request, cancellationToken);
        }
    }


    public class DenyAccessFacebook : DelegatingHandler
    {
        public DenyAccessFacebook(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     CancellationToken cancellationToken)
        {

            var host = request.RequestUri.Host.ToLower();
            Console.WriteLine($"Check in DenyAccessFacebook - {host}");
            if (host.Contains("facebook.com"))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(Encoding.UTF8.GetBytes("Không được truy cập"));
                return await Task.FromResult<HttpResponseMessage>(response);
            }
            // Chuyển truy vấn cho base (thi hành InnerHandler)
            return await base.SendAsync(request, cancellationToken);
        }
    }


}
