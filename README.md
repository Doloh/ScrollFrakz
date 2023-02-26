## Player datas / Scriptable object
Toutes les variables "physiques" qui définissent le player sont définies dans un ScriptableObject (une fichier de donnée).
Ce script object est un schéma de données (on définit les types, si ils sont éditables dans unity, on peut créer des variables computed ... 
on ne crée pas les valeurs des données)
On peut créer ce que j'ai envie d'appeler un "data set" qui est une itération de ce scriptable object, avec un lot de donnée définies.
> Clique droit dans le l'onglet du projet dans unity, create, player data. Lé on peut renseigner toutes les valeurs qu'on a rendu modifiables.
Après on crée une variable PlayerData dans notre script 
```
public PlayerData Data;
```
Ensuite il suffit d'attacher le scriptable object dans cette variable PlayerData de notre Player dans unity.

## Gestion de la direction
On init une value IsLookingRight à true, à chaque fois qu'on a un input en X on va check si on doit changer de direction.


## Physique
### Le principe d'une force
F = m * a = m * DeltaV/DeltaT
On applique des forces à chaque frame.
Une force appliqué à un instant t = une vitesse ajoutée à la vitesse à laquelle on est déjà.
Autrement dit, une accélération.
La seule chose qui différencie uen force d'une accélération c'est la prise en compte de la masse.
Mais la masse étant à 1, ça simplifie les calculs.

### Comment utiliser les forces pour le run()
On pourrait appliquer une force constante et caper une fois arrivé à la vitesse maximum.
Mais on aurait une accélération linéaire, robotique.
Pour avoir une accélération qui paraisse naturelle et responsive on va : 

#### 1 - Se baser sur le delta entre vitesse maximale et vitesse actuelle
- au plus on est loin de la vitesse qu'on veut atteindre, au plus on accélèrera vite.
- Au plus on s'approche de la vitesse voulue, au moins on accélèrera.
- on ne pourra jamais dépasser notre vitesse max
C'est une façon très efficace d'avoir un comportement naturel sans avoir de calcul très complexe.
Mais, il y a 2 downside : 
- la courbe de réponse tend vers la vitesse max sans jamais vraiment l'atteindre mathématiquement, et peut être très longue à atteindre le dernier 1% de vitesse manquant. ça implique de considérer notre vitesse max plutôt comme un interval.
Si on veut 15 de vitesse max, il faut considérer que notre vitesse max réelle est plutôt comprise entre 14.9 et 15.
On peut prédire le temps qu'on prendra à atteindre 14.9 (par exemple 50frames = 1 seconde), par contre atteindre le 15 ça peut très vite atteindre des valeurs qui ne sont pas exploitables dans nos calculs.
- le deuxième downside est qu'on manque de contrôle sur la courbe d'accélération en elle même. Notre équation de calcul de force est une fonction réccursive, elle fait appelle à son itération précédente pour calculer son itération actuelle. ça complique beaucoup de choses, c'est très difficile prévoir les résultats des futures itérations, de faire des calculs inverses etc ...

Mathématiquement, le calcul de notre force appliqué est qque chose comme ça : 
Force = F = f(x) = (Vmax - x) * coeff  => où x est la vitesse actuelle (et le coeff on verra plus bas)
et la vitesse induite est calculée comme suit : 
v = F * Delta temps donc
v = f(x) * 0.02s
Maintenant quelle est notre vitesse actuelle ? On le sait en la récupérant sur notre objet en mouvement tout simplement, mais mathématiquement elle découle directement du calcul fait à notre frame précédente.

Mettons F1 et v1 la force et la vitesse à notre frame 1, et F2 et v2 ceux à notre frame 2. On est à la frame 2.
on vient de dire que F2 = f(x2) = (Vmax - x2) * coeff
x2, c'est notre vitesse actuelle. C'est donc la vitesse atteinte à la frame 1, donc v1
x2 = v1
v1 dépend de la force F1 qu'on a appliqué à la frame 1 (formule de la vitesse dont on a parlé juste avant)
v1 = F1 * 0.02s
et bien sur, F1 peut s'écrire sous notre formule de calcul de la Force :
F1 = f(x1) = (Vmax-x0) * coeff   où x0 est la vitesse à la frame 0

Reprenons la formule de notre Force applqiué à la frame 2 :
F2 = f(x2) = (Vmax - x2) * coeff
on remplace x2 = v1 (vitesse actuelle calculée à la frame 1), on le remplace dans l'équation :
f(x2) = (Vmax - v1) * coeff
On remplace v1 par sa formule qui prend en compte F1
f(x2) = (Vmax - (F1 * 0.02s)) * coeff
finalement on remplace F1 par sa formule
f(x2) = (Vmax - ( f(x1) * 0.02s )) * coeff

On voit bien que f(x2) dépend directement de f(x1) ... C'est une fonction récursive, et c'est un putain d'enfer à représenter et à maîtriser. C'est difficile d'influencer la courbure de notre accélération, mais on peut tout de même réussir plus ou moins précisément à influencer le facteur d'accélération sous la forme "combien de frame pour passer de 0 à Vitesse max"

