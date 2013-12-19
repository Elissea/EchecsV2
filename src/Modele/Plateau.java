package Modele;

import Common.Coordonnee;
import Common.Couleur;
import Common.Valeur;
import java.util.ArrayList;

public class Plateau {

    private Case[][] cases;
    private ArrayList<Piece> piecesBlanches;
    private ArrayList<Piece> piecesNoires;
    private ArrayList<Piece> capturesBlanches;
    private ArrayList<Piece> capturesNoires;
    private Piece roiBlanc;
    private Piece roiNoir;

    public Plateau() {
        this.cases = new Case[8][8];
        this.piecesBlanches = new ArrayList<>();
        this.piecesNoires = new ArrayList<>();
    }

    public void initialiser() {
        this.piecesBlanches.clear();
        this.piecesNoires.clear();
        
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                this.cases[i][j] = new Case(new Coordonnee(i, j));
            }
        }

        for (int i = 0; i < 8; i++) {
            Piece pionBlanc = new Piece(Valeur.PION, Couleur.BLANC, new Coordonnee(1, i));
            Piece pionNoir = new Piece(Valeur.PION, Couleur.NOIR, new Coordonnee(6, i));

            this.piecesBlanches.add(pionBlanc);
            this.piecesNoires.add(pionNoir);

            this.cases[1][i].setPiece(pionBlanc);
            this.cases[6][i].setPiece(pionNoir);
        }

        Piece btour1 = new Piece(Valeur.TOUR, Couleur.BLANC, new Coordonnee(0, 0));
        Piece bcavalier1 = new Piece(Valeur.CAVALIER, Couleur.BLANC, new Coordonnee(0, 1));
        Piece bfou1 = new Piece(Valeur.FOU, Couleur.BLANC, new Coordonnee(0, 2));
        Piece broi = new Piece(Valeur.ROI, Couleur.BLANC, new Coordonnee(0, 3));
        Piece bdame = new Piece(Valeur.DAME, Couleur.BLANC, new Coordonnee(0, 4));
        Piece bfou2 = new Piece(Valeur.FOU, Couleur.BLANC, new Coordonnee(0, 5));
        Piece bcavalier2 = new Piece(Valeur.CAVALIER, Couleur.BLANC, new Coordonnee(0, 6));
        Piece btour2 = new Piece(Valeur.TOUR, Couleur.BLANC, new Coordonnee(0, 7));

        this.roiBlanc = broi;

        this.piecesBlanches.add(btour1);
        this.piecesBlanches.add(bcavalier1);
        this.piecesBlanches.add(bfou1);
        this.piecesBlanches.add(bdame);
        this.piecesBlanches.add(broi);
        this.piecesBlanches.add(bfou2);
        this.piecesBlanches.add(bcavalier2);
        this.piecesBlanches.add(btour2);

        this.cases[0][0].setPiece(btour1);
        this.cases[0][1].setPiece(bcavalier1);
        this.cases[0][2].setPiece(bfou1);
        this.cases[0][3].setPiece(broi);
        this.cases[0][4].setPiece(bdame);
        this.cases[0][5].setPiece(bfou2);
        this.cases[0][6].setPiece(bcavalier2);
        this.cases[0][7].setPiece(btour2);

        Piece ntour1 = new Piece(Valeur.TOUR, Couleur.NOIR, new Coordonnee(7, 0));
        Piece ncavalier1 = new Piece(Valeur.CAVALIER, Couleur.NOIR, new Coordonnee(7, 1));
        Piece nfou1 = new Piece(Valeur.FOU, Couleur.NOIR, new Coordonnee(7, 2));
        Piece nroi = new Piece(Valeur.ROI, Couleur.NOIR, new Coordonnee(7, 3));
        Piece ndame = new Piece(Valeur.DAME, Couleur.NOIR, new Coordonnee(7, 4));
        Piece nfou2 = new Piece(Valeur.FOU, Couleur.NOIR, new Coordonnee(7, 5));
        Piece ncavalier2 = new Piece(Valeur.CAVALIER, Couleur.NOIR, new Coordonnee(7, 6));
        Piece ntour2 = new Piece(Valeur.TOUR, Couleur.NOIR, new Coordonnee(7, 7));

        this.roiNoir = nroi;

        this.piecesNoires.add(ntour1);
        this.piecesNoires.add(ncavalier1);
        this.piecesNoires.add(nfou1);
        this.piecesNoires.add(nroi);
        this.piecesNoires.add(ndame);
        this.piecesNoires.add(nfou2);
        this.piecesNoires.add(ncavalier2);
        this.piecesNoires.add(ntour2);

        this.cases[7][0].setPiece(ntour1);
        this.cases[7][1].setPiece(ncavalier1);
        this.cases[7][2].setPiece(nfou1);
        this.cases[7][3].setPiece(nroi);
        this.cases[7][4].setPiece(ndame);
        this.cases[7][5].setPiece(nfou2);
        this.cases[7][6].setPiece(ncavalier2);
        this.cases[7][7].setPiece(ntour2);
    }

    public boolean estDansLePlateau(Coordonnee coordonnee) {
        return (coordonnee.getLigne() >= 0 && coordonnee.getLigne() < 8
                && coordonnee.getColonne() >= 0 && coordonnee.getColonne() < 8);
    }

    public Case getCaseParCoordonnee(Coordonnee coordonnee) {
        return this.cases[coordonnee.getLigne()][coordonnee.getColonne()];
    }

    public Case[][] getCases() {
        return this.cases;
    }

    public Piece getRoiBlanc() {
        return this.roiBlanc;
    }

    public Piece getRoiNoir() {
        return this.roiNoir;
    }

    public ArrayList<Piece> getPiecesBlanches() {
        return this.piecesBlanches;
    }

    public ArrayList<Piece> getPiecesNoires() {
        return this.piecesNoires;
    }

    public ArrayList<Piece> getCapturesBlanches() {
        return this.capturesBlanches;
    }
    
    public ArrayList<Piece> getCapturesNoires() {
        return this.capturesNoires;
    }
    
    public void capturerPiece(Piece piece) {
        if (piece.getCouleur() == Couleur.BLANC) {
            this.piecesBlanches.remove(piece);
         //   this.capturesBlanches.add(piece);
        } else {
            this.piecesNoires.remove(piece);
          //  this.capturesNoires.add(piece);
        }
    }
}
