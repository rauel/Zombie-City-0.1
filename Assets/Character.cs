using UnityEngine;
using System.Collections;

public class Character
{

    // Possible Skills of the character
    private int fightingSkill;
    private int craftingSkill;
    private int mechanicSkill;
    private int electricSkill;
    private int doctorSkill;
    private int gardenerSkill;
    private int authoritySkill;

    // Main Stats, every character got:

    // Current Health (of 100)
    private int health;

    // The mood (0 is unhappy, 100 is happy)
    private int mood;

    // The more initiative, the higher the chance of reacting to an effect.
    private int initiative;

    private string name;
    private bool gender;    // true, if female. false, if male
    private string description;


    public bool getGender()
    {
        return gender;
    }

}
