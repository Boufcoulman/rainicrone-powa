using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace RainicronePowa
{
    // Classe générant et gérant le jeu 

    // A la génération d'un objet plateau, on initialise un plateau, on place le joueur, on remplit le plateau avec les différentes
    // couleurs, puis on appele la méthode RoutineDeJeu qui gère le fonctionnement normal
    public class Plateau
    {
        // Dimension d'un plateau
        private static int nbLigne = 11, nbColonne = 11;

        // Timer de jeu
        Stopwatch monTimer = new Stopwatch();

        // Compteur de coups
        int nbdecoups = 0;

        // Case de départ
        private int[] startPos = { 5, 5 };
        private int[] position = new int[2];
        private int[] safePos = new int[2];

        // Nombre de points de vie
        private int pdv = 0;

        // Iniatialisation du plateau sans valeurs
        private char[,] monPlateau = new char[nbLigne, nbColonne];

        // Tableau référence des différentes couleurs popant au pif: Jaune, Vert, Rouge, Noir
        private char[] abecedaire = { 'J', 'V', 'R', 'N' };

        private Random aleatoire = new Random();

        // Saisie de la direction
        ConsoleKeyInfo direction = new ConsoleKeyInfo();

        // Création d'un plateau : initialisation aléatoire des cases sauf le point de départ, les cases à atteindre et les cases interdites
        public Plateau()
        {
            // Initialisation des positions à retenir
            position[0] = startPos[0];
            position[1] = startPos[1];
            safePos[0] = startPos[0];
            safePos[1] = startPos[1];

            for (int i = 0; i < nbLigne; i++)               // Pour chaque ligne
            {
                for (int j = 0; j < nbColonne; j++)         // Pour chaque colonne
                {
                    // Gestion des cases interdites :
                    if (j == (nbColonne / 2) && (i == 0 || i == 1 || i == (nbLigne - 1) || i == (nbLigne - 2)) ||
                       i == (nbColonne / 2) && (j == 0 || j == 1 || j == (nbLigne - 1) || j == (nbLigne - 2)))
                    {
                        monPlateau[i, j] = 'X';   //Case interdite
                    }
                    // Gestion des cases à atteindre pour gagner
                    else if ((i == 0 && j == 0) || (i == nbLigne - 1 && j == 0) || (i == 0 && j == nbColonne - 1) || (i == nbLigne - 1 && j == nbColonne - 1))
                    {
                        monPlateau[i, j] = 'W';   // Case à atteindre pour gagner
                        pdv++;                    // Ajout d'un point de vie
                    }
                    // Placement du point de départ
                    else if (i == startPos[0] && j == startPos[1])
                    {
                        monPlateau[i, j] = ' ';
                    }
                    // Génération piffée des autres cases
                    else
                    {
                        monPlateau[i, j] = abecedaire[aleatoire.Next(0, abecedaire.Length)];
                    }
                }
            }
            this.RoutineDeJeu();
        }

        // Methode qui permet au jeu de continuer tant que le joueur n'a pas gagné : on appèle l'affichage du plateau,
        // ptet l'affichage des règles, puis la demande de déplacement à l'utilisateur
        // possiblement un rappel des pdv
        private void RoutineDeJeu()
        {
            // Affichage des règles :
            Console.WriteLine("Hello ! C'est un petit jeu où vous commencez au milieu d'une grille \n(vous êtes représenté par un O)," +
                " et votre but c'est d'aller récupérer les W aux quatres coins de la grille (litéralement). \nSauf qu'il y a des cases" +
                " qui vous font vous déplacer de manière chaotique, \ndu coup gl hf ;)\nAppuyez sur une touche pour continuer !");
            Console.ReadKey();

            //Initialisation d'un timer
            monTimer.Start(); // début de la mesure

            // Tant que le joueur n'a pas atteint toutes les cellules nécessaires
            while (pdv > 0)
            {
                // Efface l'ecran
                Console.Clear();

                // Affiche l'état du plateau de jeu
                this.Affiche();

                // Affichage des mécaniques :
                this.Regles();

                // Affichage de la case sous le joueur
                this.CaseActuelle();

                // Gère la saisie du déplacement par le joueur
                this.Deplacetoastp(); 
            }

            // Fin du jeu
            monTimer.Stop(); // Fin de la mesure

            // Efface l'ecran
            Console.Clear();

            // Affiche l'état du plateau de jeu
            this.Affiche();

            // Victory
            Console.WriteLine("Gg ! T'as gagné avec une grille en " + nbLigne + "x" + nbColonne + " en " + monTimer.ElapsedMilliseconds%1000 + " secondes");
            Console.ReadKey();
        }

        // Methode qui permet d'afficher le plateau avec ses couleurs, et la position actuelle du joueur
        private void Affiche()
        {
            // monPlateau contient les couleurs des différentes cases mais pas la position du joueur qui elle est contenue dans position
            for (int i = 0; i < nbLigne; i++)               //Pour chaque ligne
            {
                for (int j = 0; j < nbColonne; j++)         //Pour chaque colonne
                {
                    // Position du joueur
                    if (i == position[0] && j == position[1])
                    {
                        Console.Write(" O ");
                    }
                    else
                    // Case classique
                    {
                        Console.Write(" " + monPlateau[i, j] + " ");
                    }
                }
                Console.WriteLine("");
            }
        }

        // Methode qui affiche la case sous laquelle est le joueur
        private void CaseActuelle()
        {
            Console.WriteLine("\nActuellement tu es sur une case '" + monPlateau[position[0], position[1]] + "'");
        }

        // Methode qui affiche les règles du jeu
        private void Regles()
        {
            Console.WriteLine("Chaque case à un effet lorsque vous atterrissez dessus : \n" +
                "J : Devient R et fais reavancer d'une case.\n" +
                "R : Devient V et change la couleur des 4 cases adjacentes de cette manière\nJ => R => V => N => J.\n" +
                "V : Devient J et vous fait revenir en arrière.\n" +
                "N : Devient R et vous fait retourner sur la dernière case vide parcourue (centre ou l’un des 4 coins de la map une fois atteint).\n" +
                "W : Objectif du jeu, une fois les 4 atteint c'est gagné :p.");
        }

        // Methode qui demande à l'utilisateur de saisir la direction dans laquelle il veut aller, puis va appeler la méthode Deplacement
        // qui permet de gérer l'activation des cellules ou d'indiquer que le déplacement est impossible
        private void Deplacetoastp()
        {
            // Gestion d'une mauvaise saisie
            bool saisieNonValide = true;

            // Instruction transmie à déplacement
            int numDirection = 42; // Valeur abérante

            // Demande d'action du joueur
            Console.WriteLine("Choisi ta direction de déplacement ! Appui sur haut, bas, droite ou gauche");

            while (saisieNonValide)
            {
                // Saisie par le joueur du déplacement voulu
                direction = Console.ReadKey();

                // Comptage du nombre de coups
                nbdecoups++;

                // Détection d'une mauvaise saisie / conversion en instruction pour la methode Deplacement
                switch (direction.Key.ToString())
                {
                    // Déplacement en haut
                    case "UpArrow":
                        {
                            numDirection = 0;
                            // Saisie valide
                            saisieNonValide = false;
                        }
                        break;

                    // Déplacement en bas
                    case "DownArrow":
                        {
                            numDirection = 1;
                            // Saisie valide
                            saisieNonValide = false;
                        }
                        break;
                    // Déplacement à droite
                    case "RightArrow":
                        {
                            numDirection = 2;
                            // Saisie valide
                            saisieNonValide = false;
                        }
                        break;
                    // Déplacement à gauche
                    case "LeftArrow":
                        {
                            numDirection = 3;
                            // Saisie valide
                            saisieNonValide = false;
                        }
                        break;
                    default:
                        {
                            // Laisse saisieNonValide à vraie
                            saisieNonValide = true;
                        }
                        break;
                }

                // Si la saisie est conforme, on envoi l'instruction à Deplacement
                if (!saisieNonValide)
                {
                    // Gère le déplacement du personnage et renvoi true si le personnage n'a pas bougé
                    saisieNonValide = this.Deplacement(numDirection);
                }

                // En cas de mauvaise saisie ou de non déplacement
                if (saisieNonValide)
                {
                    Console.WriteLine("Tu n'as pas saisi une direction valide ;)");
                }
                else
                {
                    // La saisie est valide et on vient effectuer des actions supplémentaires
                    this.HereComesTheRumble(numDirection);
                }
            }
        }

        // Methode qui en recevant une direction donnée, va tester si le déplacement sur cette case est possible, auquel cas
        // effectue la modification de pos. La modification des couleurs et les appels suivant qui en découlent sont gérés
        // par une autre méthode.
        // direction est réellement utilisé (switch sur un int)
        private bool Deplacement(int numDirection)
        {
            // Gestion du déplacement en fonction de la direction
            switch (numDirection)
            {
                // Deplacement en haut
                case 0:
                    {
                        // Exclusion du cas où le joueur est tout en haut
                        if (position[0] != 0)
                        {
                            // Exclusion du cas où le joueur tente d'aller sur une case non valide
                            if (monPlateau[position[0] - 1, position[1]] != 'X')
                            {
                                // Déplacement d'une case en haut
                                position[0]--;
                                return false;
                            }
                        }
                    }
                    break;
                // Deplacement en bas
                case 1:
                    {
                        // Exclusion du cas où le joueur est tout en bas
                        if (position[0] != nbLigne - 1)
                        {
                            // Exclusion du cas où le joueur tente d'aller sur une case non valide
                            if (monPlateau[position[0] + 1, position[1]] != 'X')
                            {
                                // Déplacement d'une case en bas
                                position[0]++;
                                return false;
                            }
                        }
                    }
                    break;
                // Deplacement à droite
                case 2:
                    {
                        // Exclusion du cas où le joueur est tout à droite
                        if (position[1] != nbColonne - 1)
                        {
                            // Exclusion du cas où le joueur tente d'aller sur une case non valide
                            if (monPlateau[position[0], position[1] + 1] != 'X')
                            {
                                // Déplacement d'une case à droite
                                position[1]++;
                                return false;
                            }
                        }
                    }
                    break;
                // Deplacement à gauche
                case 3:
                    {
                        // Exclusion du cas où le joueur est tout à gauche
                        if (position[1] != 0)
                        {
                            // Exclusion du cas où le joueur tente d'aller sur une case non valide
                            if (monPlateau[position[0], position[1] - 1] != 'X')
                            {
                                // Déplacement d'une case à gauche
                                position[1]--;
                                return false;
                            }
                        }
                    }
                    break;
                default:
                    {
                        // Ne sers pas ici
                    }
                    break;
            }
            // Le déplacement n'a pas déplacé le personnage
            return true;
        }

        // Methode qui permet l'affichage bref de l'état transitoire du jeu
        private void Transition()
        {
            // Reactualisation de l'affichage
            Console.Clear();
            this.Affiche();
            this.Regles();
            this.CaseActuelle();
            Console.WriteLine("Chaîne de déplacement en cours !");

            // Temporisation afin d'avoir le temps d'observer la situation
            Thread.Sleep(300);
        }

        // Methode qui vient changer la couleur de la case sur laquelle se trouve le personnage,
        // et effectuer les actions correspondantes si besoin
        // direction n'est que retransmis à d'autres methodes
        private void HereComesTheRumble(int numDirection)
        {
            // Booléens indiquant si l'on doit rappeler HereComesTheRumble suite à un déplacement
            bool pasDeDeplacement = true;

            // Traitement différent en fonction de la case sur laquelle on est
            switch (monPlateau[position[0], position[1]])
            {
                // Cas d'une case rose 'W'
                case 'W':
                    {
                        this.Rose();
                    }
                    break;
                // Cas d'une case vide ' ' (départ ou cases roses deja atteintes)
                case ' ':
                    {
                        // Actualisation de la dernière case safe
                        safePos[0] = position[0];
                        safePos[1] = position[1];
                    }
                    break;
                // Cas d'une case Jaune 'J'
                case 'J':
                    {
                        // Affichage bref de l'etat transitoire du jeu
                        this.Transition();

                        // Permet de savoir si un déplacement a eu lieu
                        pasDeDeplacement = this.Jaune(numDirection);
                    }
                    break;
                // Cas d'une case Rouge 'R'
                case 'R':
                    {
                        this.Rouge();
                    }
                    break;
                // Cas d'une case Verte 'V'
                case 'V':
                    {
                        // Affichage bref de l'etat transitoire du jeu
                        this.Transition();

                        // Permet de savoir si un déplacement a eu lieu
                        pasDeDeplacement = this.Verte(numDirection);
                    }
                    break;
                // Cas d'une case Noire 'N'
                case 'N':
                    {
                        // Affichage bref de l'etat transitoire du jeu
                        this.Transition();

                        this.Noire();
                    }
                    break;
                // Si les codes des lettres sont respéctés, ce cas ne devrait pas se produire
                default: 
                    {
                        Console.WriteLine("Y'a probablement une couille dans la génération de la map ^^");
                    }
                    break;
            }
            // Si l'action a entrainé un déplacement, on refait appel à HereComesTheRumble
            if (!pasDeDeplacement)
            {
                this.HereComesTheRumble(numDirection);
            }
        }

        // Actions lors d'un déplacement sur une case rose :
        // La case devient vide et on perd un point de vie
        private void Rose()
        {
            // Remplacement de la case par du vide
            monPlateau[position[0], position[1]] = ' ';
            // Perte d'un point de vie
            pdv--;
            // Actualisation de la dernière case safe
            safePos[0] = position[0];
            safePos[1] = position[1];
        }

        // Actions lors d'un déplacement sur une case jaune :
        // Avancée d'une case supplémentaire
        // direction n'est que retransmis
        private bool Jaune(int numDirection)
        {
            // Remplacement de la case par du rouge
            monPlateau[position[0], position[1]] = 'R';
            // Entraine un déplacement d'une case supplémentaire dans la même direction
            return this.Deplacement(numDirection);
        }

        // Actions lors d'un déplacement sur une case rouge :
        // Les cases adjacentes changent de couleur
        private void Rouge()
        {
            // Remplacement de la case par du vert
            monPlateau[position[0], position[1]] = 'V';

            // Initialisation des listes qui permettront les modifications des cases adjacentes
            char[] adjCases = new char[4];
            int[,] posCases = new int[4, 2];


            // Case du haut
            if (position[0] != 0)
            {
                // Récupération de la couleur actuelle
                adjCases[0] = monPlateau[position[0] - 1, position[1]];

                // Mise en mémoire de la case
                posCases[0, 0] = position[0] - 1;
                posCases[0, 1] = position[1];
            }
            // Case du bas
            if (position[0] != nbLigne - 1)
            {
                // Récupération de la couleur actuelle
                adjCases[1] = monPlateau[position[0] + 1, position[1]];

                // Mise en mémoire de la case
                posCases[1, 0] = position[0] + 1;
                posCases[1, 1] = position[1];
            }
            // Case de droite
            if (position[1] != nbColonne - 1)
            {
                // Récupération de la couleur actuelle
                adjCases[2] = monPlateau[position[0], position[1] + 1];

                // Mise en mémoire de la case
                posCases[2, 0] = position[0];
                posCases[2, 1] = position[1] + 1;
            }
            // Case de gauche
            if (position[1] != 0)
            {
                // Récupération de la couleur actuelle
                adjCases[3] = monPlateau[position[0], position[1] - 1];

                // Mise en mémoire de la case
                posCases[3, 0] = position[0];
                posCases[3, 1] = position[1] - 1;
            }


            // Modification de chacune des cases adjacentes
            for (int i = 0; i < 4; i++)
            {

                // Le switch seulement sur le char associé
                switch (adjCases[i])
                {
                    // Le changement de char se fait directement sur le monPlateau(le x, le y)
                    case 'J':
                        {
                            monPlateau[posCases[i, 0], posCases[i, 1]] = 'R';
                        }
                        break;

                    case 'R':
                        {
                            monPlateau[posCases[i, 0], posCases[i, 1]] = 'V';
                        }
                        break;

                    case 'V':
                        {
                            monPlateau[posCases[i, 0], posCases[i, 1]] = 'N';
                        }
                        break;

                    case 'N':
                        {
                            monPlateau[posCases[i, 0], posCases[i, 1]] = 'J';
                        }
                        break;

                    default:
                        {
                            // Pour les autres caractères ou si la case n'existe pas, on ne fait rien
                        }
                        break;
                }
            }
        }

        // Actions lors d'un déplacement sur une case verte :
        // Avancée dans le sens opposé (retour en arrière donc)
        // direction est changé de manière à aller dans la direction opposée
        private bool Verte(int numDirection)
        {
            // Remplacement de la case par du jaune
            monPlateau[position[0], position[1]] = 'J';
            // Entraine un déplacement d'une case supplémentaire dans la direction opposée
            int numInversion = 42; // Valeur abérante
            switch (numDirection)
            {
                // Déplacement original vers le haut
                case 0:
                    {
                        // Déplacement vers le bas
                        numInversion = 1;
                    }
                    break;
                // Déplacement original vers le bas
                case 1:
                    {
                        // Déplacement vers le haut
                        numInversion = 0;
                    }
                    break;
                // Déplacement original vers la droite
                case 2:
                    {
                        // Déplacement vers la gauche
                        numInversion = 3;
                    }
                    break;
                // Déplacement original vers la gauche
                case 3:
                    {
                        // Déplacement vers la droite
                        numInversion = 2;
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Heeeeuu, c'est pas vraiment normal x)");
                    }
                    break;
            }
            return this.Deplacement(numInversion);
        }

        // Actions lors d'un déplacement sur une case noire :
        // Retour du joueur sur la dernière case libre atteinte (objectif ou centre)
        private void Noire()
        {
            // Remplacement de la case par du rouge
            monPlateau[position[0], position[1]] = 'R';
            // Ramène le joueur à la dernière case safe
            position[0] = safePos[0];
            position[1] = safePos[1];
        }
    }
}
