package Modele;

import Common.Coordonnee;
import Common.Couleur;
import Common.Valeur;

public class Plateau {

    private Case[][] cases;

    public Plateau() {
        this.cases = new Case[8][8];

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                this.cases[i][j] = new Case(new Coordonnee(i, j));
            }
        }
    }

    public void initialiser() {
        for (int i = 0; i < 8; i++) {
            this.cases[1][i].setPiece(new Piece(Valeur.PION, Couleur.BLANC, new Coordonnee(1, i)));
            this.cases[6][i].setPiece(new Piece(Valeur.PION, Couleur.NOIR, new Coordonnee(6, i)));
        }

        this.cases[0][0].setPiece(new Piece(Valeur.TOUR, Couleur.BLANC, new Coordonnee(0, 0)));
        this.cases[0][1].setPiece(new Piece(Valeur.CAVALIER, Couleur.BLANC, new Coordonnee(0, 1)));
        this.cases[0][2].setPiece(new Piece(Valeur.FOU, Couleur.BLANC, new Coordonnee(0, 2)));
        this.cases[0][3].setPiece(new Piece(Valeur.ROI, Couleur.BLANC, new Coordonnee(0, 3)));
        this.cases[0][4].setPiece(new Piece(Valeur.DAME, Couleur.BLANC, new Coordonnee(0, 4)));
        this.cases[0][5].setPiece(new Piece(Valeur.FOU, Couleur.BLANC, new Coordonnee(0, 5)));
        this.cases[0][6].setPiece(new Piece(Valeur.CAVALIER, Couleur.BLANC, new Coordonnee(0, 6)));
        this.cases[0][7].setPiece(new Piece(Valeur.TOUR, Couleur.BLANC, new Coordonnee(0, 7)));

        this.cases[7][0].setPiece(new Piece(Valeur.TOUR, Couleur.NOIR, new Coordonnee(7, 0)));
        this.cases[7][1].setPiece(new Piece(Valeur.CAVALIER, Couleur.NOIR, new Coordonnee(7, 1)));
        this.cases[7][2].setPiece(new Piece(Valeur.FOU, Couleur.NOIR, new Coordonnee(7, 2)));
        this.cases[7][3].setPiece(new Piece(Valeur.ROI, Couleur.NOIR, new Coordonnee(7, 3)));
        this.cases[7][4].setPiece(new Piece(Valeur.DAME, Couleur.NOIR, new Coordonnee(7, 4)));
        this.cases[7][5].setPiece(new Piece(Valeur.FOU, Couleur.NOIR, new Coordonnee(7, 5)));
        this.cases[7][6].setPiece(new Piece(Valeur.CAVALIER, Couleur.NOIR, new Coordonnee(7, 6)));
        this.cases[7][7].setPiece(new Piece(Valeur.TOUR, Couleur.NOIR, new Coordonnee(7, 7)));
    }

    public boolean estDansLePlateau(Coordonnee coordonnee) {
        return (coordonnee.getLigne() >= 0 && coordonnee.getLigne() < 8
                && coordonnee.getColonne() >= 0 && coordonnee.getColonne() < 8);
    }

    public Case getCaseParCoordonnee(Coordonnee coordonnee) {
        return this.cases[coordonnee.getLigne()][coordonnee.getColonne()];
    }

    public Case[][] getCases() {
        return this.cases;
    }
}
