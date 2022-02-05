using Blazor.Server.UI.Models.Article;

namespace Blazor.Server.UI.Services;

public interface IArticlesService
{
    Task<IEnumerable<ArticlePreviewModel>> GetArticles();
}