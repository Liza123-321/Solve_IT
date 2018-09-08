using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Используем библиотеку UnityEngine.UI для управления интерфейсом (Текст, изображения и т.д).

namespace Assets.Scripts
{
    public class GameScript : MonoBehaviour
    {
        public Text QuastionQount;
        public Text AttemptsCountText;
        public Text MoneyCounText;
        public Text[] Answers; //Массив | Текст для отображения вариантов ответов.
        public Text[] Stats; //Массив | Текст для отображения статистики при ответе.
        public Button[] Bttns; //Массив | Кнопки ответов.
        public Sprite UnclickButton;
        public Sprite TrueAnswer; //Спрайт при правильном ответе.
        public Sprite FalseAnswer; //Спрайт при неправильном ответе.
        public Sprite ClickedAnswerS; // //Спрайт при нажатии на кнопку.
        public Sprite TrueAnswerPanel;
        public Sprite FalseAnswerPanel;
        public Sprite DefaultAnswerPanel;
        public Sprite EndAnswerPanel;
        public Image FadeImage; //Картинка для затухания при загрузке.
        public Image[] BttnsImages; //Массив | Тут хранятся картинки кнопок.
        public Animator StatsAnimation; // Анимация при появлении статистики.
        public GameObject PausePanel; // Панель паузы.
        public GameObject StatsPanel;
        public int Wq = 1; // Число с нумерацией вопроса.
        private int _clickedAnswer = -1; // Число (Можно так сказать индекс) нажатого варианта ответа.
        private static int _attemptsCount = 2;      // сколько осталось попыток
        private int _money; // рейтинг.
        private int _trueint = -1; // Число с правильным ответом.
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
        private action _questAction;
        private MainController _main;
        
        delegate void action();

        private void Awake() // Данная функция вызывается до инициализации всех остальных скриптов. Обычно используется для установки определенных параметров и инициализации переменных.
        {
            FadeImage.enabled = true; // Включить чёрный фон для перехода.
            _goFadeOut = true; //Делаем переход (из черноты) при загруженом уровне.
            _main = MainController.Instance;
            _money = _main.user.money;
            if (PlayerPrefs.HasKey("M") && PlayerPrefs.HasKey("wQ")) // Если в сохранённых данных есть эти значения то:
            {
                _money = PlayerPrefs.GetInt("M"); // Взять переменную из сохранений.
                Wq = PlayerPrefs.GetInt("wQ"); // Взять переменную из сохранений.
            }
            else // Если данных нет, то:
            {
                _money = _main.user.money;
                PlayerPrefs.SetInt("wQ", 1); // Сохранить переменную.
                PlayerPrefs.SetInt("M", _money); // Сохранить переменную.
            }
            PlayerPrefs.Save(); // Сохранить изменения в сохранениях :)
        }

        private void Start()
        {
            if (Wq == 1) // Если число с нумерацией вопроса = 1, то:
                _questAction = Rq1; // Запускаем 1 вопрос.
            else if (Wq == 2) // Иначе если число с нумерацией вопроса = 2, то:
                _questAction = Rq2; // Запускаем 2 вопрос. (Дальше всё тоже самое).
            else if (Wq == 3)
                _questAction = Rq3;
            else if (Wq == 4)
                _questAction = Rq4;
            else if (Wq == 5)
                _questAction = Rq5;
            else if (Wq == 6)
                _questAction = Rq6;
            else if (Wq == 7)
                _questAction = Rq7;
            else
            {
                _questAction = Rq1;
            }
            _questAction.Invoke();
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
            QuastionQount.text = string.Format("{0}/7", Wq);
            MoneyCounText.text = string.Format("{0} очков", _money);
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
            Answers[0].text = "Пароль от Wi-fi"; // Вариант ответа 1.
            Answers[1].text = "Кафе"; // Вариант ответа 2.
            Answers[2].text = "Ноутбук"; // Вариант ответа 3.
            Answers[3].text = "Wi-fi"; // Вариант ответа 4.
            Answers[4].text = "Antilite";
            Stats[1].text =
                    "История: Злоумышленники с помощью программы Antilite в одном из столичных кафе подключились по бесплатной сети" +
                    " wi-fi к компьютеру жертвы и перехватили личные данные, передающие по сети. Т.к. сеть находилась в общем доступе," +
                    " пароль от него был не нужен."; // Текст при показании статистики.
            _trueint = 0; // Правильный ответ равен 1.

            if (_clickedAnswer == 0) // Если нажатый ответ равен 1, то:
                StartCoroutine(
                    WaitForMagic()); // Запускаем корутину выигрыша (Корутина - простой и удобный способ запускать функции, которые должны работать параллельно в течение некоторого времени).
            else if (_clickedAnswer == 1 || _clickedAnswer == 2 || _clickedAnswer == 3 || _clickedAnswer == 4
            ) // Иначе если нажатый ответ равен 0 или 2 или 3, то:
                StartCoroutine(waitForDestroy()); // Запускаем корутину проигрыша.
        }

