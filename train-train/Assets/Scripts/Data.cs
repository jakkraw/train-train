﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Symbol_
{
    public string text = "";
    public Texture2D texture = null;

    public Symbol_(string t)
    {
        text = t;
    }

    public Symbol_(Texture2D t)
    {
        texture = t;
    }
}


//Klasa, która zawiera informacje zapisywane - tutaj jest miejsce na wszelkie ustawienia
[Serializable]
public class Profile
{
    //Texture2D Nie może być serializowane
    [NonSerialized]
    public List<Texture2D> drivers;
    [NonSerialized]
    public Texture2D selectedDriver;
    [NonSerialized]
    public List<Texture2D> passengers;
    [NonSerialized]
    public List<Texture2D> selectedPassengers;
    [NonSerialized]
    public List<Symbol_> symbols;
    [NonSerialized]
    public List<Symbol_> selectedSymbols;

    public string driver_string;
    public List<String> passengers_strings;
    public float trainSpeed;
    public string points;
    public bool doesEnd;

    //Needs to be run 
    public void ReconstructProfile()
    {
        this.passengers = new List<Texture2D>();
        foreach (var img in this.passengers_strings)
        {
            this.passengers.Add(Resources.Load<Texture2D>(img));
        }
        this.selectedPassengers = new List<Texture2D>();
        this.selectedPassengers.AddRange( this.passengers );

        this.drivers = new List<Texture2D>();
        this.drivers.Add( Resources.Load<Texture2D>(this.driver_string) );
        this.selectedDriver = drivers[0];

        this.symbols = new List<Symbol_>();
        for (int i = 1; i <= 7; i++)
        {
            this.symbols.Add(new Symbol_(i.ToString()));
        }
        this.selectedSymbols = new List<Symbol_>();
        this.selectedSymbols.AddRange( symbols );
    }

    public static Profile testProfile()
    {
        var p = new Profile();
        p.passengers_strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };
        p.driver_string = "Images/man";
        p.trainSpeed = 20;
        p.doesEnd = true;
        p.ReconstructProfile();
        return p;
    }
}

//[Serializable]
//public class Textures
//{
    

//    public Textures(Texture2D driver_toSave, List<Symbol> symbols_toSave, List<Texture2D> passengers_toSave)
//    {
//        //Tutaj wrzuce konwersje parametrow wejsciowych na zmienne zadeklarowane wczesniej zeby mozna bylo to serializowac
//    }
//}

//Dopisz opcje - zmiany - trainspeed - doesnend - symbols - w settings zrobic 


//Klasa zapisywalna do pliku - lista profili oraz informacja o obecnym profilu
[Serializable]
public class ProfileList
{
    public List<Profile> profiles = new List<Profile>();
    //public Textures textures; //Jakas inicjalizacja domyslna??
    //                          //currentProfile - funkcja, ktora sptawdza index z ProfileList i wtedy ona wykorzystuje informacje z opdowiedniego profilu

    //public void NewProfiles()
    //{
    //}

    //public ProfileList()
    //{
    //    profiles = new List<Profile>
    //    {
    //        Profile.testProfile(),
    //        null
    //    };
    //}
}

public class Data
{    

    static Data()
    {
        //ścieżka pliku z profilami
        currentProfile = Profile.testProfile(); //tymczasowo tutaj, potem do zmiany
        string destination = Application.dataPath + "/profilesAG.bin";
        BinaryFormatter bf = new BinaryFormatter();

        //if (File.Exists(destination))
        //{
            //FileStream file = File.OpenRead(destination);
            //Debug.Log("A");
            //ProfileList dataFromFile = (ProfileList)bf.Deserialize(file);
            //Debug.Log("B");
            //file.Close();
            //All_Profiles.profiles = dataFromFile.profiles;
            //currentProfile = All_Profiles.profiles[0];
            //currentProfile.ReconstructProfile();
        //}
        //else
        //{
            FileStream file = File.Create(destination);
            var p_list = new ProfileList();
            p_list.profiles.Insert(0, currentProfile); //profilelist wjechal pusty
            bf.Serialize(file, p_list);
            file.Close();
            Debug.Log("Profile files were created.");
        //}
    }

    public static Profile currentProfile;
    public static ProfileList All_Profiles;

}