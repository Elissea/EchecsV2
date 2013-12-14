package Controleur;

import Common.Coordonnee;
import Common.Couleur;
import Common.Valeur;
import Modele.Joueur.Joueur;
import Modele.Piece;
import Modele.Plateau;
import java.util.ArrayList;

public class ControleurPartie {

    private ControleurDeplacement controleurDeplacement;
    private Plateau plateau;
    private Joueur j1, j2;
    private int tour;

    public ControleurPartie(Joueur j1, Joueur j2, Plateau plateau) {
        this.j1 = j1;
        this.j2 = j2;
        this.tour = Couleur.BLANC;
        this.plateau = plateau;
        this.controleurDeplacement = new ControleurDeplacement(this.plateau); 
    }

    public int getTour() {
        return this.tour;
    }

    public void changeTour() {
        if (tour == Couleur.BLANC) {
            tour = Couleur.NOIR;
        } else {
            tour = Couleur.BLANC;
        }
    }

    public void jouer() {
    }

    public ArrayList<Coordonnee> getMouvementsPossibles(Coordonnee coordonnee) {
        ArrayList<Coordonnee> mouvementsPossibles = new ArrayList<>();
        Piece piece = this.plateau.getCaseParCoordonnee(coordonnee).getPiece();

        if (piece.getValeur() != Valeur.VIDE && piece.getCouleur() == this.tour) {
            mouvementsPossibles = this.controleurDeplacement.getDeplacementsPossibles(piece, false);
        }
        
        return mouvementsPossibles;
    }

    public boolean echecEtMat() {
        return false;
    }
}