package Controleur;

import Common.Couleur;
import Modele.Deplacement;
import Modele.Joueur.Joueur;
import Modele.Joueur.Joueurs.Humain;
import Modele.Joueur.Joueurs.OrdinateurFacile;
import Modele.Plateau.Pieces.Piece;
import Modele.Plateau.Pieces.Vide;
import Modele.Plateau.Plateau;
import Vue.Case;
import Vue.Fenetre;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.util.ArrayList;
import java.util.logging.Level;
import java.util.logging.Logger;

public class ControleurPartie {

    private Plateau plateau;
    private Fenetre fenetre;
    private boolean tour;
    private Joueur j1, j2, joueurCourant, joueurGagnant;

    public ControleurPartie(Plateau plateau, Fenetre fenetre) {
        this.plateau = plateau;
        this.fenetre = fenetre;
        this.fenetre.addNouvellePartieListener(new nouvellePartieListener());
        this.fenetre.getEchiquier().addClicSurCaseListener(new clicSurCaseListener());
    }

    public void demarrerPartie() {
        this.plateau.initialiser();
        this.fenetre.getEchiquier().initialiser();
        this.joueurCourant = this.j1;

        ArrayList<Piece> piecesBlanches = new ArrayList<>();
        ArrayList<Piece> piecesNoires = new ArrayList<>();

        for (Piece p : plateau.getPieces()) {
            if (p.getCouleur() == Couleur.BLANC) {
                piecesBlanches.add(p);
            } else {
                piecesNoires.add(p);
            }
        }

        this.j1 = new Humain(piecesBlanches, Couleur.BLANC, false);
        this.j2 = new Humain(piecesNoires, Couleur.NOIR, false);

        this.joueurCourant = this.j1;

        this.jouer();
    }

    public void jouer() {
        Deplacement deplacement = this.joueurCourant.jouer();
        this.effectuerDeplacement(deplacement);
    }

    public boolean echecEtMat() {
        return false;
    }

    public void changerTour() {
        if (this.joueurCourant == this.j1) {
            this.joueurCourant = this.j2;
        } else {
            this.joueurCourant = this.j1;
        }

        this.jouer();
    }

    public void effectuerDeplacement(Deplacement deplacement) {
        if (deplacement != null) {
            plateau.effectuerDeplacement(deplacement);
            fenetre.getEchiquier().effectuerDeplacement(deplacement);
            this.changerTour();
        }
    }

    public void effectuerCapture(Piece depart, Piece arrivee) {
        this.changerTour();
    }

    class nouvellePartieListener implements ActionListener {

        @Override
        public void actionPerformed(ActionEvent e) {
            demarrerPartie();
        }
    }

    class clicSurCaseListener implements MouseListener {

        @Override
        public void mouseClicked(MouseEvent e) {
        }

        @Override
        public void mousePressed(MouseEvent e) {
        }

        @Override
        public void mouseReleased(MouseEvent e) {
            if (!joueurCourant.isOrdinateur()) {
                Case case_selectionnee = (Case) e.getComponent();
                Piece case_piece = case_selectionnee.getPiece();
                Piece piece_selectionnee = plateau.getPieceSelectionnee();

                // S'il y a une pièce sur la case sélectionnée
                if (!case_piece.equals(Vide.getInstance())) {

                    // Si aucune pièce n'a encore été selectionnée 
                    if (piece_selectionnee.equals(Vide.getInstance())) {

                        // Si la pièce sélectionnée est bien la couleur active
                        if (case_piece.getCouleur() == joueurCourant.getCouleur()) {
                            plateau.selectionnerPiece(case_piece);
                            case_selectionnee.selectionner();

                            for (Point p : plateau.getDeplacementsPossibles()) {
                                fenetre.getEchiquier().getCase(p).selectionner();
                            }
                        }

                        // Si une pièce a déjà été sélectionnée
                    } else {
                        // Si la pièce sélectionnée est bien la couleur active
                        if (case_piece.getCouleur() == joueurCourant.getCouleur()) {

                            // Si c'est la même pièce
                            if (piece_selectionnee.equals(case_piece)) {
                                case_selectionnee.deselectionner();

                                for (Point p : plateau.getDeplacementsPossibles()) {
                                    fenetre.getEchiquier().getCase(p).deselectionner();
                                }

                                plateau.clearDeplacementsPossibles();
                                plateau.selectionnerPiece(Vide.getInstance());
                            } else {
                                case_selectionnee.selectionner();
                                fenetre.getEchiquier().getCase(plateau.getPieceSelectionnee().getPosition()).deselectionner();

                                for (Point p : plateau.getDeplacementsPossibles()) {
                                    fenetre.getEchiquier().getCase(p).deselectionner();
                                }
                                plateau.clearDeplacementsPossibles();

                                plateau.selectionnerPiece(case_piece);

                                for (Point p : plateau.getDeplacementsPossibles()) {
                                    fenetre.getEchiquier().getCase(p).selectionner();
                                }
                            }
                        } else {
                            if (plateau.getDeplacementsPossibles().contains(case_selectionnee.getPosition())) {
                                for (Point p : plateau.getDeplacementsPossibles()) {
                                    fenetre.getEchiquier().getCase(p).deselectionner();
                                }

                                plateau.clearDeplacementsPossibles();
                                effectuerDeplacement(new Deplacement(plateau.getPieceSelectionnee().getPosition(), case_selectionnee.getPosition()));

                                plateau.selectionnerPiece(Vide.getInstance());
                            }
                        }
                    }
                    // S'il s'agit d'un emplacement vide
                } else {
                    // S'il y a bien une pièce déjà sélectionnée
                    if (!piece_selectionnee.equals(Vide.getInstance())) {

                        case_selectionnee.deselectionner();

                        // Si l'emplacement fait bien partie des emplacements possibles
                        if (plateau.getDeplacementsPossibles().contains(case_selectionnee.getPosition())) {
                            for (Point p : plateau.getDeplacementsPossibles()) {
                                fenetre.getEchiquier().getCase(p).deselectionner();
                            }

                            plateau.clearDeplacementsPossibles();
                            effectuerDeplacement(new Deplacement(plateau.getPieceSelectionnee().getPosition(), case_selectionnee.getPosition()));

                            plateau.selectionnerPiece(Vide.getInstance());
                        }
                    }
                }

                fenetre.validate();
            }
        }

        @Override
        public void mouseEntered(MouseEvent e) {
        }

        @Override
        public void mouseExited(MouseEvent e) {
        }
    }
}