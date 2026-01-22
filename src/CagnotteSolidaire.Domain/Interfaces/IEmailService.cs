using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Interfaces;

public interface IEmailService
{
    Task SendCagnotteClosedEmailAsync(Cagnotte cagnotte);
    Task SendCagnotteCancelledEmailAsync(Cagnotte cagnotte);
}

