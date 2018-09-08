using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Используем библиотеку UnityEngine.UI для управления интерфейсом (Текст, изображения и т.д).

namespace Assets.Scripts
{
    public class LoginScript : MonoBehaviour
    {

        public Image FadeImage; // Картинка для затухания при загрузке.
        public GameObject ExitPanel; // Панель при выходе из игры.
        public InputField InputField;
        public Sprite ErrorConnection;
        public Sprite LoginInvalid;
        private float fadeTime = 4.5f; // Скорость перехода при загрузке.
        private bool _exitActive = false; // Если панель выхода активна, то = true.
        private bool clickedStart = false; // Если нажата кнопка старта, то = true.
        private bool _fade = false; // Если нужен переход при загрузке уровня, то = true.
        private Animator _animator;
        private MainController _main;
        private GameObject _statsPanel;
        private GameObject _exitPanel;

        void Awake() // Данная функция вызывается до инициализации всех остальных скриптов. Обычно используется для установки определенных параметров и инициализации переменных.
        {
            _statsPanel = GameObject.FindWithTag("StatsPanel");
            _exitPanel = GameObject.FindWithTag("ExitPanel");
            _statsPanel.SetActive(false);
            _exitPanel.SetActive(false);
            _animator = GetComponent<Animator>();
            _main = MainController.Instance;
        }

        void Start() // Вызывается один раз при запуске скрипта.
        {

        }

        void Update() // Данная функция вызывается каждый раз перед отображением очередного кадра. Самая используемая для расчетов игровых параметров.
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // Если нажата кнопка (Escape) либо кнопка назад на смартфоне, то:
            {
                if (!_exitActive) // Если выключена панель выхода, то:
                {
                    _animator.SetBool("IsShow", true);
                    _exitActive = true; // (Если панель выхода активна, то = true.)
                }
                else // Иначе
                {
                    _animator.SetBool("IsShow", false);
                    _exitActive = false; // (Если панель выхода неактивна, то = false.)
                }
            }
            if (clickedStart) // Если нажата кнопка начала игры, то:
            {
                PlayerPrefs.DeleteKey("eQ"); // Удалить ключ числа с нумерацией вопроса.
                PlayerPrefs.DeleteKey("wQ");
                PlayerPrefs.DeleteKey("M"); // Удалить ключ сохранённого числа с кол-вом денег.
                PlayerPrefs.DeleteKey("Ended?"); // Удалить ключ о завершении игры.
            }
        }
        void FixedUpdate() // Данная функция вызывается каждый раз при расчете физических показателей. Все расчеты физики следует проводить именно в ней. Но я рассчитываю анимацию перехода, и использую эту функцию для того, чтобы не было никаких тормозов.
        {
            if (_fade) // Если [Надо ли делать переход при загрузке.] = true, то:
            {
                FadeImage.color = Color.Lerp(FadeImage.color, Color.black, fadeTime * Time.deltaTime); // Делаем переход из цвета картинки в чёрный.
            }
            if (FadeImage.color == Color.black) // Если цвет картинки = чёрному, то:
            {
                SceneManager.LoadScene("Menu");  // Загрузить level 1 (Игра)
            }
        }

        public void OnClickStart() // Публичная функция (При нажатии на старт)
        {
            if (InputField.text.Length > 3)
            {
                WWW www = new WWW("http://slutskfish.ru/solveIT?auth=5");
                StartCoroutine(_main.AuthEnum(www, InputField.text));
                if (_main.user.id != -1 && _main.user.id != -2)
                    _fade = true;
                else if (_main.user.id == -2)
                    ShowLoginInvalid();
                else
                    ShowConnectionFall();
            }
            else
                ShowLoginInvalid();
        }

        private void ShowLoginInvalid()
        {
            _statsPanel.SetActive(true);
            _statsPanel.GetComponent<Image>().sprite = LoginInvalid;
            _statsPanel.GetComponent<Animator>().SetBool("Do", true);
        }

        private void ShowConnectionFall()
        {
            _statsPanel.SetActive(true);
            _statsPanel.GetComponent<Image>().sprite = ErrorConnection;
            _statsPanel.GetComponent<Animator>().SetBool("Do", true);
        }

        public void OnClickExit() // Публичная функция (При нажатии на выход)
        {
            Application.Quit();
        }
        public void OnClickBack() // Публичная функция (При нажатии назад)
        {
            _animator.SetBool("IsShow", false);
            _exitActive = false;
        }
        public void OnClickContinue() // Публичная функция (При нажатии продолжить)
        {
            _statsPanel.GetComponent<Animator>().SetBool("Do",false);
            //_statsPanel.SetActive(false);
        }
    }
}