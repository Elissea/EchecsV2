package Modele.Joueur.Joueurs;

import Modele.Deplacement;
import Modele.Joueur.Joueur;
import Modele.Plateau.Pieces.Piece;
import java.awt.Point;
import java.util.ArrayList;

public class OrdinateurFacile extends Joueur {
    public OrdinateurFacile(ArrayList<Piece> pieces, boolean couleur,boolean isOrdinateur) {
        super("Ordinateur Facile", 50, pieces, couleur, isOrdinateur);
    }

    @Override
    public Deplacement jouer() {
        return new Deplacement(this.pieces.get(0).getPosition(), new Point(3, 4));
    }

    
}
