using CagnotteSolidaire.Domain.Tests.StepProviders;
using Xunit;

public class CagnotteTests
{
    private readonly CagnotteStepProvider _steps = new();

    [Theory]
    [InlineData("Cagnotte1", "Description 1", 100, 1)]
    [InlineData("Cagnotte2", "Description 2", 500, 2)]
    public void Une_cagnotte_peut_etre_creee(string nom, string description, decimal objectif, int gestionnaireId)
        => _steps
            .GivenDesCagnottesExistantes()
            .WhenNousCreonsUneCagnotte(nom, description, objectif, gestionnaireId)
            .ThenLaCagnotteEstCreee();
}
