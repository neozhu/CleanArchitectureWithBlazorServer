namespace MudDemo.Server.Models.Article;

public class ArticlePreviewModel
{
    public int ArticleId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public string DescriptionTrimed
    {
        get
        {
            var maxLength = 40;
    
            if (Description.Length > maxLength)
                return Description.Substring(0, maxLength) + "...";
    
            return Description;
        }
    }
}