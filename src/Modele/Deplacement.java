package Modele;

import Common.Coordonnee;
import Modele.Joueur.Joueur;

public class Deplacement {

    private Joueur joueur;
    private Coordonnee depart;
    private Coordonnee arrivee;
    private Piece attaquant;
    private Piece capture;

    public Deplacement(Joueur joueur, Coordonnee depart, Coordonnee arrivee, Piece attaquant, Piece capture) {
        this.joueur = joueur;
        this.attaquant = attaquant;
        this.capture = capture;
        this.depart = depart;
        this.arrivee = arrivee;
    }

    public Deplacement(Coordonnee depart, Coordonnee arrivee, Piece attaquant, Piece capture) {
        this.attaquant = attaquant;
        this.capture = capture;
        this.depart = depart;
        this.arrivee = arrivee;
    }

    public Joueur getJoueur() {
        return this.joueur;
    }

    public void setJoueur(Joueur joueur) {
        this.joueur = joueur;
    }

    public Coordonnee getDepart() {
        return this.depart;
    }

    public void setDepart(Coordonnee depart) {
        this.depart = depart;
    }

    public Coordonnee getArrivee() {
        return this.arrivee;
    }

    public void setArrivee(Coordonnee arrivee) {
        this.arrivee = arrivee;
    }

    public Piece getAttaquant() {
        return this.attaquant;
    }

    public void setAttaquant(Piece attaquant) {
        this.attaquant = attaquant;
    }

    public Piece getCapture() {
        return this.capture;
    }

    public void setCapture(Piece capture) {
        this.capture = capture;
    }
}