using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Используем библиотеку UnityEngine.UI для управления интерфейсом (Текст, изображения и т.д).

namespace Assets.Scripts
{
    public class Task1 : MonoBehaviour
    {
        public Text QuastionQount;
        public Text AttemptsCountText;
        public Text Quastion; // Текст для отображения вопросов.
        public Text TextTask; //Тут будет остальная часть задания
        public Text[] Answers; //Массив | Текст для отображения вариантов ответов.
        public Text[] Stats; //Массив | Текст для отображения статистики при ответе.
        public Button[] Bttns; //Массив | Кнопки ответов.
        public InputField inputAnswer; // ответ,вводимый пользователем
        public Sprite UnclickButton;
        public Sprite TrueAnswer; //Спрайт при правильном ответе.
        public Sprite FalseAnswer; //Спрайт при неправильном ответе.
        public Sprite ClickedAnswerS; // //Спрайт при нажатии на кнопку.
        public Image FadeImage; //Картинка для затухания при загрузке.
        public Image[] BttnsImages; //Массив | Тут хранятся картинки кнопок.
        public Animator StatsAnimation; // Анимация при появлении статистики.
        public GameObject PausePanel; // Панель паузы.
        public int Wq = 1; // Число с нумерацией вопроса.
        private int _clickedAnswer = -1; // Число (Можно так сказать индекс) нажатого варианта ответа.
        private static int _attemptsCount = 2;      // сколько осталось попыток
        private int _money; // рейтинг.
        private int _trueint = -1; // Число с правильным ответом.

        private string _truestring = "";

        private readonly float fadeTime = 4.5f; // Скорость перехода при загрузке.
        private bool _getMoney; // Равен true если деньги уже были даны.
        private bool _lose; // Если ответ неверный, то равно true.
        private bool _goFadeIn; // Надо ли делать переход при загрузке.
        private bool _goFadeOut; // Надо ли делать переход (из черноты) при загруженом уровне.
        private readonly bool dontlikeit = false; // Игра не понравилась? # ТАГДА УХАДИ
        private readonly bool ended = false; // Закончена ли игра полностью?
        private bool _pauseActive; // Если включена пауза, то равно true.
        private bool _isLoaded; // Если уровень загружен, то равно true.
        private bool _trueA; // Равно true если ответ верный.
        private bool _falseA; // Равно true если ответ неверный.
        //private Action _questAction;

        private void Awake() // Данная функция вызывается до инициализации всех остальных скриптов. Обычно используется для установки определенных параметров и инициализации переменных.
        {
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
                PlayerPrefs.SetInt("M", 0); // Сохранить переменную.
            }
            PlayerPrefs.Save(); // Сохранить изменения в сохранениях :)
        }

        private void Start()
        {
            //if (Wq == 1) // Если число с нумерацией вопроса = 1, то:
            //    //_questAction = Rq1; // Запускаем 1 вопрос.
            //else if (Wq == 2) // Иначе если число с нумерацией вопроса = 2, то:
            //    //_questAction = Rq2; // Запускаем 2 вопрос. (Дальше всё тоже самое).
            //else if (Wq == 3)
            //    //_questAction = Rq3;
            ////_questAction.Invoke();
        }

