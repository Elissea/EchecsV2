package Controleur;

import Common.Coordonnee;
import Common.Couleur;
import Common.Valeur;
import Modele.Deplacement;
import Modele.Joueur.Joueur;
import Modele.Piece;
import Modele.Plateau;
import Modele.Vide;
import java.util.ArrayList;

public class ControleurDeplacement {

    private Plateau plateau;

    public ControleurDeplacement(Plateau plateau) {
        this.plateau = plateau;
    }

    public Deplacement effectuerDeplacement(Coordonnee depart, Coordonnee arrivee) {
        Piece source = this.plateau.getCaseParCoordonnee(depart).getPiece();
        Piece destination = this.plateau.getCaseParCoordonnee(arrivee).getPiece();

        boolean capture = !destination.equals(Vide.getInstance());

        Deplacement deplacement = new Deplacement(depart, arrivee, source, destination);

        source.setCoordonnee(arrivee);

        this.plateau.getCaseParCoordonnee(arrivee).setPiece(source);
        this.plateau.getCaseParCoordonnee(depart).setPiece(Vide.getInstance());

        if (capture) {
            this.plateau.capturerPiece(destination);
        }

        return deplacement;

    }

    public boolean validerDeplacement(Joueur joueur, Coordonnee depart, Coordonnee arrivee) {
        Piece source = this.plateau.getCaseParCoordonnee(depart).getPiece();
        Piece destination = this.plateau.getCaseParCoordonnee(arrivee).getPiece();

        if (source.getValeur() != Valeur.VIDE) {
            if (source.getCouleur() == joueur.getCouleur()) {
                ArrayList<Coordonnee> deplacementsPossibles = getDeplacementsPossibles(source);

                if (deplacementsPossibles.contains(arrivee)) {
                    boolean capture = !destination.equals(Vide.getInstance());

                    Deplacement deplacement = new Deplacement(joueur, depart, arrivee, source, destination);

                    source.setABouge(true);
                    source.setCoordonnee(arrivee);

                    this.plateau.getCaseParCoordonnee(arrivee).setPiece(source);
                    this.plateau.getCaseParCoordonnee(depart).setPiece(Vide.getInstance());

                    if (capture) {
                        this.plateau.capturerPiece(destination);
                    }

                    if (roiMenace(source.getCouleur())) {
                        this.annulerDeplacement(deplacement);
                    } else {
                        return true;
                    }
                }

            }

        }

        return false;
    }

    public void annulerDeplacement(Deplacement deplacement) {
        if (!deplacement.getCapture().equals(Vide.getInstance())) {
            if (deplacement.getAttaquant().getCouleur() == Couleur.BLANC) {
                this.plateau.getPiecesNoires().add(deplacement.getCapture());
            } else {
                this.plateau.getPiecesBlanches().add(deplacement.getCapture());
            }
        }

        deplacement.getAttaquant().setCoordonnee(deplacement.getDepart());
        deplacement.getCapture().setCoordonnee(deplacement.getArrivee());
        this.plateau.getCaseParCoordonnee(deplacement.getArrivee()).setPiece(deplacement.getCapture());
        this.plateau.getCaseParCoordonnee(deplacement.getDepart()).setPiece(deplacement.getAttaquant());
    }

    public boolean estDansLePlateau(Coordonnee coordonnee) {
        return this.plateau.estDansLePlateau(coordonnee);
    }

    public ArrayList<Coordonnee> getDeplacementsPossibles(Piece source) {
        ArrayList<Coordonnee> mouvementsPossibles = new ArrayList<>();
        ArrayList<Coordonnee> mouvementsPossiblesRoiOK = new ArrayList<>();
        Deplacement deplacement;

        switch (source.getValeur()) {
            case Valeur.PION:
                mouvementsPossibles = mouvementsPion(source);
                break;
            case Valeur.TOUR:
                mouvementsPossibles = mouvementsTour(source);
                break;
            case Valeur.CAVALIER:
                mouvementsPossibles = mouvementsCavalier(source);
                break;
            case Valeur.FOU:
                mouvementsPossibles = mouvementsFou(source);
                break;
            case Valeur.ROI:
                mouvementsPossibles = mouvementsRoi(source);
                break;
            case Valeur.DAME:
                mouvementsPossibles = mouvementsDame(source);
                break;
        }

        for (Coordonnee c : mouvementsPossibles) {
            deplacement = this.effectuerDeplacement(source.getCoordonnee(), c);

            if (!roiMenace(source.getCouleur())) {
                mouvementsPossiblesRoiOK.add(c);
            }

            this.annulerDeplacement(deplacement);
        }

        return mouvementsPossiblesRoiOK;
    }

