using Aio.Umbraco.Common.ContentModels;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NewsCrawlerApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting crawling!");

            //dynamic newsrDynamic = new ExpandoObject();
            //newsrDynamic.Name = "Test-08-09-2020-2";
            //newsrDynamic.ParentId = 1107;
            //newsrDynamic.Title = "Crawler from Vnexpress - Đà Nẵng nới lỏng cách ly xã hội";
            //newsrDynamic.Source = "Vnexpress";
            //newsrDynamic.SourceUrl = "https://vnexpress.net/da-nang-noi-long-cach-ly-xa-hoi-4157008.html";
            //newsrDynamic.Category = "Thời sự";
            //newsrDynamic.PostedDateTime = DateTime.Now.AddMinutes(-20).ToString();
            //newsrDynamic.Author = "Tony";
            //newsrDynamic.ShortDescription = "Dịch Covid-19 được kiểm soát nên thành phố Đà Nẵng sẽ giãn cách xã hội từ 0h ngày 5/9.";
            //newsrDynamic.FullDescription = "Quyết định áp dụng các biện pháp phòng, chống dịch trong tình hình mới được Chủ tịch UBND TP Đà Nẵng Huỳnh Đức Thơ ban hành chiều 4/9. Theo đó, sau 40 ngày áp dụng cách ly xã hội toàn thành phố theo chỉ thị 16, Đà Nẵng sẽ chuyển sang thực hiện chỉ thị 19 khi dịch Covid-19 đã được kiểm soát.</p><p class=\"Normal\">Theo quyết định này, thành phố cho phép các nhà hàng, cửa hàng, cơ sở kinh doanh dịch vụ ăn, uống chỉ được bán hàng cho khách mang đi, đặt hàng và bán hàng qua mạng, giao hàng tận nơi cho khách hàng. Nghiêm cấm phục vụ khách tại chỗ.";
            //newsrDynamic.MetaPageTitle = "Dịch Covid-19 được kiểm soát nên thành phố Đà Nẵng";
            //newsrDynamic.MetaKeywords = "Dịch Covid-19, Dịch Covid-19 được kiểm soát nên thành phố Đà Nẵng";
            //newsrDynamic.MetaDescription = "Dịch Covid-19 được kiểm soát nên thành phố Đà Nẵng sẽ giãn cách xã hội từ 0h ngày 5/9";
            //newsrDynamic.PictureUrl = @"D:\Working\Private\ub\NewsWeb\NewsWeb\assets\images\thumb\2.jpg";
            //newsrDynamic.PictureName = "Crawl-picture1.jpg";
            //newsrDynamic.Picture = "Crawl-picture1";
            //newsrDynamic.PictureType = "jpg";
            //PostCarAsync(newsrDynamic);

            NewsModel news1 = new NewsModel();
            news1.Name = "chuyen-gia-nen-giu-on-dinh-viec-sat-hach-lai-xe";
            news1.Title = "Chuyên gia: 'Nên giữ ổn định việc sát hạch lái xe'";
            news1.Source = "Vnexpress";
            news1.SourceUrl = "https://vnexpress.net/chuyen-gia-nen-giu-on-dinh-viec-sat-hach-lai-xe-4159231.html";
            news1.Category = "Giao thông";
            news1.ParentId = 1112;
            news1.PostedDateTime = DateTime.Now.ToString();
            news1.Author = "Đoàn Loan - Bá Đô ";
            news1.ShortDescription = "Nhiều chuyên gia cho rằng việc đào tạo, sát hạch lái xe được ngành giao thông quản lý ổn định nhiều năm nay, không nên chuyển sang Bộ Công an vì có thể gây xáo trộn.";
            news1.FullDescription = @"Nhiều chuyên gia cho rằng việc đào tạo, sát hạch lái xe được ngành giao thông quản lý ổn định nhiều năm nay, không nên chuyển sang Bộ Công an vì có thể gây xáo trộn. Dự thảo Luật bảo đảm bảo trật tự an toàn giao thông đường bộ sẽ trình Quốc hội tại kỳ họp cuối năm nay(dự kiến tháng 10) đưa ra hai phương án về vấn đề đào tạo, sát hạch và cấp giấy phép lái xe.

Theo đó, phương án một đưa vấn đề trên vào quy định trong dự thảo Luật bảo đảm trật tự an toàn giao thông đường bộ; nghĩa là sẽ chuyển công tác quản lý đào tạo, sát hạch và cấp giấy phép lái xe từ Bộ Giao thông Vận tải sang Bộ Công an phụ trách.

