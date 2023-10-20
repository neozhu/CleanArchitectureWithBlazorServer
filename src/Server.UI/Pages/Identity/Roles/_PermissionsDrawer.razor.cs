namespace CleanArchitecture.Blazor.Server.UI.Pages.Identity.Roles;
public partial class _PermissionsDrawer
{

    [EditorRequired]
    [Parameter]
    public List<PermissionModel> Permissions { get; set; } = new()!;

    [EditorRequired]
    [Parameter]
    public bool Open { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public EventCallback<PermissionModel> OnAssignChanged { get; set; }

    [EditorRequired]
    [Parameter]
    public EventCallback<List<PermissionModel>> OnAssignAllChanged { get; set; }

    [EditorRequired]
    [Parameter]
    public EventCallback<bool> OnOpenChanged { get; set; }

    [Parameter]
    public bool Waiting { get; set; }

    private string Keyword { get; set; } = string.Empty;

    private async Task OnAssignAll(string? groupName)
    {
        var list = new List<PermissionModel>();
        foreach (var t in Permissions.Where(t => t.Group == groupName))
        {
            t.Assigned = !t.Assigned;
            list.Add(t);
        }
        await OnAssignAllChanged.InvokeAsync(list);
    }

}