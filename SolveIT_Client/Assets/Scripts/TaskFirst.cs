using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Используем библиотеку UnityEngine.UI для управления интерфейсом (Текст, изображения и т.д).

namespace Assets.Scripts
{
    public class TaskFirst : MonoBehaviour
    {
        public Text QuastionQount;
        public Text MoneyCounText;
        public Image Bg;
        public Button Bttns; //Массив | Кнопки ответов.
        public Sprite BgFirst; //Спрайт при правильном ответе.
        public Sprite BgSecond; //Спрайт при неправильном ответе.
        public Sprite BgThird; // //Спрайт при нажатии на кнопку.
        public Image FadeImage; //Картинка для затухания при загрузке.
        public Text[] Stats;
        public Animator Animator;
        public InputField Input;
        public int Wq = 1; // Число с нумерацией вопроса.
        private int _money; // рейтинг.
        private readonly float fadeTime = 4.5f; // Скорость перехода при загрузке.
        private bool _getMoney; // Равен true если деньги уже были даны.
        private bool _lose; // Если ответ неверный, то равно true.
        private bool _goFadeIn; // Надо ли делать переход при загрузке.
        private bool _goFadeOut; // Надо ли делать переход (из черноты) при загруженом уровне.
        private readonly bool dontlikeit = false; // Игра не понравилась? # ТАГДА УХАДИ
        private readonly bool ended = false; // Закончена ли игра полностью?
        private bool _isLoaded; // Если уровень загружен, то равно true.
        private bool _trueA; // Равно true если ответ верный.
        private bool _falseA; // Равно true если ответ неверный.
        private action _questAction;
        private bool _isAnswer = false;
        private GameObject _exitPanel;

        delegate void action();

        private void Awake()
        {
            //_exitPanel = GameObject.FindWithTag("ExitPanel");
            //_exitPanel.SetActive(false);
            FadeImage.enabled = true; // Включить чёрный фон для перехода.
            _goFadeOut = true; //Делаем переход (из черноты) при загруженом уровне.

            if (PlayerPrefs.HasKey("M") && PlayerPrefs.HasKey("eQ")) // Если в сохранённых данных есть эти значения то:
            {
                _money = PlayerPrefs.GetInt("M"); // Взять переменную из сохранений.
                Wq = PlayerPrefs.GetInt("eQ"); // Взять переменную из сохранений.
            }
            else // Если данных нет, то:
            {
                PlayerPrefs.SetInt("eQ", 1); // Сохранить переменную.
                MainController main = MainController.Instance;
                _money = main.user.money;
                PlayerPrefs.SetInt("M", main.user.money); // Сохранить переменную.
            }
            PlayerPrefs.Save(); // Сохранить изменения в сохранениях :)
        }

        private void Start()
        {
            _getMoney = false;
            switch (Wq)
            {
                case 1:
                    Bg.sprite = BgFirst;
                    _questAction = Rq1;
                    break;
                case 2:
                    Bg.sprite = BgSecond; // Запускаем 2 вопрос. (Дальше всё тоже самое).
                    _questAction = Rq2;
                    break;
                case 3:
                    Bg.sprite = BgThird;
                    _questAction = Rq3;
                    break;
            }

            _questAction.Invoke();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) // Если нажата кнопка (Escape) либо кнопка назад на смартфоне, то:
            {
                if (!_exitPanel.activeInHierarchy) // Если выключена панель выхода, то:
                {
                    _exitPanel.GetComponent<Animator>().SetBool("IsShow", true);
                }
                else // Иначе
                {
                    _exitPanel.GetComponent<Animator>().SetBool("IsShow", false);
                }
            }
            QuastionQount.text = string.Format("{0}/3", Wq);
            MoneyCounText.text = string.Format("{0} очков", _money);
        }

        private void FixedUpdate()
        {
            if (_goFadeIn) // Если [Надо ли делать переход при загрузке.] = true, то:
                FadeImage.color =
                    Color.Lerp(FadeImage.color, Color.black,
                        fadeTime * Time.deltaTime); // Делаем переход из цвета картинки в чёрный.
            else if (_goFadeOut) // Иначе если [Надо ли делать переход (из черноты) при загруженом уровне.] = true, то:
                FadeImage.color =
                    Color.Lerp(FadeImage.color, Color.clear,
                        fadeTime * Time.deltaTime); // Делаем переход из цвета картинки (чёрный) в прозрачный.
            if (FadeImage.color == Color.black) // Если цвет картинки = чёрному, то:
                if (_isLoaded == false) // Если уровень не загружен, то:
                    ContinueLoad(); // Запускаем функцию continueLoad (416 строка).
        }

        private void Rq1() // Приватная функция (Вопрос 1)
        {
            Stats[1].text = "SHIMAN – заглавные буквы авторов песен.\n " +
                            "Sting\n" +
                            "Hurts\n" +
                            "Iggy Pop\n" +
                            "Madonna\n" +
                            "Abba\n" +
                            "Nirvana\n";
            if (_isAnswer)
            {
                if (Input.text == "SHIMAN" || Input.text == "shiman")
                    StartCoroutine(WaitForMagic());
                else // Иначе если нажатый ответ равен 0 или 2 или 3, то:
                    StartCoroutine(WaitForDestroy()); // Запускаем корутину проигрыша.  
            }
            
        }

