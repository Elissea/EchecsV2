package Vue;

import Common.Couleur;
import Modele.Deplacement;
import Modele.Plateau.Pieces.Piece;
import Modele.Plateau.Pieces.Vide;
import Modele.Plateau.Plateau;
import java.awt.Color;
import java.awt.GridLayout;
import java.awt.Point;
import java.awt.event.MouseListener;
import javax.swing.JPanel;

public class Echiquier extends JPanel {

    private Plateau plateau;
    private int taille;
    private Case[][] cases;
    private boolean couleurActive;

    public Echiquier(int taille, Plateau plateau, boolean couleurActive) {
        this.plateau = plateau;
        this.taille = taille;
        this.couleurActive = couleurActive;

        this.cases = new Case[taille][taille];

        this.setLayout(new GridLayout(taille, taille));

        for (int i = 0; i < taille; i++) {
            for (int j = 0; j < taille; j++) {
                if ((i + j) % 2 == 0) {
                    this.cases[i][j] = new Case(Color.WHITE, i, j);
                } else {
                    this.cases[i][j] = new Case(Color.BLACK, i, j);
                }

                this.add(this.cases[i][j]);
            }
        }

    }

    public void initialiser() {
        this.couleurActive = Couleur.BLANC;
        
        for (int i = 0; i < taille; i++) {
            for (int j = 0; j < taille; j++) {
                if ((i + j) % 2 == 0) {
                    this.cases[i][j].setPiece(Vide.getInstance());
                } else {
                    this.cases[i][j].setPiece(Vide.getInstance()); 
                }
                
                this.cases[i][j].deselectionner();
               
            }
        }
 
        for (Piece piece : this.plateau.getPieces()) {
            int x = (int) piece.getPosition().getX();
            int y = (int) piece.getPosition().getY();
            this.cases[x][y].setPiece(piece);
        }

    }

    
    public void switchCouleur() {
        this.couleurActive = !this.couleurActive;
    }
    
    public boolean getCouleurActive() {
       return this.couleurActive;
    }
    
    public Case getCase(Point coordonnees) {
        return this.cases[(int) coordonnees.getX()][(int) coordonnees.getY()];
    }

    public void effectuerDeplacement(Deplacement deplacement) {
        Case case_depart = this.cases[(int) deplacement.getDepart().getX()][(int) deplacement.getDepart().getY()];
        Case case_arrivee = this.cases[(int) deplacement.getArrivee().getX()][(int) deplacement.getArrivee().getY()];

        case_arrivee.setPiece(case_depart.getPiece());
        case_arrivee.deselectionner();

        case_depart.setPiece(Vide.getInstance());
        case_depart.deselectionner();
    }

    public void addClicSurCaseListener(MouseListener clicSurCaseListener) {
        for (int i = 0; i < this.taille; i++) {
            for (int j = 0; j < this.taille; j++) {
                this.cases[i][j].addMouseListener(clicSurCaseListener);
            }
        }
    }
}
