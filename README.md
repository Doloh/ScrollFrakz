## Player datas / Scriptable object
Toutes les variables "physiques" qui définissent le player sont définies dans un ScriptableObject (une fichier de donnée).
Ce script object est un schéma de données (on définit les types, si ils sont éditables dans unity, on peut créer des variables computed ... 
on ne crée pas les valeurs des données)
On peut créer ce que j'ai envie d'appeler un "data set" qui est une itération de ce scriptable object, avec un lot de donnée définies.
> Clique droit dans le l'onglet du projet dans unity, create, player data. Là on peut renseigner toutes les valeurs qu'on a rendu modifiables.
Après on crée une variable PlayerData dans notre script 
```
public PlayerData Data;
```
Ensuite il suffit d'attacher le scriptable object dans cette variable PlayerData de notre Player dans unity.