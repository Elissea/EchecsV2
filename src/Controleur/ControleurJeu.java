/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package Controleur;

import Vue.Fenetre;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

/**
 *
 * @author Elissea
 */
public class ControleurJeu {

    private Fenetre fenetre;
    private ControleurPartie controleur;

    public ControleurJeu(Fenetre fenetre) {
        this.fenetre = fenetre;
        this.fenetre.addNouvellePartieListener(new nouvellePartieListener());
    }

    class nouvellePartieListener implements ActionListener {
        @Override
        public void actionPerformed(ActionEvent e) {
            controleur.demarrerPartie();
        }
    }
}
