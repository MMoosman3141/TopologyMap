using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TopologyMap {
  public interface IMapItem {
    MapCoordinate Coordinate { get; set; }

    public decimal Height { get; set; }
    public decimal Slope { get; set; }

    public delegate void ChangedHandler(object sender, MapChangeProperties changeProperties);
    public event ChangedHandler PropertyChanged;
  }
}