        private void Rq2() // Смотрите на вопрос 1.
        {
            if (_isAnswer)
            {
                Stats[1].text = "Париж, Пекин, Оттава, Рига, Минск";
                if (Input.text.Contains("Париж") && Input.text.Contains("Пекин") && Input.text.Contains("Оттава") && 
                    Input.text.Contains("Рига") && Input.text.Contains("Минск"))
                    StartCoroutine(WaitForMagic());
                else
                {
                    StartCoroutine(WaitForDestroy());
                }
            }
            
        }

        private void Rq3() // Смотрите на вопрос 1.
        {
            Stats[1].text = "4415719";
            if (_isAnswer)
                StartCoroutine(Input.text == "4415719" ? WaitForMagic() : WaitForDestroy());           
        }

     

        private IEnumerator WaitForMagic() // Корутина выигрыша.
        {
            _trueA = true; // Если ответ правильный, то равно true.
            yield return new WaitForSeconds(1f); // Продолжить через 3 сек.
            StatsWindow(); // Запустить функцию статистики.
            StopCoroutine(WaitForMagic()); // Остановить корутину выигрыша.
        }

        private IEnumerator WaitForDestroy() // Корутина проигрыша.
        {
            _falseA = true; // Если ответ неправильный, то равно true.
            yield return new WaitForSeconds(1f); // Продолжить через 3 сек.
            if (dontlikeit) // Если dontlikeit = true, то:
            {
                Application.Quit(); // Выйти из игры. Возможна ошибка при выходе.
            }
            else // Иначе:
            {
                yield return new WaitForSeconds(1f); // Продолжить через 3 сек.
                StatsWindow(); // Запустить функцию статистики.
                StopCoroutine(WaitForDestroy()); // Остановить корутину проигрыша.
            }
        }

        private void StatsWindow() // Приватная функция (Окно статистики)
        {
            if (_trueA) // Если нажат правильный ответ, то:
            {
                if (_getMoney == false) // Если деньги не давались, то:
                {
                    _money += 5;
                    _getMoney = true; // (Если деньги уже даны, то равно true).
                    PlayerPrefs.SetInt("MR", _money); // Сохранить кол-во денег для отображения при окончании игры.
                    PlayerPrefs.SetInt("M", _money); // Сохранить кол-во денег для игры.
                    PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
                    _getMoney = true;
                }
                _lose = false; // (Если проиграл, то равен true). В этом случае false.
                Animator.SetBool("Do", true);
                Stats[0].text = "ПОЗДРАВЛЯЕМ! ТЫ ВЫБРАЛ ПРАВИЛЬНЫЙ ОТВЕТ!"; // Присвоить тексту строку.
            }
            else if (_falseA) // Иначе если ответ неправильный, то:
            {
                if (_getMoney == false)
                {
                    _money += 1; // Присвоить деньгам нулевое число.
                    _lose = true; // (Если проиграл, то равно true)
                    _getMoney = true;
                }
                
                Animator.SetBool("Do", true);
                Stats[0].text = "НЕПРАВИЛЬНЫЙ ОТВЕТ"; // Присвоить тексту строку.
                PlayerPrefs.SetInt("MR", _money); // Сохранить кол-во денег для отображения при окончании игры.
                PlayerPrefs.SetInt("M", _money); // Сохранить кол-во денег для игры.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
            }
        }

        public void ContinueBttn() // Публичная функция (Кнопка продолжения)
        {
            _goFadeOut = false; // (Делать переход (из черноты) при загруженом уровне если = true).
            _goFadeIn = true; // (Делать переход в черноту при загрузке уровня если = true).
        }

        private void ContinueLoad() // Приватная функция (Продолжить загрузку)
        {
            if (Wq == 3) // Если прошли полностью игру, то:
            {
                MainController main = MainController.Instance;
                StartCoroutine(main.SaveStage(new WWW(
                    String.Format("http://slutskfish.ru/solveIT?login={0}&stage=2&money={1}", main.user.name,
                        _money))));
                
                SceneManager.LoadScene("Battleship"); // Загружаем 0 level (Меню).
                //PlayerPrefs.SetString("Ended?", "Ended"); // Сохраняем строку Ended.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях. 
            }
            else // Иначе (Если не прошли)
            {
                Wq++; // Прибавить 1 к числу с нумерацией вопроса.
                _isLoaded = true; // (Если уровень загружен, то равно true.)
                PlayerPrefs.SetInt("eQ", Wq); // Сохраняем число с нумерацией вопроса.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
                SceneManager.LoadScene("Task1"); // Загрузить 1 level (Перезапускаем уровень с игрой для нового вопроса).
            }
        }

        public void SelectedBttn() // Публичная функция (Нажатая кнопка)
        {
            if (Input.text.Length > 3)
            {
                _isAnswer = true;
                _questAction.Invoke();
            }
        }

        public void OnClickMenu() // Публичная функция (При нажатии на кнопку в меню)
        {
            SceneManager.LoadScene("Menu"); // Загрузить 0 level (Меню)
        }

        public void OnClickExit() // Публичная функция (При нажатии на выход)
        {
            Process.GetCurrentProcess().Kill(); // Полностью закрываем игру.
        }
    }
}