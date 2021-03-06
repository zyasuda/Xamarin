﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Views;
using System;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Graphics;
using Android.Webkit;
using Android.Graphics.Drawables;

/*--- EveryPhoneのランチャーメニューアプリ(inversenet)
//--- develop start:2017/3/18~ author:keigo yasuda(sapporo kiyota) */

namespace myMenu2
{
    [Activity
     (
         Label = "MyMenu",                                      //--- 画面まわりの設定
         MainLauncher = true,
         Icon = "@drawable/icon",
         ScreenOrientation = ScreenOrientation.Portrait,
         Theme = "@android:style/Theme.Holo.Light.NoActionBar.TranslucentDecor"
        )]  // .Fullscreen
            //[IntentFilter(new[] { Intent.ActionAssist }, Categories = new[] { Intent.CategoryDefault })]

    public class MainActivity : Activity
    {
        #region vars

        int myViewWidth;
        int myViewHeight;

        ImageButton centerButton;
        List<ImageButton> BtnId;                                //--- 配置ボタンのリスト
        public static List<TableItem> appsList;                 //--- インストアプリのリスト（他のアクティビティでも使う予定）
		List<TableItem> yamada;                                 //--- ヤマダ用のアプリリスト

		TextView dateTextView, yearTextView;                    //---, weatherTextView1, weatherTextView2;
		string marqueeText;

        GridLayout centerFlm;

        private const int BtnCnt = 16;
        //private const int RequestCode = 123;

        #endregion

        //--- プログラムスタート
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);       //--- タイトルを非表示にする
																//--- http://www.buildinsider.net/mobile/xamarintips/0002
			SetContentView(Resource.Layout.Main);

            InitProc();                                         //--- 初期化処理へ
            GetMenuItem();                                      //--- 起動可能なアプリをリストへセット
			SetMenuView();                                      //--- ランチャーボタンにいろいろセット

			centerButton.Click += (sender, args) =>             //--- センターボタンタップ　デリゲート
			{
                var uri = Android.Net.Uri.Parse("http://google.com");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
                Toast.MakeText(this, "センターボタンをタップしました。", ToastLength.Short).Show();
            };

            centerButton.LongClick += (sender, args) =>         //--- センターボタン長押し　デリゲート
			{
                /*StartActivityForResult(typeof(AppsListActivity), RequestCode);
				Toast.MakeText(this, "アプリ登録リストへ移動します。", ToastLength.Short).Show();
				longClickProc(sender, args);*/

                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps"));      //--- 実験！
				StartActivity(intent);
            };

