## Q1 -

Les quatre services sont déclarés pour préparer les différentes séances du TP : PostgreSQL, MongoDB, Redis et Neo4j. Aujourd’hui, seul PostgreSQL est utilisé, mais les autres seront nécessaires pour les séances suivantes.

## Q2 -

Redis est le seul service sans section volumes. Ses données sont donc stockées uniquement dans le conteneur et peuvent être perdues lorsque celui-ci est supprimé ou recréé. C’est cohérent avec son rôle de cache, de classement ou de matchmaking temporaire.

## Q3 -

15432 : port exposé sur la machine hôte;
5432 : port utilisé par PostgreSQL dans le conteneur.
On se connecte depuis la machine avec localhost:15432, tandis que PostgreSQL écoute toujours sur 5432 à l’intérieur du conteneur.

## Q4 -

Pour un TP en local, ces mots de passe peuvent être acceptables car ils servent uniquement à un environnement de développement contrôlé. Sur un serveur de production, ce serait mauvais, car les secrets doivent être stockés de manière sécurisées et non pas en clair dans le code.

## Q5 -

"docker compose exec" permet d’exécuter une commande dans un conteneur déjà démarré par Docker Compose.

redis -> désigne le service ou conteneur cible ;
redis-cli ping -> est la commande exécutée.

La commande redis-cli s’exécute donc à l’intérieur du conteneur Redis, et non directement sur la machine hôte.

## Q6 -

Cela fonctionne car l'API est lancée directement sur la machine avec "dotnet run". Le port 15432 de la machine est redirigé vers le port 5432 de PostgreSQL dans le conteneur. localhost:15432 permet donc à l'API d'accéder à la base Docker. Si l'API était elle-même dans Docker, il faudrait utiliser "Host=postgres;Port=5432".

## Q7 - 

Non, les trois joueurs ne sont pas dupliqués. Les joueurs sont ajoutés uniquement si la table est vide. 

