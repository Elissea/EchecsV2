package Modele.Plateau.Pieces;

import java.awt.Point;
import java.util.ArrayList;


public abstract class Piece {
    private Point coordonnees;
    private String denomination;
    private boolean couleur;
    
    public Piece(Point coordonnees, String denomination, boolean couleur) {
        this.coordonnees = coordonnees;
        this.denomination = denomination;
        this.couleur = couleur;
    }
    
    public abstract ArrayList<Point> getDeplacements();
    
    public void deplacer(Point coordonnees) {
        this.coordonnees = coordonnees; 
    }
    
    public Point getPosition() {
        return this.coordonnees;
    }
    
    public boolean getCouleur() {
        return this.couleur;
    }
    
    public String getDenomination() {
        return this.denomination;
    }
}
