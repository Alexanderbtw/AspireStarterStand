using MailKit.Net.Smtp;

namespace MailDev.Client.MailKit;

public sealed class MailKitClientFactory(
    MailKitClientSettings settings) : IDisposable
{
    private SmtpClient? _client;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(
        initialCount: 1,
        maxCount: 1);

    public void Dispose()
    {
        _client?.Dispose();
        _semaphore.Dispose();
    }

    public async Task<ISmtpClient> GetSmtpClientAsync(
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (_client is null)
            {
                _client = new SmtpClient();

                await _client
                    .ConnectAsync(
                        uri: settings.Endpoint,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return _client;
    }
}
