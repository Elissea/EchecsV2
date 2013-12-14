package echecefrei;

import Modele.Plateau;
import Vue.Fenetre;

public class EchecEfrei {

    public static void main(String[] args) {
        Plateau plateau = new Plateau();
        Fenetre fenetre = new Fenetre(plateau);
    }
}
