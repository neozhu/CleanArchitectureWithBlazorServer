using MudDemo.Server.Models.Article;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
namespace MudDemo.Server.Services;

public class ArticlesService : IArticlesService
{
    private const string UriRequest = "sample-data/articles.json";
    private readonly IWebHostEnvironment _environment;

    public ArticlesService(
        IWebHostEnvironment  environment
        )
    {
        _environment = environment;
    }

    public async Task<IEnumerable<ArticlePreviewModel>> GetArticles()
    {
        var jsonstring =File.ReadAllText(Path.Combine(_environment.WebRootPath,UriRequest));
        var articles = JsonSerializer.Deserialize<IEnumerable<ArticlePreviewModel>>(jsonstring, new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        });
        return await Task.FromResult(articles) ?? throw new InvalidOperationException();
    }
}