using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
	class UserOnDB
	{
		[BsonId]
		public ObjectId Id { get; set; }
		[BsonElement("username")]
		public string Username { get; set; }
		[BsonElement("full_name")]
		public string Full_name { get; set; }
		[BsonElement("email")]
		public string Email { get; set; }
		[BsonElement("password")]
		public string Pass { get; set; }
		[BsonElement("admin")]
		public bool Admin { get; set; }
		[BsonElement("__v")]
		public int Sdfsf { get; set; }
		[BsonElement("status")]
		public string Status { get; set; }
		[BsonElement("playtime")]
		public int Playtime { get; set; }
		
	}
}
