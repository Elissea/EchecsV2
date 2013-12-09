/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Vue;

import Common.Couleur;
import Modele.Plateau.Pieces.Piece;
import Modele.Plateau.Pieces.Vide;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Point;
import javax.swing.JLabel;

/**
 *
 * @author Elissea
 */
public class Case extends JLabel {

    private Point coordonnees;
    private Piece piece;
    private Color couleur;
    private boolean isSelectionnee;

    public Case(Color couleur, int x, int y) {
        this.coordonnees = new Point(x, y);
        this.piece = Vide.getInstance();
        this.isSelectionnee = false;
        this.couleur = couleur;
        this.setPreferredSize(new Dimension(75, 75));
        this.setBackground(couleur);
        this.setOpaque(true);
    }

    public void setIcone(String icone) {
        this.setText(icone);
        this.setForeground(Color.RED);
    }

    public String getIcone() {
        return this.getText();
    }

    public Point getPosition() {
        return this.coordonnees;
    }

    public void setPiece(Piece piece) {
        this.piece = piece;
        
        if (piece.equals(Vide.getInstance())) {
            this.setIcone("");
        } else {
            String couleur = piece.getCouleur() == Couleur.BLANC ? "Blanc" : "Noir";
            String denomination = piece.getDenomination() + "_" + couleur;
            this.setIcone(denomination);
        }
    }

    public void removePiece() {
        this.piece = Vide.getInstance();
    }

    public boolean isSelectionnee() {
        return this.isSelectionnee;
    }

    public void selectionner() {
        this.isSelectionnee = true;
        this.setBackground(Color.yellow);
    }

    public void deselectionner() {
        this.isSelectionnee = false;
        this.setBackground(this.couleur);
    }

    public Piece getPiece() {
        return this.piece;
    }
    
}
