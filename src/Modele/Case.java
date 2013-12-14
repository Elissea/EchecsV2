package Modele;

import Common.Coordonnee;

public class Case {

    private Coordonnee coordonnee;
    private Piece piece;

    public Case(Coordonnee coordonnee) {
        this.coordonnee = coordonnee;
        this.piece = Vide.getInstance();
    }

    public Coordonnee getCoordonnee() {
        return this.coordonnee;
    }

    public void setCoordonnees(Coordonnee coordonnee) {
        this.coordonnee = coordonnee;
    }

    public Piece getPiece() {
        return this.piece;
    }

    public void setPiece(Piece piece) {
        this.piece = piece;
    }
}
