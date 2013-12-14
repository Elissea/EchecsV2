package Main;

import Modele.Plateau;
import Vue.Fenetre;

public class Main {

    public static void main(String[] args) {
        Plateau plateau = new Plateau();
        Fenetre fenetre = new Fenetre(plateau);
    }
}
