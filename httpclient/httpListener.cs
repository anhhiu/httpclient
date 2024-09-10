using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace httpclient
{
    public class MyhtttpServer // lớp chính quản lý các yêu cầu đến từ client
    {
        HttpListener listener;  //biến này: lắng nghe yêu cầu  http gửi đến máy chủ, xây dụng ứng dụng  máy chủ nhỏ mà k cân den asp.net

        public MyhtttpServer(string[] prifixes) // hàm tạo, nhận các danh sách url
        {
            if (!HttpListener.IsSupported) // kiểm tra 
            {
                throw new Exception(" khong ho tro httplistener");
            }

            listener = new HttpListener(); // khởi tạo một đối tượng HttpListener
            foreach(string prifix in prifixes) listener.Prefixes.Add(prifix); // duyệt các danh sách  tiền tố và thêm chúng vào  đối tương , may chủ sẽ lắng nghe tất cả các url

        }
        public async Task Start() // khoi tao, lang nghe  cac kêt noi tu client
        {
            listener.Start(); // lang nghe yeu cau tu http
            do // vobf lap chya vo hạn cho den khi may chủ dừng lắng nghe
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + "waiting a client a connect");
                var context = await listener.GetContextAsync(); // đợi khi có yêu cầu từ  client, tạo đối từng HttpListener bao gồm cả yêu cầu và phàn hồi
                Console.WriteLine(DateTime.Now.ToLongTimeString() + "client connected");

                await ProcessRequest(context); // phương thức xử lý yêu cầu và gửi phản hồi
                Console.WriteLine();

            } while (listener.IsListening); // khi máy chủ ngừng lắng nghe thì dừng vòng l
        }

        public async Task ProcessRequest(HttpListenerContext context) // phương thức: xử lý yêu cầu http được gửi từ client
        {
            HttpListenerRequest request = context.Request; // trích xuất đối tượng yêu cầu từ ngữ cảnh context, chứa thongn tin phương thức http(get, post...) va tu url của yêu cầu
            HttpListenerResponse response = context.Response; // trích xuất đói tượng phản hồi, sẽ được gửi lại client

            Console.WriteLine($"{request.HttpMethod} {request.RawUrl} {request.Url.AbsolutePath}"); 
            var outputStream = response.OutputStream;

            switch (request.Url.AbsolutePath) // dựa vào request.url.absolutePath máy chủ sẽ quyết định xử lý 
            {
                case "/": // client  truy cap dia chi goc máy chủ sẽ tra ve 1 chuôi xin chao
                    {
                        var buffer = Encoding.UTF8.GetBytes("xin chao");
                        response.ContentLength64 = buffer.Length;
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        
                    }
                    break;
                case "/json": // client truy cap  dia chi http://localhost:8080/json , sever sẽ trả về 1 chuỗi json đại diện cho một đối tượng sp vơi cac thuộc tính  id, ten, gia
                    {
                        response.Headers.Add("Context-Type", "application/json");

                        var product = new
                        {
                            id = 1,
                            ten = "sach",
                            gia = 200

                        };

                        var json = JsonSerializer.Serialize(product);
                        var buffer = Encoding.UTF8.GetBytes(json);
                        response.ContentLength64 = buffer.Length;
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    break;

                default: // đường dẫn k hop le thi tra ve loi NOT FOUND
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        var buffer = Encoding.UTF8.GetBytes("NOT FOUND");
                        response.ContentLength64 = buffer.Length;
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    break;

               
            }

            outputStream.Close();
        }
    }

    public class httpListener
    {
        public async Task testListener()
        {

            if (HttpListener.IsSupported)
            {
                Console.WriteLine("co support");
            }
            else
            {
                Console.WriteLine("khong suppost");
                throw new Exception("khong support httpListener");
            }

            var server = new HttpListener();
            server.Prefixes.Add("http://localhost:8080/");
            server.Start();
            Console.WriteLine("Server dang lang nghe tren cong  8080...");

            try
            {
                do
                {
                    // Chờ kết nối từ client
                    var context = await server.GetContextAsync();
                    Console.WriteLine("Client ket noi thanh cong");

                    var response = context.Response;
                    var outputStream = response.OutputStream;

                    // Cấu hình phản hồi là HTML
                    response.Headers.Add("content-type", "text/html");

                    // Nội dung HTML
                    var html = "<h1>di an com thoi!</h1>";
                    var bytes = Encoding.UTF8.GetBytes(html);

                    // Gửi dữ liệu phản hồi về client
                    await outputStream.WriteAsync(bytes, 0, bytes.Length);
                    outputStream.Close();
                } while (server.IsListening);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"da xay ra loi: {ex.Message}");
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Server da dung.");
            }
        }
    }
}
