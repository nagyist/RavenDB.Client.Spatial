﻿using System;
using System.Collections.Generic;
using System.Linq;
using Geo;
using Geo.Abstractions.Interfaces;
using Geo.Geometries;
using Geo.IO.GeoJson;

namespace Raven.Client.Spatial.Geo
{
	internal class ShapeConverter : IShapeConverter
	{
		public bool IsValid(object obj)
		{
			return obj is Point
				|| obj is LineString
				|| obj is Polygon
				|| obj is MultiPoint
				|| obj is MultiLineString
				|| obj is MultiPolygon
				|| obj is GeometryCollection
				|| obj is Feature
				|| obj is FeatureCollection;
		}

		public GeoJsonObjectType GetGeoJsonObjectType(object obj)
		{
			if (obj is Point)
				return GeoJsonObjectType.Point;
			if (obj is LineString)
				return GeoJsonObjectType.LineString;
			if (obj is Polygon)
				return GeoJsonObjectType.Polygon;
			if (obj is MultiPoint)
				return GeoJsonObjectType.MultiPoint;
			if (obj is MultiLineString)
				return GeoJsonObjectType.MultiLineString;
			if (obj is MultiPolygon)
				return GeoJsonObjectType.MultiPolygon;
			if (obj is GeometryCollection)
				return GeoJsonObjectType.GeometryCollection;
			if (obj is Feature)
				return GeoJsonObjectType.Feature;
			if (obj is FeatureCollection)
				return GeoJsonObjectType.FeatureCollection;

			throw new ArgumentException("obj");
		}

		public WktObjectType GetWktObjectType(object obj)
		{
			if (obj is Point)
				return WktObjectType.Point;
			if (obj is LineString)
				return WktObjectType.LineString;
			if (obj is Polygon)
				return WktObjectType.Polygon;
			if (obj is MultiPoint)
				return WktObjectType.MultiPoint;
			if (obj is MultiLineString)
				return WktObjectType.MultiLineString;
			if (obj is MultiPolygon)
				return WktObjectType.MultiPolygon;
			if (obj is GeometryCollection)
				return WktObjectType.GeometryCollection;

			throw new ArgumentException("obj");
		}

	    private Coordinate MakeCoordinate(CoordinateInfo coordinate)
        {
            if (coordinate.Z.HasValue && coordinate.M.HasValue)
                return new CoordinateZM(coordinate.Y, coordinate.X, coordinate.Z.Value, coordinate.M.Value);
            if (coordinate.Z.HasValue)
                return new CoordinateZ(coordinate.Y, coordinate.X, coordinate.Z.Value);
            if (coordinate.M.HasValue)
                return new CoordinateM(coordinate.Y, coordinate.X, coordinate.M.Value);
            return new Coordinate(coordinate.Y, coordinate.X);
	    }

		public object ToPoint(CoordinateInfo coordinates)
		{
			if (coordinates == null)
				return Point.Empty;
			return new Point(MakeCoordinate(coordinates));
		}

		public object ToLineString(CoordinateInfo[] coordinates)
		{
			if (coordinates.Length == 0)
				return LineString.Empty;
			return new LineString(coordinates.Select(MakeCoordinate));
		}

		public object ToLinearRing(CoordinateInfo[] coordinates)
		{
			if (coordinates.Length == 0)
				return LinearRing.Empty;
			return new LinearRing(coordinates.Select(MakeCoordinate));
		}

		public object ToPolygon(CoordinateInfo[][] coordinates)
		{
			if (coordinates.Length == 0)
				return Polygon.Empty;
			return new Polygon(
				new LinearRing(coordinates.First().Select(MakeCoordinate)),
				coordinates.Skip(1).Select(x => new LinearRing(x.Select(MakeCoordinate)))
				);
		}

		public object ToMultiPoint(CoordinateInfo[] coordinates)
		{
			if (coordinates.Length == 0)
				return MultiPoint.Empty;
			return new MultiPoint(coordinates.Select(ToPoint).Cast<Point>());
		}

