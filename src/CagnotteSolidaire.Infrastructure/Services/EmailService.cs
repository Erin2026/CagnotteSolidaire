using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CagnotteSolidaire.Infrastructure.Services;


public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendCagnotteClosedEmailAsync(Cagnotte cagnotte)
    {
        // TODO: Implémenter l'envoi réel d'emails (SendGrid, SMTP, etc.)
        // Pour l'instant, on simule avec des logs

        _logger.LogInformation(
            "?? EMAIL - Cagnotte clôturée : {CagnotteNom} (ID: {CagnotteId})",
            cagnotte.Nom,
            cagnotte.Id);

        if (cagnotte.Participations == null || !cagnotte.Participations.Any())
        {
            _logger.LogWarning("Aucun participant à notifier pour la cagnotte {CagnotteId}", cagnotte.Id);
            return;
        }

        foreach (var participation in cagnotte.Participations)
        {
            var participant = participation.Participant;
            if (participant == null) continue;

            _logger.LogInformation(
                "  ? Envoi email à {Email} ({NomComplet}) - Montant promis: {Montant}€",
                participant.Email,
                participant.NomComplet,
                participation.Montant);

            // Simulation d'envoi d'email
            var emailContent = $@"
Bonjour {participant.NomComplet},

La cagnotte ""{cagnotte.Nom}"" a atteint son objectif de {cagnotte.ObjectifFinancier}€ !

Montant collecté : {cagnotte.MontantCollecte}€
Votre participation : {participation.Montant}€

Merci d'honorer votre promesse de don en contactant l'association.

Cordialement,
L'équipe Cagnotte Solidaire
            ";

            _logger.LogDebug("Contenu de l'email : {EmailContent}", emailContent);
        }

        await Task.CompletedTask;
    }

    public async Task SendCagnotteCancelledEmailAsync(Cagnotte cagnotte)
    {
        _logger.LogInformation(
            "?? EMAIL - Cagnotte annulée : {CagnotteNom} (ID: {CagnotteId})",
            cagnotte.Nom,
            cagnotte.Id);

        if (cagnotte.Participations == null || !cagnotte.Participations.Any())
        {
            _logger.LogWarning("Aucun participant à notifier pour la cagnotte {CagnotteId}", cagnotte.Id);
            return;
        }

        foreach (var participation in cagnotte.Participations)
        {
            var participant = participation.Participant;
            if (participant == null) continue;

            _logger.LogInformation(
                "  ? Envoi email à {Email} ({NomComplet})",
                participant.Email,
                participant.NomComplet);

            var emailContent = $@"
Bonjour {participant.NomComplet},

La cagnotte ""{cagnotte.Nom}"" a été annulée car l'objectif de {cagnotte.ObjectifFinancier}€ n'a pas été atteint.

Montant collecté : {cagnotte.MontantCollecte}€
Votre participation : {participation.Montant}€

Aucun don ne sera collecté. Merci pour votre soutien !

Cordialement,
L'équipe Cagnotte Solidaire
            ";

            _logger.LogDebug("Contenu de l'email : {EmailContent}", emailContent);
        }

        await Task.CompletedTask;
    }
}
