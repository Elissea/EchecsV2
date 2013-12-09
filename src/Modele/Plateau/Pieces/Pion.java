/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package Modele.Plateau.Pieces;

import Common.Couleur;
import java.awt.Point;
import java.util.ArrayList;

/**
 *
 * @author 20130330
 */
public class Pion extends Piece {
    public Pion(Point coordonnees, String denomination, boolean couleur) {
        super(coordonnees, denomination, couleur);
    }
    
    @Override
    public ArrayList<Point> getDeplacements() {
        ArrayList<Point> deplacements = new ArrayList<>();
        int x = (int)this.getPosition().getX();
        int y = (int)this.getPosition().getY();
        int multiplicateur = this.getCouleur() == Couleur.BLANC ? 1 : -1;
        
        
        deplacements.add(new Point(x-(1*multiplicateur), y));
        deplacements.add(new Point(x-(2*multiplicateur), y));
        
        return deplacements;
    }
    
}
