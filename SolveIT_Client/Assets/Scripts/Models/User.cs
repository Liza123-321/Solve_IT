using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour {

    public int id { get; set; }
    public string name { get; set; }
    public string team { get; set; }
    public int money { get; set; }
    public int stage { get; set; }

    public User(int id, string name, string team, int money, int stage):base()
    {
        
        this.id = id;
        this.name = name;
        this.team = team;
        this.money = money;
        this.stage = stage;
    }

    public User():base()
    {
        
    }
}
