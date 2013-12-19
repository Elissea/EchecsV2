package Vue;

import Common.Coordonnee;
import Common.Couleur;
import Common.Valeur;
import Modele.Plateau;
import Modele.Case;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics2D;
import java.awt.GridLayout;
import java.awt.Toolkit;
import javax.swing.Icon;
import javax.swing.ImageIcon;
import javax.swing.JPanel;

public class Echiquier extends JPanel {

    private Plateau plateau;
    private Vue.Case[][] cases;

    public Echiquier(Plateau plateau) {
        this.plateau = plateau;
        this.cases = new Vue.Case[8][8];

        this.setLayout(new GridLayout(8, 8));

        for (int i = 7; i >= 0; i--) {
            for (int j = 0; j < 8; j++) {
                if ((i + j) % 2 == 0) {
                    this.cases[i][j] = new Vue.Case(Color.WHITE, new Coordonnee(i, j));
                } else {
                    this.cases[i][j] = new Vue.Case(Color.BLACK, new Coordonnee(i, j));
                }

                this.cases[i][j].setText(i + "/" + j);
                this.cases[i][j].setForeground(Color.lightGray);
                this.add(this.cases[i][j]);
            }
        }

        this.setPreferredSize(new Dimension(800, 800));
        this.setOpaque(true);
    }

    public void paintComponent(Graphics2D g) {
        Graphics2D g2d = (Graphics2D) getGraphics();
        g2d.scale(1, -1);
    }

    public void initialiser() {
        Toolkit toolkit = getToolkit();

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                
                Case emplacement = this.plateau.getCaseParCoordonnee(new Coordonnee(i, j));
                                
                String imageUrl = emplacement.getPiece().getCouleur() == Couleur.BLANC ? "/Images/Blancs/" : "/Images/Noirs/";
                Icon icon;

                switch (emplacement.getPiece().getValeur()) {
                    case Valeur.PION:
                        icon = new ImageIcon(getClass().getResource(imageUrl + "pion.gif"));
                        this.cases[i][j].setImage(icon);
                        break;
                    case Valeur.TOUR:
                        icon = new ImageIcon(getClass().getResource(imageUrl + "tour.gif"));
                        this.cases[i][j].setImage(icon);
                        break;
                    case Valeur.CAVALIER:
                        icon = new ImageIcon(getClass().getResource(imageUrl + "cavalier.gif"));
                        this.cases[i][j].setImage(icon);
                        break;
                    case Valeur.FOU:
                        icon = new ImageIcon(getClass().getResource(imageUrl + "fou.gif"));
                        this.cases[i][j].setImage(icon);
                        break;
                    case Valeur.ROI:
                        icon = new ImageIcon(getClass().getResource(imageUrl + "roi.gif"));
                        this.cases[i][j].setImage(icon);
                        break;
                    case Valeur.DAME:
                        icon = new ImageIcon(getClass().getResource(imageUrl + "reine.gif"));
                        this.cases[i][j].setImage(icon);
                        break;
                    case Valeur.VIDE :
                        this.cases[i][j].setVide();
                        break;
                }
            }
        }
    }
        
    public Vue.Case[][] getCases() {
        return this.cases;
    }
}
