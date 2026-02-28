using Microsoft.Agents.AI;

namespace CleanArchitecture.Blazor.Server.UI.Services.AI;

public sealed class ProjectIntroductionContextProvider : AIContextProvider
{
    private readonly string _projectIntroduction;

    public ProjectIntroductionContextProvider(string projectIntroduction)
    {
        _projectIntroduction = projectIntroduction;
    }

    protected override ValueTask<AIContext> ProvideAIContextAsync(
        InvokingContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_projectIntroduction))
        {
            return ValueTask.FromResult(new AIContext());
        }

        return ValueTask.FromResult(new AIContext
        {
            Instructions =
                """
                You are assisting inside the CleanArchitecture.Blazor project.
                Use the following project introduction as authoritative background context for your answer:
                """ + Environment.NewLine + _projectIntroduction
        });
    }
}
