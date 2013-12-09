/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package Modele.Plateau.Pieces;

import java.awt.Point;
import java.util.ArrayList;

/**
 *
 * @author 20130330
 */
public class Cavalier extends Piece {

    public Cavalier(Point coordonnes, String denomination, boolean couleur) {
        super(coordonnes, denomination, couleur);
    }
    
    @Override
    public ArrayList<Point> getDeplacements() {
        throw new UnsupportedOperationException("Not supported yet."); //To change body of generated methods, choose Tools | Templates.
    }
    
}
