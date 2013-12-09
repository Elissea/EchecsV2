package echecefrei;

import Common.Couleur;
import Controleur.ControleurPartie;
import Modele.Joueur.Joueurs.Humain;
import Modele.Joueur.Joueurs.OrdinateurFacile;
import Modele.Plateau.Pieces.Piece;
import Modele.Plateau.Plateau;
import Vue.Echiquier;
import Vue.Fenetre;
import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.GridLayout;
import java.util.ArrayList;
import javax.swing.JButton;
import javax.swing.JFrame;

public class EchecEfrei {

    public static void main(String[] args) {
        Plateau plateau = new Plateau(8);
        Fenetre fenetre = new Fenetre(plateau);

        ArrayList<Piece> piecesBlanches = new ArrayList<>();
        ArrayList<Piece> piecesNoires = new ArrayList<>();

        for (Piece p : plateau.getPieces()) {
            if (p.getCouleur() == Couleur.BLANC) {
                piecesBlanches.add(p);
            } else {
                piecesNoires.add(p);
            }


        }

        ControleurPartie controleur = new ControleurPartie(plateau, fenetre);
    }
}