        private void Rq2() // Смотрите на вопрос 1.
        {
            Answers[0].text = "Верёвка с кровью";
            Answers[1].text = "Офис на 7 этаже";
            Answers[2].text = "Флешка с вредоносным ПО";
            Answers[3].text = "Ноутбук";
            Answers[4].text = "Перчатки";
            Stats[1].text =
                    "Злоумышленник с помощью верёвки пробрался в офис директора крупной компании, который находился на 7 этаже. Чтобы взломать ноутбук и скопировать все необходимые" +
                    " ему данные, он использовал флешку с вредоносным ПО. На веревке не было бы крови, если бы злоумышленник не забыл надеть перчатки.";
            _trueint = 4;
            if (_clickedAnswer == 4)
                StartCoroutine(WaitForMagic());
            else if (_clickedAnswer == 0 || _clickedAnswer == 1 || _clickedAnswer == 2 || _clickedAnswer == 3)
                StartCoroutine(waitForDestroy());
        }

        private void Rq3() // Смотрите на вопрос 1.
        {
            Answers[0].text = "Настоящие доменные имена";
            Answers[1].text = "Хостинг";
            Answers[2].text = "Администраторы сайта";
            Answers[3].text = "Похожие доменные имена";
            Answers[4].text = "СМС- рассылка";
            Stats[1].text =
                    "Злоумышленник произвел рассылку электронных сообщений, в которых сообщал администраторам сайтов о попытке неизвестных лиц зарегистрировать" +
                    " доменные имена, похожие на адреса, принадлежащие им. И предлагал купить им эти похожие доменные имена. Следовательно, хостинг не фигурирует в данном преступлении.";
            _trueint = 1;

            if (_clickedAnswer == 1)
                StartCoroutine(WaitForMagic());
            else if (_clickedAnswer == 0 || _clickedAnswer == 2 || _clickedAnswer == 3 || _clickedAnswer == 4)
                StartCoroutine(waitForDestroy());
        }

        private void Rq4() // Смотрите на вопрос 1.
        {
            Answers[0].text = "Сиквел, размещенный в интернете";
            Answers[1].text = "Костюмы для сиквела";
            Answers[2].text = "Актеры для сиквела";
            Answers[3].text = "Сюжет фильма";
            Answers[4].text = "Авторское право";
            Stats[1].text =
                    "История: Студент ГИТИСа , основываясь на сюжете популярного фильма, снял собственное продолжение (сиквел) и разметил его в Интернете." +
                    " Его сокурсники выступили актерами в этом фильме, а также помогли создать костюмы. Парень не приобрел авторское право, разрешающее снимать продолжение," +
                    " и из-за этого его действия оказались преступными.";
            _trueint = 4;

            if (_clickedAnswer == 4)
                StartCoroutine(WaitForMagic());
            else if (_clickedAnswer == 0 || _clickedAnswer == 1 || _clickedAnswer == 2 || _clickedAnswer == 3)
                StartCoroutine(waitForDestroy());
        }

        private void Rq5() // Смотрите на вопрос 1.
        {
            Answers[0].text = "Сим-карта";
            Answers[1].text = "Мобильный банк";
            Answers[2].text = "Социальная сеть";
            Answers[3].text = "Деньги";
            Answers[4].text = "Банковская карта";
            Stats[1].text =
                    "История: Преступник под видом игры Pokémon GO распространял вредоносное ПО. Программа перехватывала смс, присланные банком," +
                    " а также обеспечивала доступ к интернет-банкингу клиентов. Деньги можно было списывать со счета без наличия самой банковской карты.";
            _trueint = 4;

            if (_clickedAnswer == 4)
                StartCoroutine(WaitForMagic());
            else if (_clickedAnswer == 0 || _clickedAnswer == 1 || _clickedAnswer == 2 || _clickedAnswer == 3)
                StartCoroutine(waitForDestroy());
        }

