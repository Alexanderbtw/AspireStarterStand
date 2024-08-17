using MailKit.Net.Smtp;

namespace MailDev.Client.MailKit;

public sealed class MailKitClientFactory(MailKitClientSettings settings) : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private SmtpClient? _client;

    public async Task<ISmtpClient> GetSmtpClientAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (_client is null)
            {
                _client = new SmtpClient();

                await _client.ConnectAsync(settings.Endpoint, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return _client;
    }

    public void Dispose()
    {
        _client?.Dispose();
        _semaphore.Dispose();
    }
}