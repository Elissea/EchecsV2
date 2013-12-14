/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Vue;

import Common.Coordonnee;
import Common.Couleur;
import Modele.Piece;
import Modele.Vide;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Point;
import javax.swing.Icon;
import javax.swing.ImageIcon;
import javax.swing.JLabel;
import static javax.swing.SwingConstants.CENTER;
import javax.swing.border.Border;

/**
 *
 * @author Elissea
 */
public class Case extends JLabel {

    private Coordonnee coordonnee;

    public Case(Color couleur, Coordonnee coordonnee) {
        this.coordonnee = coordonnee;

        this.setPreferredSize(new Dimension(75, 75));
        this.setBackground(couleur);
        this.setOpaque(true);
    }

    public Coordonnee getCoordonnee() {
        return this.coordonnee;
    }

    public void setImage(Icon icone) {
        this.setIcon(icone);
    }

    public void selectionner() {
        this.setBorder(javax.swing.BorderFactory.createLineBorder(Color.ORANGE, 5));
    }

    public void deselectionner() {
        this.setBorder(javax.swing.BorderFactory.createLineBorder(Color.ORANGE, 0));
    }
}
