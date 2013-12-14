package Common;

public class Coordonnee {
    private int ligne;
    private int colonne;
    
    public Coordonnee(int ligne, int colonne) {
        this.ligne = ligne;
        this.colonne = colonne;
    }

    public int getLigne() {
        return this.ligne;
    }

    public void setLigne(int ligne) {
        this.ligne = ligne;
    }

    public int getColonne() {
        return this.colonne;
    }

    public void setColonne(int colonne) {
        this.colonne = colonne;
    }
    
    @Override
    public String toString() {
        return this.ligne + " / " + this.colonne; 
    }
}
