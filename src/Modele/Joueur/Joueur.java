package Modele.Joueur;

import Modele.Deplacement;
import Modele.Plateau.Pieces.Piece;
import java.util.ArrayList;

public abstract class Joueur {

    protected String nom;
    protected Timer timer;
    protected ArrayList<Piece> pieces;
    protected boolean couleur;
    protected boolean isOrdinateur;

    public Joueur(String nom, int temps, ArrayList<Piece> pieces, boolean couleur, boolean isOrdinateur) {
        this.nom = nom;
        this.timer = new Timer();
        this.pieces = pieces;
        this.couleur = couleur;
        this.isOrdinateur = isOrdinateur;
    }

    public abstract Deplacement jouer();

    public boolean getCouleur() {
        return this.couleur;
    }

    public boolean isOrdinateur() {
        return this.isOrdinateur;
    }
}
