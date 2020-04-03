using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public int language = 1;

    System.String[,] firstNamesFemale = new System.String[,] {  /*ENGLISH */ { "Ginny", "Andrea", "Maria", "Jane", "Kathrin", "Kate", "Mary", "Patricia", "Barbara", "Linda", "Susan", "Margaret", "Dorothy", "Elizabeth", "Jenny", "Olivia", "Emily", "Mia" },
                                                                /*GERMAN  */ { "Helga", "Monika", "Franziska", "Marina", "Sophia", "Andrea", "Maria", "Anja", "Jana", "Christina", "Anke", "Julia", "Katharina", "Sonja", "Beate", "Heidi", "Eva", "Melanie" }};

    System.String[,] firstNamesMale = new System.String[,] {    /*ENGLISH */ { "Joe", "John", "George", "Will", "Bill", "Franklin", "Thomas", "James", "Robert", "Michael", "David", "Richard", "Charles", "Joseph", "Oliver", "Noah", "Ethan", "Alex", "Rick" },
                                                                /*GERMAN  */ { "Daniel", "Markus", "Lukas", "Marc", "Rauel", "René", "Peter", "Thomas", "Andreas", "Uwe", "Werner", "Frank", "Jürgen", "Dieter", "Hans", "Heinz", "Jan", "Dirk", "Till" }};

    System.String[,] lastNamePrefixes = new System.String[,] {  /*ENGLISH */ { "Mc", "Mc" },
                                                                /*GERMAN  */ { "", "" }};

    System.String[,] lastNames = new System.String[,] {         /*ENGLISH */ { "Miller", "Smith", "Anderson", "Murphy", "Brown", "Wilson", "Green", "Grimes", "Johnson", "Jones", "Moore", "Jackson", "White", "Clark", "Taylor", "Walker", "Scott", "Adams" },
                                                                /*GERMAN  */ { "Müller", "Schmidt", "Anders", "Kowalski", "Schrammen", "Bengs", "Schneider", "Meyer", "Schulz", "Wagner", "Neumann", "Schröder", "Krause", "Schäfer", "Richter", "Weber", "Wolf", "Becker" }};


    void Start ()
    {

    }

    public void CreateRandom()
    {
        Survivor survivor = new Survivor();

        survivor.gender = SetGender();

        survivor.firstName = AddFirstName(survivor.gender);
        
        survivor.lastName = AddLastName();
        
    }

    private bool SetGender()
    {
        bool isFemale = true;

        if (UnityEngine.Random.Range(0, 2) >= 1)
        {
            isFemale = false;
        }

        return isFemale;
    }
    private String AddFirstName(bool gender)
    {
        String firstname = "";

        if(gender)   // gender == true --> female
        {
            firstname += firstNamesFemale[language, UnityEngine.Random.Range(0, firstNamesFemale.GetLength(1) )];
        }
        else
        {
            firstname += firstNamesMale[language, UnityEngine.Random.Range(0, firstNamesMale.GetLength(1) )];
        }

        // Add a second forename
        if (randomNumber(0, 20) == 0)
        {
            if (gender)   // gender == true --> female
            {
                firstname += "-" + firstNamesFemale[language, UnityEngine.Random.Range(0, firstNamesFemale.GetLength(1) )];
            }
            else
            {
                firstname += "-" + firstNamesMale[language, UnityEngine.Random.Range(0, firstNamesMale.GetLength(1) )];
            }
        }

        // Add a second forename
        if (randomNumber(0, 1000) == 0)
        {
            if (gender)   // gender == true --> female
            {
                firstname += "-" + firstNamesFemale[language, UnityEngine.Random.Range(0, firstNamesFemale.GetLength(1))];
            }
            else
            {
                firstname += "-" + firstNamesMale[language, UnityEngine.Random.Range(0, firstNamesMale.GetLength(1))];
            }
        }

        return firstname;
    }

    private String AddLastName()
    {
        String lastname = "";

        if (randomNumber(0, 20) == 0)
        {
            lastname += lastNamePrefixes[language, UnityEngine.Random.Range(0, lastNamePrefixes.GetLength(1) )];
        }

        lastname += lastNames[language, UnityEngine.Random.Range(0, lastNames.GetLength(1) )];

        return lastname;
    }

    




    public int randomNumber(int minNumber, int maxNumber)
    {
        System.Random random = new System.Random();
        int chance = random.Next(minNumber, maxNumber);
        return chance;
    }
}