    public boolean caseOccupee(Coordonnee coordonnee) {
        return this.plateau.getCaseParCoordonnee(coordonnee).getPiece().getValeur() != Valeur.VIDE;
    }

    public boolean caseOccupee(Coordonnee coordonnee, int couleur) {
        return this.plateau.getCaseParCoordonnee(coordonnee).getPiece().getValeur() != Valeur.VIDE && this.plateau.getCaseParCoordonnee(coordonnee).getPiece().getCouleur() == couleur;
    }

    public boolean roiMenace(int couleur) {
        ArrayList<Piece> ennemis = couleur == Couleur.BLANC ? this.plateau.getPiecesNoires() : this.plateau.getPiecesBlanches();
        ArrayList<Coordonnee> mouvementsPossibles = new ArrayList<>();
        Piece roi = couleur == Couleur.BLANC ? this.plateau.getRoiBlanc() : this.plateau.getRoiNoir();

        for (Piece piece : ennemis) {
            switch (piece.getValeur()) {
                case Valeur.PION:
                    mouvementsPossibles = mouvementsPion(piece);
                    break;
                case Valeur.TOUR:
                    mouvementsPossibles = mouvementsTour(piece);
                    break;
                case Valeur.CAVALIER:
                    mouvementsPossibles = mouvementsCavalier(piece);
                    break;
                case Valeur.FOU:
                    mouvementsPossibles = mouvementsFou(piece);
                    break;
                case Valeur.ROI:
                    mouvementsPossibles = mouvementsRoi(piece);
                    break;
                case Valeur.DAME:
                    mouvementsPossibles = mouvementsDame(piece);
                    break;
            }

            if (mouvementsPossibles.contains(roi.getCoordonnee())) {
                return true;
            }
        }

        return false;
    }

    public ArrayList<Coordonnee> mouvementsPion(Piece piece) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;

        if (piece.getCouleur() == Couleur.BLANC) {

            /* AVANCE D'UNE CASE */
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);

                /* AVANCE DE DEUX CASES SI PREMIER TOUR */
                if (!piece.getABouge()) {

                    nouvellePosition = new Coordonnee(positionPiece.getLigne() + 2, positionPiece.getColonne());

                    if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                        mouvements.add(nouvellePosition);
                    }
                }
            }

            /* AVANCE EN DIAGONALE SI ENNEMI A CAPTURER */
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() + 1);

            if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }

            nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() - 1);

            if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }
        } else {
            /* AVANCE D'UNE CASE */
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);

                /* AVANCE DE DEUX CASES SI PREMIER TOUR */
                if (!piece.getABouge()) {

                    nouvellePosition = new Coordonnee(positionPiece.getLigne() - 2, positionPiece.getColonne());

                    if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                        mouvements.add(nouvellePosition);
                    }
                }
            }

            /* AVANCE EN DIAGONALE SI ENNEMI A CAPTURER */
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() - 1);

            if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }

            nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() + 1);

            if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }
        }

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsTour(Piece piece) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;

        /* AVANT */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + i, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* ARRIERE */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - i, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* DROITE */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + i, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* GAUCHE */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - i, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsCavalier(Piece piece) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;


        /* DROITE HAUT */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 2, positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* GAUCHE HAUT */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 2, positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* DROITE BAS */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 2, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* GAUCHE BAS  */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 2, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* HAUT DROITE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() + 2);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* HAUT GAUCHE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() + 2);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* BAS DROITE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() - 2);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* BAS GAUCHE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() - 2);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsFou(Piece piece) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;


        /* DIAGONALE HAUT DROITE */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + i, positionPiece.getColonne() + i);

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* DIAGONALE HAUT GAUCHE */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - i, positionPiece.getColonne() + i);

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* DIAGONALE BAS DROITE */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + i, positionPiece.getColonne() - i);

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* DIAGONALE BAS GAUCHE */
        for (int i = 1; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - i, positionPiece.getColonne() - i);

            if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && caseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsDame(Piece piece) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        mouvements.addAll(mouvementsFou(piece));
        mouvements.addAll(mouvementsTour(piece));

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsRoi(Piece piece) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;

        nouvellePosition = new Coordonnee(positionPiece.getLigne(), positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne(), positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne());

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne());

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !caseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        }

        return mouvements;
    }
}
