using System;
namespace MapBox.Abstractions
{
	public interface IDisplayMetrics
	{
		double getNativeScale();
		double getWidth();
		double getHeight();
	}
}
