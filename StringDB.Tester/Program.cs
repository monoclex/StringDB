﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StringDB.Tester
{
    class Program
    {
		static void Main(string[] args) {
			var db = Database.FromFile("blank.db");

			foreach(var i in db) {

			}

			Console.ReadLine();
		}
	}


	public static class GenerateItems {
		public const int ItemsToInsert = 10_000;

		public const int MinIncome = 100;
		public const int MaxIncome = 10_000;

		public const int FriendsToGenerate = 20;

		private static Random _random;
		public static Random Rng => _random ?? (_random = new Random());

		internal static int LastDatabaseIDGenerated = Rng.Next(0, int.MaxValue / 2);
		public static Database NewStringDB() => Database.FromFile(GenerateDatabaseName(LastDatabaseIDGenerated++));

		public static string GenerateDatabaseName(int id) => $"{id}-stringdb.db";

		public static readonly string[] RandomNames = {
			"Jimbo",
			"Josh",
			"Shelby",
			"Kelly",
			"Jimmy",
			"John",
			"Sarah",
			"Hailee",
			"Kevin",
			"Alex",
			"Elizabeth",
			"Skyler"
		};

		public static string RandomName => RandomNames[Rng.Next(0, RandomNames.Length)];

		//we are NOT using yield return because we will get random things *every time* we iterate over it

		public static IEnumerable<Item> GetItems(int items) {
			var res = new List<Item>();

			for (var i = 0; i < items; i++) {
				string usersName = RandomName;
				res.Add(new Item() {
					Identifier = $"{i}.{usersName}",
					Name = $"{usersName} {RandomName}",
					Dollars = Rng.Next(GenerateItems.MinIncome, GenerateItems.MaxIncome),
					Friends = GenerateFriends(GenerateItems.FriendsToGenerate).ToArray()
				});
			}

			return res;
		}

		public static IEnumerable<KeyValuePair<string, string>> GetItemsAsKVP(IEnumerable<Item> items) {
			foreach (var i in items)
				yield return new KeyValuePair<string, string>(i.Identifier, JsonConvert.SerializeObject(i));
		}

		public static IEnumerable<string> GenerateFriends(int amount) {
			for (var i = 0; i < amount; i++)
				yield return RandomName;
		}

		public class Item {
			public string Identifier { get; set; }

			public string Name { get; set; }
			public int Dollars { get; set; }
			public string[] Friends { get; set; }
		}
	}
}
