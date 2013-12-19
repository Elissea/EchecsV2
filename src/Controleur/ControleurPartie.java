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
    private Joueur j1, j2, joueurActif;
    private int tour;

    public ControleurPartie(Joueur j1, Joueur j2, Plateau plateau) {
        this.j1 = j1;
        this.j2 = j2;
        this.joueurActif = j1;
        this.tour = Couleur.BLANC;
        this.plateau = plateau;
        this.controleurDeplacement = new ControleurDeplacement(this.plateau);
    }

    public int getTour() {
        return this.tour;
    }

    public void changeTour() {
        if (this.tour == Couleur.BLANC) {
            this.joueurActif = j2;
            this.tour = Couleur.NOIR;
        } else {
            this.joueurActif = j1;
            tour = Couleur.BLANC;
        }
    }

    public ArrayList<Coordonnee> getDeplacementsPossibles(Coordonnee coordonnee) {
        ArrayList<Coordonnee> deplacementsPossibles = new ArrayList<>();
        Piece piece = this.plateau.getCaseParCoordonnee(coordonnee).getPiece();

        if (piece.getValeur() != Valeur.VIDE && piece.getCouleur() == this.tour) {
            deplacementsPossibles = this.controleurDeplacement.getDeplacementsPossibles(piece);
        }

        return deplacementsPossibles;
    }

    public boolean effectuerDeplacement(Coordonnee depart, Coordonnee arrivee) {
        if (this.controleurDeplacement.validerDeplacement(this.joueurActif, depart, arrivee)) {
            this.changeTour();
            return true;
        } else {
            return false;
        }
    }

    public boolean echecEtMat() {
        return false;
    }
}