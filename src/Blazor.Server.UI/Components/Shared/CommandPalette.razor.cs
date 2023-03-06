using Toolbelt.Blazor.HotKeys2;

namespace Blazor.Server.UI.Components.Shared;

public partial class CommandPalette : IDisposable
{
    private readonly Dictionary<string, string> _pages = new();
    private HotKeysContext? _hotKeysContext;
    private Dictionary<string, string> _pagesFiltered = new();
    private string _search=String.Empty;
    [Inject] private HotKeys HotKeys { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;

    public void Dispose()
    {
        _hotKeysContext?.Dispose();
    }

    protected override void OnInitialized()
    {
        _pages.Add("App", "/");

        _pagesFiltered = _pages;

        _hotKeysContext = HotKeys.CreateContext()
            .Add(Key.Escape, () => MudDialog.Close(), "Close command palette.");
    }

    private void SearchPages(string value)
    {
        _pagesFiltered = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(value))
            _pagesFiltered = _pages
                .Where(x => x.Key
                    .Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .ToDictionary(x => x.Key, x => x.Value);
        else
            _pagesFiltered = _pages;
    }
}