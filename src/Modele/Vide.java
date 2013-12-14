package Modele;

import Common.Valeur;

public class Vide extends Piece {
    private static Vide vide = null;
    
    private Vide() {
        super(Valeur.VIDE);
    }
    
    public static Vide getInstance() {
        if(vide == null) {
            vide = new Vide();
        }
        
        return vide;
    }
          
}