Phương án hai, giữ như hiện hành, dự thảo Luật Giao thông đường bộ(sửa đổi) tiếp tục điều chỉnh vấn đề đào tạo, sát hạch và cấp giấy phép lái xe.

Ông Nguyễn Văn Quyền -Chủ tịch Hiệp hội vận tải ôtô Việt Nam, cho hay trước năm 1995, Bộ Công an chịu trách nhiệm quản lý nhà nước về đào tạo, sát hạch và cấp giấy phép lái xe. Năm 1995, Chính phủ ban hành nghị định 36 chuyển toàn bộ lĩnh vực này sang Bộ Giao thông Vận tải.

Lúc đó, một số vấn đề không thuộc mảng quốc phòng, an ninh và đủ điều kiện dân sự hóa được chuyển cơ quan quản lý, để Bộ Công an tập trung hơn vào nhiệm vụ chính, ông Quyền nói.

Theo ông Quyền, một lĩnh vực quản lý nhà nước chuyển đi, chuyển lại giữa hai bộ là không cần thiết, có thể gây xáo trộn và ảnh hưởng đến người dân trong khi chưa thể khẳng định cơ quan nào quản lý sẽ tốt hơn.

Hiện nay ngành giao thông vận tải phụ trách sát hạch bằng lái, quản lý tài xế và phương tiện; còn cảnh sát giao thông tuần tra, xử phạt vi phạm hành chính; như vậy là hai bộ quản lý hai lĩnh vực, vừa hỗ trợ, vừa giám sát lẫn nhau. Nếu tập trung vào một đầu mối có thể không đảm bảo tính khách quan, ông Quyền nêu vấn đề..";

            news1.MetaPageTitle = "Chuyên gia: 'Nên giữ ổn định việc sát hạch lái xe' - Aionews";
            news1.MetaKeywords = "Đào tạo, sát hạch, lái xe, Bộ Giao thông Vận tải, Bộ Công an, chuyển đổi, Tin nóng, Giao thông, Ghi nhận";
            news1.MetaDescription = "Nhiều chuyên gia cho rằng việc đào tạo, sát hạch lái xe được ngành giao thông quản lý ổn định nhiều năm nay, không nên chuyển sang Bộ Công an";
            news1.PictureUrl = @"D:\Working\Private\ub\NewsWeb\NewsWeb\assets\images\thumb\2.jpg";
            news1.PictureName = "Crawl-picture1.jpg";
            news1.Picture = "Crawl-picture1";
            news1.PictureExternalUrl = "https://i1-vnexpress.vnecdn.net/2020/09/10/xesathach86881571838030-159970-9388-1408-1599709856.jpg?w=680&h=0&q=100&dpr=1&fit=crop&s=qcl_UF_Lyx6b-UZqsp3HRw";

            PostNewsAsync(news1);

            //Insert Category
            Console.WriteLine("Starting crawling Category!");
            CategoryModel cat = new CategoryModel();
            cat.Name = "Test1-Thoi-Su";
            cat.Title = "Test Thời Sự";
            cat.ParentId = 1107;
           
            PostCategoryAsync(cat);
            Console.WriteLine("Starting crawling Update Category!");
            CategoryModel cat1 = new CategoryModel();
            cat1.Name = "Test1-Thoi-Su";
            cat1.Title = "Test Thời Sự";
            cat1.ParentId = 1107;

            PostCategoryAsync(cat1);

            Console.ReadLine();
        }

        static async Task PostNewsAsync(dynamic car)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:26268/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(JsonConvert.SerializeObject(car), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/umbraco/api/NewsApi/InsertNews", content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Data posted");
            }
            else
            {
                Console.WriteLine($"Failed to poste data. Status code:{response.StatusCode}");
            }
        }
        static async Task PostNewsAsync(NewsModel newsModel)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:26268/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(JsonConvert.SerializeObject(newsModel), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/umbraco/api/NewsApi/InsertNews", content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Data posted");
            }
            else
            {
                Console.WriteLine($"Failed to poste data. Status code:{response.StatusCode}");
            }
        }

        static async Task GetNewsAsync()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:26268/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetStringAsync("/umbraco/api/NewsApi/GetAllNews");
            if (!string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Data retrived: " + response.ToString());
            }
            else
            {
                Console.WriteLine("Failed to poste data");
            }
        }

        static async Task PostCategoryAsync(CategoryModel categoryModel)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:26268/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(JsonConvert.SerializeObject(categoryModel), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/umbraco/api/CategoryApi/InsertCategory", content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Data posted");
            }
            else
            {
                Console.WriteLine($"Failed to poste data. Status code:{response.StatusCode}");
            }
        }
    }
}
