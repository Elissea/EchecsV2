package Modele.Joueur;


public abstract class Joueur {

    private int couleur; 
    
    public Joueur(int couleur) {
        this.couleur = couleur;
    }
    
    public int getCouleur() {
        return this.couleur;
    }
    

}
