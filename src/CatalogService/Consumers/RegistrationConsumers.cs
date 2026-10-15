using CatalogService.Data;
using CatalogService.Models;
using MassTransit;
using Shared.Contracts.Sagas;

namespace CatalogService.Consumers;

public class CreateProfileConsumer(ClientRepository repo) : IConsumer<CreateProfile>
{
    public async Task Consume(ConsumeContext<CreateProfile> ctx)
    {
        var m = ctx.Message;
        var client = repo.Add(new Client
        {
            FullName = m.FullName, Email = m.Email, Phone = m.Phone, City = m.City
        });
        await ctx.Publish(new ProfileCreated(m.RegistrationId, client.Id));
    }
}

public class ActivateProfileConsumer : IConsumer<ActivateProfile>
{
    public async Task Consume(ConsumeContext<ActivateProfile> ctx)
    {
        var m = ctx.Message;

        // навмисна відмова для демонстрації компенсації
        if (m.Email.EndsWith("@fail.test", StringComparison.OrdinalIgnoreCase))
        {
            await ctx.Publish(new ProfileActivationFailed(m.RegistrationId, "тестова відмова активації"));
            return;
        }

        await ctx.Publish(new ProfileActivated(m.RegistrationId));
    }
}

public class DeleteProfileConsumer(ClientRepository repo) : IConsumer<DeleteProfile>
{
    public Task Consume(ConsumeContext<DeleteProfile> ctx)
    {
        // компенсація: видаляємо профіль, створений першим кроком
        repo.Delete(ctx.Message.ClientId);
        return Task.CompletedTask;
    }
}