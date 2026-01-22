using CagnotteSolidaire.Domain.Commands.Cagnottes;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Queries.Cagnottes;
using CagnotteSolidaire.Domain.Tests.Mocks;
using Xunit;

namespace CagnotteSolidaire.Domain.Tests.StepProviders;

/// <summary>
/// StepProvider pour les tests de Cagnotte.
/// Suit le pattern Given/When/Then (GWT).
/// </summary>
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
    private ParticipationRepositoryMock _participationRepo = new();
    private UtilisateurRepositoryMock _utilisateurRepo = new();

    // Données de test
    private int _currentCagnotteId;
    private Cagnotte? _currentCagnotte;
    private ApplicationException? _lastException;
    private List<CagnottesDTO>? _cagnottesQueryResult;
    private CagnotteDTO? _cagnotteDetailResult;

    private CreateCagnotteCommand? _createCagnotteCommand;
    private int _createCagnotteCommandResult;

    private ParticiperCagnotteCommand? _createParticipationCommand;
    private int? _createParticipationCommandResult;

    private ApplicationException? _exception;

    #region Given

    /// <summary>
    /// GIVEN : Des cagnottes existantes (utilise le Fixture)
    /// EXACTEMENT COMME LibRator !
    /// </summary>
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
        var command = new CreateCagnotteCommand(nom, description, objectif, gestionnaireId, imageUrl);

        try
        {
            _createCagnotteCommandResult = new CreateCagnotteCommandHandler(_cagnotteRepo).Handle(_createCagnotteCommand).Result;
        }
        catch (ApplicationException exception) {

            _exception = exception;
        }
        return this;
        
    }

    internal CagnotteStepProvider WhenWeParticipateToCagnotte(int cagnotteId, int participantId, decimal montant)
    {
            _createParticipationCommand = new(cagnotteId, participantId,  montant);
            var handler = new ParticiperCagnotteCommandHandler(_cagnotteRepo, _participationRepo);

            // Recharger la cagnotte
            _currentCagnotte = _cagnotteRepo.GetCagnotteById(_currentCagnotteId);
            _lastException = null;
        try
        {
            _createParticipationCommandResult = handler.Handle(_createParticipationCommand).Result;
        }
        catch (AggregateException ex) when (ex.InnerException is ApplicationException appEx)
        {
            _lastException = appEx;
        }
        catch (ApplicationException ex)
        {
            _lastException = ex;
        }

        return this;
    }

    /// <summary>
    /// Action : Clôturer la cagnotte courante
    /// </summary>
    internal CagnotteStepProvider WhenWeCloseCagnotte()
    {
        try
        {
            var handler = new CloturerCagnotteCommandHandler(_cagnotteRepo);
            var command = new CloturerCagnotteCommand(_currentCagnotteId);

            handler.Handle(command, CancellationToken.None).Wait();

            // Recharger la cagnotte
            _currentCagnotte = _cagnotteRepo.GetCagnotteById(_currentCagnotteId);
            _lastException = null;
        }
        catch (AggregateException ex) when (ex.InnerException is ApplicationException appEx)
        {
            _lastException = appEx;
        }
        catch (ApplicationException ex)
        {
            _lastException = ex;
        }

        return this;
    }

    /// <summary>
    /// Action : Annuler la cagnotte courante
    /// </summary>
    internal CagnotteStepProvider WhenWeCancelCagnotte()
    {
        try
        {
            var handler = new AnnulerCagnotteCommandHandler(_cagnotteRepo);
            var command = new AnnulerCagnotteCommand(_currentCagnotteId);

            handler.Handle(command, CancellationToken.None).Wait();

            // Recharger la cagnotte
            _currentCagnotte = _cagnotteRepo.GetCagnotteById(_currentCagnotteId);
            _lastException = null;
        }
        catch (AggregateException ex) when (ex.InnerException is ApplicationException appEx)
        {
            _lastException = appEx;
        }
        catch (ApplicationException ex)
        {
            _lastException = ex;
        }

        return this;
    }

    /// <summary>
    /// Action : Récupérer les cagnottes d'un gestionnaire
    /// </summary>
    internal CagnotteStepProvider WhenWeGetCagnottesForGestionnaire(int gestionnaireId)
    {
        var handler = new GetCagnottesGestionnaireQueryHandler(_cagnotteRepo);
        var query = new GetCagnottesGestionnaireQuery(gestionnaireId);

        _cagnottesQueryResult = handler.Handle(query, CancellationToken.None).Result;

        return this;
    }

    /// <summary>
    /// Action : Récupérer le détail d'une cagnotte
    /// </summary>
    internal CagnotteStepProvider WhenWeGetCagnotteDetails(int cagnotteId)
    {
        var handler = new GetCagnotteQueryHandler(_cagnotteRepo);
        var query = new GetCagnotteQuery(cagnotteId);

        _cagnotteDetailResult = handler.Handle(query, CancellationToken.None).Result;

        return this;
    }

    // ============================================
    // THEN (Assert - Vérifier les résultats)
    // ============================================

    /// <summary>
    /// Assertion : La cagnotte a bien été créée
    /// </summary>
    internal CagnotteStepProvider ThenCagnotteIsCreated()
    {
        Assert.True(_currentCagnotteId > 0, "La cagnotte devrait avoir un ID > 0");
        Assert.NotNull(_currentCagnotte);
        Assert.Null(_lastException);
        Assert.Equal(StatutCagnotte.Ouverte, _currentCagnotte.Statut);

        return this;
    }

    /// <summary>
    /// Assertion : La création de la cagnotte a échoué avec un message spécifique
    /// </summary>
    internal CagnotteStepProvider ThenCagnotteCreationFailed(string expectedMessagePart)
    {
        Assert.NotNull(_lastException);
        Assert.Contains(expectedMessagePart, _lastException.Message);

        return this;
    }

    /// <summary>
    /// Assertion : La cagnotte est clôturée
    /// </summary>
    internal CagnotteStepProvider ThenCagnotteIsClosed()
    {
        Assert.NotNull(_currentCagnotte);
        Assert.Equal(StatutCagnotte.Cloturee, _currentCagnotte.Statut);
        Assert.Null(_lastException);

        return this;
    }

    /// <summary>
    /// Assertion : La clôture a échoué
    /// </summary>
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

    /// <summary>
    /// Assertion : La participation a été ajoutée
    /// </summary>
    internal CagnotteStepProvider ThenParticipationIsAdded(decimal expectedAmount)
    {
        Assert.NotNull(_currentCagnotte);
        Assert.True(_currentCagnotte.Participations.Count > 0);
        Assert.Equal(expectedAmount, _currentCagnotte.MontantTotal);
        Assert.Null(_lastException);

        return this;
    }

    /// <summary>
    /// Assertion : La participation a échoué
    /// </summary>
    internal CagnotteStepProvider ThenParticipationFailed(string expectedMessagePart)
    {
        Assert.NotNull(_lastException);
        Assert.Contains(expectedMessagePart, _lastException.Message);

        return this;
    }

    /// <summary>
    /// Assertion : La query retourne plusieurs cagnottes
    /// </summary>
    internal CagnotteStepProvider ThenMultipleCagnottesAreReturned(int expectedCount)
    {
        Assert.NotNull(_cagnottesQueryResult);
        Assert.Equal(expectedCount, _cagnottesQueryResult.Count);

        return this;
    }

    /// <summary>
    /// Assertion : Le détail de la cagnotte est retourné
    /// </summary>
    internal CagnotteStepProvider ThenCagnotteDetailsAreReturned()
    {
        Assert.NotNull(_cagnotteDetailResult);
        Assert.True(_cagnotteDetailResult.Id > 0);

        return this;
    }
}