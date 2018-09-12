using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathProps : MonoBehaviour {

    public GameObject[] ChalkOutlines;
    public GameObject Leon_knife;
    public GameObject Leon_journal;
    public GameObject Will_journal;
    public GameObject Zahra_pillow;
    public GameObject Akira_pillow;
    public GameObject Bar_T_1;
    public GameObject Bar_T_1_OT;
    public GameObject Bar_T_2;
    public GameObject Bar_T_2_OT;
    public GameObject HH_book;
    public GameObject Leon_maps;
    public GameObject Will_tags;
    public GameObject Akira_vial;
    public GameObject Zahra_note;

    //Props to turn off
    public GameObject bar_chair_1;
    public GameObject bar_chair_2;

    public void setOutlines(int day, Char_Movement_Controller[] Chars)
    {
        //Eddie dies
        if (day == 1)
        {
            ChalkOutlines[0].SetActive(true);
            Leon_knife.SetActive(true);
        }

        //Leon Dies
        if (!Chars[3].isAlive)
        {
            ChalkOutlines[1].SetActive(true);
            Leon_journal.SetActive(false);
            Will_journal.SetActive(true);
            Zahra_pillow.SetActive(false);
            Akira_pillow.SetActive(true);
        }

        //Akira Dies
        if (!Chars[1].isAlive)
        {
            ChalkOutlines[2].SetActive(true);
            Bar_T_1.SetActive(false);
            Bar_T_1_OT.SetActive(true);
            Bar_T_2.SetActive(false);
            Bar_T_2_OT.SetActive(true);
            Leon_maps.SetActive(false);
            HH_book.SetActive(true);
            Leon_knife.SetActive(false);
            bar_chair_1.SetActive(false);
            bar_chair_2.SetActive(false);
        }

        //Zahra and unimportant lady I forget about die
        if (!Chars[6].isAlive)
        {
            ChalkOutlines[3].SetActive(true);
            Will_tags.SetActive(true);
            Akira_vial.SetActive(false);
            Zahra_note.SetActive(true);
        }

    }
}
