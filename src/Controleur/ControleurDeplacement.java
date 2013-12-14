package Controleur;

import Common.Coordonnee;
import Common.Couleur;
import Common.Valeur;
import Modele.Joueur.Joueur;
import Modele.Piece;
import Modele.Plateau;
import java.util.ArrayList;

public class ControleurDeplacement {

    private Plateau plateau;

    public ControleurDeplacement(Plateau plateau) {
        this.plateau = plateau;
    }

    public boolean effectuerDeplacement(Joueur joueur, Coordonnee depart, Coordonnee arrivee) {
        Piece source = this.plateau.getCaseParCoordonnee(depart).getPiece();

        if (source.getValeur() != Valeur.VIDE) {
            if (source.getCouleur() == joueur.getCouleur()) {
                ArrayList<Coordonnee> deplacementsPossibles = getDeplacementsPossibles(source, false);
            }

        }

        return false;
    }

    public boolean estDansLePlateau(Coordonnee coordonnee) {
        return this.plateau.estDansLePlateau(coordonnee);
    }

    public ArrayList<Coordonnee> getDeplacementsPossibles(Piece source, boolean captureRoi) {
        switch (source.getValeur()) {
            case Valeur.PION:
                return mouvementsPion(source, captureRoi);
            case Valeur.TOUR:
                return mouvementsTour(source, captureRoi);
            case Valeur.CAVALIER:
                return mouvementsCavalier(source, captureRoi);
            case Valeur.FOU:
                return mouvementsFou(source, captureRoi);
            case Valeur.ROI:
                return mouvementsRoi(source, captureRoi);
            case Valeur.DAME:
                return mouvementsDame(source, captureRoi);
        }
        return null;
    }

    public boolean estCaseOccupee(Coordonnee coordonnee) {
        return this.plateau.getCaseParCoordonnee(coordonnee).getPiece().getValeur() != Valeur.VIDE;
    }

    public boolean estCaseOccupee(Coordonnee coordonnee, int couleur) {
        return this.plateau.getCaseParCoordonnee(coordonnee).getPiece().getValeur() != Valeur.VIDE && this.plateau.getCaseParCoordonnee(coordonnee).getPiece().getCouleur() == couleur;
    }

    public ArrayList<Coordonnee> mouvementsPion(Piece piece, boolean captureRoi) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;


        if (piece.getCouleur() == Couleur.BLANC) {

            /* AVANCE D'UNE CASE */
            nouvellePosition = new Coordonnee(positionPiece.getLigne()+1, positionPiece.getColonne());
            
            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            }

            /* AVANCE DE DEUX CASES SI PREMIER TOUR */
            if (!piece.getABouge()) {

                nouvellePosition = new Coordonnee(positionPiece.getLigne()+2, positionPiece.getColonne());

                if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
                    mouvements.add(nouvellePosition);
                }
            }

            /* AVANCE EN DIAGONALE SI ENNEMI A CAPTURER */
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() + 1);

            if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }

            nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() - 1);

            if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }
        } else {
            /* AVANCE D'UNE CASE */
            nouvellePosition = new Coordonnee(positionPiece.getLigne()-1, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
                mouvements.add(nouvellePosition);
            }

            /* AVANCE DE DEUX CASES SI PREMIER TOUR */
            if (!piece.getABouge()) {

                nouvellePosition = new Coordonnee(positionPiece.getLigne()-2, positionPiece.getColonne());

                if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
                    mouvements.add(nouvellePosition);
                }
            }

            /* AVANCE EN DIAGONALE SI ENNEMI A CAPTURER */
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() - 1);

            if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }

            nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() + 1);

            if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
            }
        }

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsTour(Piece piece, boolean captureRoi) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;


        /* AVANT */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne(), positionPiece.getColonne() + i);

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* ARRIERE */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne(), positionPiece.getColonne() - i);

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* DROITE */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + i, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }

        /* GAUCHE */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - i, positionPiece.getColonne());

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            } else {
                break;
            }

        }


        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsCavalier(Piece piece, boolean captureRoi) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;


        /* DROITE HAUT */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 2, positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* GAUCHE HAUT */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 2, positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* DROITE BAS */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 2, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* GAUCHE BAS  */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 2, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* HAUT DROITE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() + 2);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* HAUT GAUCHE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() + 2);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* BAS DROITE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() - 2);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        /* BAS GAUCHE */
        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() - 2);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(nouvellePosition)) {
            mouvements.add(nouvellePosition);
        } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
            mouvements.add(nouvellePosition);
        }

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsFou(Piece piece, boolean captureRoi) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;


        /* DIAGONALE HAUT DROITE */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + i, positionPiece.getColonne() + i);

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            }

        }

        /* DIAGONALE HAUT GAUCHE */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - i, positionPiece.getColonne() + i);

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            }

        }

        /* DIAGONALE BAS DROITE */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() + i, positionPiece.getColonne() - i);

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            }

        }

        /* DIAGONALE BAS GAUCHE */
        for (int i = 0; i < 8; i++) {
            nouvellePosition = new Coordonnee(positionPiece.getLigne() - i, positionPiece.getColonne() - i);

            if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
                mouvements.add(nouvellePosition);
            } else if (estDansLePlateau(nouvellePosition) && estCaseOccupee(nouvellePosition, ennemi)) {
                mouvements.add(nouvellePosition);
                break;
            }

        }


        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsDame(Piece piece, boolean captureRoi) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;

        mouvements.addAll(mouvementsFou(piece, captureRoi));
        mouvements.addAll(mouvementsTour(piece, captureRoi));

        return mouvements;
    }

    public ArrayList<Coordonnee> mouvementsRoi(Piece piece, boolean captureRoi) {
        ArrayList<Coordonnee> mouvements = new ArrayList<>();

        Coordonnee nouvellePosition;
        Coordonnee positionPiece = piece.getCoordonnee();
        int ennemi = piece.getCouleur() == Couleur.BLANC ? Couleur.NOIR : Couleur.BLANC;

        nouvellePosition = new Coordonnee(positionPiece.getLigne(), positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne(), positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne());

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne());

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() + 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() + 1, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        nouvellePosition = new Coordonnee(positionPiece.getLigne() - 1, positionPiece.getColonne() - 1);

        if (estDansLePlateau(nouvellePosition) && !estCaseOccupee(positionPiece)) {
            mouvements.add(nouvellePosition);
        }

        return mouvements;
    }
}