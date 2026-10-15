using System.Collections.Concurrent;

namespace NotificationService.Consumers;

public class ProcessedMessageStore
{
    private readonly ConcurrentDictionary<Guid, byte> _processed = new();

    public bool Contains(Guid messageId) => _processed.ContainsKey(messageId);

    public void Add(Guid messageId) => _processed.TryAdd(messageId, 0);
}