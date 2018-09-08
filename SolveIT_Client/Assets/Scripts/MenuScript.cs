// MenuScript for Open Source Game by Android Helper Games
// v.1.0
// Если увидите ошибки, то поймите меня, я писал быстро)
// Скрипт на 5 % оптимизирован, так что могут быть баги и т.д.

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Используем библиотеку UnityEngine.UI для управления интерфейсом (Текст, изображения и т.д).

public class MenuScript : MonoBehaviour {

    public Image fadeImage; // Картинка для затухания при загрузке.
    //public GameObject exitPanel; // Панель при выходе из игры.
    public Text MoneyText;
    public Text TeamText;
    private float fadeTime = 4.5f; // Скорость перехода при загрузке.
    private string ended; // Строка где хранится сохранённая информация о окончании игры.
    private int money; // Число где хранится сохранённая информация о кол-ве денег.
    private int debugtggl = 0; // Число с настройкой (Включен ли дебаг [1 - да, 0 - нет]).
    private bool exitActive = false; // Если панель выхода активна, то = true.
    private bool clickedStart = false; // Если нажата кнопка старта, то = true.
    private bool fade = false; // Если нужен переход при загрузке уровня, то = true.
    private Animator animator;
    private int level = 0;
    private MainController _main;
    private GameObject _statePanel;

    void Awake () // Данная функция вызывается до инициализации всех остальных скриптов. Обычно используется для установки определенных параметров и инициализации переменных.
    {
        _main = MainController.Instance;
        _statePanel = GameObject.FindWithTag("StatsPanel"); 
        animator = GetComponent<Animator>();
        if (PlayerPrefs.HasKey("Ended?")) // Если сохранена информация об окончании игры, то:
        {
            ended = PlayerPrefs.GetString("Ended?"); // Присвоить строке сохранённые данные.
            money = PlayerPrefs.GetInt("MR"); // Присвоить числу сохранённые данные.
        }
    }
    void Start() // Вызывается один раз при запуске скрипта.
    {
    }
    void Update() // Данная функция вызывается каждый раз перед отображением очередного кадра. Самая используемая для расчетов игровых параметров.
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Если нажата кнопка (Escape) либо кнопка назад на смартфоне, то:
        {
            if (!exitActive) // Если выключена панель выхода, то:
            {
                animator.SetBool("IsShow", true);
                exitActive = true; // (Если панель выхода активна, то = true.)
            }
            else // Иначе
            {
                animator.SetBool("IsShow", false);
                exitActive = false; // (Если панель выхода неактивна, то = false.)
            }
        }
        if (clickedStart) // Если нажата кнопка начала игры, то:
        {
            PlayerPrefs.DeleteKey("eQ"); // Удалить ключ числа с нумерацией вопроса.
            PlayerPrefs.DeleteKey("M"); // Удалить ключ сохранённого числа с кол-вом денег.
            PlayerPrefs.DeleteKey("Ended?"); // Удалить ключ о завершении игры.
        }
        level = _main.user.stage;
        MoneyText.text = String.Format("Очки: {0}", _main.user.money);
        TeamText.text = String.Format("Команда: {0}", _main.user.team);

    }
    void FixedUpdate() // Данная функция вызывается каждый раз при расчете физических показателей. Все расчеты физики следует проводить именно в ней. Но я рассчитываю анимацию перехода, и использую эту функцию для того, чтобы не было никаких тормозов.
    {
        if (fade == true) // Если [Надо ли делать переход при загрузке.] = true, то:
        {
            fadeImage.color = Color.Lerp(fadeImage.color, Color.black, fadeTime * Time.deltaTime); // Делаем переход из цвета картинки в чёрный.
        }
        if (fadeImage.color == Color.black) // Если цвет картинки = чёрному, то:
        {
            switch (level)
            {
                case 1: SceneManager.LoadScene("Task1");
                    break;
                case 2: SceneManager.LoadScene("Battleship");
                    break;
                case 3:
                    SceneManager.LoadScene("Task3");
                    break;
                default: SceneManager.LoadScene("Menu");
                    break;
            }
        }
    }
    public void OnClickStart() // Публичная функция (При нажатии на старт)
    {
        if (_main.user.stage != 4)
        {
            fade = true; // (Если нужен переход при загрузке уровня, то = true).
            clickedStart = true; // (Если нажата кнопка старта, то = true).
        }
        else
        {
            _statePanel.GetComponent<Animator>().SetBool("Do",true);   
        }
    }
    public void OnClickExit() // Публичная функция (При нажатии на выход)
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill(); // Полностью закрываем игру.
        Application.Quit();
    }

    public void OnClickRating()
    {
        //SceneManager.LoadScene("Reting");
    }

    public void OnClickBack() // Публичная функция (При нажатии назад)
    {
        animator.SetBool("IsShow", false);
        exitActive = false;
    }
    public void OnClickContinue() // Публичная функция (При нажатии продолжить)
    {
        _statePanel.GetComponent<Animator>().SetBool("Do", false);
    }
}