        private void
            Update() // Данная функция вызывается каждый раз перед отображением очередного кадра. Самая используемая для расчетов игровых параметров.
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // Если нажата кнопка (Escape), либо кнопка назад на телефоне, то:
                if (_pauseActive == false) // Если пауза не активна, то:
                {
                    PausePanel.SetActive(true); // Включить панель паузы.
                    _pauseActive = true; // Если включена пауза, то равно true.
                }
                else // Если пауза активна, то:
                {
                    PausePanel.SetActive(false); // Выключить панель паузы.
                    _pauseActive = false; //Если выключена пауза, то равно false.
                }
            QuastionQount.text = string.Format("{0}/8", Wq);
            AttemptsCountText.text = string.Format("У тебя осталось {0} попыток!", _attemptsCount);
        }

        private void
            FixedUpdate() // Данная функция вызывается каждый раз при расчете физических показателей. Все расчеты физики следует проводить именно в ней. Но я рассчитываю анимацию перехода, и использую эту функцию для того, чтобы не было никаких тормозов.
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
                    continueLoad(); // Запускаем функцию continueLoad (416 строка).
        }

        private void Rq1() // Приватная функция (Вопрос 1)
        {
            Quastion.text = "Разгадайте зашифрованное имя существительное " +
                "(может быт собственным или нарицательным)."; // Задание.
            TextTask.text = "Shape of my heart" +
                "Wonderful life" +
                "The Passanger" +
                "Masterpiece" +
                "Waterloo" +
                "Smells like a teen spirit";
            //Answers[0].text = "Пароль от Wi-fi"; // Вариант ответа 1.
            //Answers[1].text = "Кафе"; // Вариант ответа 2.
            //Answers[2].text = "Ноутбук"; // Вариант ответа 3.
            //Answers[3].text = "Wi-fi"; // Вариант ответа 4.
            //Answers[4].text = "Antilite";
            Stats[1].text =
                    "SHIMAN – заглавные буквы авторов песен. " +
                    "Sting" +
                    "Hurts" +
                    "Iggy Pop" +
                    "Madonna" +
                    "Abba" +
                    "Nirvana"; // Текст при показании статистики.
                               // _trueint = 0; // Правильный ответ равен 1.
            _truestring = "SHIMAN";

            if (inputAnswer.text=="SHIMAN"||inputAnswer.text=="shiman") // если введенный вариант верный
                StartCoroutine(
                    WaitForMagic()); // Запускаем корутину выигрыша (Корутина - простой и удобный способ запускать функции, которые должны работать параллельно в течение некоторого времени).
            else
                StartCoroutine(waitForDestroy()); // Запускаем корутину проигрыша.
        }

        private void Rq2() // Смотрите на вопрос 1.
        {
            Quastion.text = "Готовится ограбление банков в нескольких столицах." +
                "Найдите в сообщении, банки каких столиц будут ограблены"; // Задание.
            TextTask.text = "11 мая, 17:00, моя поездка к Шарлотте" +
                "Вчера я села на поезд до Парижа.Впервые смотрела в купе" +
                " кинофильм «Притяжение» на своем iPad." +
                "По прибытию я узнала, что подруга меня не встретит: " +
                "Шарлотта в академии была загружена работой. Чтобы скоротать " +
                "время я пошла в магазин, где купила вино, салями, сыр и газировку." +
                "Оказалось, что мой французский не так уж и плох." +
                "Как только я вышла из магазина, мне набрала Шарлотта" +
                " и сказала: «Босс отпустил меня домой, приезжай на «Ле - Минс» к кофейне «Де Флор»";

            //Answers[0].text = "Пароль от Wi-fi"; // Вариант ответа 1.
            //Answers[1].text = "Кафе"; // Вариант ответа 2.
            //Answers[2].text = "Ноутбук"; // Вариант ответа 3.
            //Answers[3].text = "Wi-fi"; // Вариант ответа 4.
            //Answers[4].text = "Antilite";
            Stats[1].text =" Париж, Пекин, Оттава, Рига, Минск."; // Текст при показании статистики.
                                                       
            _truestring = "Париж, Пекин, Оттава, Рига, Минск";

            if (inputAnswer.text == "SHIMAN" || inputAnswer.text == "shiman") // если введенный вариант верный
                StartCoroutine(
                    WaitForMagic()); // Запускаем корутину выигрыша (Корутина - простой и удобный способ запускать функции, которые должны работать параллельно в течение некоторого времени).
            else
                StartCoroutine(waitForDestroy()); // Запускаем корутину проигрыша.
        }

        private void Rq3() // Смотрите на вопрос 1.
        {
            Quastion.text = "Расшифруйте 7-значный номер телефона жертвы, который спрятан в сообщении."; // Задание.
            TextTask.text = "JACK RECEIVED YOUR" +
                "BEAUTIFUL BOOK ON JAVA" +
                "LANGUAGE PROGRAMMING ART" +
                "ON NOVEMBER 3.BUT HE HAD" +
                "TO LEAVE MADRID IMMEDIATELY. " +
                "WITH LOVE TO YOU," +
                "IDA PETREX";

            Stats[1].text = "4415819"; // Текст при показании статистики.

            _truestring = "4415719";

            if (inputAnswer.text == "4415719") // если введенный вариант верный
                StartCoroutine(
                    WaitForMagic()); // Запускаем корутину выигрыша (Корутина - простой и удобный способ запускать функции, которые должны работать параллельно в течение некоторого времени).
            else
                StartCoroutine(waitForDestroy()); // Запускаем корутину проигрыша.
        }

    
        private IEnumerator WaitForMagic() // Корутина выигрыша.
        {
            _trueA = true; // Если ответ правильный, то равно true.
            BttnsImages[_clickedAnswer].sprite = ClickedAnswerS; // Присвоить кнопке спрайт нажатой кнопки.
            yield return new WaitForSeconds(3f); // Продолжить через 3 сек.

            BttnsImages[_clickedAnswer].overrideSprite = TrueAnswer; // Присвоить кнопке спрайт правильно нажатой кнопки.
            yield return new WaitForSeconds(2f); // Продолжить через 2 сек.
            StatsWindow(); // Запустить функцию статистики.
            StopCoroutine(WaitForMagic()); // Остановить корутину выигрыша.
        }

        private IEnumerator waitForDestroy() // Корутина проигрыша.
        {
            BttnsImages[_clickedAnswer].sprite = ClickedAnswerS; // Присвоить кнопке спрайт нажатой кнопки.
            _falseA = true; // Если ответ неправильный, то равно true.
            yield return new WaitForSeconds(3f); // Продолжить через 3 сек.
            BttnsImages[_clickedAnswer].overrideSprite = FalseAnswer; // Присвоить кнопке спрайт неправильно нажатой кнопки.
            if (_attemptsCount != 2)
                BttnsImages[_trueint].sprite = TrueAnswer; // Присвоить спрайт кнопке, которая была правильным ответом.
            if (dontlikeit) // Если dontlikeit = true, то:
            {
                Application.Quit(); // Выйти из игры. Возможна ошибка при выходе.
            }
            else // Иначе:
            {
                yield return new WaitForSeconds(3f); // Продолжить через 3 сек.
                StatsWindow(); // Запустить функцию статистики.
                StopCoroutine(waitForDestroy()); // Остановить корутину проигрыша.
            }
        }

        private void overTime() // Приватная функция (overTime)
        {
            StatsWindow(); // Запустить функцию (Статистика)
        }

        private void StatsWindow() // Приватная функция (Окно статистики)
        {
            if (_trueA) // Если нажат правильный ответ, то:
            {
                if (_getMoney == false) // Если деньги не давались, то:
                {
                    _money += _attemptsCount != 1 ? 5 : 3;
                    _getMoney = true; // (Если деньги уже даны, то равно true).
                    PlayerPrefs.SetInt("MR", _money); // Сохранить кол-во денег для отображения при окончании игры.
                    PlayerPrefs.SetInt("M", _money); // Сохранить кол-во денег для игры.
                    PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
                }
                _lose = false; // (Если проиграл, то равен true). В этом случае false.
                Stats[0].text = "Правильный ответ"; // Присвоить тексту строку.
                Stats[2].text = "Вопрос: " + Wq + "/ 8"; // Присвоить тексту строку.
                Stats[3].text = "Ваш рейтинг: " + _money; // Присвоить тексту строку.
            }
            else if (_falseA) // Иначе если ответ неправильный, то:
            {
                _money += 0; // Присвоить деньгам нулевое число.
                _lose = true; // (Если проиграл, то равно true)
                Stats[0].text = "Неправильный ответ"; // Присвоить тексту строку.
                if (_attemptsCount == 2)
                    Stats[1].text = "У вас осталась одна попытка";
                Stats[2].text = "Вопрос: " + Wq + "/ 8"; // Присвоить тексту строку.
                Stats[3].text = "Ваш рейтинг: " + _money; // Присвоить тексту строку.
                PlayerPrefs.SetInt("MR", _money); // Сохранить кол-во денег для отображения при окончании игры.
                PlayerPrefs.SetInt("M", _money); // Сохранить кол-во денег для игры.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
            }
            StatsAnimation.SetTrigger("Do"); // Выбрать триггер анимации на Do. (Триггер создан в Animator)
        }

        public void ContinueBttn() // Публичная функция (Кнопка продолжения)
        {
            if (_attemptsCount == 1 || _lose == false) // Если lose = false, то:
            {
                _goFadeOut = false; // (Делать переход (из черноты) при загруженом уровне если = true).
                _goFadeIn = true; // (Делать переход в черноту при загрузке уровня если = true).
                _attemptsCount = 2;
            }
            else // Иначе (Если мы проиграли)
            {
                SceneManager.LoadScene(_attemptsCount-- == 2 ? "Game" : "Menu");
                //SceneManager.LoadScene("Game");
                //Application.LoadLevel(0); // Загружаем 0 level (Меню).
            }
        }

        private void continueLoad() // Приватная функция (Продолжить загрузку)
        {
            if (ended) // Если прошли полностью игру, то:
            {
                SceneManager.LoadScene("Menu"); // Загружаем 0 level (Меню).
                PlayerPrefs.SetString("Ended?", "Ended"); // Сохраняем строку Ended.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях. 
            }
            else // Иначе (Если не прошли)
            {
                Wq++; // Прибавить 1 к числу с нумерацией вопроса.
                _isLoaded = true; // (Если уровень загружен, то равно true.)
                PlayerPrefs.SetInt("eQ", Wq); // Сохраняем число с нумерацией вопроса.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
                SceneManager.LoadScene("Game"); // Загрузить 1 level (Перезапускаем уровень с игрой для нового вопроса).
            }
        }

        public void selectedBttn(int clickBttn) // Публичная функция (Нажатая кнопка)
        {
            _clickedAnswer = clickBttn; // Число нажатого ответа = clickBttn.
            if (BttnsImages[_clickedAnswer].sprite != ClickedAnswerS)
            {
                for (int i = 0; i < 5; i++)
                {
                    Bttns[i].animator.SetBool("Do", i == _clickedAnswer);    // анимация кнопки
                    BttnsImages[i].sprite = i == _clickedAnswer ? ClickedAnswerS : UnclickButton;    // Присвоить кнопке спрайт нажатой кнопки
                }
                
            }
            //else
            //{
            //    _questAction.Invoke();
            //}
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