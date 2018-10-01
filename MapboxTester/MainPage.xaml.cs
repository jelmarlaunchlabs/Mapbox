using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapBox;
using MapBox.Factory;
using MapBox.Models;
using MapBox.Offline;
using Xamarin.Forms;

namespace MapboxTester
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

			map.initialCameraUpdate = CameraPerspectiveFactory.fromCenterAndZoomLevel(new Position(10.309852, 123.893151), 10);

			map.pins.Add(new Pin {
				image = "Resources.car.png",
				IsCenterAndFlat = true,
				heading = 0,
				position = new Position(10.318342, 123.90506)
			});
			map.pins.Add(new Pin {
				image = "Resources.car.png",
				IsCenterAndFlat = true,
				heading = 0,
				position = new Position(10.31581, 123.97906)
			});
			map.pins.Add(new Pin {
				image = "Resources.car.png",
				IsCenterAndFlat = true,
				heading = 0,
				position = new Position(10.304416, 123.91161)
			});
			map.pins.Add(new Pin {
				image = "Resources.car.png",
				IsCenterAndFlat = true,
				heading = 0,
				position = new Position(10.311782, 123.91805)
			});

			//Device.StartTimer(TimeSpan.FromSeconds(10), () => {
			//	map.pins[0].isVisible = false;
			//	map.pins[1].isVisible = true;
			//	return false;
			//});

			//Device.StartTimer(TimeSpan.FromSeconds(8), () => {
			//	map.moveMapToRegion(CameraPerspectiveFactory.fromCoordinatesAndPadding(
			//		new List<Position>{
			//			map.pins[0].position,
			//			map.pins[1].position,
			//			map.pins[2].position,
			//			map.pins[3].position
			//		},
			//		new Thickness(50+25, 50+25, 50+25, 50+25)));
			//	return false;
			//});
			////map.initialCameraUpdate = CameraPerspectiveFactory.fromCenterAndZoomLevel(new Position(10.317119, 123.764238), 10);
			////map.moveMapToRegion(CameraPerspectiveFactory.fromCenterAndZoomLevel(new Position(10.317119, 123.764238), 10));
			//// Initialize pins
			//map.pins = new ObservableCollection<Pin>{
			//	//new Pin{
			//	//	image = "Resources.car.png",
			//	//	IsCenterAndFlat = false,
			//	//	heading = 0,
			//	//	position = new Position(0,0),
			//	//	iconOffset = new Point(0,-50)
			//	//},
			//	new Pin{
			//		image = "Resources.car.png",
			//		IsCenterAndFlat = true,
			//		heading = 180,
			//		position = new Position(-5,-5),
			//		width = 90,
			//		height = 90
			//	},
			//	new Pin{
			//		image = "Resources.car.png",
			//		IsCenterAndFlat = true,
			//		heading = 270,
			//		position = new Position(5,5),
			//		width = 90,
			//		height = 90
			//	},
			//	//new Pin{
			//	//	image = "Resources.car.png",
			//	//	IsCenterAndFlat = true,
			//	//	heading = 45,
			//	//	position = new Position(3,3)
			//	//}
			//};

			//map.routes = new ObservableCollection<Route> {
			//	new Route {
			//		points = {
			//			new Position(0,0),
			//			new Position(1,1),
			//			new Position(2,2)
			//		},
			//		lineWidth=4,
			//		borderLineWidth=1,
			//		lineColor= "#ff0000",
			//		borderLineColor = "#00FF00",
			//	},
			//	new Route {
			//		points = {
			//			new Position(0,0),
			//			new Position(1,-1),
			//			new Position(2,-2)
			//		},
			//		lineWidth=4,
			//		borderLineWidth=8,
			//		lineColor= "#ff00f0",
			//		borderLineColor = "#0f0F00",
			//	},
			//	new Route {
			//		points = {
			//			new Position(0,0),
			//			new Position(-1,1),
			//			new Position(-2,2)
			//		},
			//		lineWidth=4,
			//		borderLineWidth=8,
			//		lineColor= "#ff00f0",
			//		borderLineColor = "#0f0F00",
			//	},
			//	new Route {
			//		points = {
			//			new Position(0,0),
			//			new Position(-1,-1),
			//			new Position(-2,-2),
			//		}
			//	},
			//};

			//bool flag = true;

			////// Sychronized update
			////Device.StartTimer(TimeSpan.FromSeconds(3), () => {
			////	var p1 = map.pins[0];
			////	var p2 = map.pins[1];
			////	p1.position = new Position(flag ? -15 : -5, flag ? -15 : -5);
			////	p2.position = new Position(flag ? 15 : 5, flag ? 15 : 5);
			////	flag = !flag;
			////	return true;
			////});

			////// One update only
			////Device.StartTimer(TimeSpan.FromSeconds(3), () => {
			////	var p1 = map.pins[0];
			////	p1.position = new Position(flag ? -15 : -5, flag ? -15 : -5);
			////	flag = !flag;
			////	return true;
			////});

			//// Asychronous update
			//Device.StartTimer(TimeSpan.FromSeconds(8), () => {
			//	Device.BeginInvokeOnMainThread(async () => {
			//		var p1 = map.pins[0];
			//		var p2 = map.pins[1];
			//		p1.position = new Position(flag ? -15 : -5, flag ? -15 : -5);
			//		await Task.Delay(4000);
			//		p2.position = new Position(flag ? 15 : 5, flag ? 15 : 5);
			//		flag = !flag;
			//	});
			//	return true;
			//});

			////// Add pin
			////Device.StartTimer(TimeSpan.FromSeconds(10), () => {
			////	map.pins.Add(
			////		new Pin {
			////			image = "Resources.carBlack.jpg",
			////			IsCenterAndFlat = true,
			////			heading = 90,
			////			position = new Position(4, 4)
			////		});

			////	map.routes.Add(
			////		new Route {
			////			points = {
			////				new Position(-3,-3),
			////				new Position(-4,-4),
			////				new Position(-5,-5),
			////			},
			////			lineWidth = 5,
			////			borderLineWidth = 10,
			////			lineColor = "#0000FF",
			////			borderLineColor = "#0FF00F"
			////		});

			////	// Remove pin
			////	Device.StartTimer(TimeSpan.FromSeconds(10), () => {
			////		map.moveMapToRegion(CameraPerspectiveFactory.fromCenterAndZoomLevel(new Position(11.273095, 124.057075), 10));
			////		map.pins.Remove(map.pins[2]);
			////		map.routes.Remove(map.routes[2]);

			////		// Clear pins
			////		Device.StartTimer(TimeSpan.FromSeconds(10), () => {
			////			map.pins.Clear();
			////			map.routes.Clear();

			////			Device.StartTimer(TimeSpan.FromSeconds(5), () => {
			////				map.pins = new ObservableCollection<Pin>{
			////					new Pin{
			////						image = "Resources.car.png",
			////						IsCenterAndFlat = true,
			////						heading = 180,
			////						position = new Position(1,1),
			////						width = 90,
			////						height = 90
			////					}
			////				};
			////				map.routes = new ObservableCollection<Route> {
			////					new Route {
			////						points = {
			////							new Position(0,0),
			////							new Position(1,1),
			////							new Position(2,2)
			////						},
			////						lineWidth=4,
			////						borderLineWidth=8,
			////						lineColor= "#ff0000",
			////						borderLineColor = "#00FF00",
			////					}
			////				};

			////				return false;
			////			});

			//			//// Change location of certain pin
			//			//Device.StartTimer(TimeSpan.FromSeconds(10), () => {
			//			//	map.pins[1].position = new Position(-5, -5);
			//			//	map.pins[1].heading = 225;
			//			//	return false;
			//			//});
			//			return false;
			//		});
			//		return false;
			//	});
			//	return false;
			//});

			map.CameraMoveStarted += Map_CameraMoveStarted;
			map.CameraMoving += Map_CameraMoving;
			map.CameraIdled += Map_CameraIdled;
			map.MapClicked += Map_MapClicked;

			map.DefaultPins = new ObservableCollection<DefaultPin>();

			Device.StartTimer(TimeSpan.FromSeconds(3), () => {
				offlineService = DependencyService.Get<IOfflineStorageService>();
				offlineService.OfflinePackProgressChanged += OfflineService_OfflinePackProgressChanged;
				return false;
			});

		}

		void OfflineService_OfflinePackProgressChanged(object sender, OSSEventArgs e)
		{
			var progress = e.OfflinePack.Progress;
			float percentage = 0;
			if (progress.CountOfResourcesExpected > 0) {
				percentage = (float)progress.CountOfResourcesCompleted / progress.CountOfResourcesExpected;
			}
			Debug.WriteLine($"Downloaded resources: {progress.CountOfResourcesCompleted} ({percentage * 100} %)");
			Debug.WriteLine($"Downloaded tiles: {progress.CountOfTilesCompleted}");
			if (progress.CountOfResourcesExpected == progress.CountOfResourcesCompleted) {
				Debug.WriteLine("Download completed");
			}
		}


        IOfflineStorageService offlineService;
		async void DownloadMap_Clicked(object sender, System.EventArgs e)
		{
			if (offlineService == null)
				return;
			var region = new OfflinePackRegion() {
				StyleURL = "mapbox://styles/mapbox/streets-v9",
				MaximumZoomLevel = 14,
				MinimumZoomLevel = 1,
				Bounds = new Bounds(new Position(10.309852 + 0.01, 123.893151 - 0.005), new Position(10.309852 - 0.01, 123.893151 + 0.005))
			};
			var pack = await offlineService.DownloadMap(region, new System.Collections.Generic.Dictionary<string, string>() {
				{"name", "test"},
				{"started_at", DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")}
			});
			if (pack != null) {
				offlineService.RequestPackProgress(pack);
			} else {

			}
		}

		async void loadMap(object sender, System.EventArgs e)
		{
			var packs = await offlineService.GetPacks();
			if (packs != null && packs.Length != 0) {
				var buttons = new List<string>();
				foreach (OfflinePack pack in packs) {
					if (pack.Info != null
						&& pack.Info.TryGetValue("name", out string name)
						&& pack.Info.TryGetValue("started_at", out string startTime)) {
						buttons.Add(name + " - " + startTime);
					}
				}
				//var chosen = await UserDialogs.Instance.ActionSheetAsync("Load offline pack", "Cancel", null, null, buttons.ToArray());
				//var chosenIndex = buttons.IndexOf(chosen);
				//if (chosenIndex >= 0 && chosenIndex < packs.Length) {
				//	var chosenPack = packs[chosenIndex];
				//	//forcedRegion = chosenPack.Region;
				//	//CurrentMapStyle = new MapStyle(chosenPack.Region.StyleURL);
				//	ApplyOfflinePackFunc?.Invoke(chosenPack);
				//}
			} else {
				Debug.WriteLine("There's no offline pack to load");
			}
		}

		void clearMap(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}

		void Map_CameraMoveStarted(object sender, EventArgs e)
		{
			Console.WriteLine("Camera Move Started");
			mapAction.Text = "Camera Move Started";

			map.DefaultPins.Clear();
		}

		void Map_CameraMoving(object sender, EventArgs e)
		{
			Console.WriteLine("Camera Moving");
			mapAction.Text = "Camera Moving";
		}

		void Map_CameraIdled(object sender, Bounds e)
		{
			Console.WriteLine("Camera Idled - NorthEast: " + e.NorthEast.latitude + " - " + e.NorthEast.longitude);
			Console.WriteLine("Camera Idled - NorthWest: " + e.NorthWest.latitude + " - " + e.NorthWest.longitude);
			Console.WriteLine("Camera Idled - SouthEast: " + e.SouthEast.latitude + " - " + e.SouthEast.longitude);
			Console.WriteLine("Camera Idled - SouthWest: " + e.SouthWest.latitude + " - " + e.SouthWest.longitude);

			Console.WriteLine("Camera Idled - Center: " + e.Center.latitude + " - " + e.Center.longitude);
			mapAction.Text = "Camera Idled - Center: " + e.Center.latitude + " - " + e.Center.longitude;
		}

		void Map_MapClicked(object sender, Position e)
		{
			Console.WriteLine("Map Clicked: " + e.latitude + " - " + e.longitude);
			mapAction.Text = "Map Clicked: " + e.latitude + " - " + e.longitude;

			//map.DefaultPins.Add(new DefaultPin() { Title = "Map Clicked", Position = e });

			map.pins.FirstOrDefault().position = e;
		}

		void MoveCamera_Clicked(object sender, System.EventArgs e)
		{
			map.moveMapToRegion(CameraPerspectiveFactory.fromCoordinates(new Position(10.336712, 123.929958), (sender as Button).ClassId.Equals("animated")));
		}

		bool isVisible = true;
		Random rnd = new Random();

		void Visibility_Clicked(object sender, System.EventArgs e)
		{
			isVisible = !isVisible;
			map.pins.All((arg) => { arg.isVisible = isVisible; return true; });
		}

		void Heading_Clicked(object sender, System.EventArgs e)
		{
			var x = rnd.Next(0, 360);
			Console.WriteLine("heading pin one: " + x);
			map.pins[0].heading = x;
			map.pins[1].heading = rnd.Next(0, 360);
			map.pins[2].heading = rnd.Next(0, 360);
			map.pins[3].heading = rnd.Next(0, 360);
		}
	}
}