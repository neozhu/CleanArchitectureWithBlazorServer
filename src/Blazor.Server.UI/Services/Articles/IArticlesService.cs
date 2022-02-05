using MudDemo.Server.Models.Article;

namespace MudDemo.Server.Services;

public interface IArticlesService
{
    Task<IEnumerable<ArticlePreviewModel>> GetArticles();
}