#### 2 - Appliquer un coefficient d'accélération/decélération à ce delta
C'est le coeff cité dans la formule au point 1.
La base logique de ce coeff c'est : "combien de frame pour passer de 0 à vitesse max"
mettons x le nombre de frame qu'on veut pour vitesse max, le coeff se calcul comme ça : 
Coeff = 1/Delta temps * 1/x = 1/0.02 * 1/x = 50 / x

Explication pseudo mathématique : 
mettons Frame 0 avec v0 et F0 , et Frame 1 avec v1 et F1 etc... mêmes formules qu'au dessus, et vmax
à F0, on a aucun déplacement , vu qu'on cherche à passer de 0 à vmax.
donc v0 = 0

Donc pour notre force à appliquer , sans prendre en compte le coeff pour le moment:
F1 = f(x1) = Vmax - x1 = Vmax - v0 = Vmax - 0 = Vmax
et v1 = F1 * 0.02s = F1 / 50 = Vmax / 50

Et donc la en gros l'idée c'est de se rendre compte que la vitesse en une frame = Force / 50
Donc si on veut une vmax en une frame, il faut multiplier cette vitesse par 50
Et si on applique pas de coeff 50, on mettra 1 seconde de base pour atteindre vMax si l'accélération est continue
Donc avec la formule du coeff = 50/x, x est le nombre de frame pour atteindre vMax
ce qui donne la formule finale pour le calcul de la force : 
f(x) = (Vmax - x) * coeff = (Vmax - x) * 50 / y      où y est une constante => le nombre de frame pour passer de 0 à Vmax

TOUT CA, c'est mathématiquement correct et vérifiable dans unity, SI on a une accélération linéaire.
Ce qui n'est plus notre cas, vu qu'on a appliqué une fonction réccursive pour avoir une accélération plus smooth.
Et le problème, c'est que c'est très difficile de prévoir mathématique cette courbe et de l'utiliser pour réussir à avoir un résultat précis de 
"combien de frame pour passer Vmax" avec cette courbe.

DONC
Pour contourner ça, comme dit plus haut on prend en compte une marge d'erreur sur Vmax de 0.1, genre si Vmax = 15, on dit que 14.9 c'est la base pour calculer le nombre de frame pour passer vitesse max.
Et à partir de là, en utilisant des statistiques, on peut grosso modo trouver des coeff intéressant humainement, et de proposer ces coeffs dans unity pour controler approximativement le feel qu'on veut.
Et il se trouve que ce qui était avant un "nombre de frame pour vmax", se rapproche persque parfaitement de "nombre de 0.1s pour vMax" (voir mon fichier excel)
AU FINAL, on peut se retrouver donc avec un slider de 1 à 10 exploitable
1 => Vmax en une frame, ça c'est fiable à 100%, mathémaitquement ça bougera jamais
et ensuite de 2 à 10, c'est le GROSSO MODO nombre de dixième de seconde pour atteindre vmax
donc
5 => 0.5s seconde
10 => 1 seconde

#### Conclusion
On a une fonction de run qui
- nous fait accélérer plus vite au plus on est loin de notre vitesse voulue (ça inclue le fait de changer de direction) et moins vite quand on s'approche de notre vitesse voulue => feel très humain responsive et agréable
- est une fonction récursive qui induit des faiblesses
- très difficile de prévoir mathématiquement les choses, que ça soit savoir le temps pour atteindre une vitesse, prévoir la distance parcourue en plusieurs frames etc... ça sera toujours des calculs mathématiquement approximatif ou alors très lourd à faire, mais peut être fiable humainement
- on manque de controle sur l'easing de la courbe, c'est un ease-out point barre, et on a juste la main sur l'ease time
- pour gérer l'ease time (le temps pour passer à vmax) on utilise un slider de 1 à 10 qui induit une valeur d'accélération "instantanée" (1) et 1seconde (10)

ça me plait pas trop que ça soit pas fiable mathématiquement, mais bon c'est des ptits détail qui se ressentiront surement pas humainement, et si un moment on a besoin d'être ultra précis, on pourra toujours contourner ça ponctuellement en utilisant de la vélocité ou simplifiant la formule.
A noter qu'on pourrait très bien lvl up de ouf mathématiquement un moment et comprendre comment controler une fonction réccursive, c'est pas exclu.

## Recup list dawnosaur
- Jump simple
- Jump calculé (avec jump height + time to apex + jump force + gravity strength)
- Gravity au fall + gravity fall cap (pour éviter une accélération infinie dans de gros niveau verticaux)
- Jump cut
- Accel rate du run si en l'air
- conserve momentum (si on jump, on applique pas de force qui ralentirait le joueur s'il continue dans la direction dans laquelle il a sauté)
- Air time à l'apex + treshold de l'air time
- Bonus d'acceleration à l'air time
- Fast fall + fast fall capp
- ensuite on rescannera pour le double jump, wall jump dash slide etc
- Animation du perso