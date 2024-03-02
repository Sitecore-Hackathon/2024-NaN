﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Demo.Init.Model;
using Sitecore.Demo.Init.Extensions;
using Microsoft.Extensions.Logging;

namespace Sitecore.Demo.Init.Jobs
{
	class DeactivateMobileDeviceLayout : TaskBase
	{
		public DeactivateMobileDeviceLayout(InitContext initContext)
			: base(initContext)
		{
		}

		public async Task Run()
		{
			if (this.IsCompleted())
			{
				Log.LogWarning($"{this.GetType().Name} is already complete, it will not execute this time");
				return;
			}

			var hostCM = Environment.GetEnvironmentVariable("HOST_CM");
			var user = Environment.GetEnvironmentVariable("ADMIN_USER_NAME").Replace("sitecore\\", string.Empty);
			var password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

			Log.LogInformation($"{this.GetType().Name} started on {hostCM}");
			using var client = new HttpClient { BaseAddress = new Uri(hostCM) };
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/sitecore/api/ssc/auth/login")
			{
				Content = new StringContent($"{{\"domain\":\"sitecore\",\"username\":\"{user}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json")
			};

			var response = await client.SendAsync(request);
			var contents = await response.Content.ReadAsStringAsync();
			var token = JsonConvert.DeserializeObject<SscLoginResponse>(contents).Token;

			UpdateValues(hostCM, token, "B039EBE1-5813-4243-81AE-EA55B2352D80", "master", "Fallback device", string.Empty);
			UpdateValues(hostCM, token, "B039EBE1-5813-4243-81AE-EA55B2352D80", "master", "Agent", string.Empty);

			Log.LogInformation($"{response.StatusCode} {contents}");
			Log.LogInformation($"{this.GetType().Name} complete");
			await Complete();
		}

		private void UpdateValues(string hostCM, string token, string itemId, string dbName, string itemFieldName, string itemFieldValue)
		{
			var client = new CookieWebClient();
			client.Encoding = System.Text.Encoding.UTF8;
			client.Headers.Add("token", token);
			client.Headers.Add("Content-Type", "application/json");
			client.UploadData(
				new Uri(hostCM + $"/sitecore/api/ssc/item/{itemId}?database={dbName}"),
				"PATCH",
				System.Text.Encoding.UTF8.GetBytes($"{{\"{itemFieldName}\": \"{itemFieldValue}\" }}"));
		}
	}
}
