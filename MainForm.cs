using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace Day015_01_컬러영상처리_Beta1_
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        //전역변수 선언부
        byte[,,] inImage = null;
        byte[,,] outImage = null;
        int inH, inW, outH, outW;
        string fileName;
        const int RGB = 3, RR = 0, GG = 1, BB = 2; //숫자보다는 기호로 쓰는 게 좋다
        // /////////////
        // 조작 함수부
        /// 임시 파일용 배열과 임시 파일 개수
        string[] tmpFiles = new string[500]; // 최대 500개
        int tmpIndex = 0;
        void saveTempFile()
        {
            //////////////////////////////////////////
            // 영상처리 효과가 계속 누적되도록 함.
            //////////////////////////////////////////
            /// (1) 입력영상을 디스크에 저장 
            string saveFname = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            Bitmap image = new Bitmap(inH, inW); // 빈 비트맵(종이) 준비
            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    Color c; int r, g, b;
                    r = inImage[0, i, k];
                    g = inImage[1, i, k];
                    b = inImage[2, i, k];
                    c = Color.FromArgb(r, g, b);
                    image.SetPixel(i, k, c); // 종이에 콕콕 찍기
                }
            image.Save(saveFname, System.Drawing.Imaging.ImageFormat.Png);

            tmpFiles[tmpIndex++] = saveFname;

            /// (2) 출력영상 --> 입력영상

            inH = outH; inW = outW;
            inImage = new byte[RGB, inH, inW];
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                        inImage[rgb, i, k] = outImage[rgb, i, k];
        }
        void restoreTempFile()
        {
            if (tmpIndex <= 0)
                return;
            fileName = tmpFiles[--tmpIndex];
            // 파일 --> 비트맵(bitmap)
            bitmap = new Bitmap(fileName);
            // 중요! 입력이미지의 높이, 폭 알아내기
            inW = bitmap.Height;
            inH = bitmap.Width;
            inImage = new byte[RGB, inH, inW]; // 메모리 할당
            // 비트맵(bitmap) --> 메모리 (로딩)
            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    Color c = bitmap.GetPixel(i, k);
                    inImage[RR, i, k] = c.R;
                    inImage[GG, i, k] = c.G;
                    inImage[BB, i, k] = c.B;
                }

            equal_image();

            // System.IO.File.Delete(fileName); // 임시파일 삭제 
        }
        // ////////////
        //공통 함수부
        void openImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();  // 객체 생성
            ofd.DefaultExt = "";
            ofd.Filter = "칼라 필터 | *.png; *.jpg; *.bmp; *.tif"; ;
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            fileName = ofd.FileName;
            //파일-->비트맵
            bitmap = new Bitmap(fileName); //bitmap 라이브러리 불러옴

            // 중요! 입력이미지의 높이, 폭 알아내기-비트맵에서 불러옴(H,W바꿔야 함)
            inW = bitmap.Height;
            inH = bitmap.Width;
            inImage = new byte[RGB, inH, inW]; // 메모리 할당...면-행-렬
                                               //비트맵-->메모리()
            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    Color c = bitmap.GetPixel(i, k);//컬러 변수 c에 비트맵 함수로 픽셀의 값을 불러온다.
                    inImage[RR, i, k] = c.R;        //컬러 변수 c의 Red값을 불러와, RR번째 면에 넣는다.
                    inImage[GG, i, k] = c.G;
                    inImage[BB, i, k] = c.B;
                }


            equal_image();
        }
        void saveImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "";
            sfd.Filter = "PNG File(*.png) | *.png";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            String saveFname = sfd.FileName;
            Bitmap image = new Bitmap(outH, outW); // 빈 비트맵(종이) 준비
            for (int i = 0; i < outH; i++)
                for (int k = 0; k < outW; k++)
                {
                    Color c;
                    int r, g, b;
                    r = outImage[0, i, k];
                    g = outImage[1, i, k];
                    b = outImage[2, i, k];
                    c = Color.FromArgb(r, g, b);
                    image.SetPixel(i, k, c);  // 종이에 콕콕 찍기
                }
            // 상단에 using System.Drawing.Imaging; 추가해야 함
            image.Save(saveFname, ImageFormat.Png); // 종이를 PNG로 저장
            toolStripStatusLabel1.Text = saveFname + "으로 저장됨.";
        }
        void displayImage()
        {
            // 벽, 게시판, 종이 크기 조절
            paper = new Bitmap(outH, outW); // 종이
            pictureBox1.Size = new Size(outH, outW); // 캔버스
            this.Size = new Size(outH + 20, outW + 80); // 벽

            Color pen; // 펜(콕콕 찍을 용도)
            for (int i = 0; i < outH; i++)
                for (int k = 0; k < outW; k++)
                {
                    byte r = outImage[RR, i, k]; // 잉크(색상값)R
                    byte g = outImage[GG, i, k]; // 잉크(색상값)G
                    byte b = outImage[BB, i, k]; // 잉크(색상값)B
                    pen = Color.FromArgb(r, g, b); // 펜에 잉크 묻히기
                    paper.SetPixel(i, k, pen); // 종이에 콕 찍기
                }
            pictureBox1.Image = paper; // 게시판에 종이를 붙이기.
            toolStripStatusLabel1.Text =
                outH.ToString() + "x" + outW.ToString() + "  " + fileName;


        }
        double getValue()
        {
            subForm01 sub = new subForm01();
            if (sub.ShowDialog() == DialogResult.Cancel)
                return 0;
            double value = (double)(sub.numUp1.Value); //서로 두 폼을 연결할 땐 Public으로 바꿔주기. 
            return value;
        }
        Tuple<byte, byte> Getdistance()
        {
            subForm02 sub = new subForm02(); //서브폼 준비
            if (sub.ShowDialog() == DialogResult.Cancel)
                return new Tuple<byte, byte>(0, 0);
            byte moveH = (byte)(sub.moveH_num.Value);
            byte moveW = (byte)(sub.moveW_num.Value);
            return new Tuple<byte, byte>(moveH, moveW);
        }
        // /////////////
        //영상처리 함수부
        // /////////////
        // 1) 화소점 처리
        void equal_image()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i, k];
                    }
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void brightness()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            sbyte value = (sbyte)getValue();

            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        if ((inImage[rgb, i, k] + value) > 255)
                            outImage[rgb, i, k] = 255;
                        else if ((inImage[rgb, i, k] + value) < 0)
                            outImage[rgb, i, k] = 0;
                        else
                            outImage[rgb, i, k] = (byte)(inImage[rgb, i, k] + value);
                    }
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void gray_scale()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***

            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    byte rgb = (byte)(0.299 * inImage[RR, i, k] + 0.587 * inImage[GG, i, k] + 0.114 * inImage[BB, i, k]);

                    outImage[RR, i, k] = rgb;
                    outImage[GG, i, k] = rgb;
                    outImage[BB, i, k] = rgb;
                }
            /////////////////////////////////////////////
            displayImage();
        }
        void bw()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        if (inImage[rgb, i, k] > 128)
                            outImage[rgb, i, k] = 255;
                        else
                            outImage[rgb, i, k] = 0;
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void bw_avg()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            int sum = 0, avg = 0;
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        sum += inImage[rgb, i, k];
                        avg = sum / (inH * inW);
                        if (inImage[rgb, i, k] > avg)
                            outImage[rgb, i, k] = 255;
                        else
                            outImage[rgb, i, k] = 0;
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void gamma()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            double gammaLCD = 2.5;      //LCD의 감마값이 2.5라고 가정
            double gamma = 1.0 / gammaLCD;
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = (byte)(255.0 * Math.Pow(inImage[rgb, i, k] / 255.0, gamma));
                    }
            /////////////////////////////////////////////
            displayImage();

        }
        void draw_histogram()
        {
            long[] rHisto = new long[256];
            long[] gHisto = new long[256];
            long[] bHisto = new long[256];


            for (int i = 0; i < outH; i++)
                for (int k = 0; k < outW; k++)
                {
                    rHisto[outImage[RR, i, k]]++;
                    gHisto[outImage[GG, i, k]]++;
                    bHisto[outImage[BB, i, k]]++;

                }

            histoForm hform = new histoForm(rHisto, gHisto, bHisto);
            hform.ShowDialog();
        }
        void parabola() //파라볼라
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = (byte)(255 * Math.Pow(((double)inImage[rgb, i, k] / 128.0) - 1.0, 2));
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void solarizing()   //솔러라이징
        {

            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = (byte)(255 - 255 * Math.Pow(((double)inImage[rgb, i, k] / 128.0) - 1.0, 2));
                    }
            /////////////////////////////////////////////
            displayImage();

        }
        void multiple() //선명도 조절
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            double c = getValue();
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        if (c * inImage[rgb, i, k] > 255)
                            outImage[rgb, i, k] = 255;
                        else if (c * inImage[rgb, i, k] <= 0)
                            outImage[rgb, i, k] = 0;
                        else
                            outImage[rgb, i, k] = (byte)(c * inImage[rgb, i, k]);
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void poster()
        {

            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //명암 단계 0 32 64 96 128 160 192 224 255
            //const byte step = 32;
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        if (inImage[rgb, i, k] <= 0)
                            outImage[rgb, i, k] = 0;
                        else if (inImage[rgb, i, k] > 0 && inImage[rgb, i, k] <= 32)
                            outImage[rgb, i, k] = 32;
                        else if (inImage[rgb, i, k] > 32 && inImage[rgb, i, k] <= 64)
                            outImage[rgb, i, k] = 64;
                        //else if (inImage[rgb, i, k] > 64 && inImage[rgb, i, k] <= 96)
                        //    outImage[rgb, i, k] = 96;
                        else if (inImage[rgb, i, k] > 64 && inImage[rgb, i, k] <= 128)
                            outImage[rgb, i, k] = 128;
                        //else if (inImage[rgb, i, k] > 128 && inImage[rgb, i, k] <= 160)
                        //    outImage[rgb, i, k] = 160;
                        else if (inImage[rgb, i, k] > 128 && inImage[rgb, i, k] <= 192)
                            outImage[rgb, i, k] = 192;
                        else if (inImage[rgb, i, k] > 192 && inImage[rgb, i, k] <= 224)
                            outImage[rgb, i, k] = 224;
                        else
                            outImage[rgb, i, k] = 255;
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void change_satur()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //RGB-->HSV. 2. 한 점만 바꾼다(1. 전체를 바꾸거나 2. 한 점만 바꾸거나)
            Color c; //한 점 색상 모델
            double hh, ss, vv; //색상 채도 밝기
            int rr, gg, bb; //레드 그린 블루


            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    rr = inImage[RR, i, k];
                    gg = inImage[GG, i, k];
                    bb = inImage[BB, i, k];
                    //HSV로 변환
                    c = Color.FromArgb(rr, gg, bb);
                    hh = c.GetHue();
                    ss = c.GetSaturation();
                    vv = c.GetBrightness();
                    //(핵심!)채도 올리기
                    ss += 0.15;

                    //HSV->RGB변환
                    HsvToRgb(hh, ss, vv, out rr, out gg, out bb);
                    outImage[RR, i, k] = (byte)rr;
                    outImage[GG, i, k] = (byte)gg;
                    outImage[BB, i, k] = (byte)bb;
                }
            /////////////////////////////////////////////
            displayImage();
        }   //채도 조절
        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {
                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;
                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;
                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;
                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;
                    default:
                        R = G = B = V;
                        break;
                }
            }
            r = CheckRange((int)(R * 255.0));
            g = CheckRange((int)(G * 255.0));
            b = CheckRange((int)(B * 255.0));

            int CheckRange(int i)
            {
                if (i < 0) return 0;
                if (i > 255) return 255;
                return i;
            }
        }

        void stretch()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            int value = 30;
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        if ((255 / (255 - 2 * value) * (inImage[rgb, i, k] - value)) > 255)
                            outImage[rgb, i, k] = 255;
                        else if ((255 / (255 - 2 * value) * (inImage[rgb, i, k] - value)) < 0)
                            outImage[rgb, i, k] = 0;
                        else
                            outImage[rgb, i, k] = (byte)(255 / (255 - 2 * value) * (inImage[rgb, i, k] - value));
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void compress()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            int value = 30;
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        if ((int)((255.0 - 2.0 * value) / 255.0 * (inImage[rgb, i, k]) + value) > 255)
                            outImage[rgb, i, k] = 255;
                        else if ((int)((255.0 - 2.0 * value) / 255.0 * (inImage[rgb, i, k]) + value) < 0)
                            outImage[rgb, i, k] = 0;
                        else
                            outImage[rgb, i, k] = (byte)((255.0 - 2.0 * value) / 255.0 * (inImage[rgb, i, k]) + value);
                    }

            displayImage();
        }
        void reverse()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = (byte)(255-inImage[rgb, i, k]);
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void stress()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        if (inImage[rgb, i, k] >= 0 && inImage[rgb, i, k] < 140)                      //기본 경계값:128&192
                            outImage[rgb, i, k] = inImage[rgb, i, k];
                        else if (inImage[rgb, i, k] >= 180 && inImage[rgb, i, k] < 255)
                            outImage[rgb, i, k] = inImage[rgb, i, k];
                        else
                            outImage[rgb, i, k] = 255;
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void histo_equal() // 히스토그램 스트래칭
        {
            if (inImage == null)
                return;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // 수식 :
            // Out = (In - min) / (max - min) * 255.0
            byte min_val = inImage[0, 0, 0], max_val = inImage[0, 0, 0];
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    if (min_val > inImage[rgb, i, k])
                        min_val = inImage[rgb, i, k];
                    if (max_val < inImage[rgb, i, k])
                        max_val = inImage[rgb, i, k];
                }
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    // Out = (In - min) / (max - min) * 255.0
                    outImage[rgb, i, k] = (byte)
                        ((double)(inImage[rgb, i, k] - min_val) / (max_val - min_val) * 255.0);
                }
            /////////////////////////////////////////////
            displayImage();
        }
        void end_in()//엔드 인
        {
            if (inImage == null)
                return;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];

            // 수식 :
            // Out = (In - min) / (max - min) * 255.0
            byte min_val = inImage[0, 0, 0], max_val = inImage[0, 0, 0];
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    if (min_val > inImage[rgb, i, k])
                        min_val = inImage[rgb, i, k];
                    if (max_val < inImage[rgb, i, k])
                        max_val = inImage[rgb, i, k];
                }
            //min,max를 강제로 변경
            min_val += 30;
            max_val -= 30;
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    if (inImage[rgb, i, k] <= min_val)
                        outImage[rgb, i, k] = 0;
                    else if (inImage[rgb, i, k] >= max_val)
                        outImage[rgb, i, k] = 255;
                    else
                        outImage[rgb, i, k] = (byte)
                            ((double)(inImage[rgb, i, k] - min_val) / (max_val - min_val) * 255.0);
                }
            /////////////////////////////////////////////
            displayImage();
        }
        // ////////
        // 2)기하학 변환
        void zoom_in() //확대
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            double scale = getValue();
            outH = (int)(inH * scale); outW = (int)(inW * scale);
            outImage = new byte[RGB, outH, outW];
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = (inImage[rgb, (byte)(i / scale), (byte)(k / scale)]);
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void zoom_out() //축소
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            double scale = getValue();
            outH = (int)(inH / scale); outW = (int)(inW / scale);
            outImage = new byte[RGB, outH, outW];
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = (inImage[rgb, (byte)(i / scale), (byte)(k / scale)]);
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void lrMirror()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, outH - 1 - i, k];
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void tbMirror()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i, outW - 1 - k];
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void translate()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            var distance = Getdistance();
            byte moveH = distance.Item1;
            byte moveW = distance.Item2;
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        outImage[rgb, i + moveH, k + moveW] = inImage[rgb, i, k];
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void rotate()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 회전하고 싶은 각도 입력
            const double PI = 3.1415926;
            double theta;
            int rotate_h, rotate_w;
            theta = getValue();
            double rad = (double)theta * PI / 180;
            double c = Math.Cos(rad); double s = Math.Sin(rad);
            // 회전했을 때 크기 구하기
            int rotH, rotW;
            rotH = (int)(Math.Abs(inW * s) + Math.Abs(inH * c));
            rotW = (int)(Math.Abs(inH * s) + Math.Abs(inW * c));
            // 이미지 중심 구하기 (영상 중심으로 회전시키기 위해)
            int centerH = rotH / 2, centerW = rotW / 2;
            // 회전했을 때의 크기로 rotateimage 메모리 할당받아 중앙에 inputImage를 넣음
            byte[,,] rotateimage = new byte[RGB, rotH, rotW];
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = centerH - (inH / 2), n = 0; i < centerH + (inH / 2); i++, n++)
                    for (int k = centerW - (inW / 2), m = 0; k < centerW + (inW / 2); k++, m++)
                        rotateimage[rgb, i, k] = inImage[rgb, n, m];

            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            // outImage메모리 할당
            outH = rotH; outW = rotW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                    {
                        rotate_h = (int)((i - centerH) * Math.Cos(rad) - (k - centerW) * Math.Sin(rad) + centerH);
                        rotate_w = (int)((i - centerH) * Math.Sin(rad) + (k - centerW) * Math.Cos(rad) + centerW);
                        if ((rotate_w >= 0 && rotate_w < outW) && (rotate_h >= 0 && rotate_h < outH))
                            outImage[rgb, i, k] = rotateimage[rgb, rotate_h, rotate_w];
                        else
                        {
                            outImage[rgb, i, k] = 0;
                        }
                    }
            /////////////////////////////////////////////
            displayImage();
        }
        // //////////////////
        // 3)영역처리 함수

        double[,,] color_maskProcess(byte[,,] inImage, double[,] mask)  //컬러용 마스크프로세스
        {
            const int MSIZE = 3;
            //임시 입력, 출력 메모리 할당
            double[,,] tmpInput = new double[RGB, inH + 2, inW + 2];
            double[,,] tmpOutput = new double[RGB, outH, outW];
            //임시 입력을 중간값(혹은 평균값)으로 초기화
            int sum = 0, avg = 0;
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        sum += inImage[rgb, i, k];
                        avg = sum / (inH * inW);
                    }
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                    {
                        tmpInput[rgb, i, k] = (double)avg;
                    }
            //입력을 임시 입력으로 복사
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                        tmpInput[rgb, i + 1, k + 1] = inImage[rgb, i, k];

            // *** 진짜 영상처리 알고리즘을 구현 ***
            double S = 0.0; //누적시킬 매개변수
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        for (int m = 0; m < MSIZE; m++)
                            for (int n = 0; n < MSIZE; n++)
                                S += tmpInput[rgb, i + m, k + n] * mask[m, n];
                        tmpOutput[rgb, i, k] = S;
                        S = 0.0; //다음 점을 계산하기 위해 초기화 해준다.
                    }
                }
            }
            //후처리: 마스크의 합이 0이면 127을 더하기
            double sumMask = 0;
            for (int i = 0; i < MSIZE; i++)
            {
                for (int k = 0; k < MSIZE; k++)
                {
                    sumMask += mask[i, k];
                }
            }
            if (sumMask == 0)
            {
                for (int rgb = 0; rgb < RGB; rgb++)
                    for (int i = 0; i < inH; i++)
                    {
                        for (int k = 0; k < inW; k++)
                        {
                            tmpOutput[rgb, i, k] += 127;
                        }
                    }
            }
            return tmpOutput;
        }
        void emboss_image()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //마스크 결정
            // 중요! 마스크 결정
            double[,] mask = {        { -1.0, 0.0, 0.0 },
                                            {  0.0, 0.0, 0.0 },
                                            {  0.0, 0.0, 1.0 }   };
            double[,,] tmpOutput = color_maskProcess(inImage, mask);
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                    {
                        double d = tmpOutput[rgb, i, k];
                        if (d > 255.0)
                            d = 255.0;
                        else if (d < 0.0)
                            d = 0.0;
                        outImage[rgb, i, k] = (byte)d;

                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void blur()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //마스크 결정
            // 중요! 마스크 결정
            double[,] mask = {     { 1/9.0, 1/9.0, 1/9.0 },
                                  {  1/9.0, 1/9.0, 1/9.0 },
                                  {  1/9.0, 1/9.0, 1/9.0 }     };
            double[,,] tmpOutput = color_maskProcess(inImage, mask);
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                    {
                        double d = tmpOutput[rgb, i, k];
                        if (d > 255.0)
                            d = 255.0;
                        else if (d < 0.0)
                            d = 0.0;
                        outImage[rgb, i, k] = (byte)d;

                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void sharpen1()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //마스크 결정
            double[,] mask = {      { -1.0, -1.0, -1.0 },
                                        {  -1.0, 9.0, -1.0 },
                                        {  -1.0, -1.0, -1.0 }     };
            double[,,] tmpOutput = color_maskProcess(inImage, mask);
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                    {
                        double d = tmpOutput[rgb, i, k];
                        if (d > 255.0)
                            d = 255.0;
                        else if (d < 0.0)
                            d = 0.0;
                        outImage[rgb, i, k] = (byte)d;

                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void sharpen2()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //마스크 결정
            double[,] mask = {      { 0.0, -1.0, 0.0 },
                                        {  -1.0, 5.0, -1.0 },
                                        {  0.0, -1.0, 0.0 }     };
            double[,,] tmpOutput = color_maskProcess(inImage, mask);
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                    {
                        double d = tmpOutput[rgb, i, k];
                        if (d > 255.0)
                            d = 255.0;
                        else if (d < 0.0)
                            d = 0.0;
                        outImage[rgb, i, k] = (byte)d;

                    }
            /////////////////////////////////////////////
            displayImage();
        }
        void gaussian_filter()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //마스크 결정
            // 중요! 마스크 결정
            double[,] mask = {     { 1/16.0, 1/8.0, 1/16.0 },
                                  {  1/8.0, 1/4.0, 1/8.0 },
                                  {  1/16.0, 1/8.0, 1/16.0 }     };
            //임시 입력, 출력 메모리 할당
            double[,,] tmpOutput = color_maskProcess(inImage, mask);
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < outH; i++)
                    for (int k = 0; k < outW; k++)
                    {
                        double d = tmpOutput[rgb, i, k];
                        if (d > 255.0)
                            d = 255.0;
                        else if (d < 0.0)
                            d = 0.0;
                        outImage[rgb, i, k] = (byte)d;

                    }
            /////////////////////////////////////////////
            displayImage();
        }
        // /////////////////////
        //4) 에지(edge,엣지) 검출
        double[,,] double_edge_maskProcess(double[,,] inImage, double[,] mask) //double용 프로세스
        {
            //에지용 마스크 프로세스 메커니즘: 그레이 스케일링->마스크 회선->아웃풋 이미지 변환
            const int MSIZE = 3;
            //임시 입력, 출력 메모리 할당
            double[,,] tmpInput = new double[RGB, inH + 2, inW + 2];
            double[,,] tmpOutput = new double[RGB, outH, outW];

            // *** 진짜 영상처리 알고리즘을 구현 ***
            //입력을 임시 입력으로 복사
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                    for (int k = 0; k < inW; k++)
                        tmpInput[rgb, i + 1, k + 1] = inImage[rgb, i, k];

            double S = 0.0; //누적시킬 매개변수
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        for (int m = 0; m < MSIZE; m++)
                            for (int n = 0; n < MSIZE; n++)
                                S += tmpInput[rgb, i + m, k + n] * mask[m, n];
                        tmpOutput[rgb, i, k] = S;
                        S = 0.0; //다음 점을 계산하기 위해 초기화 해준다.
                    }
                }
            }
            return tmpOutput;
        }
        byte[,,] edge3D_maskProcess(byte[,,] inImage, double[,] mask)  //에지용 마스크프로세스
        {   //에지용 마스크 프로세스 메커니즘: 그레이 스케일링->마스크 회선->아웃풋 이미지 변환
            const int MSIZE = 3;
            //임시 입력, 출력 메모리 할당
            double[,,] tmpInput = new double[RGB, inH + 2, inW + 2];
            double[,,] tmpOutput = new double[RGB, outH, outW];
            //그레이 스케일로 바꿔주기
            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    byte rgb = (byte)(0.299 * inImage[RR, i, k] + 0.587 * inImage[GG, i, k] + 0.114 * inImage[BB, i, k]);

                    tmpInput[RR, i, k] = rgb;
                    tmpInput[GG, i, k] = rgb;
                    tmpInput[BB, i, k] = rgb;
                }
            // *** 진짜 영상처리 알고리즘을 구현 ***
            double S = 0.0; //누적시킬 매개변수
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        for (int m = 0; m < MSIZE; m++)
                            for (int n = 0; n < MSIZE; n++)
                                S += tmpInput[rgb, i + m, k + n] * mask[m, n];
                        tmpOutput[rgb, i, k] = S;
                        S = 0.0; //다음 점을 계산하기 위해 초기화 해준다.
                    }
                }
            }
            
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        double d = tmpOutput[rgb, i, k];

                        if (d > 255.0)
                            d = 255.0;
                        else if (d < 0.0)
                            d = 0.0;
                        else
                            outImage[rgb, i, k] = (byte)tmpOutput[rgb, i, k];
                    }
                }
            return outImage;
        }
        void vertical_edge()    //수직 엣지 검출
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //중요! 마스크 결정
            double[,] mask = {    {  0.0, 0.0, 0.0 },
                                  { -1.0, 1.0, 0.0 },
                                  {  0.0, 0.0, 0.0 }     };
            outImage = edge3D_maskProcess(inImage, mask);
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void horizontal_edge()    //수평 엣지 검출
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //중요! 마스크 결정
            double[,] mask = {    {  0.0,-1.0, 0.0 },
                                  {  0.0, 1.0, 0.0 },
                                  {  0.0, 0.0, 0.0 }     };
            outImage = edge3D_maskProcess(inImage, mask);
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void homogen_edge()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            double max;
            const int MSIZE = 3;
            //임시 입력, 출력 메모리 할당
            double[,,] tmpInput = new double[RGB, inH + 2, inW + 2];
            double[,,] tmpOutput = new double[RGB, outH, outW];
            //그레이 스케일로 바꿔주기->임시 입력으로 보냄
            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    byte rgb = (byte)(0.299 * inImage[RR, i, k] + 0.587 * inImage[GG, i, k] + 0.114 * inImage[BB, i, k]);

                    tmpInput[RR, i, k] = rgb;
                    tmpInput[GG, i, k] = rgb;
                    tmpInput[BB, i, k] = rgb;
                }
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //유사 연산자 에지 알고리즘
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    max = 0.0; //블록이 이동할 때마다 최대값 초기화
                    for (int m = 0; m < MSIZE; m++)
                        for (int n = 0; n < MSIZE; n++)
                        {
                            if ((double)Math.Abs((tmpInput[rgb, i + 1, k + 1] - tmpInput[rgb, i + m, k + n])) >= max)
                                max = (double)(Math.Abs(tmpInput[rgb, i + 1, k + 1] - tmpInput[rgb, i + m, k + n]));
                        }
                    tmpOutput[rgb, i, k] = max;
                }
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    if (tmpOutput[rgb, i, k] > 255.0)
                        tmpOutput[rgb, i, k] = 255.0;
                    else if (tmpOutput[rgb, i, k] < 0.0)
                        tmpOutput[rgb, i, k] = 0.0;

                }
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    outImage[rgb, i, k] = (byte)tmpOutput[rgb, i, k];
                }
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void roberts_row_edge()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //중요! 마스크 결정
            double[,] mask = {    { -1.0, 0.0, 0.0 },
                                  {  0.0, 1.0, 0.0 },
                                  {  0.0, 0.0, 0.0 }     };
            outImage = edge3D_maskProcess(inImage, mask);
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void roberts_col_edge()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //중요! 마스크 결정
            double[,] mask = {    {  0.0, 0.0, -1.0 },
                                      {  0.0, 1.0,  0.0 },
                                      {  0.0, 0.0,  0.0 }     };
            outImage = edge3D_maskProcess(inImage, mask);
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void roberts_edge()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            //그레이 스케일로 바꿔주기
            double[,,] grayInput = new double[RGB, inH , inW ];
            for (int i = 0; i < inH; i++)
                for (int k = 0; k < inW; k++)
                {
                    byte rgb = (byte)(0.299 * inImage[RR, i, k] + 0.587 * inImage[GG, i, k] + 0.114 * inImage[BB, i, k]);

                    grayInput[RR, i, k] = rgb;
                    grayInput[GG, i, k] = rgb;
                    grayInput[BB, i, k] = rgb;
                }
            // *** 진짜 영상처리 알고리즘을 구현 ***
            double[,,] tmpOutput1 = new double[RGB, outH, outW];
            double[,,] tmpOutput2 = new double[RGB, outH, outW];
            double[,] mask_row = {    { -1.0, 0.0, 0.0 },
                                  {  0.0, 1.0, 0.0 },
                                  {  0.0, 0.0, 0.0 }     };
            tmpOutput1 = double_edge_maskProcess(grayInput, mask_row);
            double[,] mask_col = {    {  0.0, 0.0, -1.0 },
                                      {  0.0, 1.0,  0.0 },
                                      {  0.0, 0.0,  0.0 }     };
            tmpOutput2 = double_edge_maskProcess(tmpOutput1, mask_col);
            for (int rgb = 0; rgb < RGB; rgb++)
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        double d = tmpOutput2[rgb, i, k];

                        if (d > 255.0)
                            d = 255.0;
                        else if (d < 0.0)
                            d = 0.0;
                        else
                            outImage[rgb, i, k] = (byte)tmpOutput2[rgb, i, k];
                    }
                }
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }
        void laplacian_edge()
        {
            if (inImage == null)
                return;
            if (outImage != null) //이전 실행값이 있으면 이전값에 덮어 씌운다.
                inImage = outImage;
            // 중요! 출력이미지의 높이, 폭을 결정  --> 알고리즘에 영향
            outH = inH; outW = inW;
            outImage = new byte[RGB, outH, outW];
            // *** 진짜 영상처리 알고리즘을 구현 ***
            //중요! 마스크 결정
            double[,] mask = {    {  0.0,  1.0, 0.0 },
                                  {  1.0, -4.0, 1.0 },
                                  {  0.0,  1.0, 0.0 }     };
            outImage = edge3D_maskProcess(inImage, mask);
            /////////////////////////////////////////////
            displayImage();
            saveTempFile();
        }


        //메뉴 이벤트 처리부
        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outImage = null;    //새로운 파일을 열 때 그 전의 결과이미지는 지운다.
            openImage();

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreTempFile();

        }

        private void redoMenuItem_Click(object sender, EventArgs e)
        {

        }

        Bitmap paper, bitmap;

        private void 상하반전ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbMirror();
        }

        private void 이동ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            translate();
        }

        private void 파라볼라ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parabola();
        }

        private void 회전ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotate();
        }

        private void 저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveImage();
        }

        private void 엠보싱ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emboss_image();
        }

        private void 블러ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            blur();
        }

        private void 샤프닝1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sharpen1();
        }

        private void 샤프닝2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sharpen2();
        }

        private void 수직ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vertical_edge();
        }

        private void 채도변경ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            change_satur();
        }

        private void 파라볼라ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            parabola();
        }

        private void 솔러라이징ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            solarizing();
        }

        private void 엠보싱ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            emboss_image();
        }

        private void 반전ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reverse();
        }

        private void 흑백이미지ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bw();
        }

        private void 선명도조절ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multiple();
        }

        private void 포스터ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            poster();
        }

        private void 감마변환ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gamma();
        }

        private void 스트레치ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stretch();
        }

        private void 압축ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compress();
        }

        private void 강조ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stress();
        }

        private void 엔드인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            end_in();
        }

        private void 수평ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            horizontal_edge();
        }

        private void 유사연산자에지검출ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            homogen_edge();
        }

        private void 로버츠ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            roberts_edge();
        }

        private void 수평ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            roberts_col_edge();
        }

        private void 수평수직ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            roberts_edge();
        }

        private void 수직ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            roberts_row_edge();
        }

        private void 라플라시안ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            laplacian_edge();
        }

        private void 좌우반전ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lrMirror();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ㅍ일ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                    restoreTempFile(); break;

                }
            }

            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    // case Keys.Y:
                    //this.Redo(); break;
                }
            }
        }
        private void 밝기조절ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brightness();
        }
        private void 그레이스케일ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gray_scale();
        }
        private void 히스토그램그리기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            draw_histogram();
        }
    }
}

