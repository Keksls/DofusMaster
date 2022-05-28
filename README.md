DISCLAMER :

Théoriquement indétectable, ce n'est PAS un bot, ça ne touche pas à dofus (ni au client, ni au launcher).
Ce n'est pas dofusMaster qui lance vos comptes, à vous de les lancer avec le launcher.
Pas de MITM ici, ni de pixel detection, juste des évenement windows bas niveau qui font croire à l'OS (pas à dofus) que vous faites des Alt+Tab et des cliques souris rapidement.
C'était une pratique toléré par Ankama, visiblement ça ne l'est plus.
Cependant, honetement, ça leut coûterais cher en log pour détecter ça. C'est théoriquement possible, si ils logs chaques actions (clics, focus windows) et qu'ils les lient aux comptes par IP.
C'est coûteux à transferer et stocker pour très peut de use case, je m'en sert depuis un moment sans pb.

Pour ceux qui pense que c'est un 'virus' (*bruit de fantôme qui fait peur*), le code est sur github, c'est cadeau, clonez le et refaites vos builds si vous êtres pas confiant.
Pour ceux qui aurait peur que des petits malin fassent un fork avec un keylogger ou autre dedans, assurez-vous d'avoir téléchargé sur le lien officiel (en bas de cette page).
Pour les plus perspicaces qui ont remarqué les .dll 'Gma.System.MouseKeyHook' et 'WindowsInput'. 
Tout à fait possible de faire un keylogger avec ça, je suis pas sur DM soit pas signalé comme virus par certains antivirus a cause de ça.
Cependant, bien obligé de hook les IO du PC (Clavier / souris) pour que le processus de DM sache quand le joueur clique ou utilise un racourci clavier pour switch de fenêtre, sans avoir le focus.

Le but est de remplacer Naoi/Organizer/vieux scipts autoit. ça à été dev en une aprèm, si vous voulez des modifs dites-le sur github.



Switch de compte :

vous pouvez switcher de compte avec les touches [NextKey] et [PreviewKey].
vous pouvez déselectionner une compte pour qu'il ne soit pas pris en compte par dofus master.
vous pouvez directement switcher sur un compte en pressant la touche du numpad correspondant à sont ordre dans la liste (1, 2, 3, etc).
vous pouvez réorganiser l'ordre des comptes pour boucler dans l'ordre de la timeline de combat de dofus. (pas de détection auto d'initiative, on reste flou sur la légalitée).
Par defaut, les touche pour changer de comptes sont Shift + X et Shift + C (vous pouvez changer ça dans la config => save.json)


aide à la config :

MinMovementDelay / MaxMovementDelay : durrée entre deux simulation de clique lorsque l'utilisateur veux répliquer une action de clique sur plusieurs fenêtres. Min doit être inférieur à Max.
AccountShortcutCtrl : true ou false, si activé, vous ne pourrez utiliser les racourcis de comptes (1, 2, 3 => pavé numérique) que si la touche Control est enfoncée
NextKey : code de la touche clavier qui passe au compte suivant
PreviewKey : code de la touche clavier qui passe au compte précédent
instantKey : code de la touche clavier qui passe le mode de réplication à 'Instant'
smoothKey : code de la touche clavier qui passe le mode de réplication à 'Smooth'
NextPreviexCtrl : true ou false, si activé, vous ne pourrez utiliser les racourcis Next et Preview que si la touche Control est enfoncée
NextPreviexShift : true ou false, si activé, vous ne pourrez utiliser les racourcis Next et Preview que si la touche Shift est enfoncée



codes de touche clavier :

Pour trouver les code de votre clavier, vous pouvez cliquer sur le boutton 'KeyCode tool', une petite fenêtre s'ouvre, il vous suffi de presser une touche pour voir son code.



Replication :

la fonction réplication vous permet d'envoyer un clique sur plusiers fenêtre dofus.
Lorsque vous êtres sur une fenêtre dofus, il vous suffi de presser la touche [instantKey] ou [smoothKey] (par defaut, Ctlr et Alt) puis de cliquer sur une zonne de la fenêtre, 
le cliqu sera répliqué sur toutes les autres fenêtres dofus.
 - Instant replication : Envoie un éveneùent de clique à un endroit spécifique d'une fenêtre. Il permet de ne pas donner le focus à la fenêtre avant de cliquer. 
Le plus rapide, cependant, dofus à besoin que la souris soit sur certains éléments avant de cliquer dessus. Il ne marche pas pour changer de map ou parler à un PNJ
 - Smooth replication : Switch sur toutes les fenêtres une par une et effectie le clique. 
la vitesse de switch des fenêtres est un nombre aléatoire compris entre [MinMovementDelay] et [MaxMovementDelay]. à vous de chercher la plus petite valeurs qui ne ralenti pas votre PC (dépendant du tempss de switch de focus windows).
Lors d'une smooth replication, ne bougez pas la souris. sinon le clique se fera à la nouvelle position souris pour le reste de la routine.



Send to console :

Simplement tapez le texte que vous voulez envoyer dans la console, cliquez sur 'send to console', le texte sera tapé dans tous les comptes ouverts et envoyé.



Travel :

Rentrez simplement une position en jeu puis cliquez sur 'travel'. 
C'est une simple automatisation de la commande "/travel x y", il vous faut dont être sur une monture autopilotée.




LIEN DU GIT OFFICIEL : https://github.com/Keksls/DofusMaster
LIEN DE TELECHARGEMENT : https://github.com/Keksls/DofusMaster/releases




Bon jeux à tous,
on s'est jamais vu.