        private void Rq6() // Смотрите на вопрос 1.
        {
            Answers[0].text = "Пляж";
            Answers[1].text = "Фотоаппарат Nikon Coolpix P900";
            Answers[2].text = "Гонорар";
            Answers[3].text = "Бинокль";
            Answers[4].text = "Дженнифер Лопес";
            Stats[1].text =
                    "Начинающий журналист, желая получить хороший гонорар, отправился на пляж Miami Beach в поисках откровенных фотографий Дженнифер Лопес." +
                    " Чтобы остаться незамеченным при фотосъёмке он использовал фотоаппарат Nikon Coolpix P900 с большим зумом. Следовательно, бинокль был ему" +
                    " не нужен из-за хорошего увеличения фотоаппарата.";
            _trueint = 3;

            if (_clickedAnswer == 3)
                StartCoroutine(WaitForMagic());
            else if (_clickedAnswer == 0 || _clickedAnswer == 1 || _clickedAnswer == 2 || _clickedAnswer == 3)
                StartCoroutine(waitForDestroy());
        }

        private void Rq7() // Смотрите на вопрос 1.
        {
            Answers[0].text = "Youtube";
            Answers[1].text = "Аккаунт german_brain от Youtube";
            Answers[2].text = "Программа Icecream Screen Recorder";
            Answers[3].text = "Авторское право";
            Answers[4].text = "Выпуск Comedy Club от 21.04.2017";
            Stats[1].text =
                    "Чтобы раскрутить свой аккаунт(german_brain) на Youtube тинейджер с помощью программы записи с экрана (Icecream Screen Recorder) переснял " +
                    "Comedy Club от 21.04.2017.Но его ролик был удалён в связи с нарушением авторского права.";
            _trueint = 3;

            if (_clickedAnswer == 3)
                StartCoroutine(WaitForMagic());
            else if (_clickedAnswer == 0 || _clickedAnswer == 1 || _clickedAnswer == 2 || _clickedAnswer == 4)
                StartCoroutine(waitForDestroy());
        }

        private void Rq8() // Смотрите на вопрос 1.
        {
            Answers[0].text = "Интернет-банкинг";
            Answers[1].text = "Pоcemon GO";
            Answers[2].text = "Банковская карта";
            Answers[3].text = "Смс-рассылка от банка";
            Answers[4].text = "Вредоносное ПО";
            Stats[1].text =
                    "Преступник под видом игры Pokémon GO распространял вредоносное ПО. Программа перехватывала смс, присланные банком, а также обеспечивала доступ" +
                    " к интернет-банкингу клиентов. Деньги можно было списывать со счета без наличия самой банковской карты.";
            _trueint = 2;

            if (_clickedAnswer == 2)
                StartCoroutine(WaitForMagic());
            else if (_clickedAnswer == 0 || _clickedAnswer == 1 || _clickedAnswer == 3 || _clickedAnswer == 4)
                StartCoroutine(waitForDestroy());
        }

        private IEnumerator WaitForMagic() // Корутина выигрыша.
        {
            Bttns[0].interactable = false; // Отключить нажатие кнопки 1.
            Bttns[1].interactable = false; // Отключить нажатие кнопки 2.
            Bttns[2].interactable = false; // Отключить нажатие кнопки 3.
            Bttns[3].interactable = false; // Отключить нажатие кнопки 4.
            Bttns[4].interactable = false; // Отключить нажатие кнопки 4.
            _trueA = true; // Если ответ правильный, то равно true.
            BttnsImages[_clickedAnswer].sprite = ClickedAnswerS; // Присвоить кнопке спрайт нажатой кнопки.
            yield return new WaitForSeconds(1f); // Продолжить через 3 сек.
            BttnsImages[_clickedAnswer].overrideSprite = TrueAnswer; // Присвоить кнопке спрайт правильно нажатой кнопки.
            yield return new WaitForSeconds(1f); // Продолжить через 2 сек.
            StatsWindow(); // Запустить функцию статистики.
            StopCoroutine(WaitForMagic()); // Остановить корутину выигрыша.
        }

        private IEnumerator waitForDestroy() // Корутина проигрыша.
        {
            Bttns[0].interactable = false; // Отключить нажатие кнопки 1.
            Bttns[1].interactable = false; // Отключить нажатие кнопки 2.
            Bttns[2].interactable = false; // Отключить нажатие кнопки 3.
            Bttns[3].interactable = false; // Отключить нажатие кнопки 4.
            Bttns[4].interactable = false; // Отключить нажатие кнопки 4.
            BttnsImages[_clickedAnswer].sprite = ClickedAnswerS; // Присвоить кнопке спрайт нажатой кнопки.
            _falseA = true; // Если ответ неправильный, то равно true.
            yield return new WaitForSeconds(1f); // Продолжить через 3 сек.
            BttnsImages[_clickedAnswer].overrideSprite = FalseAnswer; // Присвоить кнопке спрайт неправильно нажатой кнопки.
            if (_attemptsCount != 2)
            {
                BttnsImages[_trueint].sprite = TrueAnswer; // Присвоить спрайт кнопке, которая была правильным ответом.
                Answers[_trueint].color = Color.white;
            }
            if (dontlikeit) // Если dontlikeit = true, то:
            {
                Application.Quit(); // Выйти из игры. Возможна ошибка при выходе.
            }
            else // Иначе:
            {
                yield return new WaitForSeconds(1f); // Продолжить через 3 сек.
                StatsWindow(); // Запустить функцию статистики.
                StopCoroutine(waitForDestroy()); // Остановить корутину проигрыша.
            }
        }

