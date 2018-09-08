using UnityEngine;
using UnityEngine.UI;

public class Rating : MonoBehaviour
{

    public GameObject[] Lines;
    public Text debug;
    private MainController _main;

    void Awake()
    {
        _main = MainController.Instance;
        debug.text = _main.Users.Count.ToString();
    }

    // Use this for initialization
    //void Start () {
      //  debug.text = _main.Users.Count.ToString();
        //for (int i = 0; i < 8; i++)
        //{
        //    var texts = Lines[i].GetComponentsInChildren<Text>();
        //    if (i < 2)
        //    {
        //        texts[1].text = _main.Users[i].name;
        //        texts[2].text = _main.Users[i].money.ToString();
        //    }
        //}
    
}
