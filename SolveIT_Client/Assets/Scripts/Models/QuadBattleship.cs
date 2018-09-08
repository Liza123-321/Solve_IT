using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Models
{
    public class QuadBattleship : MonoBehaviour
    {

        public int Row;
        public int Cell;

        public delegate void action(int a, int b);

        public event action Shot;

        public event action Click;

        public void OnPointerDown()
        {
            if (Shot != null) Shot.Invoke(Row, Cell);
        }

        public void OnClick()
        {
            if(Click != null) Click.Invoke(Row, Cell);
        }
    }
}
