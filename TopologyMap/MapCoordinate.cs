using System;
using System.Collections.Generic;
using System.Text;

namespace TopologyMap {
  public class MapCoordinate {
    public int Row { get; set; }
    public int Col { get; set; }

    public override bool Equals(object obj) {
#pragma warning disable IDE0046
      if(ReferenceEquals(this, obj)) {
        return true;
      }

      if(!(obj is MapCoordinate)) {
        return false;
      }

      MapCoordinate comp = (MapCoordinate)obj;

      if(Row != comp.Row) {
        return false;
      }
      if(Col != comp.Col) {
        return false;
      }

      return true;
#pragma warning restore IDE0046
    }

    public override int GetHashCode() {
      int value = 0;

      value += Row.GetHashCode() * 2;
      value += Col.GetHashCode() * 3;

      return value;
    }

    public override string ToString() {
      return $"({Row}, {Col})";
    }

  }
}