		public object ToMultiLineString(CoordinateInfo[][] coordinates)
		{
			if (coordinates.Length == 0)
				return MultiLineString.Empty;
			return new MultiLineString(coordinates.Select(ToLineString).Cast<LineString>());
		}

		public object ToMultiPolygon(CoordinateInfo[][][] coordinates)
		{
			if (coordinates.Length == 0)
				return MultiPolygon.Empty;
			return new MultiPolygon(coordinates.Select(ToPolygon).Cast<Polygon>());
		}

		public object ToGeometryCollection(object[] geometries)
		{
			if (geometries.Length == 0)
				return GeometryCollection.Empty;
			return new GeometryCollection(geometries.Cast<IGeometry>());
		}

		public object ToFeature(object geometry, object id, Dictionary<string, object> properties)
		{
			var feature = new Feature((IGeoJsonGeometry)geometry, properties);
			if (id != null)
				feature.Id = id;
			return feature;
		}

		public object ToFeatureCollection(object[] features)
		{
			return new FeatureCollection(features.Cast<Feature>());
		}

	    private CoordinateInfo MakeCoordinate(Coordinate coordinate)
        {
            return new CoordinateInfo()
            {
                X = coordinate.Longitude,
                Y = coordinate.Latitude,
                Z = coordinate.Is3D ? ((Is3D)coordinate).Elevation : (double?)null,
                M = coordinate.IsMeasured ? ((IsMeasured)coordinate).Measure : (double?)null
            };
	    }

		public CoordinateInfo FromPoint(object point)
		{
			if (((Point)point).IsEmpty)
                return null;
			return MakeCoordinate(((Point)point).Coordinate);
		}

		public CoordinateInfo[] FromLineString(object lineString)
		{
			if (((LineString)lineString).IsEmpty)
                return new CoordinateInfo[0];
			return ((LineString) lineString).Coordinates.Select(MakeCoordinate).ToArray();
		}

		public CoordinateInfo[] FromLinearRing(object lineString)
		{
			if (((LinearRing)lineString).IsEmpty)
                return new CoordinateInfo[0];
			return ((LinearRing)lineString).Coordinates.Select(MakeCoordinate).ToArray();
		}

		public CoordinateInfo[][] FromPolygon(object polygon)
		{
			if (((Polygon)polygon).IsEmpty)
                return new CoordinateInfo[0][];

			var p = (Polygon) polygon;
			var list = new List<CoordinateInfo[]>();
			list.Add(p.Shell.Coordinates.Select(MakeCoordinate).ToArray());
			list.AddRange(p.Holes.Select(x => x.Coordinates.Select(MakeCoordinate).ToArray()).ToArray());
			return list.ToArray();
		}

		public CoordinateInfo[] FromMultiPoint(object multiPoint)
		{
			if (((MultiPoint)multiPoint).IsEmpty)
                return new CoordinateInfo[0];

			return ((MultiPoint) multiPoint).Geometries.Cast<Point>().Select(FromPoint).ToArray();
		}

		public CoordinateInfo[][] FromMultiLineString(object multiLineString)
		{
			if (((MultiLineString)multiLineString).IsEmpty)
                return new CoordinateInfo[0][];

			return ((MultiLineString) multiLineString).Geometries.Cast<LineString>().Select(FromLineString).ToArray();
		}

		public CoordinateInfo[][][] FromMultiPolygon(object multiPolygon)
		{
			if (((MultiPolygon)multiPolygon).IsEmpty)
                return new CoordinateInfo[0][][];

			return ((MultiPolygon)multiPolygon).Geometries.Cast<Polygon>().Select(FromPolygon).ToArray();
		}

		public object[] FromGeometryCollection(object geometryCollection)
		{
			return ((GeometryCollection) geometryCollection).Geometries.Cast<object>().ToArray();
		}

		public object FromFeature(object feature, out object id, out Dictionary<string, object> properties)
		{
			var feat = (Feature) feature;
			id = feat.Id;
			properties = feat.Properties;
			return feat.Geometry;
		}

		public object[] FromFeatureCollection(object featureCollection)
		{
			return ((FeatureCollection) featureCollection).Features.Cast<object>().ToArray();
		}
	}
}