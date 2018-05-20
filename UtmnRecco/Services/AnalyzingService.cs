using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Decompositions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UtmnRecco.Models;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;

namespace UtmnRecco.Services
{
	public class AnalyzingService
	{
		static readonly int? UsersLimit = 300;
		const int UniversityId = 863;
		static readonly uint[] Years =
		{
			2017,
			2016,
			2015,
			2014,
			2013
		};
		const string selectionFileName = "selection.json";

		VkApi vkApi;
		ILogger logger;
		List<FetchedUser> users = new List<FetchedUser>();
		SingularValueDecomposition svd;
		Task fetchingTask;

		public bool IsFetchingCompleted => fetchingTask.IsCompleted;

		public List<FetchedUser> Users => fetchingTask.IsCompleted
			? users
			: throw new FetchingInProgressException();

		string SelectionPath => $"{Environment.CurrentDirectory}\\{selectionFileName}";

		public AnalyzingService(string vkLogin, string vkPassword, VkApi vkApi, ILogger logger)
		{
			this.vkApi = vkApi;
			this.logger = logger;

			vkApi.Authorize(new ApiAuthParams
			{
				ApplicationId = 6282019,
				Login = vkLogin,
				Password = vkPassword,
				Settings = Settings.All
			});

			fetchingTask = new Task(() =>
			{
				if (File.Exists(SelectionPath))
				{
					logger.LogDebug("Local selection file found. Fetching from local.");
					users = FetchLocal();
					logger.LogDebug("Fetching from local done.");
				}
				else
				{
					logger.LogDebug("Local selection file not found. Fetching from remote.");
					users = FetchRemote(vkLogin, vkPassword);
					logger.LogDebug("Fetching from remote done. Saving fetched selection to file.");
					SaveLocal();
					logger.LogDebug("Saving fetched selection to file done.");
				}
			});
			fetchingTask.Start();
		}

		public Dictionary<Faculties, double> Analyze(FetchedUser user)
		{
			if (!fetchingTask.IsCompleted)
				throw new NotImplementedException();

			// form doc-term matrix
			var docs = new List<Faculties>();
			docs.AddRange(FacultiesExtensions.EnumerateReal());
			docs.Add(Faculties.Target);
			var body = users.SelectMany(u => u.GetWords())
				.Where(w => w != string.Empty && w.Length > 2);

			var terms = users.SelectMany(u => u.GetWords())
				.Take(300)
				.Distinct()
				.ToArray();

			var dtMat = new double[docs.Count, terms.Count()];
			for (int i = 0; i < dtMat.GetLength(0); i++)
			{
				IEnumerable<string> facultyBody;
				if (dtMat.GetLength(0) - 1 == i) // target faculty
				{
					facultyBody = user.GetWords();
				}
				else
				{
					facultyBody = users
						.Where(u => u.Universities != null && u.Universities
							.Select(uni => uni.FacultyId)
							.Contains((int)docs[i]))
						.SelectMany(u => u.GetWords());
				}
				for (int j = 0; j < dtMat.GetLength(1); j++)
					dtMat[i, j] = facultyBody.Count(w => w == terms[j]);
			}

			svd = new SingularValueDecomposition(dtMat);

			// compute low-ranked matrix
			var lowRankedMat = Matrix.Dot(svd.LeftSingularVectors, svd.DiagonalMatrix); // did not low-rank here

			// form index model
			var indexModel = new Dictionary<Faculties, double[]>();
			for (int i = 0; i < docs.Count; i++)
			{
				var vector = new double[svd.Rank];
				for (int j = 0; j < vector.Length; j++)
					vector[j] = lowRankedMat[i, j];
				indexModel.Add(docs[i], vector);
			}

			// calculate ranges
			var target = indexModel[Faculties.Target];
			var facs = FacultiesExtensions.EnumerateReal();
			var result = new Dictionary<Faculties, double>();
			foreach (var faculty in facs)
			{
				var vector = indexModel[faculty];
				double sum = 0;
				for (int i = 0; i < svd.Rank; i++)
					sum += Math.Pow(vector[i] - target[i], 2);
				var range = Math.Sqrt(sum);
				result.Add(faculty, range);
			}
			return result.OrderBy(r => r.Value).ToDictionary(ks => ks.Key, vs => vs.Value);
		}

		List<FetchedUser> FetchRemote(string vkLogin, string vkPassword)
		{
			var userIds = new List<long>();

			foreach (var year in Years)
				foreach (int facultyId in FacultiesExtensions.EnumerateReal())
				{
					var response = vkApi.Users.Search(new UserSearchParams
					{
						University = UniversityId,
						UniversityFaculty = facultyId,
						UniversityYear = year,
						Count = 1000
					});
					userIds.AddRange(response.Select(u => u.Id));
				}

			var results = new List<FetchedUser>();
			int i = 0;
			foreach (var userId in userIds)
			{
				// any fuckin vk.net error
				try
				{
					if (UsersLimit != null && i >= UsersLimit)
						break;
					var infoResponse = vkApi.Call(
						"execute.getUserInfo",
						new VkNet.Utils.VkParameters(new Dictionary<string, string>
						{
							["id"] = userId.ToString()
						}));
					i++;

					// try deserialize if wall is not hidden
					try
					{
						var fetchedResponse = JsonConvert.DeserializeObject<FetchedResponse>(infoResponse.RawJson);
						results.Add(fetchedResponse.Response);
					}
					catch (JsonSerializationException)
					{
						continue;
					}
				}
				catch (Exception ex)
				{
					logger.LogWarning($"User with ID {userId} is skipped during error {ex.Message}");
					continue;
				}
			}
			return results;
		}

		List<FetchedUser> FetchLocal()
		{
			var serializer = new JsonSerializer();
			using (var reader = new StreamReader(SelectionPath))
			{
				var readResult = serializer.Deserialize(reader, typeof(List<FetchedUser>));
				return (List<FetchedUser>)readResult;
			}
		}

		void SaveLocal()
		{
			var serializer = new JsonSerializer();
			using (var writer = new StreamWriter(SelectionPath, false))
			{
				serializer.Serialize(writer, users, typeof(List<FetchedUser>));
				writer.Flush();
			}
		}
	}

	public class FetchingInProgressException : Exception
	{
		public FetchingInProgressException() : base("Fetching in progress") { }
	}
}