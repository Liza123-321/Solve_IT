using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainController : MonoBehaviour
{
    private static readonly MainController _instance = new MainController();
    public static MainController Instance
    {
        get { return _instance; }
    }

    private MainController()
    {
        Users = new List<User>();
    }

    static MainController()
    {
        
    }

    public User user { get; private set; }

    public List<User> Users { get; set; }

    public IEnumerator AuthEnum(WWW www, string name)
    {
        yield return www;
        _instance.user = new User(-1, "name", "team", 0, 1);
        if (www.error == null)
        {
            User[] obj = JsonConvert.DeserializeObject<User[]>(www.text);
            foreach (var instance in obj)
            {
                _instance.Users.Add(instance);
            }
            if (Users.Count != 0)
                _instance.user = Users.First(i=>i.name == name);
            else
                _instance.user = new User(-2, "name", "team", 0, 1);
            //var obj = JsonUtility.FromJson<User>(www.text);
            //var users = JsonConvert.DeserializeObject<List<User>>(www.text);
            //user = users;
        }
    }

    public IEnumerator Rating(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            User[] obj = JsonConvert.DeserializeObject<User[]>(www.text);
            foreach (var user in obj.Reverse())
            {
                Users.Add(user);
            }
        }
        
    }

    public IEnumerator SaveStage(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            var obj = JsonConvert.DeserializeObject<User[]>(www.text);
            _instance.user = obj[0];
            //var obj = JsonUtility.FromJson<User>(www.text);
            //var users = JsonConvert.DeserializeObject<List<User>>(www.text);
            //user = users;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
