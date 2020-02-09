using System;
using System.Collections.Generic;
using TopologyMap;
using Xunit;

namespace TestTopologyMap {
  public class MapTests {
    [Fact]
    public void TestGetCoordinates() {
      Map map = new Map {
        Rows = 10,
        Cols = 10
      };

      MapCoordinate start = new MapCoordinate() { Row = 5, Col = 5 };

      HashSet<MapCoordinate> coordinates = new HashSet<MapCoordinate>();

      int distance = 3;

      for (int dir = 0; dir < 360; dir++) {
        coordinates.Add(map.GetCoordinate(start, dir, distance));
      }

      Assert.Equal(8 * distance, coordinates.Count);

    }
  }
}
