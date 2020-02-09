using NemMvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TopologyMap {
  public class Map : NotifyPropertyChanged {
    private readonly HashSet<IMapItem> _items;
    private readonly HashSet<MapCoordinate> _fixedHeights;
    private IMapItem[,] _itemsMap;
    private decimal[,] _topography;
    private int _rows;
    private int _cols;

    public int Rows {
      get => _rows;
      set {
        if(_rows == value) {
          return;
        }

        _rows = value;
        RaisePropertyChanged(nameof(Rows));
      }
    }
    public int Cols {
      get => _cols;
      set {
        if(_cols == value) {
          return;
        }

        _cols = value;
        RaisePropertyChanged(nameof(Cols));
      }
    }

    public decimal[,] Topography {
      get => _topography;
      set => SetProperty(ref _topography, value);
    }

    public Map() {
      _items = new HashSet<IMapItem>();
      _fixedHeights = new HashSet<MapCoordinate>();
      PropertyChanged += Map_PropertyChanged;
    }

    public bool AddMapItem(IMapItem mapItem) {
      if(_items == null) {
        return false;
      }

      mapItem.PropertyChanged += MapItem_PropertyChanged;

      _items.Add(mapItem);
      _fixedHeights.Add(mapItem.Coordinate);
      _itemsMap[mapItem.Coordinate.Row, mapItem.Coordinate.Col] = mapItem;
      AdjustTopographyAround(mapItem.Coordinate, mapItem.Height, mapItem.Slope);

      return true;
    }

    

    private void AdjustTopographyAround(MapCoordinate coordinate, decimal height, decimal slope) {
      _topography[coordinate.Row, coordinate.Col] = height;
      if(height == 0 || slope == 0) {
        return;
      }

      decimal changeAmount = height * slope;
      int steps = (int)(Math.Abs(height / changeAmount) - 1);

      if(steps <= 0) {
        return;
      }

      AdjustHeight(coordinate, changeAmount, steps);

      RaisePropertyChanged(nameof(Topography));
    }

    private void AdjustHeight(MapCoordinate coord, decimal changeAmount, int steps) {
      if(steps <= 0) {
        return;
      }

      decimal initial = Topography[coord.Row, coord.Col];

      HashSet<MapCoordinate> filled = new HashSet<MapCoordinate>();

      for(int dist = 1; dist <= steps; dist++) {
        for(int dir = 0; dir < 360; dir++) {
          MapCoordinate pos = GetCoordinate(coord, dir, dist);
          if(_fixedHeights.Contains(pos) || filled.Contains(pos)) {
            continue;
          }

          decimal existingHeight = Topography[pos.Row, pos.Col];
          decimal newHeight = initial - (changeAmount * dist);

          Topography[pos.Row, pos.Col] = existingHeight != 0 ? (existingHeight + newHeight) / 2 : newHeight;
          filled.Add(pos);
        }
      }

    }

    public MapCoordinate GetCoordinate(MapCoordinate startCoord, int direction, double distance) {
      double dirRadians = (Math.PI / 180) * direction;
      double sinOfDir = Math.Sin(dirRadians);
      double cosOfDir = Math.Cos(dirRadians);

      int yNegMod = sinOfDir < 0 ? -1 : 1;
      int xNegMod = cosOfDir < 0 ? -1 : 1;

      double longDist = Math.Sqrt(2) * distance;

      int deltaY = (int)Math.Round((longDist) * sinOfDir);
      deltaY = (int)Math.Min(distance, Math.Abs(deltaY)) * yNegMod;

      int deltaX = (int)Math.Round((distance) * cosOfDir);
      deltaX = (int)Math.Min(distance, Math.Abs(deltaX)) * xNegMod;

      int row = startCoord.Row;
      int col = startCoord.Col;

      MapCoordinate newCoord = new MapCoordinate() {
        Row = row + deltaY,
        Col = col + deltaX
      };

      if(newCoord.Row >= Rows) {
        newCoord.Row -= Rows;
      }
      if(newCoord.Row < 0) {
        newCoord.Row += Rows;
      }
      if(newCoord.Col >= Cols) {
        newCoord.Col -= Cols;
      }
      if(newCoord.Col < 0) {
        newCoord.Col += Cols;
      }

      return newCoord;
    }


    private void PopulateItemsMap() {
      _itemsMap = new IMapItem[Rows, Cols];

      foreach(IMapItem mapItem in _items) {
        AddMapItem(mapItem);
      }
    }

    private void Map_PropertyChanged(object sender, PropertyChangedEventArgs changeProperties) {
      if(changeProperties.PropertyName == nameof(Rows) || changeProperties.PropertyName == nameof(Cols)) {
        if (Rows > 0 && Cols > 0) {
          _itemsMap = new IMapItem[Rows, Cols];
          Topography = new decimal[Rows, Cols];
          for(int r = 0; r < Rows; r++) {
            for(int c = 0; c < Cols; c++) {
              _topography[r, c] = 0;
            }
          }
          
        }
        PopulateItemsMap();
      }
    }

    private void MapItem_PropertyChanged(object sender, MapChangeProperties changeProperties) {
      if(changeProperties.PropertyName == nameof(IMapItem.Coordinate)) {
        PopulateItemsMap();
      }
    }

  }
}
