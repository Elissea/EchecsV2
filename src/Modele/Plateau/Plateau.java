package Modele.Plateau;

import Common.Couleur;
import Modele.Deplacement;
import Modele.Plateau.Pieces.Cavalier;
import Modele.Plateau.Pieces.Dame;
import Modele.Plateau.Pieces.Denominations;
import Modele.Plateau.Pieces.Fou;
import Modele.Plateau.Pieces.Piece;
import Modele.Plateau.Pieces.Pion;
import Modele.Plateau.Pieces.Roi;
import Modele.Plateau.Pieces.Tour;
import Modele.Plateau.Pieces.Vide;
import java.awt.Point;
import java.util.ArrayList;

public class Plateau {

    private int taille;
    private ArrayList<Piece> pieces;
    private ArrayList<Piece> captures;
    private ArrayList<Point> deplacementsPossibles;
    private Piece selectionnee;

    public Plateau(int taille) {
        this.taille = taille;
        this.pieces = new ArrayList<>();
        this.captures = new ArrayList<>();
        this.selectionnee = Vide.getInstance();
    }

    public void initialiser() {
        this.pieces.clear();
        this.selectionnee = Vide.getInstance();

        for (int k = 0; k < this.taille; k++) {
            this.pieces.add(new Pion(new Point(1, k), Denominations.Pion, Couleur.NOIR));
            this.pieces.add(new Pion(new Point(6, k), Denominations.Pion, Couleur.BLANC));
        }

        int ligne = 0;
        boolean couleur = Couleur.NOIR;

        for (int j = 0; j < 2; j++) {
            for (int i = 0; i < this.taille; i++) {
                switch (i) {
                    case 0:
                    case 7:
                        this.pieces.add(new Tour(new Point(ligne, i), Denominations.Tour, couleur));
                        break;

                    case 1:
                    case 6:
                        this.pieces.add(new Cavalier(new Point(ligne, i), Denominations.Cavalier, couleur));
                        break;

                    case 2:
                    case 5:
                        this.pieces.add(new Fou(new Point(ligne, i), Denominations.Fou, couleur));
                        break;

                    case 3:
                        this.pieces.add(new Roi(new Point(ligne, i), Denominations.Roi, couleur));
                        break;

                    case 4:
                        this.pieces.add(new Dame(new Point(ligne, i), Denominations.Dame, couleur));
                        break;
                }

            }

            ligne = 7;
            couleur = !couleur;
        }
    }

    public ArrayList<Piece> getPieces() {
        return this.pieces;
    }

    public void selectionnerPiece(Piece piece) {
        this.selectionnee = piece;
        this.checkPieceSelectionnee(piece);
    }

    public Piece getPieceSelectionnee() {
        return this.selectionnee;
    }

    public void checkPieceSelectionnee(Piece piece) {
        this.deplacementsPossibles = piece.getDeplacements();;
    }

    public ArrayList<Point> getDeplacementsPossibles() {
        return this.deplacementsPossibles;
    }

    public void clearDeplacementsPossibles() {
        this.deplacementsPossibles.clear();
    }

    public Piece getPieceByPoint(Point point) {
        int i = 0;

        while (i < this.pieces.size()) {
            if (this.pieces.get(i).getPosition() == point) {
                return this.pieces.get(i);
            }
            i++;
        }

        return null;
    }

    public void capturer(Piece piece) {
        this.captures.add(piece);
        this.pieces.remove(piece);
    }

    public void effectuerDeplacement(Deplacement deplacement) {
        if (this.getPieceByPoint(deplacement.getArrivee()) != null) {
            this.getPieceByPoint(deplacement.getArrivee());
        }

        this.getPieceByPoint(deplacement.getDepart()).deplacer(deplacement.getArrivee());
        this.selectionnee = Vide.getInstance();
    }
}
