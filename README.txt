INFO IMPORTANTE :
Le projet à été réaliser sur Visual studio 2026 : Nous n'étions pas au courant mais le fichier .slnx n'est ouvrable que sur la version 2026
on a essayer sur les anciennes et on a beaucoup de problème donc si ça peut aider lors de l'inspection de notre travail la version 2026 est recommandé

Le projet tourne sur .NET 10 SDK

Example pour l'inscription gestionnaire :

avec RNA : ASS02200
avec Nom : "ASSOCIATION SYNDICALE DE LA RESIDENCE LE 4 DES CHAMPS DE WIHR"

Nous avons eu quelque problème avec l'api du journal puisque beaucoup d'association 
n'ont pas leurs siren remplie l'option est disponible mais elle n'a pas été testé faute d'exemple qui fonctionne


le swagger est disponible ici :
http://localhost:5163/swagger/

Le site normalement ici : http://localhost:5025/

Pour supprimer et recréer complètement la base :

Dans CagnotteSolidaire.API ->
 Remove-Item -Path "*.db*" -Force
Dans CagnotteSolidaire.Infrastructure ->
dotnet ef database update --startup-project ..\CagnotteSolidaire.API




Pour lancer aller dans 
Dans CagnotteSolidaire.API ->
 dotnet clean
 dotnet build
 dotnet run
 
Dans CagnotteSolidaire.Web ->
 dotnet clean
 dotnet build
 dotnet run
 
Pour executer les tests :
Dans CagnotteSolidaire.Application.Tests ->
  dotnet test
 
Les scripts de migration sont disponible dans CagnotteSolidaire.Infrastructure.Migrations 
 
 Point à amélioré : 
 -Plusieurs Inscription peuvent être effectué sur le même Nom d'association c'est un peu incohérent si il y a un seul gestionnaire 
 mais si ils y en a plusieurs ça fonctionne.
 -On a eu des soucis lorsque le Gestionnaire est identifié et qu'il veut contribuer a sa propre cagnotte (ça peut être cohérent mais un peu bizarre) 
 donc on a préféré mettre une condition ou lorsque le gestionnaire est connecté il ne peut pas contribué
 -On a utilisé SQLite au lieu de SQL Server préconisé dans le sujet, on l'a fait sans faire attention mais on aurait pu rebasculer sur SQL Server
 (ducoup on a utiliser DBBrowser pour inspecter la base de donnée plus facilement)
 