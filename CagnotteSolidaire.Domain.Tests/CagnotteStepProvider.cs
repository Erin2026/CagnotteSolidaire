using CagnotteSolidaire.Domain.Commands.Cagnottes;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Queries.Cagnottes;
using CagnotteSolidaire.Domain.Tests.Mocks;
using Xunit;

namespace CagnotteSolidaire.Domain.Tests.StepProviders;
internal class CagnotteStepProvider
{

    public static Cagnotte[] Fixture =>
    [
        new Cagnotte(1, "Aide aux enfants", "Pour aider les enfants en difficulté", 5000m, 1, ""),
        new Cagnotte(2, "Projet sportif", "Financement équipement sportif", 3000m, 1, ""),
        new Cagnotte(3, "Événement culturel", "Organisation festival de musique", 10000m, 2, ""),
        new Cagnotte(4, "Rénovation locale", "Rénovation salle des fêtes", 15000m, 2, ""),
        new Cagnotte(5, "Aide alimentaire", "Distribution de repas", 2000m, 3, ""),
        new Cagnotte(6, "Formation professionnelle", "Ateliers de formation", 8000m, 3, ""),
        new Cagnotte(7, "Protection environnement", "Nettoyage des rivières", 4500m, 1, ""),
        new Cagnotte(8, "Bibliothèque mobile", "Achat de livres et véhicule", 12000m, 2, ""),
        new Cagnotte(9, "Jardin partagé", "Création espace vert communautaire", 6000m, 3, ""),
        new Cagnotte(10, "Aide aux seniors", "Services à domicile pour seniors", 7000m, 1, "")
    ];

    // Repositories mocks
    private CagnotteRepositoryMock _cagnotteRepo = new([], DateTime.MinValue);

    // Données de test
    private int _currentCagnotteId;
    private Cagnotte? _currentCagnotte;
    private ApplicationException? _lastException;
    private List<CagnottesDTO>? _cagnottesQueryResult;
    private CagnotteDTO? _cagnotteDetailResult;

    private CreateCagnotteCommand? _createCagnotteCommand;
    private int _createCagnotteCommandResult;

    private ApplicationException? _exception;

    #region Given

    internal CagnotteStepProvider GivenExistingCagnottes()
    {
        _cagnotteRepo = new(Fixture, DateTime.MinValue);
        return this;
    }

    #endregion

    #region When

    internal CagnotteStepProvider WhenWeCreateCagnotte(
        string nom,
        string description,
        decimal objectif,
        int gestionnaireId,
        string? imageUrl = null)
    {
        _createCagnotteCommand = new CreateCagnotteCommand(nom, description, objectif, gestionnaireId, imageUrl);

        try
        {
            _createCagnotteCommandResult = new CreateCagnotteCommandHandler(_cagnotteRepo).Handle(_createCagnotteCommand).Result;
        }
        catch (ApplicationException exception) {

            _exception = exception;
        }
        return this;
        
    }

    internal CagnotteStepProvider WhenWeCloseCagnotte(int cagnotteId)
    {
        var command = new CloturerCagnotteCommand(cagnotteId);

        try
        {
           new CloturerCagnotteCommandHandler(_cagnotteRepo).Handle(command).GetAwaiter().GetResult();
           
        }
        catch (ApplicationException exception)
        {
            _lastException = exception;
        }

        return this;
    }

    internal CagnotteStepProvider WhenWeCancelCagnotte(int cagnotteId)
    {
        var command = new AnnulerCagnotteCommand(cagnotteId);
        try
        {
            new AnnulerCagnotteCommandHandler(_cagnotteRepo).Handle(command).GetAwaiter().GetResult();
        }
        catch (ApplicationException exception)
        {
            _exception = exception;
        }

        return this;
    }

    internal CagnotteStepProvider WhenWeGetCagnottesForGestionnaire(int gestionnaireId)
    {
        var handler = new GetCagnottesGestionnaireQueryHandler(_cagnotteRepo);
        var query = new GetCagnottesGestionnaireQuery(gestionnaireId);

        _cagnottesQueryResult = handler.Handle(query, CancellationToken.None).Result;

        return this;
    }

    internal CagnotteStepProvider WhenWeGetCagnotteDetails(int cagnotteId)
    {
        var handler = new GetCagnotteQueryHandler(_cagnotteRepo);
        var query = new GetCagnotteQuery(cagnotteId);

        _cagnotteDetailResult = handler.Handle(query, CancellationToken.None).Result;

        return this;
    }

    #endregion

    #region then

    internal CagnotteStepProvider ThenCagnotteIsCreated()
    {
        Assert.NotNull(_currentCagnotte);
        Assert.Null(_lastException);
        Assert.Equal(StatutCagnotte.Ouverte, _currentCagnotte.Statut);

        return this;
    }

    internal CagnotteStepProvider ThenCagnotteIsNotCreated(String message)
    {
        Assert.NotNull(_createCagnotteCommand);
        Assert.NotNull(_exception);
        Assert.Equal(message, _exception.Message);;

        return this;
    }

    internal CagnotteStepProvider ThenCagnotteIsClosed()
    {
        Assert.NotNull(_currentCagnotte);
        Assert.Equal(StatutCagnotte.Cloturee, _currentCagnotte.Statut);
        Assert.Null(_lastException);

        return this;
    }

    internal CagnotteStepProvider ThenCloseShouldFail(string expectedMessagePart)
    {
        Assert.NotNull(_lastException);
        Assert.Contains(expectedMessagePart, _lastException.Message);

        return this;
    }

    /// <summary>
    /// Assertion : La cagnotte est annulée
    /// </summary>
    internal CagnotteStepProvider ThenCagnotteIsCancelled()
    {
        Assert.NotNull(_currentCagnotte);
        Assert.Equal(StatutCagnotte.Annulee, _currentCagnotte.Statut);
        Assert.Null(_lastException);

        return this;
    }

    internal CagnotteStepProvider ThenMultipleCagnottesAreReturned(int expectedCount)
    {
        Assert.NotNull(_cagnottesQueryResult);
        Assert.Equal(expectedCount, _cagnottesQueryResult.Count);

        return this;
    }

    internal CagnotteStepProvider ThenCagnotteDetailsAreReturned()
    {
        Assert.NotNull(_cagnotteDetailResult);
        Assert.True(_cagnotteDetailResult.Id > 0);

        return this;
    }
    #endregion
}