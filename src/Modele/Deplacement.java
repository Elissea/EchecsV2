package Modele;

import Modele.Plateau.Pieces.Piece;
import java.awt.Point;

public class Deplacement {
    private Point depart;
    private Point arrivee;
    
    public Deplacement(Point depart, Point arrivee) {
        this.depart = depart;
        this.arrivee = arrivee;
    }
    
    public Point getDepart() {
       return this.depart;
    }
    
    public Point getArrivee() {
        return this.arrivee;
    }
}
