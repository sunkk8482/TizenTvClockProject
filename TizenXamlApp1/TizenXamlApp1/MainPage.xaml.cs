using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace TizenXamlApp1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private ImageSource[] gifSources;  // GIF 파일의 경로를 저장할 배열
        private ImageSource[] pngSources; //png파일의 경로 저장할 배열
        private int currentGifIndex = 0;  // 현재 표시할 GIF 파일의 인덱스
        private int currentPngIndex = 0;  // 현재 표시할 GIF 파일의 인덱스
        int[] num = new int[7]; //로또 번호를 담을 배열
        

        //디지털 시계
        struct HandParams
        {
            public HandParams(double width, double height, double offset) : this()
            {
                Width = width;
                Height = height;
                Offset = offset;
            }

            public double Width { private set; get; }   
            public double Height { private set; get; } 
            public double Offset { private set; get; }  
        }

        static readonly HandParams secondParams = new HandParams(0.02, 0.65, 0.85);
        static readonly HandParams minuteParams = new HandParams(0.05, 0.6, 0.9);
        static readonly HandParams hourParams = new HandParams(0.08, 0.4, 0.9);
        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        public MainPage()
        {
            InitializeComponent();


            //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ배경화면
            // 배경화면을 20초마다 fade형식으로 전환
            gifSources = new ImageSource[]
            {
                ImageSource.FromFile("sun.gif"),
                ImageSource.FromFile("moon.gif")
            };

            pngSources = new ImageSource[]
            {
                ImageSource.FromFile("sun.png"),
                ImageSource.FromFile("moon.png")
            };

            // 첫 번째 GIF 파일 표시
            gifImageView.Source = gifSources[currentGifIndex];
            pngImageView.Source = pngSources[currentPngIndex];
            // 타이머 설정하여 일정 시간마다 GIF 파일 변경
            Device.StartTimer(TimeSpan.FromSeconds(20), () =>
            {
                // 다음 GIF 파일 인덱스로 이동
                currentGifIndex = (currentGifIndex + 1) % gifSources.Length;
                currentPngIndex = (currentPngIndex + 1) % pngSources.Length;
                // 이미지 뷰에 다음 GIF 파일 표시
                AnimateGifChange();
                AnimatePngChange();
                return true;  // 타이머를 계속 실행하도록 true 반환
            });

            //구름이 움직이는 애니메이션 함수
            skyMarqueeEffect(skyImageView);
            //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ디지털시계
            //디지털 시계 구현부분
            // 타이머 설정하여 1초마다 디지털 시계 갱신
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                UpdateClock();
                return true;  // 타이머를 계속 실행하도록 true 반환
            });
            //배경화면에 따라 디지털 시계의 색상변경
            Device.StartTimer(TimeSpan.FromSeconds(1.0), () =>
            {
                
                if (currentGifIndex % 2 == 1)
                {
                    Updatecolor(-1);
                }
                else
                {
                    Updatecolor(0);
                }
                return true;  // 타이머를 계속 실행하도록 true 반환
            });
            //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ날짜
             //현재 날짜 구현부분
            DateTime today = DateTime.Today;
            string tod = today.ToString("yyyy년-MM월-dd일");
            whatistoday.Text = tod;
            //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ버튼 호출
            // 버튼 클릭 이벤트 핸들러 추가
            Button.Clicked += OnButtonClicked;
            
            Button2.Clicked += OnButtonClicked2;

            //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ로또

            //로또 기능 구현부분
            correct.Text = "당첨번호 0개 일치";

            // 가져올 로또 회차
            DateTime startDate = new DateTime(2002, 12, 8); // 시작 날짜 설정
            DateTime currentDate = DateTime.Now.Date; // 현재 날짜 가져오기

            DayOfWeek startDayOfWeek = startDate.DayOfWeek; // 시작 날짜의 요일 가져오기
            DayOfWeek currentDayOfWeek = currentDate.DayOfWeek; // 현재 날짜의 요일 가져오기

            int weekCount = 0; // 회차 변수 초기화

            if (currentDate >= startDate)
            {
                TimeSpan span = currentDate - startDate;
                int totalDays = (int)span.TotalDays;
                int weeks = totalDays / 7;

                if (startDayOfWeek == DayOfWeek.Sunday)
                {
                    weekCount = weeks + 1;
                }
                else
                {
                    weekCount = weeks;
                    int remainingDays = totalDays % 7;
                    if (remainingDays >= (int)startDayOfWeek)
                    {
                        weekCount++;
                    }
                }
            }




            //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡAPI 사용 부분
            //날씨 api와 로또 api를 가져와 표시하는 부분
            string apiUrl = $"https://www.dhlottery.co.kr/common.do?method=getLottoNumber&drwNo={weekCount}";
            string apiKey = "fa04bbddf0aa01d0e9fc44ce7a387ed2";
            string url = $"http://api.openweathermap.org/data/2.5/weather?q=Seoul,kr&mode=xml&units=metric&appid={apiKey}";
            using (WebClient client = new WebClient())
            {
                try
                {
                    aresultLabel.Text = $"로또 {weekCount}회차";
                    string xmlData = client.DownloadString(apiUrl);

                    int[] drwtNos = ExtractDrwtNos(xmlData);
                    int bnusNo = ExtractBnusNo(xmlData);

                    for (int i = 0; i < drwtNos.Length; i++)
                    {
                        Console.WriteLine("drwtNo" + (i + 1) + ": " + drwtNos[i]);
                    }

                    Console.WriteLine("bnusNo: " + bnusNo);
                    aresultLabel1.Text = drwtNos[0].ToString();
                    aresultLabel2.Text = drwtNos[1].ToString();
                    aresultLabel3.Text = drwtNos[2].ToString();
                    aresultLabel4.Text = drwtNos[3].ToString();
                    aresultLabel5.Text = drwtNos[4].ToString();
                    aresultLabel6.Text = drwtNos[5].ToString();
                    aresultLabel7.Text = bnusNo.ToString();

                    num[0] = drwtNos[0];
                    num[1] = drwtNos[1];
                    num[2] = drwtNos[2];
                    num[3] = drwtNos[3];
                    num[4] = drwtNos[4];
                    num[5] = drwtNos[5];
                    num[6] = bnusNo;

                    xmlData = client.DownloadString(url);

                    // XML 파싱하여 기온 추출
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlData);

                    string temperature = xmlDoc.SelectSingleNode("//temperature").Attributes["value"].Value;



                    weather.Text = $"현재 기온: {temperature}℃";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ배경효과 함수
        //배경화면 변경효과 함수
        private async void AnimateGifChange()
        {
            // 이미지 뷰를 투명하게 만들고 0.3초 동안 애니메이션 적용
            await gifImageView.FadeTo(0, 300);

            // 이미지 소스 변경
            gifImageView.Source = gifSources[currentGifIndex];

            // 이미지 뷰를 다시 보이도록 만들고 0.3초 동안 애니메이션 적용
            await gifImageView.FadeTo(1, 300);
        }
        private async void AnimatePngChange()
        {
            // 이미지 뷰를 투명하게 만들고 0.3초 동안 애니메이션 적용
            await pngImageView.FadeTo(0, 300);
            // 이미지 소스 변경
            pngImageView.Source = pngSources[currentPngIndex];

            // 이미지 뷰를 다시 보이도록 만들고 0.3초 동안 애니메이션 적용
            await pngImageView.FadeTo(1, 300);
        }
        //구름이 움직이는 효과 구현
        public static async void skyMarqueeEffect(Image label)
        {
            while (true)
            {

                await label.TranslateTo(-2000, 0, 40000);
                label.TranslationX = 2000;
                await label.TranslateTo(0, 0, 40000);
            }

        }
        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ시계 함수
        //시계 업데이트

        void OnAbsoluteLayoutSizeChanged(object sender, EventArgs args)
        {

            Point center = new Point(absoluteLayout.Width / 2, absoluteLayout.Height / 2);
            double radius = 0.45 * Math.Min(absoluteLayout.Width, absoluteLayout.Height);


            LayoutHand(secondHand, secondParams, center, radius);
            LayoutHand(minuteHand, minuteParams, center, radius);
            LayoutHand(hourHand, hourParams, center, radius);
        }

        void LayoutHand(BoxView boxView, HandParams handParams, Point center, double radius)
        {
            double width = handParams.Width * radius;
            double height = handParams.Height * radius;
            double offset = handParams.Offset;

            AbsoluteLayout.SetLayoutBounds(boxView,
                new Rectangle(center.X - 0.5 * width,
                              center.Y - offset * height,
                              width, height));

            boxView.AnchorY = handParams.Offset;

        }
        private bool UpdateClock()
        {
            // 현재 시간을 포맷에 맞게 가져와서 디지털 시계 Label에 표시
            DateTime currentTime = DateTime.Now;
            clockLabel.Text = currentTime.ToString("HH:mm:ss");



            hourHand.Rotation = 30 * (currentTime.Hour % 12) + 0.5 * currentTime.Minute;
            minuteHand.Rotation = 6 * currentTime.Minute + 0.1 * currentTime.Second;

            // Do an animation for the second hand.
            double t = currentTime.Millisecond / 1000.0;

            if (t < 0.5)
            {
                t = 0.5 * Easing.SpringIn.Ease(t / 0.5);
            }
            else
            {
                t = 0.5 * (1 + Easing.SpringOut.Ease((t - 0.5) / 0.5));
            }

            secondHand.Rotation = 6 * (currentTime.Second + t);
            return true;
        }

        private bool Updatecolor(int num)
        {
            DateTime currentTime = DateTime.Now;

            if (num == -1)
            {
                clockLabel.TextColor = Color.White;
                whatistoday.TextColor = Color.White;
                weather.TextColor = Color.White;
            }
            else
            {
                clockLabel.TextColor = Color.Black;
                whatistoday.TextColor = Color.Black;
                weather.TextColor = Color.Black;
            }
            return true;
        }

        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ버튼 함수
        //버튼클릭 이벤트
        private void OnButtonClicked(object sender, EventArgs e)
        {
            if (lottoTask == null || lottoTask.IsCompleted)
            {
                lottoTask = lotto();
            }
        }
        
        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ로또 함수

        
        static int[] ExtractDrwtNos(string json)
        {
            int[] drwtNos = new int[6];

            // 정규식 패턴을 사용하여 "drwtNo1"부터 "drwtNo6" 값을 추출합니다.
            for (int i = 1; i <= 6; i++)
            {
                string pattern = "\"drwtNo" + i + "\":(\\d+)";
                Match match = Regex.Match(json, pattern);
                if (match.Success)
                {
                    drwtNos[i - 1] = int.Parse(match.Groups[1].Value);
                }
                else
                {
                    Console.WriteLine("Failed to extract drwtNo" + i);
                }
            }
            return drwtNos;
        }
        static int ExtractBnusNo(string json)
        {
            string pattern = "\"bnusNo\":(\\d+)";
            Match match = Regex.Match(json, pattern);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                Console.WriteLine("Failed to extract bnusNo");
                return -1; // 반환할 값이 없을 경우에 대한 처리
            }
        }

        private Task lottoTask;
        private async Task lotto()
        {
            var random = new Random();
            var numbers = new List<int>();

            while (numbers.Count < 7)
            {
                int randomNumber = random.Next(1, 46);
                if (!numbers.Contains(randomNumber))
                {
                    numbers.Add(randomNumber);
                }
            }

            // 결과를 레이블에 표시
            ImageAnimation();
            await Task.Delay(5000); // 3초 대기
            resultLabel1.Text = numbers[0].ToString();
            resultLabel2.Text = numbers[1].ToString();
            resultLabel3.Text = numbers[2].ToString();
            resultLabel4.Text = numbers[3].ToString();
            resultLabel5.Text = numbers[4].ToString();
            resultLabel6.Text = numbers[5].ToString();


            aresult1.Source = "rcircle_image.png";
            aresult2.Source = "rcircle_image.png";
            aresult3.Source = "rcircle_image.png";
            aresult4.Source = "rcircle_image.png";
            aresult5.Source = "rcircle_image.png";
            aresult6.Source = "rcircle_image.png";
            aresult7.Source = "rcircle_image.png";

            int core = 0;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (numbers[i] == num[j])
                    {
                        if (j == 0)
                        {
                            aresult1.Source = "circle_image.png";
                            core++;
                        }
                        else if (j == 1)
                        {
                            aresult2.Source = "circle_image.png";
                            core++;
                        }
                        else if (j == 2)
                        {
                            aresult3.Source = "circle_image.png";
                            core++;
                        }
                        else if (j == 3)
                        {
                            aresult4.Source = "circle_image.png";
                            core++;
                        }
                        else if (j == 4)
                        {
                            aresult5.Source = "circle_image.png";
                            core++;
                        }
                        else if (j == 5)
                        {
                            aresult6.Source = "circle_image.png";
                            core++;
                        }
                        else if (j == 6)
                        {
                            aresult7.Source = "circle_image.png";
                            core++;
                        }
                    }
                }

            }

            correct.Text = $"당첨번호 {core}개 일치";
            await Task.Delay(1000);
        }



        private async void ImageAnimation()
        {
            await Task.WhenAny<bool>
                (
                    result1.RotateTo(720, 3000, Easing.CubicInOut),
                    result1.TranslateTo(1720, 0, 3000, Easing.CubicInOut),
                    result2.RotateTo(720, 3000, Easing.CubicInOut),
                    result2.TranslateTo(1720, 0, 3000, Easing.CubicInOut),
                    result3.RotateTo(720, 3000, Easing.CubicInOut),
                    result3.TranslateTo(1720, 0, 3000, Easing.CubicInOut),
                    result4.RotateTo(720, 3000, Easing.CubicInOut),
                    result4.TranslateTo(1720, 0, 3000, Easing.CubicInOut),
                    result5.RotateTo(720, 3000, Easing.CubicInOut),
                    result5.TranslateTo(1720, 0, 3000, Easing.CubicInOut),
                    result6.RotateTo(720, 3000, Easing.CubicInOut),
                    result6.TranslateTo(1720, 0, 3000, Easing.CubicInOut)


                );
            await Task.WhenAny<bool>
                (
                    result1.RotateTo(0, 3000, Easing.CubicInOut),
                    result1.TranslateTo(0, 0, 3000, Easing.CubicInOut),
                    result2.RotateTo(0, 3000, Easing.CubicInOut),
                    result2.TranslateTo(0, 0, 3000, Easing.CubicInOut),
                    result3.RotateTo(0, 3000, Easing.CubicInOut),
                    result3.TranslateTo(0, 0, 3000, Easing.CubicInOut),
                    result4.RotateTo(0, 3000, Easing.CubicInOut),
                    result4.TranslateTo(0, 0, 3000, Easing.CubicInOut),
                    result5.RotateTo(0, 3000, Easing.CubicInOut),
                    result5.TranslateTo(0, 0, 3000, Easing.CubicInOut),
                    result6.RotateTo(0, 3000, Easing.CubicInOut),
                    result6.TranslateTo(0, 0, 3000, Easing.CubicInOut)
 

                );


        }

        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ 운세함수
        //운세 뽑기 부분

        private Task marqueeTask;

        private void OnButtonClicked2(object sender, EventArgs e)
        {
            if (marqueeTask == null || marqueeTask.IsCompleted)
            {
                marqueeTask = MarqueeEffect(fortune, fortunelayout);
            }

        }


        public static async Task MarqueeEffect(Label label, AbsoluteLayout back)
        {

                await back.TranslateTo(0, 150, 1000);
                string[] farr = new string[10];
                farr[0] = "운세 총운은 암중모색입니다. 자신감으로 인해 더욱 빛나는 당신을 만날 수 있습니다. 당신이 어떻게 하고자 하는 마음이 오늘 당신의 운세를 조정하는 가장 중요한 요인입니다. 주변 환경에 따라가는 것이 아니라 자신이 그 주변환경을 이끄는 사람이 되어야 합니다. 자신의 마음이 평화롭고 안정되기 때문에 하는 일에 있어 더욱 빛을 발할 수 있습니다. 또한 어려운 일이 있다고 실망하거나 포기하지 마세요. 그러한 어려운 상황속에서 더욱 당신의 능력을 발휘할 수 있습니다. 당신 이라면 능히 얻을 수 있는 행운입니다.";
                farr[1] = "운세 총운은 대략성공입니다. 악연이던 사람이 나의 편이 되는 날이니 당신에게 오늘은 큰 기운을 얻는 날입니다. 평소에 불편하게 생각했던 사람이 있다면 사소한 계기로도 자신과 친구가 될 수 있습니다. 믿지 못했던 사람에게 믿음을 돌려 줄 수 있으며 자신에 대한 사람들의 관심 또한 달라질 수 있습니다. 자신의 조그만한 노력 하나에도 모든 사람들과의 관계에 호전을 가져다 줍니다. 또한 주변 동료들이 어색하지 않도록 같이 칭찬 받을 수 있는 분위기를 만들어 본다면 금상첨화 일 것입니다. 은근히 다른 사람에게 공을 돌리는 것도 당신을 더욱 돋보이게 하는 일입니다.";
                farr[2] = "운세 총운은 기세등등입니다. 별다른 일 없이도 괜히 웃음이 나오고 타인에게 호감을 줄 수 있는 날이 될 것으로 보입니다. 원하는 사람을 우연하게 만나는 경우도 생길 수 있으며 자신과 사이가 좋지 않았던 사람 사이에서도 좋은 기운이 가득한 날입니다. 자신이 하는 일에 있어 기대한 만큼의 성과를 얻는다거나 만나는 사람으로 하여금 좋은 인상을 줄 수 있습니다. 그로 인해 사람들과의 관계도 호전적으로 발전할 수 있습니다. 만나자 만나자 해놓고 오랫동안 보지 못했던 친구가 있다면 오늘 만나 보십시오. 더없이 즐거울 것입니다.";
                farr[3] = "운세 총운은 기호지세입니다. 가만히 앉아 있기만 해도 일들이 알아서 잘 처리되고 해결되니 걱정할 것이 없는 하루입니다. 마음을 여유롭게 가지니 답답함이나 스트레스가 없어지고 세상을 보는 시각이 한 없이 넓어질 수 있는 날입니다. 자신이 남에게 크게 베풀지 않아도 그보다 더 많이 돌아오니 자신에게 있어 그보다 더 좋은 일은 없습니다. 하나를 하더라도 차분한 마음으로 대하는 태도를 가지면 더없이 편하고 가볍게 일상을 만들어 갈 수 있을 것이니 편하고 상쾌한 마음으로 아침을 열어 보십시오.";
                farr[4] = "운세 총운은 일보후퇴입니다. 어딘지 모르게 나사가 하나 빠져 있는 것처럼 보이는 날입니다. 평소와 다름없이 행동하지만 마음 한 구석이 허전한 것을 느낄 수 있습니다. 괜히 무기력한 자신을 발견할 수 있으니 이럴 때일 수록 움직이는 횟수를 늘리는 게 좋습니다. 마음만 그럴 뿐 하는 일에는 별다른 장애나 방해를 받지 않고 평소와 다름없이 진행되니 눈에 보이는 결과는 다른 또한 집중력을 회복하는 데에는 그리 오랜 시간이 걸리지 않을 것이고 당신의 존재는 어딘 가든지 환영을 받을 수 있으니 자신감을 잃지 마십시오.";
                farr[5] = "운세 총운은 우공이산입니다. 상쾌한 마음으로 하루를 시작할 수 있을 것이고 이러한 상태는 종일 계속될 것으로 보이는군요. 하는 만큼 일도 진행되기 때문에 스스로에 대한 자부심도 그만큼 생길 수 있습니다. 변화를 꿈꾼다면 새로운 일에 도전하는 것도 좋습니다. 행운의 여신은 자신의 편이기 때문에 하는 일마다 자신의 능력을 발휘해 해결해 나가는데 별 다른 무리를 느끼지 않습니다. 또한 자신에게 화내는 사람에게도 웃을 수 있는 정도의 여유까지 있는 날이니 평소에 까다롭게 여기고 있던 사람을 만나는 것도 좋습니다.";
                farr[6] = "운세 총운은 기대충만입니다. 그야말로 말 그대로 운이 따르는 날입니다. 너무 어렵게 생각해서 쉽게 포기했던 일이 있다면 그것을 다시금 도전해볼 수 있는 날입니다. 백프로 성공을 기대하기 보다는 자신이 목표한 것까지 도달하는데 필요한 여러 가지를 배우는데 만족했다면 더 이상 바라는 게 없을 것입니다. 또한 포기, 실패했다 싶었던 일도 상황의 반전으로 인해 당신에게 유리한 방향으로 전개가 되고 오랫동안 연락이 닿질 않아 만나지 못했던 귀인을 다시 만날 수 있는 날이니 힘차게 나서 보십시오.";
                farr[7] = "운세 총운은 계포일낙입니다. 기분 좋은 일이 하나 둘씩 벌어지니 당신의 마음은 점점 상승가도를 달리게 될 것입니다. 자신이 생각하고 있지 않았던 결과를 좋은 것으로 얻을 수 있으며 자신의 일상에 활력소가 될 수 있습니다. 또한 가족에게도 기분 좋은 소식이 들릴 수 있으니 자신과 자신을 둘러싼 주변에 전체적인 행운이 따를 것입니다. 작은 것부터 시작해서 크고 중요한 일까지 말이지요. 오늘의 이런 기회를 잘 활용할 수 있다면 내일의 당신은, 내년의 당신은 지금보다 훨씬 높은 위치에 있을 것입니다.";
                farr[8] = "운세 총운은 외유내강입니다. 별다른 일 없이도 기분이 좋으니 깔끔하고 상쾌한 하루가 될 것으로 보입니다. 며칠 전부터 고민하고 스트레스를 받고 있던 일이 있다면 그 일의 마무리가 잘 이루어질 것입니다. 그로 인해 마음까지 맑아지는 것을 느낄 수 있습니다. 또한 사람과의 만남에서도 좋은 성과를 얻을 수 있으며 자신의 능력을 인정해주는 일이 생기기 쉽습니다. 또한 인간 관계를 확장할 수 있을 만한 좋은 기회도 따를 것이니 만나는 모든 사람에게 미소를 보여준다면 더없이 좋은 하루가 될 것입니다.";
                farr[9] = "운세 총운은 견토지쟁입니다. 별다른 무리 없이 하루를 진행해 나갈 수 있을 것이나 작은 문제들로부터도 완전히 피해갈 수는 없을 것입니다.그리 큰 문제는 아닐지라도 사소한 문제에 소홀해진다면 그것은 결국 안 좋은 결과를 초래할 수 있습니다. 자신이 나아가는 길에 있어서 조금의 장애가 발생한다면 그것을 발판으로 삼아 다른 기회를 만드는 것에 노력하세요. 사소한 문제부터 해결하려는 마음이 결국 큰 문제를 피하는 지름길이 될 것입니다. 조금만 더 신중해 진다면 그만한 결과가 따른다는 점을 잊지 않도록 하십시오.";

                Random random = new Random();
                int randomNumber = random.Next(10);

                label.Text = farr[randomNumber];

            
                await back.TranslateTo(0, 0, 1000);

            await label.TranslateTo(-20000, 0, 100000);
            label.Text = "버튼을 눌러 운세를 뽑아주세요";
                label.TranslationX = 0;
            await Task.Delay(1000);

        }
        

            
        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
       
    }

}
