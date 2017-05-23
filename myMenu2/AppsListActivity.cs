using Android.App;
using Android.OS;
using System;
using Android.Widget;
using System.Threading.Tasks;
using Android.Content;
using System.Collections.Generic;
using Android.Views;

namespace myMenu2
{
	[Activity(Label = "AppList", MainLauncher = true, Icon = "@mipmap/icon")]
	public class AppsListActivity : Activity

	{
		TextView text1;
		Task _timer;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView(Resource.Layout.activity_apps_list);

			text1 = FindViewById<TextView>(Resource.Id.textView1); // タスクでタイマー

			_timer = new Task(async () =>
			{
				while (true)
				{
					RunOnUiThread(() =>
					{
						text1.Text = DateTime.Now.ToString("HH:mm:ss");
					});
					await Task.Delay(1000);
				}
			});
			_timer.Start();


			// ------------ ローカルデータの保存＆他アクティビティ
			/*var intent = this.Intent;
			Console.WriteLine("これが来た:" + intent.GetStringExtra("tbl_text"));
			*/

			var pref = GetSharedPreferences("PREF", FileCreationMode.Private);
			var str = pref.GetString("yaju_name", null);
			var it = pref.GetInt("yaju_age", 0);
			Console.WriteLine($"ローカルデータ読んだよ：「{str} - {it}」");

			//var apl = MainActivity.appsList[0].name;


			/*var mainIntent = new Intent(Intent.ActionMain, null);
			mainIntent.AddCategory(Intent.CategoryLauncher);
			var ar = PackageManager.QueryIntentActivities(mainIntent, 0);

			var appList = new List<TableItem>();

			foreach (var a in ar)
			{
				//アイコン
				var _icon = a.ActivityInfo.LoadIcon(PackageManager);
				//アプリ名
				var _name = a.LoadLabel(PackageManager);
				//アクティビティ名                
				var _fullName = a.ActivityInfo.Name;
				appList.Add(new TableItem()
				{
					icon = _icon,
					name = _name,
					label = _fullName

				});
			}

			//Console.WriteLine($"アプリ数だよ：{ar.Count}");

			var listView = FindViewById<ListView>(Resource.Id.listView1);
			var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItemSingleChoice);
			//var bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Icon);

			for (var i = 0; i < MainActivity.appsList.Count; i++)
			{
				adapter.Add($"{i + 1}: " + MainActivity.appsList[i].name);
			}

			var temp = MainActivity.appsList[0];
			MainActivity.appsList[0] = MainActivity.appsList[5];
			MainActivity.appsList[5] = temp;

			//var flash = new MainActivity();
			//flash.SetMenuView();

			listView.Adapter = adapter;

			//行の移動
			listView.SetSelection(0);   //10番目の行(item_10)を一番上に表示する*/

			List<TableItem> a = new List<TableItem>();

			for (var i = 0; i < 16; i++)
			{
				a.Insert(i, new TableItem()
				{
					name = MainActivity.appsList[i].name,
					//name = $"アプリ名：{MainActivity.appsList[i].name}",
					label = $"ラベル {MainActivity.appsList[i].label}",
					icon = MainActivity.appsList[i].icon,
					idx = MainActivity.appsList[i].idx
				});
			}

			var listView = FindViewById<ListView>(Resource.Id.listView1);
			var customAdapter = new CustomListAdapter(this, a);
			listView.Adapter = customAdapter;
			listView.SetSelection(0);   //10番目の行(item_10)を一番上に表示する*/

		}

		public override void OnBackPressed()
		{
			SetResult(Result.Ok, new Intent("戻ります"));
			Finish();                   // バックキーが押されたら、このアクティビティを破棄
		}
	}

	public class CustomListAdapter : BaseAdapter<TableItem>
	{
		List<TableItem> items;
		Activity context;

		public CustomListAdapter(Activity context, List<TableItem> items)
		{
			this.context = context;
			this.items = items;
		}

		public override long GetItemId(int position)
		{
			return position;
		}
		public override TableItem this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];
			string[] btnPos = new string[] { "右上", "左上", "右下", "左下", "１", "２", "３", "４", "５", "６", "７", "８", "９", "＊", "０", "＃" };

			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.ListItemRow, null);

			// BaseAdapter<T>の対応するプロパティを割り当て
			view.FindViewById<TextView>(Resource.Id.appNumber).Text = btnPos[item.idx];
			view.FindViewById<TextView>(Resource.Id.NameText).Text = item.name;
			view.FindViewById<TextView>(Resource.Id.labelText).Text = item.label;
			//view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);

			view.FindViewById<ImageView>(Resource.Id.Image).SetImageDrawable(item.icon);

			return view;
		}
	}
}

