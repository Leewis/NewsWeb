using Aio.Umbraco.Common.ContentModels;
using Aio.Umbraco.Services.Interfaces;
//using Lucene.Net.Util;
using System;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;
using Umbraco.Core.Composing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Web.Models;
using Umbraco.Core.PropertyEditors;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace NewsWeb.Controllers
{
    public class NewsApiController : UmbracoApiController
    {
        private readonly INewsService _iNewsService;
        private readonly ICategoryService _iCategoryService;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
        private readonly IDataTypeService _dataTypeService;
        private readonly ISearchHelper _searchHelper;

        NewsSearchModel vm = new NewsSearchModel();

        NewsModel _newModel = new NewsModel();

        public NewsApiController(INewsService iNewsService, ICategoryService iCategoryService, IContentTypeBaseServiceProvider contentTypeBaseServiceProvider, IDataTypeService dataTypeService, ISearchHelper searchHelper)
        {
            _iNewsService = iNewsService;
            _iCategoryService = iCategoryService;
            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
            _dataTypeService = dataTypeService;
            _searchHelper = searchHelper;
        }
        public int GetAllNews()
        {
            IList<IContent> rootNode = Services.ContentService.GetRootContent().ToList();

            return rootNode.FirstOrDefault().Id;
        }

        public HttpResponseMessage InsertNews(NewsModel news)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                //Check if this News has existed or not
                var existedNews = GetNews(news.Title);
                if (existedNews < 0)
                {
                    //imageId = UploadImage(1115, filePath, news.PictureTitle, news.PictureName);

                    var category = _iCategoryService.GetCategory(news.Category);
                    //IMedia media = Services.MediaService.GetById(imageId);               
                    IContent createdNews = null;
                    if (category != null)
                    {
                        createdNews = Services.ContentService.Create(news.Name, category.Id, "news");
                    }
                    else
                    {
                        createdNews = Services.ContentService.Create(news.Name, news.ParentId, "news");
                    }

                    if (createdNews != null)
                    {
                        //TODO
                        //IList<string> categoryName = new List<string>();
                        //categoryName.Add(category.Name);
                        //createdNews.AssignTags("category", categoryName.ToArray());

                        //var categoryUdi = Udi.Create(Constants.UdiEntityType.Document, category.Key);
                        //var searchCategoryPage = Umbraco.PublishedContent(categoryUdi);
                        //createdNews.SetValue("category", searchCategoryPage);                   

                        //IList <Link> links = new List<Link>();
                        //links.Add(new Link()
                        //{
                        //    Name = "Test 1",
                        //    Type = LinkType.Content,
                        //    Udi = locaUdi,
                        //    Url = "/home/thoi-su/giao-thong/xe-qua-tai-giam-manh-tren-quoc-lo-5/"
                        //});

                        //TODO: this field will be set data later
                        //createdNews.SetValue("relatedNews", links);

                        //TODO: this field will be set data later
                        bool imageSaved = false;
                        if (!string.IsNullOrEmpty(news.PictureUrl))
                        {
                            using (Stream stream = System.IO.File.OpenRead(news.PictureUrl))
                            {
                                createdNews.SetValue(_contentTypeBaseServiceProvider, "picture", news.PictureName, stream);
                                imageSaved = true;
                            }
                        }
                        else if (!string.IsNullOrEmpty(news.PictureExternalUrl))
                        {
                            try
                            {
                                //TODO
                                // Image is Empty when run with await' upload image
                                //ImportFirstImageAsync(createdNews, news.PictureExternalUrl, news.PictureName);

                                DownloadImage(news.PictureExternalUrl, news.PictureName, createdNews);
                                //createdNews.SetValue(_contentTypeBaseServiceProvider, "picture", news.PictureName, GetImage(news.PictureExternalUrl).Result);
                            }
                            catch (Exception exception)
                            {
                                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                            }
                        }


                        if (imageSaved)
                        {
                            //this is a work around for the SetValue method to save a file, since it doesn't currently take into account the image cropper
                            //which we are using so we need to fix that.
                            var propType = createdNews.Properties["picture"].PropertyType;
                            var cropperValue = CreateImageCropperValue(propType, createdNews.GetValue("picture"), _dataTypeService);
                            createdNews.SetValue("picture", cropperValue);
                        }

                        //TOD
                        //createdNews.SetValue("thumbnailImage", imageId); //media.Path = -1,newsId,mediaId;

                        PopulateContentFields(createdNews, news);

                        var newsResult = Services.ContentService.SaveAndPublish(createdNews);

                        if (newsResult.Success)
                        {
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            return response;
                        }
                        else
                        {
                            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                        }
                    }
                }
                else
                {
                    var content = Services.ContentService.GetById(existedNews);

                    bool imageSaved = false;
                    if (!string.IsNullOrEmpty(news.PictureUrl))
                    {
                        using (Stream stream = System.IO.File.OpenRead(news.PictureUrl))
                        {
                            content.SetValue(_contentTypeBaseServiceProvider, "picture", news.PictureName, stream);
                            imageSaved = true;
                        }
                    }
                    else if (!string.IsNullOrEmpty(news.PictureExternalUrl))
                    {
                        try
                        {
                            //TODO
                            // Image is Empty when run with await' upload image
                            //ImportFirstImageAsync(createdNews, news.PictureExternalUrl, news.PictureName);

                            DownloadImage(news.PictureExternalUrl, news.PictureName, content);
                            //createdNews.SetValue(_contentTypeBaseServiceProvider, "picture", news.PictureName, GetImage(news.PictureExternalUrl).Result);
                        }
                        catch (Exception exception)
                        {
                            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                        }
                    }

                    if (imageSaved)
                    {
                        //this is a work around for the SetValue method to save a file, since it doesn't currently take into account the image cropper
                        //which we are using so we need to fix that.
                        var propType = content.Properties["picture"].PropertyType;
                        var cropperValue = CreateImageCropperValue(propType, content.GetValue("picture"), _dataTypeService);
                        content.SetValue("picture", cropperValue);
                    }

                    //Update News
                    PopulateContentFields(content, news);

                    var newsResult = Services.ContentService.SaveAndPublish(content);

                    if (newsResult.Success)
                    {
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        return response;
                    }
                    else
                    {
                        response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = System.Net.HttpStatusCode.RequestTimeout;
            }

            return response;
        }

        public async Task<HttpResponseMessage> DeteleNews(NewsModel news)
        {
            IContent existedNews = Services.ContentService.GetById(news.Id);
            HttpResponseMessage response = new HttpResponseMessage();
            if (existedNews == null)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
            }
            else
            {
                Services.ContentService.Unpublish(existedNews);
                Services.ContentService.Delete(existedNews);
                response.StatusCode = System.Net.HttpStatusCode.OK;

            }
            return response;
        }

        public int GetNews(string title)
        {
            NewsSearchModel newsSearch = new NewsSearchModel();
            newsSearch.CurrentPage = 1;
            newsSearch.ItemsPerPage = 1;
            newsSearch.Keywords = title;
            var news = _searchHelper.Search(newsSearch);

            return news != null && news.News != null && news.News.Any() ? news.News.FirstOrDefault().Id : -1;
        }

        public string[] InsertListNews(IEnumerable<NewsModel> listNews)
        {
            List<string> results = new List<string>();
            List<IContent> newsContents = new List<IContent>();
            foreach (var news in listNews)
            {
                //Check if this News has existed or not
                var existedNews = _iNewsService.GetNewsByCategoryAndName(news.Category, news.Name);

                if (existedNews == null)
                {
                    //imageId = UploadImage(1115, filePath, news.PictureTitle, news.PictureName);

                    var category = _iCategoryService.GetCategory(news.Category);

                    try
                    {
                        //IMedia media = Services.MediaService.GetById(imageId);               
                        IContent createdNews = null;
                        if (category != null)
                        {
                            createdNews = Services.ContentService.Create(news.Name, category.Id, "news");
                        }
                        else
                        {
                            createdNews = Services.ContentService.Create(news.Name, news.ParentId, "news");
                        }

                        if (createdNews != null)
                        {
                            //TODO
                            //IList<string> categoryName = new List<string>();
                            //categoryName.Add(category.Name);
                            //createdNews.AssignTags("category", categoryName.ToArray());

                            //var categoryUdi = Udi.Create(Constants.UdiEntityType.Document, category.Key);
                            //var searchCategoryPage = Umbraco.PublishedContent(categoryUdi);
                            //createdNews.SetValue("category", searchCategoryPage);                   

                            //IList <Link> links = new List<Link>();
                            //links.Add(new Link()
                            //{
                            //    Name = "Test 1",
                            //    Type = LinkType.Content,
                            //    Udi = locaUdi,
                            //    Url = "/home/thoi-su/giao-thong/xe-qua-tai-giam-manh-tren-quoc-lo-5/"
                            //});

                            //TODO: this field will be set data later
                            //createdNews.SetValue("relatedNews", links);

                            //TODO: this field will be set data later
                            bool imageSaved = false;
                            if (!string.IsNullOrEmpty(news.PictureUrl))
                            {
                                using (Stream stream = System.IO.File.OpenRead(news.PictureUrl))
                                {
                                    createdNews.SetValue(_contentTypeBaseServiceProvider, "picture", news.PictureName, stream);
                                    imageSaved = true;
                                }
                            }
                            else if (!string.IsNullOrEmpty(news.PictureExternalUrl))
                            {
                                try
                                {
                                    //TODO
                                    DownloadImage(news.PictureExternalUrl, news.PictureName, createdNews);
                                    //createdNews.SetValue(_contentTypeBaseServiceProvider, "picture", news.PictureName, GetImage(news.PictureExternalUrl).Result);
                                }
                                catch (Exception exception)
                                {
                                    //return false;
                                }
                            }


                            if (imageSaved)
                            {
                                //this is a work around for the SetValue method to save a file, since it doesn't currently take into account the image cropper
                                //which we are using so we need to fix that.
                                var propType = createdNews.Properties["picture"].PropertyType;
                                var cropperValue = CreateImageCropperValue(propType, createdNews.GetValue("picture"), _dataTypeService);
                                createdNews.SetValue("picture", cropperValue);
                            }

                            //TOD
                            //createdNews.SetValue("thumbnailImage", imageId); //media.Path = -1,newsId,mediaId;

                            PopulateContentFields(createdNews, news);

                            newsContents.Add(createdNews);
                            var newsResult = Services.ContentService.SaveAndPublish(createdNews);

                            if (newsResult.Success)
                            {
                                results.Add(createdNews.Name + " status: " + true);
                            }
                            else
                            {
                                results.Add(createdNews.Name + " status: " + false);
                            }
                            //return true;

                            System.Threading.Thread.Sleep(20000); // wait to create file and save it to umbraco
                        }
                    }
                    catch (Exception ex)
                    {
                        // Error when create and save news
                    }
                }
                else
                {
                    //Update News
                    //Ignore update news to check the database lock issue - TODO
                    //var content = Services.ContentService.GetById(existedNews.Id);

                    //PopulateContentFields(content, news);

                    //newsContents.Add(content);

                    //var newsResult = Services.ContentService.SaveAndPublish(content);

                    //if (newsResult.Success)
                    //{
                    //    results.Add(existedNews.Name + " status: " + true);
                    //}
                    //else
                    //{
                    //    results.Add(existedNews.Name + " status: " + false);
                    //}
                    //return true;
                }
            }
            //if (newsContents.Any())
            //{
            //    var savedResults = Services.ContentService.Save(newsContents);

            //    if (savedResults.Success)
            //    {
            //        results.Add("Saved sucessfully - status = " + true);
            //    }
            //    else
            //    {
            //        results.Add("Saved unsucessfully - status =" + false);

            //    }
            //}

            return results.ToArray(); ;
        }

        public HttpResponseMessage DeleteNews(string title)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            //Check if this News has existed or not
            var existedNewsId = GetNews(title);

            if (existedNewsId > 0)
            {
                var content = Services.ContentService.GetById(existedNewsId);

                var newsResult = Services.ContentService.Delete(content);

                if (newsResult.Success)
                {
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    return response;
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
            }

            return response;
        }
        public string[] DeleteNewsByCategory(int id)
        {
            //Check if this News has existed or not
            var existedNews = _iNewsService.GetNewsByCategoryId(id);
            List<string> results = new List<string>();
            if (existedNews != null)
            {
                foreach (var news in existedNews)
                {
                    if (news.ContentType.Alias.Equals("category"))
                    {
                        continue;
                    }

                    var content = Services.ContentService.GetById(news.Id);

                    var newsResult = Services.ContentService.Delete(content);

                    if (newsResult.Success)
                    {
                        results.Add(news.Name + " status: " + true);
                    }
                    else
                    {
                        results.Add(news.Name + " status: " + false);
                    }
                }
            }

            return results.ToArray();
        }

        #region Helpers
        private static HttpClient Client { get; } = new HttpClient();
        public async Task<Stream> GetImage(string url)
        {
            var stream = await Client.GetStreamAsync(url);

            //var result = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StreamContent(stream),
            //};
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "README.md" };
            return stream;
        }

        private int UploadImage(int parentId, string imageSrc, string imageTitle, string imageFile)
        {
            try
            {
                // Open a new stream to the file
                using (Stream stream = System.IO.File.OpenRead(imageSrc))
                {

                    // Initialize a new image at the root of the media archive
                    IMedia media = Services.MediaService.CreateMedia(imageTitle, parentId, "Image");

                    // Set the property value (Umbraco will handle the underlying magic)
                    //media.SetValue(Services.ContentTypeBaseServices, "umbracoFile", imageFile, stream);

                    media.SetValue(_contentTypeBaseServiceProvider, "umbracoFile", imageFile, stream);

                    // Save the media
                    Services.MediaService.Save(media);

                    return media.Id;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return 0;
        }

        private void PopulateContentFields<T>(IContent content, T model) where T : BaseModel
        {
            content.Name = model.Name;
            var properties = typeof(T).GetProperties();
            foreach (var prop in content.Properties)
            {
                var property = properties.FirstOrDefault(p => string.Equals(p.Name, prop.Alias, StringComparison.InvariantCultureIgnoreCase));
                if (property == null || !property.CanRead) continue;

                if (string.Equals("category", prop.Alias, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var value = property.GetValue(model);
                content.SetValue(prop.Alias, value);
            }
        }
        private void PopulateContentFields(IContent content, NewsModel model)
        {
            content.Name = model.Name;

            content.SetValue("title", model.Title);
            content.SetValue("author", model.Author);
            content.SetValue("fullDescription", model.FullDescription);
            content.SetValue("postedDateTime", model.PostedDateTime);
            content.SetValue("shortDescription", model.ShortDescription);
            content.SetValue("sourceUrl", model.SourceUrl);
            content.SetValue("source", model.Source);

            content.SetValue("metaKeywords", model.MetaKeywords);
            content.SetValue("metaPageTitle", model.MetaPageTitle);
            content.SetValue("metaDescription", model.MetaDescription);

            if (model.IsHotGroupNews)
            {
                content.SetValue("IsHotGroupNews", model.IsHotGroupNews);
            }

            if (!string.IsNullOrEmpty(model.Picture))
            {
                content.SetValue("PictureExternalUrl", model.Picture);
            }

            content.CreateDate = Convert.ToDateTime(model.PostedDateTime);
        }

        //borrowed from CMS core until SetValue is fixed with a stream
        private string CreateImageCropperValue(PropertyType propertyType, object value, IDataTypeService dataTypeService)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return null;

            // if we don't have a json structure, we will get it from the property type
            var val = value.ToString();
            if (val.DetectIsJson())
                return val;

            var configuration = dataTypeService.GetDataType(propertyType.DataTypeId).ConfigurationAs<ImageCropperConfiguration>();
            var crops = configuration?.Crops ?? Array.Empty<ImageCropperConfiguration.Crop>();

            return JsonConvert.SerializeObject(new
            {
                src = val,
                crops = crops
            });
        }
        private async Task ImportFirstImageAsync(IContent news, string imageUrl, string imageName)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var stream = await client.GetStreamAsync(imageUrl))
                    {
                        news.SetValue(_contentTypeBaseServiceProvider, "picture", imageName, stream);
                    }
                }
            }
            catch (Exception exception)
            {
            }
        }
        private System.Drawing.Image DownloadImage(string imageUrl, string imageName, IContent news)
        {
            System.Drawing.Image image = null;

            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;
                webRequest.ServicePoint.ConnectionLeaseTimeout = 5000;
                webRequest.ServicePoint.MaxIdleTime = 5000;

                using (System.Net.WebResponse webResponse = webRequest.GetResponse())
                {

                    using (System.IO.Stream stream = webResponse.GetResponseStream())
                    {
                        news.SetValue(_contentTypeBaseServiceProvider, "picture", imageName, stream);
                        //image = System.Drawing.Image.FromStream(stream);
                    }
                }

                webRequest.ServicePoint.CloseConnectionGroup(webRequest.ConnectionGroupName);
                webRequest = null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);

            }

            return image;
        }

        #endregion
    }
}