        private void overTime() // Приватная функция (overTime)
        {
            Bttns[0].interactable = false; // Отключить нажатие кнопки 1.
            Bttns[1].interactable = false; // Отключить нажатие кнопки 2.
            Bttns[2].interactable = false; // Отключить нажатие кнопки 3.
            Bttns[3].interactable = false; // Отключить нажатие кнопки 4.
            Bttns[4].interactable = false; // Отключить нажатие кнопки 4.
            StatsWindow(); // Запустить функцию (Статистика)
        }

        private void StatsWindow() // Приватная функция (Окно статистики)
        {
            if (Wq != 7)
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
                    //StatsPanel.GetComponent<Image>().sprite = TrueAnswerPanel;
                    Stats[0].text = "ПОЗДРАВЛЯЕМ! ТЫ ВЫБРАЛ ПРАВИЛЬНЫЙ ОТВЕТ!"; // Присвоить тексту строку.
                    PlayerPrefs.SetInt("MR", _money); // Сохранить кол-во денег для отображения при окончании игры.
                    PlayerPrefs.SetInt("M", _money); // Сохранить кол-во денег для игры.
                    //Stats[2].text = "Вопрос: " + Wq + "/ 8"; // Присвоить тексту строку.
                    //Stats[3].text = "Ваш рейтинг: " + _money; // Присвоить тексту строку.
                }
                else if (_falseA) // Иначе если ответ неправильный, то:
                {
                    _money += 1; // Присвоить деньгам нулевое число.
                    _lose = true; // (Если проиграл, то равно true)
                    StatsPanel.GetComponent<Image>().sprite = DefaultAnswerPanel;
                    Stats[0].text = "НЕПРАВИЛЬНЫЙ ОТВЕТ"; // Присвоить тексту строку.
                    if (_attemptsCount == 2)
                        Stats[1].text = "У ВАС ОСТАЛАСЬ ОДНА ПОПЫТКА";
                    //Stats[2].text = "Вопрос: " + Wq + "/ 8"; // Присвоить тексту строку.
                    //Stats[3].text = "Ваш рейтинг: " + _money; // Присвоить тексту строку.
                    PlayerPrefs.SetInt("MR", _money); // Сохранить кол-во денег для отображения при окончании игры.
                    PlayerPrefs.SetInt("M", _money); // Сохранить кол-во денег для игры.
                    PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
                }
            }
            else
            {

                StatsPanel.GetComponent<Image>().sprite = EndAnswerPanel;
                foreach (var stat in Stats)
                {
                    stat.text = "";
                }
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
                SceneManager.LoadScene(_attemptsCount-- == 2 ? "Task3" : "Menu");
                //SceneManager.LoadScene("Game");
                //Application.LoadLevel(0); // Загружаем 0 level (Меню).
            }
        }

        private void continueLoad() // Приватная функция (Продолжить загрузку)
        {
            if (Wq == 7) // Если прошли полностью игру, то:
            {
                Wq = 0;
                MainController main = MainController.Instance;
                StartCoroutine(main.SaveStage(new WWW(
                    String.Format("http://slutskfish.ru/solveIT?login={0}&stage=4&money={1}", main.user.name,
                        _money))));
                SceneManager.LoadScene("Menu"); // Загружаем 0 level (Меню).
                PlayerPrefs.SetString("Ended?", "Ended"); // Сохраняем строку Ended.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях. 
            }
            else // Иначе (Если не прошли)
            {
                Wq++; // Прибавить 1 к числу с нумерацией вопроса.
                _isLoaded = true; // (Если уровень загружен, то равно true.)
                PlayerPrefs.SetInt("wQ", Wq); // Сохраняем число с нумерацией вопроса.
                PlayerPrefs.Save(); // Сохранить изменения в сохранениях.
                SceneManager.LoadScene("Task3"); // Загрузить 1 level (Перезапускаем уровень с игрой для нового вопроса).
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
                    Answers[i].color = i == _clickedAnswer ? Color.white : Color.black;
                }
                
            }
            else
            {
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