using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Battleship
{
    public class Controller : MonoBehaviour
    {
        public Sprite CellPast;
        public Sprite CellGot;
        public Sprite CellKill;
        public Sprite CellSelect;
        public Sprite CellDefault;
        //public Text[] StatsTexts;
        public Image FadeImage; //Картинка для затухания при загрузке.
        public GameObject Animator;
        public GameObject PastShip;
        public GameObject GotShip;
        private enum Cell
        { 
            Empty = 0,        // хз
            Engaged = 1,      // занято (в ячейке есть корабль)
            Got = 2,          // попал
            Past = 3,         // мимо
            Kill = 4,         // убил
            Disable = 5       // клетка возле коробля
        }

        private Animator _animator;
        private Animator _animatorEnd;
        private GameObject _arCamera;
        private int _money;                      // рейтинг          
        private bool _goFadeIn;                  // переход при загрузке.
        private readonly float fadeTime = 4.5f;  // Скорость перехода при загрузке.
        private QuadBattleship[] _quads;         // ячейки поля
        private List<List<Cell>> _battlefild;    // матрица для растоновки кароблей
        private MainController _main;

        private readonly List<List<Cell>> _alignment1 = new List<List<Cell>>
        {
            new List<Cell> {Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty},
            new List<Cell> {Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged},
            new List<Cell> {Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Engaged},
            new List<Cell> {Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Engaged}
        };  // заготовка растоновки

        private readonly List<List<Cell>> _alignment2 = new List<List<Cell>>
        {
            new List<Cell> {Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Engaged, Cell.Empty},
            new List<Cell> {Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty},
            new List<Cell> {Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty},
            new List<Cell> {Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Engaged, Cell.Engaged}
        };

        private readonly List<List<Cell>> _alignment3 = new List<List<Cell>>
        {
            new List<Cell> {Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Engaged},
            new List<Cell> {Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged},
            new List<Cell> {Cell.Engaged, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty},
            new List<Cell> {Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Engaged, Cell.Empty}
        };

        private readonly List<List<Cell>> _alignment4 = new List<List<Cell>>
        {
            new List<Cell> {Cell.Engaged, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Engaged, Cell.Engaged, Cell.Empty},
            new List<Cell> {Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty},
            new List<Cell> {Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty},
            new List<Cell> {Cell.Empty, Cell.Empty, Cell.Empty, Cell.Empty, Cell.Engaged, Cell.Engaged, Cell.Engaged}
        };

        void Start ()
        {
            _main = MainController.Instance;
            _money = _main.user.money;
            List<List<List<Cell>>> _aliggments = new List<List<List<Cell>>>()
            {
                _alignment1,
                _alignment2,
                _alignment3,
                _alignment4
            };
            _arCamera = GameObject.FindWithTag("ARCamera");
            _arCamera.SetActive(false);
            _animator = Animator.GetComponent<Animator>();
            _animatorEnd = GameObject.FindWithTag("StatsPanel").GetComponent<Animator>();
            _battlefild = _aliggments[new System.Random().Next(4)];
            _quads = FindObjectsOfType<QuadBattleship>().Where(i => i.tag == "Quad").ToArray();
            var imgTargets = FindObjectsOfType<QuadBattleship>().Where(i => i.tag != "Quad").ToArray();
            foreach (var quadBattleship in _quads)
                quadBattleship.Click += SelectСell;
            foreach (var img in imgTargets)
            {
                img.Shot += QuadBattleshipOnShot;
                GameObject clone = _battlefild[img.Row][img.Cell] != Cell.Engaged ?
                    Instantiate(PastShip, img.transform) : Instantiate(GotShip, img.transform);
                clone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
                
        }

        void Update ()
        {
            //StatsTexts[4].text = _money.ToString();
            if (IsWin())
                if (!_arCamera.activeInHierarchy)
                {
                    MainController main = MainController.Instance;
                    StartCoroutine(main.SaveStage(new WWW(
                        String.Format("http://slutskfish.ru/solveIT?login={0}&stage=3&money={1}", main.user.name,
                            _money))));
                    _animatorEnd.SetBool("Do", true);
                }
                    
                //_animator.SetBool("Do", true);
        }

        private void FixedUpdate()
        {
            if (_goFadeIn) // Если [Надо ли делать переход при загрузке.] = true, то:
                FadeImage.color =
                    Color.Lerp(FadeImage.color, Color.black,
                        fadeTime * Time.deltaTime); // Делаем переход из цвета картинки в чёрный.
            if (FadeImage.color == Color.black) // Если цвет картинки = чёрному, то:
                    SceneManager.LoadScene("Task3");
        }

        public void SelectСell(int row, int cell)
        {
            if (_battlefild[row][cell] == Cell.Engaged || _battlefild[row][cell] == Cell.Empty)
            {
                foreach (var quad in _quads)
                    if (quad.gameObject.GetComponent<Image>().sprite == CellSelect)
                        quad.gameObject.GetComponent<Image>().sprite = CellDefault;
                _quads.First(i => i.Cell == cell && i.Row == row).gameObject.GetComponent<Image>().sprite = CellSelect;
            }
        }

        public void ShowRule()
        {
            _animator.SetBool("Show",false);
        }

        public void HideRule()
        {
            _animator.SetBool("Show", true);
        }

        public void OnOffCamera()
        {
            _arCamera.SetActive(!_arCamera.activeInHierarchy);
        }

        // сделали выстрел
        private void QuadBattleshipOnShot(int row, int cell)
        {
            //if (_quads.First(i => i.Cell == cell && i.Row == row).gameObject.GetComponent<Image>().sprite == CellSelect)
            //{
            if (_battlefild[row][cell] == Cell.Engaged || _battlefild[row][cell] == Cell.Empty)
            {
                _quads.First(i => i.Cell == cell && i.Row == row).GetComponentInChildren<Text>().enabled = false;
                if (_battlefild[row][cell] == Cell.Engaged)
                {
                    _battlefild[row][cell] = Cell.Got;
                    _quads.First(i => i.Cell == cell && i.Row == row).gameObject.GetComponent<Image>().sprite = CellGot;
                    _money += 7;
                    IsKill(row, cell);
                }
                else
                {
                    _money -= 1;
                    _battlefild[row][cell] = Cell.Past;
                    _quads.First(i => i.Cell == cell && i.Row == row).gameObject.GetComponent<Image>().sprite =
                        CellPast;
                }
            }
            //}
        }

        // проверяет, остались ли корабли
        private bool IsWin()
        {
            return _battlefild.All(i => i.All(k => k != Cell.Engaged));
        }

        public void TapToContinue()
        {
            _goFadeIn = true;
        }

        // убили или ранели
        private bool IsKill(int row, int coll)
        {
            // проверка верхних клеток
            for (int i = row; i >= 0; i--)
            {
                if (_battlefild[i][coll] == Cell.Engaged)   // если есть целые части
                    return false;
                if (_battlefild[i][coll] == Cell.Empty || _battlefild[i][coll] == Cell.Past)    // если пустота
                    break;
            }
            // нижних
            for (int i = row; i <= 3; i++)
            {
                if (_battlefild[i][coll] == Cell.Engaged)
                    return false;
                if (_battlefild[i][coll] == Cell.Empty || _battlefild[i][coll] == Cell.Past)
                    break;
            }
            // левых
            for (int i = coll; i >= 0; i--)
            {
                if (_battlefild[row][i] == Cell.Engaged)
                    return false;
                if (_battlefild[row][i] == Cell.Empty || _battlefild[row][i] == Cell.Past)
                    break;
            }
            // правых
            for (int i = coll; i <= 6; i++)
            {
                if (_battlefild[row][i] == Cell.Engaged)
                    return false;
                if (_battlefild[row][i] == Cell.Empty || _battlefild[row][i] == Cell.Past)
                    break;
            }
            KillShip(row, coll);
            return true;
        }

        // уничтожить корабль
        private void KillShip(int row, int coll)
        {
            // проверка верхних клеток
            for (int i = row; i >= 0; i--)
            {
                if (_battlefild[i][coll] == Cell.Got) 
                {
                    _quads.First(j => j.Cell == coll && j.Row == i).gameObject.GetComponent<Image>().sprite = CellKill;
                    DisableAroundCell(i, coll);
                }
                if (_battlefild[i][coll] == Cell.Empty || _battlefild[i][coll] == Cell.Past) // если пустота
                    break;
            }
            // нижних
            for (int i = row; i <= 3; i++)
            {
                if (_battlefild[i][coll] == Cell.Got)
                {
                    _quads.First(j => j.Cell == coll && j.Row == i).gameObject.GetComponent<Image>().sprite = CellKill;
                    DisableAroundCell(i, coll);
                }
                if (_battlefild[i][coll] == Cell.Empty || _battlefild[i][coll] == Cell.Past) // если пустота
                    break;
            }
            // левых
            for (int i = coll; i >= 0; i--)
            {
                if (_battlefild[row][i] == Cell.Got)
                {
                    _quads.First(j => j.Cell == i && j.Row == row).gameObject.GetComponent<Image>().sprite = CellKill;
                    DisableAroundCell(row, i);
                }
                if (_battlefild[row][i] == Cell.Empty || _battlefild[row][i] == Cell.Past) // если пустота
                    break;
            }
            // правых
            for (int i = coll; i <= 6; i++)
            {
                if (_battlefild[row][i] == Cell.Got)
                {
                    _quads.First(j => j.Cell == i && j.Row == row).gameObject.GetComponent<Image>().sprite = CellKill;
                    DisableAroundCell(row, i);
                }
                if (_battlefild[row][i] == Cell.Empty || _battlefild[row][i] == Cell.Past) // если пустота
                    break;
            }
        }

        // закрасить все клетки вокруг убитой
        private void DisableAroundCell(int row, int coll)
        {
            _battlefild[row][coll] = Cell.Kill;
            if (row + 1 <= 3 && _battlefild[row + 1][coll] != Cell.Got && _battlefild[row + 1][coll] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll && j.Row == row + 1).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll && i.Row == row + 1).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row + 1][coll] = Cell.Disable;
            }

            if (row - 1 >= 0 && _battlefild[row - 1][coll] != Cell.Got && _battlefild[row - 1][coll] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll && j.Row == row - 1).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll && i.Row == row - 1).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row - 1][coll] = Cell.Disable;
            }

            if (coll - 1 >= 0 && _battlefild[row][coll - 1] != Cell.Got && _battlefild[row][coll - 1] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll - 1 && j.Row == row).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll - 1 && i.Row == row).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row][coll - 1] = Cell.Disable;
            }

            if (coll + 1 <= 6 && _battlefild[row][coll + 1] != Cell.Got && _battlefild[row][coll + 1] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll + 1 && j.Row == row).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll + 1 && i.Row == row).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row][coll + 1] = Cell.Disable;
            }

            if (row + 1 <= 3 && coll - 1 >= 0 && _battlefild[row + 1][coll - 1] != Cell.Got && _battlefild[row + 1][coll - 1] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll - 1 && j.Row == row + 1).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll - 1 && i.Row == row + 1).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row + 1][coll - 1] = Cell.Disable;
            }

            if (row + 1 <= 3 && coll + 1 <= 6 && _battlefild[row + 1][coll + 1] != Cell.Got && _battlefild[row + 1][coll + 1] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll + 1 && j.Row == row + 1).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll + 1 && i.Row == row + 1).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row + 1][coll + 1] = Cell.Disable;
            }

            if (row - 1 >= 0 && coll + 1 <= 6 && _battlefild[row - 1][coll + 1] != Cell.Got && _battlefild[row - 1][coll + 1] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll + 1 && j.Row == row - 1).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll + 1 && i.Row == row - 1).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row - 1][coll + 1] = Cell.Disable;
            }

            if (row - 1 >= 0 && coll - 1 >= 0 && _battlefild[row - 1][coll - 1] != Cell.Got && _battlefild[row - 1][coll - 1] != Cell.Kill)
            {
                _quads.First(j => j.Cell == coll - 1 && j.Row == row - 1).gameObject.GetComponent<Image>().sprite = CellPast;
                _quads.First(i => i.Cell == coll - 1 && i.Row == row - 1).GetComponentInChildren<Text>().enabled = false;
                _battlefild[row - 1][coll - 1] = Cell.Disable;
            }
            
        }
    }
}
