/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Modele.Plateau.Pieces;

import Common.Couleur;
import java.awt.Point;
import java.util.ArrayList;

/**
 *
 * @author Elissea
 */
public class Vide extends Piece {
    private static Vide vide = null;
    
    private Vide(Point coordonnees, String denomination, boolean couleur) {
        super(coordonnees, denomination, couleur);
    }
    
    public static Vide getInstance() {
        if(vide == null) {
            vide = new Vide(new Point(0,0), Denominations.Vide, Couleur.BLANC);
        }
        
        return vide;
    }
        
    @Override
    public ArrayList<Point> getDeplacements() {
        return new ArrayList<>();
    }
    
}
