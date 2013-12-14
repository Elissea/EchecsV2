package Modele;

import Common.Coordonnee;
import java.util.Map;

public class Piece {

    private int valeur;
    private int couleur;
    private Coordonnee coordonnee;
    private boolean aBouge;

    public Piece(int valeur) {
        this.valeur = valeur;
    }

    public Piece(int valeur, int couleur, Coordonnee coordonnee) {
        this.valeur = valeur;
        this.couleur = couleur;
        this.coordonnee = coordonnee;
        this.aBouge = false;
    }

    public int getValeur() {
        return this.valeur;
    }

    public void setValeur(int valeur) {
        this.valeur = valeur;
    }

    public boolean getABouge() {
        return this.aBouge;
    }

    public void setABouge(boolean aBouge) {
        this.aBouge = aBouge;
    }

    public int getCouleur() {
        return this.couleur;
    }

    public void setCouleur(int couleur) {
        this.couleur = couleur;
    }

    public Coordonnee getCoordonnee() {
        return this.coordonnee;
    }

    public void setCoordonnee(Coordonnee coordonnee) {
        this.coordonnee = coordonnee;
    }
}
