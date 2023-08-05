using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace RadioNet.Utilities;

public class JSInterop : IJSObjectReference, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public JSInterop(IJSRuntime jsRuntime, string jsFilePath)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", jsFilePath).AsTask());
    }

    public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, params object[] args)
    {
        IJSObjectReference module = await _moduleTask.Value;

        return await module.InvokeAsync<TValue>(identifier, args);
    }

    public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, params object[] args)
    {
        IJSObjectReference module = await _moduleTask.Value;

        return await module.InvokeAsync<TValue>(identifier, cancellationToken, args);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            IJSObjectReference module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}