using System;
using DotSpatial.Topology;
using Raven.Imports.Newtonsoft.Json;

namespace Raven.Client.Spatial.DotSpatial
{
	public class GeoJsonConverter : JsonConverter
	{
		private readonly GeoJsonReader _reader;
		private readonly GeoJsonWriter _writer;

		public GeoJsonConverter() : this(GeometryFactory.Default)
		{
		}

		public GeoJsonConverter(IGeometryFactory geometryFactory)
		{
			var maker = new ShapeConverter(geometryFactory);
			_reader = new GeoJsonReader(maker);
			_writer = new GeoJsonWriter(maker);
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(IGeometry).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return _reader.ReadJson(reader, objectType, existingValue, serializer);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			_writer.WriteJson(writer, value, serializer);
		}
	}
}