            for (int i = 0; i < BtnCnt; i++)                    //--- アプリボタン１６個のデリゲートをセット
			{
                BtnId[i].Click += (sender, args) => ClickProc(sender);
                BtnId[i].LongClick += LongClickProc;
            }
        }

		//--- 初期化いろいろ
		void InitProc(){
			myViewWidth = Resources.DisplayMetrics.WidthPixels;
			myViewHeight = Resources.DisplayMetrics.HeightPixels;

			Toast.MakeText(this, "画面解像度：" + myViewWidth + "x" + myViewHeight, ToastLength.Long).Show();

			centerButton = FindViewById<ImageButton>(Resource.Id.centerBtn);

			dateTextView = FindViewById<TextView>(Resource.Id.dateText);
			yearTextView = FindViewById<TextView>(Resource.Id.yearText);

			var webView = (WebView)FindViewById(Resource.Id.webView1);

			centerFlm = FindViewById<GridLayout>(Resource.Id.centerLayout);
			centerFlm.Visibility = ViewStates.Visible;          //--- センターフレームはとりあえず非表示

			BtnId = new List<ImageButton>();
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.appBtn1));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.appBtn2));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.appBtn3));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.appBtn4));

			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn1));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn2));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn3));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn4));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn5));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn6));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn7));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn8));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn9));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtnAsta));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtn0));
			BtnId.Add(FindViewById<ImageButton>(Resource.Id.numBtnSharp));

			/*/ --------------プリファレンスにローカルデータを保存
            var pref = GetSharedPreferences("PREF", FileCreationMode.Private).Edit();
            pref.PutString("yaju_name", "yaju_senpai");
            pref.PutInt("yaju_age", 24);
            pref.Commit();*/

			dateTextView.Text = DateTime.Now.ToString("M");
			yearTextView.Text = DateTime.Now.ToString("dddd yyyy年");

			marqueeText = "[NEWS] ここはテロップが流れます。★☆♫";
			var speed = "4";
			var summary =
				$"<html><FONT size='4' color='yellow' FACE='Noto Sans CJK'><marquee behavior='scroll' direction='left' scrollamount={speed}>{marqueeText}</marquee></FONT></html>";
			webView.SetBackgroundColor(Color.Transparent);
			webView.LoadDataWithBaseURL(null, summary, "text/html", "UTF8", null);

			yamada = new List<TableItem>
			{
				new TableItem
				{   name = "ヤマダモール",
					label = "http://ymall.jp" ,
					icon = GetDrawable(Resource.Drawable.yamada_icon)
				},
				new TableItem
				{   name = "ヤマダゲーム",
					label = "http://gpf.mymd.jp/",
					icon = GetDrawable(Resource.Drawable.GAME)
				},
				new TableItem
				{   name = "ヤマダの保険",
                    //label = "https://sp.mymd.jp/smrt/ymdapp/?p=aHR0cHM6Ly9zcC5teW1kLmp wL3NtcnQvaW5kZXgucGhwP21vZHVsZT1pbnN1cmFuY2UmYWN0aW",
                    label = "https://goo.gl/5zUcKM",
					icon = GetDrawable(Resource.Drawable.hoken)
				},
				new TableItem
				{   name = "ヤマダのでんき",
					label = "http://www.yamadanodenki.com/",
					icon = GetDrawable(Resource.Drawable.electoronics)
				},
				new TableItem
				{   name = "ヤマダの冠婚葬祭",
					label = "https://sp.mymd.jp/pc/index.php?a=familysupport.index",
					icon = GetDrawable(Resource.Drawable.Kankon)
				},
				new TableItem
				{   name = "ヤマダの家電電子保証書",
					label = "http://sp.mymd.jp/smrt/info/?p=aHR0cHM6Ly9zcC5teW1kLmpwL3NtcnQvaW5kZXgucGhwP21vZHVsZT1zaG9waGlzdG9yeSZhY3Rpb249c2hvcDAwMSZvdj0w",
					icon = GetDrawable(Resource.Drawable.Warranty)
				},
				new TableItem
				{   name = "ヤマダトータルリフォーム",
					label = "http://www.yamada-denki.jp/service/reform/",
					icon = GetDrawable(Resource.Drawable.reform)
				},
				new TableItem
				{   name = "ヤマダの住宅",
					label = "http://www.sxl.co.jp/",
					icon = GetDrawable(Resource.Drawable.SxL_s1)
				},
				new TableItem
				{   name = "ヤマダウッドハウス",
					label = "http://yamadawoodhouse.jp/",
					icon = GetDrawable(Resource.Drawable.WOODHOUSE)
				},
				new TableItem
				{   name = "ヤマダの旅行・宿泊",
					label = "https://yamada-familysupport.fc-club.com/sp/searchCategory/1",
					icon = GetDrawable(Resource.Drawable.Travel)
				},
				new TableItem
				{   name = "やまだ書店",
					label = "http://yamadashoten.com/",
					icon = GetDrawable(Resource.Drawable.book)
				},
				new TableItem
				{   name = "ヤマダ動画",
					label = "http://mov.mymd.jp/",
					icon = GetDrawable(Resource.Drawable.movie)
				}
			};
        }

		//--- タップ処理
		void ClickProc(object sender)
        {
            var pm = PackageManager;
            for (int i = 0; i < BtnCnt; i++)
            {
                if (BtnId[i] == sender)
                {
                    //var intent = pm.GetLaunchIntentForPackage(appsList[i].label);
                    //StartActivity(intent);

                    OpenOtherApp(appsList[i].label);    //--- アプリラベルはアプリIDのStringを送る

					Toast.MakeText(this, appsList[i].label + "を起動します。", ToastLength.Short).Show();
                }
            }
        }

		//--- 長押し処理
		void LongClickProc(object sender, EventArgs e)
        {
            PopupMenu menu = new PopupMenu(this, centerButton);
            menu.MenuInflater.Inflate(Resource.Layout.popup_menu, menu.Menu);

            menu.MenuItemClick += (s, arg) =>
            {
                Toast.MakeText(this, string.Format("{0}が選択されました。", arg.Item.TitleFormatted), ToastLength.Short).Show();
            };

            menu.DismissEvent += (s, arg) =>
            {
                Toast.MakeText(this, "メニューを閉じました", ToastLength.Short).Show();

            };

            //StartActivityForResult(typeof(AppsListActivity), RequestCode);

			menu.Show();            //--- メニューが開きます

			Toast.MakeText(this, "アプリ登録リストへ移動します。", ToastLength.Short).Show();
        }

		//--- 起動可能なアプリをリストへセット
		void GetMenuItem()
        {
			//--- Intent.ActionMain:起動可能なインテント
			var mainIntent = new Intent(Intent.ActionMain, null);

			//--- Intent.CategoryLauncher デスクトップから可能なインテント(通常アプリ)
			mainIntent.AddCategory(Intent.CategoryLauncher);

            var instAppTbl = PackageManager.QueryIntentActivities(mainIntent, 0);


            appsList = new List<TableItem>();

            foreach (ResolveInfo anApp in instAppTbl)
            {
                var tempApli = new TableItem                               //--- 変数だけの外部クラス
				{
                    name = anApp.LoadLabel(PackageManager),                //--- アプリ名
					label = anApp.ActivityInfo.PackageName,                //--- パッケージ名
					icon = anApp.ActivityInfo.LoadIcon(PackageManager),    //--- アイコン
																		   //--- idx = 0 //--- 配置場所
				};
                appsList.Add(tempApli);

                Console.WriteLine("アクティビティ名：" + tempApli.name + " -> " + tempApli.label);      //--- 中身チェック！
			}

            for (int j = 0; j < 12; j++)
            {                                //--- ヤマダアイテムをテーブルに入れる
				appsList.Add(yamada[j]);

            }

            appsList[5] = yamada[5];        //--- 確認用
        }

		//--- ランチャーボタンにいろいろセット
		void SetMenuView()
        {
            for (int i = 0; i < BtnCnt; i++)
            {
                BtnId[i].SetImageDrawable(appsList[i].icon);
                //BtnId[i].SetImageResource(Resource.Drawable.hoken);
                BtnId[i].SetAlpha(200);
                appsList[i].idx = i;
            }
			//BtnId[5].SetImageResource(Resource.Drawable.Kankon);
			//BtnId[1].SetImageDrawable(appsList[35].icon);               //--- アイコンの上書きチェック
			//BtnId[5].SetImageDrawable(appsList[36].icon);
		}

		//--- アクティビティ戻り処理
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            SetMenuView();
            Toast.MakeText(this, string.Format("resultCode:{0} data:{1}", requestCode, data), ToastLength.Short).Show();
        }

		//--- 外部のアプリを開く. もしそのアプリが端末に入っていなかったら GooglePlay の対象のインストールページヘ飛ばす
		void OpenOtherApp(string package_name)
        {
            if (IsInstalledApp(package_name))
            {
                //var url = "http://" + package_name;
                // 目的のアプリを起動する
                //var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                var pm = PackageManager;
                var intent = pm.GetLaunchIntentForPackage(package_name);
                StartActivity(intent);
            }
			//--- まだ対象のアプリが端末にインストールされていなかったら
			else
            {
				//--- GooglePlay のインストールページに飛ばすところだが、ヤマダのurlに飛ばす（ヤマダアプリのURLには未対応）
				var intent = new Intent(Intent.ActionView
				//, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + package_name)
				, Android.Net.Uri.Parse(package_name)
				);
                StartActivity(intent);
            }
        }

		//--- アプリがインストールされているかチェック
		bool IsInstalledApp(string package_name)
        {
            var pm = PackageManager;
            try
            {
                pm.GetPackageInfo(package_name, PackageInfoFlags.Activities);
                return true;    //--- インストールされている
			}
            catch (PackageManager.NameNotFoundException)
            { }
            return false;       //--- インストールされていない
		}
    }
}