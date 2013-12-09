/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Vue;

import Common.Couleur;
import Modele.Plateau.Plateau;
import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionListener;
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
public class Fenetre extends JFrame {

    private Plateau plateau;
    private JMenuBar barreMenu;
    private JMenu menu;
    private JMenuItem nouvellePartie;
    private Echiquier echiquier;
    
    public Fenetre(Plateau plateau) {
        this.plateau = plateau;
        
        this.barreMenu = new JMenuBar();
        this.menu = new JMenu("Partie");
        this.nouvellePartie = new JMenuItem("Nouvelle partie");
        
        this.barreMenu.add(this.menu);
        this.menu.add(this.nouvellePartie);
                
        this.echiquier = new Echiquier(8, this.plateau, Couleur.BLANC);
        this.setLayout(new BorderLayout());

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
    
    public void addNouvellePartieListener(ActionListener nouvellePartieListener) {
        this.nouvellePartie.addActionListener(nouvellePartieListener);
    }
}
