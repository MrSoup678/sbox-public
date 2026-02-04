using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Sandbox;

public class TestAppSystem : AppSystem
{
	public override void Init()
	{
		GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
		var GameFolder = System.Environment.GetEnvironmentVariable( "FACEPUNCH_ENGINE", EnvironmentVariableTarget.Process );
		if ( GameFolder is null ) throw new Exception( "FACEPUNCH_ENGINE not found" );

		NetCore.InitializeInterop( GameFolder );

		var nativeDllPath="";
			if (OperatingSystem.IsWindows())
			{
				
			 	nativeDllPath = $"{GameFolder}\\bin\\win64";
				//
				// If we don't load sentry specifically from this directly, it'll
				// try to load the one from the managed folder
				//
				NativeLibrary.TryLoad( $"{nativeDllPath}\\sentry.dll", out _ );
				//NativeLibrary.TryLoad( $"{nativeDllPath}\\tier0.dll", out _ );
				//NativeLibrary.TryLoad( $"{nativeDllPath}\\engine2.dll", out _ );
			} else if (OperatingSystem.IsLinux())
			{
				nativeDllPath = $"{GameFolder}/bin/linuxsteamrt64";
				NativeLibrary.TryLoad($"{nativeDllPath}/libsentry.so",out _);
			} else if (OperatingSystem.IsMacOS())
			{
				nativeDllPath = $"{GameFolder}/bin/osxarm64";
				NativeLibrary.TryLoad($"{nativeDllPath}/libsentry.dylib",out _);
			} else 
				throw new Exception("Unsupported system");

			

		//
		// Put our native dll path first so that when looking up native dlls we'll
		// always use the ones from our folder first
		//
		var path = System.Environment.GetEnvironmentVariable( "PATH" );
		path = $"{nativeDllPath};{path}";
		System.Environment.SetEnvironmentVariable( "PATH", path );

		CreateGame();

		var createInfo = new AppSystemCreateInfo()
		{
			Flags = AppSystemFlags.IsGameApp | AppSystemFlags.IsUnitTest
		};

		InitGame( createInfo, "" );
	}
}
