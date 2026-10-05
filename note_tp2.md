pixelhub> db.jeux.countDocuments()
10

pixelhub> db.jeux.findOne()
{
  _id: ObjectId('6ac39339a3c9a60033717d1a'),
  titre: 'Counter-Strike 2',
  genre: 'FPS',
  note: 4.5,
  anneeSortie: 2023,
  plateformes: [ 'PC' ],
  tags: [ 'compétitif', 'tir', 'équipe' ],
  joueursParEquipe: 5,
  cartes: [ 'Dust II', 'Mirage', 'Inferno', 'Nuke' ],
  classementCompetitif: true
}

## Q1 -

Le champ `_id` a été généré par `mongoimport` avant l’envoi du document à MongoDB, puis enregistré par le serveur. À l’inverse, l’`Id = 4` de Pixel a été choisi par PostgreSQL lors de l’insertion et nous ne l’avons connu qu’après.

pixelhub> db.jeux.find({ genre: "FPS" })
[
  {
    _id: ObjectId('6ac39339a3c9a60033717d1a'),
    titre: 'Counter-Strike 2',
    genre: 'FPS',
    note: 4.5,
    anneeSortie: 2023,
    plateformes: [ 'PC' ],
    tags: [ 'compétitif', 'tir', 'équipe' ],
    joueursParEquipe: 5,
    cartes: [ 'Dust II', 'Mirage', 'Inferno', 'Nuke' ],
    classementCompetitif: true
  },
  {
    _id: ObjectId('6ac39339a3c9a60033717d20'),
    titre: 'Valorant',
    genre: 'FPS',
    note: 4.1,
    anneeSortie: 2020,
    plateformes: [ 'PC' ],
    tags: [ 'compétitif', 'tir', 'héros' ],
    joueursParEquipe: 5,
    nombreAgents: 26,
    classementCompetitif: true
  }
]

pixelhub> db.jeux.find({ note: { $gte: 4.5 } }, { titre: 1, note: 1, _id: 0 })
[
  { titre: 'Counter-Strike 2', note: 4.5 },
  { titre: 'Mario Kart 8 Deluxe', note: 4.6 },
  { titre: 'Stardew Valley', note: 4.8 },
  { titre: 'Hades II', note: 4.7 },
  { titre: 'Terraria', note: 4.7 },
  { titre: "Baldur's Gate 3", note: 4.9 }
]

pixelhub> db.jeux.find({ plateformes: "Switch" }, { titre: 1, _id: 0 })
[
  { titre: 'Vampire Survivors' },
  { titre: 'Mario Kart 8 Deluxe' },
  { titre: 'Stardew Valley' },
  { titre: 'Hades II' },
  { titre: 'Rocket League' },
  { titre: 'Terraria' }
]

## Q2 -

SELECT j.titre 
FROM jeux j 
JOIN jeu_plateforme jp ON jp.jeu_id = j.id 
JOIN plateformes p ON p.id = jp.plateforme_id 
WHERE p.nom = 'Switch';

pixelhub> db.jeux.find({ tags: "coopératif" }, { titre: 1, _id: 0 })
[
  { titre: 'Stardew Valley' },
  { titre: 'Terraria' },
  { titre: "Baldur's Gate 3" }
]

pixelhub> db.jeux.find({ joueursParEquipe: { $exists: true } },
|              { titre: 1, joueursParEquipe: 1, _id: 0 })
[
  { titre: 'Counter-Strike 2', joueursParEquipe: 5 },
  { titre: 'Valorant', joueursParEquipe: 5 },
  { titre: 'Rocket League', joueursParEquipe: 3 }
]

## Q3 -

`--authenticationDatabase admin` indique à MongoDB que le compte `pixelhub` est défini dans la base système `admin`. Les données restent, elles, dans la base `pixelhub` : il faut donc préciser `admin` pour authentifier le compte avant d’accéder à `pixelhub`.

pixelhub> db.jeux.insertOne({
|   titre: "Un jeu de test",
|   genre: "Puzzle",
|   note: 3.5,
|   nombreNiveaux: 120,
|   champCompletementInvente: "ça marche quand même"
| })
{
  acknowledged: true,
  insertedId: ObjectId('6ac39c652a07b6933d67b146')
}

pixelhub> db.jeux.countDocuments()
12

## Q4 -

C’est une bonne nouvelle pour prototyper rapidement, mais un problème après six mois à quatre développeurs : chacun peut ajouter des champs ou faire des fautes, créant des documents incohérents et des requêtes difficiles à maintenir. Il faudrait donc définir une structure commune, ajouter une validation de schéma et utiliser des tests pour éviter la dérive des données.

