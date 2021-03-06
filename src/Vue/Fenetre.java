/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Vue;

import Common.Coordonnee;
import Common.Couleur;
import Controleur.ControleurPartie;
import Modele.Joueur.Humain;
import Modele.Plateau;
import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.util.ArrayList;
import java.util.EventListener;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JPanel;

/**
 *
 * @author Elissea
 */
public class Fenetre extends JFrame implements ActionListener, MouseListener {

    private ControleurPartie controleurPartie;
    private Plateau plateau;
    private Vue.Case selection;
    private JMenuBar barreMenu;
    private JMenu menu;
    private JMenuItem hvsh;
    private JMenuItem hvsia;
    private JMenuItem iavsia;
    private Echiquier echiquier;
    private ArrayList<Coordonnee> deplacementsPossibles;

    public Fenetre(Plateau plateau) {
        this.plateau = plateau;
        this.deplacementsPossibles = new ArrayList<>();

        this.barreMenu = new JMenuBar();
        this.menu = new JMenu("Partie");
        this.hvsh = new JMenuItem("Humain vs Humain");
        this.hvsia = new JMenuItem("Humain vs Ordinateur");
        this.iavsia = new JMenuItem("Ordinateur vs Ordinateur");

        this.barreMenu.add(this.menu);
        this.menu.add(this.hvsh);
        this.menu.add(this.hvsia);
        this.menu.add(this.iavsia);

        this.echiquier = new Echiquier(this.plateau);
        this.setLayout(new BorderLayout());

        this.hvsh.addActionListener(this);
        this.hvsia.addActionListener(this);
        this.iavsia.addActionListener(this);

        this.setPreferredSize(new Dimension(600, 600));
        this.add(this.echiquier, BorderLayout.CENTER);
        this.setJMenuBar(this.barreMenu);
        this.setVisible(true);

        this.pack();
        this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
    }

    public Echiquier getEchiquier() {
        return this.echiquier;
    }

    public void actionPerformed(ActionEvent e) {
        if (e.getSource().equals(hvsh)) {
            controleurPartie = new ControleurPartie(new Humain(Couleur.BLANC), new Humain(Couleur.NOIR), plateau);
            plateau.initialiser();
            getEchiquier().initialiser();

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    getEchiquier().getCases()[i][j].addMouseListener(this);
                }
            }
        }
        
        repaint();
    }

    @Override
    public void mouseClicked(MouseEvent e) {
        Vue.Case emplacementSelectionne = (Vue.Case) e.getSource();

        boolean exists = false;

        if (deplacementsPossibles.contains(emplacementSelectionne.getCoordonnee())) {
            if (controleurPartie.effectuerDeplacement(selection.getCoordonnee(), emplacementSelectionne.getCoordonnee())) {
                getEchiquier().initialiser();

                for (Coordonnee c : deplacementsPossibles) {
                    this.getEchiquier().getCases()[c.getLigne()][c.getColonne()].deselectionner();
                }

                selection = null;
                deplacementsPossibles.clear();
            }

            for (Coordonnee c : deplacementsPossibles) {
                this.getEchiquier().getCases()[c.getLigne()][c.getColonne()].deselectionner();
            }
        } else {
            if (selection == null) {
                selection = emplacementSelectionne;

                deplacementsPossibles = controleurPartie.getDeplacementsPossibles(emplacementSelectionne.getCoordonnee());

                if (deplacementsPossibles.size() > 0) {
                    for (Coordonnee coordonnee : deplacementsPossibles) {
                        this.getEchiquier().getCases()[coordonnee.getLigne()][coordonnee.getColonne()].selectionner();
                    }
                }
            } else if (selection.equals(emplacementSelectionne)) {
                selection = null;

                deplacementsPossibles = controleurPartie.getDeplacementsPossibles(emplacementSelectionne.getCoordonnee());

                if (deplacementsPossibles.size() > 0) {
                    for (Coordonnee coordonnee : deplacementsPossibles) {
                        this.getEchiquier().getCases()[coordonnee.getLigne()][coordonnee.getColonne()].deselectionner();
                    }
                }

                deplacementsPossibles.clear();
            } else {
                deplacementsPossibles = controleurPartie.getDeplacementsPossibles(selection.getCoordonnee());

                if (deplacementsPossibles.size() > 0) {
                    for (Coordonnee coordonnee : deplacementsPossibles) {
                        this.getEchiquier().getCases()[coordonnee.getLigne()][coordonnee.getColonne()].deselectionner();
                    }
                }

                deplacementsPossibles.clear();

                selection = emplacementSelectionne;

                deplacementsPossibles = controleurPartie.getDeplacementsPossibles(emplacementSelectionne.getCoordonnee());

                if (deplacementsPossibles.size() > 0) {
                    for (Coordonnee coordonnee : deplacementsPossibles) {
                        this.getEchiquier().getCases()[coordonnee.getLigne()][coordonnee.getColonne()].selectionner();
                    }
                }
            }
        }
    }

    @Override
    public void mousePressed(MouseEvent e) {
    }

    @Override
    public void mouseReleased(MouseEvent e) {
    }

    @Override
    public void mouseEntered(MouseEvent e) {
    }

    @Override
    public void mouseExited(MouseEvent e) {
    }
}
