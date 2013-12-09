package Modele.Joueur.Joueurs;

import Modele.Deplacement;
import Modele.Joueur.Joueur;
import Modele.Plateau.Pieces.Piece;
import java.util.ArrayList;

public class Humain extends Joueur {

    public Humain(ArrayList<Piece> pieces, boolean couleur, boolean isOrdinateur) {
        super("Humain", 50, pieces, couleur, isOrdinateur);
    }

    @Override
    public Deplacement jouer() {
        return null;
    }
}