pixelhub> db.Jeux.find({ genre: "FPS" })

## Q5 -

Jeux avec un J majuscule ça fonctionne pas, il faut que ce soit en minuscule

pixelhub> // 10. Corriger une note
| db.jeux.updateOne({ titre: "Valorant" }, { $set: { note: 4.3 } })
| db.jeux.find({ titre: "Valorant" }, { titre: 1, note: 1, _id: 0 })
|
| // 11. Ajouter un tag à un jeu (sans écraser les tags existants)
| db.jeux.updateOne({ titre: "Terraria" }, { $push: { tags: "sandbox" } })
|
| // 12. Ajouter un champ qui n'existait pas encore sur ce document
| db.jeux.updateOne({ titre: "Terraria" }, { $set: { nbVotes: 0 } })
|
| // 13. Incrémenter un compteur
| db.jeux.updateOne({ titre: "Terraria" }, { $inc: { nbVotes: 1 } })
| db.jeux.updateOne({ titre: "Terraria" }, { $inc: { nbVotes: 1 } })
|
| // 14. Supprimer un champ
| db.jeux.updateOne({ titre: "Terraria" }, { $unset: { nbVotes: "" } })
|
| // 15. Marquer tous les FPS d'un coup
| db.jeux.updateMany({ genre: "FPS" }, { $set: { competitif: true } })
{
  acknowledged: true,
  insertedId: null,
  matchedCount: 2,
  modifiedCount: 2,
  upsertedCount: 0
}

pixelhub> db.jeux.updateOne({ titre: "Valorant" }, { note: 4.4 })
MongoInvalidArgumentError: Update document requires atomic operators

## Q6 -

MongoDB refuse car `updateOne` attend un opérateur atomique comme `$set`, et `{ note: 4.4 }` n'en contient aucun. Pour remplacer tout le document, il faudrait utiliser `db.jeux.replaceOne({ titre: "Valorant" }, { titre: "Valorant", genre: "FPS", note: 4.4, anneeSortie: 2020, plateformes: ["PC"], tags: ["compétitif", "tir", "héros"], joueursParEquipe: 5, nombreAgents: 26, classementCompetitif: true })`.

## Q7 -

En SQL, il faudrait faire `ALTER TABLE jeux ADD COLUMN nbVotes INTEGER;`. La colonne serait ajoutée à toutes les lignes, avec la valeur `NULL` pour les dix autres jeux ; il faudrait ensuite faire un `UPDATE` pour donner `0` à Terraria.

## Q8 -

Le mot « Mongo » n'apparaît jamais dans `IGameCatalog` : l'interface décrit seulement les opérations du catalogue. C'est important car on peut remplacer MongoDB par une autre base ou un faux dépôt de test sans modifier le code qui utilise cette interface.

## Q9 -

L'appel renvoie `HTTP/1.1 500 Internal Server Error` avec : `System.FormatException: Element 'joueursParEquipe' does not match any field or property of class PixelHub.Api.Models.Game.` Le driver a essayé de désérialiser le document BSON en objet `Game`, mais cette propriété n'existe pas dans le modèle C# ; les champs `cartes` et `classementCompetitif` poseraient aussi problème.

## Q10 -

`[BsonIgnoreExtraElements]` évite l'erreur, mais `joueursParEquipe` est ignoré, il n'est donc pas présent dans l'objet `Game` ni dans la réponse de `/games`. Le champ reste toutefois conservé dans le document MongoDB.

## Q11 -

`[BsonIgnoreExtraElements]` est simple et tolérant, mais les champs inconnus sont perdus dans l'objet C#. `[BsonExtraElements]` conserve la souplesse et les données variables, mais elles sont moins typées et nécessitent une conversion pour le JSON. Une hiérarchie comme `FpsGame : Game` offre le meilleur typage métier et une API claire, mais elle demande davantage de classes et devient plus rigide quand les variantes se multiplient.

## Q12 -

En SQL, on écrirait : 

SELECT genre, AVG(note) AS note_moyenne, COUNT(*) AS nombre 
FROM jeux 
GROUP BY genre 
ORDER BY note_moyenne DESC; 

C'est l'équivalent de l'agrégation MongoDB et cela ne constitue pas un avantage particulier de MongoDB. Son intérêt dans ce TP est plutôt la souplesse du schéma documentaire : tableaux, champs optionnels et structures différentes selon les jeux, sans modifier une table ni multiplier les jointures.

