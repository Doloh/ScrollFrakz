## Player datas / Scriptable object
Toutes les variables "physiques" qui d�finissent le player sont d�finies dans un ScriptableObject (une fichier de donn�e).
Ce script object est un sch�ma de donn�es (on d�finit les types, si ils sont �ditables dans unity, on peut cr�er des variables computed ... 
on ne cr�e pas les valeurs des donn�es)
On peut cr�er ce que j'ai envie d'appeler un "data set" qui est une it�ration de ce scriptable object, avec un lot de donn�e d�finies.
> Clique droit dans le l'onglet du projet dans unity, create, player data. L� on peut renseigner toutes les valeurs qu'on a rendu modifiables.
Apr�s on cr�e une variable PlayerData dans notre script 
```
public PlayerData Data;
```
Ensuite il suffit d'attacher le scriptable object dans cette variable PlayerData de notre Player dans unity.