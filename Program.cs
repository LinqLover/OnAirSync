using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using CommandLine;
using Microsoft.Win32;
using RegistryUtils;


class Options
{
    [Option('d', "devices", Required = false, HelpText = "Devices to monitor.")]
    public string Devices { get; set; }

    [Option('i', "item", Required = true, HelpText = "The openHAB item.")]
    public string OpenHabItem { get; set; }

    [Option('s', "server", HelpText = "The openHAB server.")]
    public string OpenHabServer { get; set; }
}


public class OnAirSync {
	public static string[] Devices { get; private set; }
	public static string OpenHabItem { get; private set; }
	public static string OpenHabServer { get; private set; }
	private static RegistryHive Hive = RegistryHive.CurrentUser;
	private static string BaseKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore";

	public static int Main(string[] args) {
		Parser.Default.ParseArguments<Options>(args)
			.WithParsed<Options>(o => {
				Devices = o.Devices?.Split(',') ?? new[] { "microphone", "webcam" };
				OpenHabItem = o.OpenHabItem;
				OpenHabServer = o.OpenHabServer ?? "http://openhab:8080";
			})
			.WithNotParsed(errors => {
				Environment.Exit(1);
			});

		Console.WriteLine("OnAirSync [STARTING]");
		UpdateState();
		Console.WriteLine();
		Console.WriteLine("OnAirSync [READY]");
		Console.WriteLine();

		var monitors = new Dictionary<string, RegistryMonitor>();
		foreach (var device in Devices) {
			var key = $"{BaseKey}\\{device}\\NonPackaged";
			var monitor = new RegistryMonitor(Hive, key);
			monitor.RegChanged += new EventHandler(OnRegChanged);
			monitors.Add(device, monitor);
		}
		foreach (var monitor in monitors.Values) {
			monitor.Start();
		}
 
		Console.CancelKeyPress += delegate {
			Console.WriteLine("OnAirSync [EXITING]");
			foreach (var monitor in monitors.Values) {
				monitor.Stop();
			}
			Console.WriteLine("OnAirSync [EXIT]");
	    };

		while(true);
	}

	public static void PrintState() {
		foreach (var device in Devices)
		{
			Console.WriteLine($"{device} is {(IsDeviceOn(device) ? "ON" : "OFF")}");
		}
	}
	
	public static void UpdateState() {
		PrintState();

		var state = Devices.Any(IsDeviceOn);
		Console.Write("Informing openHAB ...");
		UpdateOpenHab(state);
		Console.Write(" ");
		Console.WriteLine("Done.");
		Console.WriteLine();
	}

	private static void OnRegChanged(object sender, EventArgs e) {
		Console.WriteLine("[{0:s}] UPDATE:", DateTime.Now);
		UpdateState();
	}

	private static void UpdateOpenHab(bool state) {
		var url = $"{OpenHabServer}/rest/items/{OpenHabItem}";
		using (var webClient = new WebClient()) {
			webClient.Headers.Add("Accept", "application/json");
			webClient.Headers.Add("Content-Type", "text/plain");
			webClient.UploadString(url, "POST", state ? "ON" : "OFF");
		}
	}
	
	private static bool IsDeviceOn(string device) {
		var users = GetDeviceUsers(device).ToList();
		Console.WriteLine($"Users of {device}: {string.Join(", ", users)}");
		return users.Any();
	}

	private static IEnumerable<string> GetDeviceUsers(string device) {
		var deviceTimes = GetDeviceTimes(device);
		foreach (var user in deviceTimes.Keys) {
			var times = deviceTimes[user];
			if (times.start > times.stop) {
				yield return user;
			}
		}
	}

	private static IDictionary<string, (DateTime? start, DateTime? stop)> GetDeviceTimes(string device) {
		var deviceTimes = new Dictionary<string, (DateTime?, DateTime?)>();
		
		var hiveKey = RegistryKey.OpenBaseKey(Hive, RegistryView.Default);
		using (var key = hiveKey.OpenSubKey($"{BaseKey}\\{device}\\NonPackaged", RegistryKeyPermissionCheck.ReadSubTree)) {
			var users = key.GetSubKeyNames();
			foreach (var user in users)
			{
				using (var subkey = key.OpenSubKey(user)) {
					var objStart = subkey.GetValue("LastUsedTimeStart");
					var objStop = subkey.GetValue("LastUsedTimeStop");
					var start = objStart != null ? DateTime.FromFileTime((long)objStart) : default(DateTime?);
					var stop = objStop != null ? DateTime.FromFileTime((long)objStop) : default(DateTime?);
					deviceTimes.Add(user, (start, stop));
				}
			}
		}

		return deviceTimes;
	}
}
