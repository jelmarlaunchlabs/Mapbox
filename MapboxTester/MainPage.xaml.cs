using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapBox;
using MapBox.Models;
using Xamarin.Forms;

namespace MapboxTester
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
			InitializeComponent();
			// Initialize pins
			map.pins = new ObservableCollection<Pin>{
				new Pin{
					image = "Resources.car.png",
					IsCenterAndFlat = false,
					heading = 0,
					position = new Position(0,0),
					iconOffset = new Point(0,-50)
				},
				new Pin{
					image = "Resources.car.png",
					IsCenterAndFlat = true,
					heading = 180,
					position = new Position(1,1),
					width = 90,
					height = 90
				},
				new Pin{
					image = "Resources.car.png",
					IsCenterAndFlat = true,
					heading = 270,
					position = new Position(2,2),
					width = 90,
					height = 90
				},
				new Pin{
					image = "Resources.car.png",
					IsCenterAndFlat = true,
					heading = 45,
					position = new Position(3,3)
				}
			};

			// Add pin
			Device.StartTimer(TimeSpan.FromSeconds(10), () => {
				map.pins.Add(
					new Pin {
						image = "Resources.carBlack.jpg",
						IsCenterAndFlat = true,
						heading = 90,
						position = new Position(4, 4)
					});

				// Remove pin
				Device.StartTimer(TimeSpan.FromSeconds(10), () => {
					map.pins.Remove(map.pins[2]);

					//// Clear pins
					//Device.StartTimer(TimeSpan.FromSeconds(10), () => {
					//map.pins.Clear();

					// Change location of certain pin
					Device.StartTimer(TimeSpan.FromSeconds(10), () => {
						map.pins[1].position = new Position(-5, -5);
						map.pins[1].heading = 225;
						return false;
					});
					//	return false;
					//});
					return false;
				});
				return false;
			});
        }
    }
}
