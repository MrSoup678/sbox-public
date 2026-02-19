#if !WIN


using System.Reflection;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Sandbox;

public class SboxNativesResolver
{
	private static readonly Dictionary<string, IntPtr> cachedLibHandles = new Dictionary<string, IntPtr>();
	private static bool isSDL3System = false;
	public static void SetupResolvers()
	{
		Assembly skiaSharpAssebmlyRef = typeof( SKAlphaType ).Assembly,
				harfBuzzSharpAssemblyRef = typeof( HarfBuzzSharp.Font ).Assembly,
				selfAssembly = typeof( SboxNativesResolver ).Assembly;
		NativeLibrary.SetDllImportResolver( skiaSharpAssebmlyRef, SkiaSharpImportResolver );
		NativeLibrary.SetDllImportResolver( harfBuzzSharpAssemblyRef, HarfBuzzSharpImportResolver );
		NativeLibrary.SetDllImportResolver( selfAssembly, SelfImportResolver );
	}


	~SboxNativesResolver()
	{
		foreach ( KeyValuePair<string, IntPtr> libHandle in cachedLibHandles )
		{
			NativeLibrary.Free( libHandle.Value );
		}
	}


	private static IntPtr SelfImportResolver( string libraryName, Assembly assembly, DllImportSearchPath? searchPath )
	{

		//TODO: Steam_api resolution goes here. Can't do this right now due to StructPlatformPackSize also needing to be changed.
		//Also that is lifted straight from Facepunch.Steamworks. (or other way around) Maybe introduce said changes there first.
		if ( libraryName == "SDL3" )
		{
			if ( !isSDL3System && cachedLibHandles.TryGetValue( libraryName, out IntPtr outPtr ) )
			{
				return outPtr;
			}
			IntPtr libHandle;
			try
			{
				if ( OperatingSystem.IsLinux() )
				{
					libHandle = NativeLibrary.Load( $"{NetCore.NativeDllPath}/libHarfBuzzSharp.so.0.60830.0" );
					cachedLibHandles.Add( libraryName, libHandle );
					return libHandle;
				}
				else if ( OperatingSystem.IsMacOS() )
				{
					libHandle = NativeLibrary.Load( $"{NetCore.NativeDllPath}/libHarfBuzzSharp.dylib" );
					cachedLibHandles.Add( libraryName, libHandle );
					return libHandle;
				}
			}
			catch ( DllNotFoundException )
			{
				if ( OperatingSystem.IsLinux() )
				{
					//that's fine. Try to pull from system/sniper.
					return IntPtr.Zero;
				}
				else
				{
					throw;
				}
			}
			;


		}
		return IntPtr.Zero;
	}
	//TODO: Rename these files.
	private static IntPtr HarfBuzzSharpImportResolver( string libraryName, Assembly assembly, DllImportSearchPath? searchPath )
	{
		if ( libraryName == "libHarfBuzzSharp" )
		{
			if ( cachedLibHandles.TryGetValue( libraryName, out IntPtr outPtr ) )
			{
				return outPtr;
			}
			IntPtr libHandle;
			if ( OperatingSystem.IsLinux() )
			{
				libHandle = NativeLibrary.Load( $"{NetCore.NativeDllPath}/libHarfBuzzSharp.so.0.60830.0" );
				cachedLibHandles.Add( libraryName, libHandle );
				return libHandle;
			}
			else if ( OperatingSystem.IsMacOS() )
			{
				libHandle = NativeLibrary.Load( $"{NetCore.NativeDllPath}/libHarfBuzzSharp.dylib" );
				cachedLibHandles.Add( libraryName, libHandle );
				return libHandle;
			}
		}
		return IntPtr.Zero;
	}


	private static IntPtr SkiaSharpImportResolver( string libraryName, Assembly assembly, DllImportSearchPath? searchPath )
	{

		if ( libraryName == "libSkiaSharp" )
		{
			if ( cachedLibHandles.TryGetValue( libraryName, out IntPtr outPtr ) )
			{
				return outPtr;
			}
			IntPtr libHandle;
			if ( OperatingSystem.IsLinux() )
			{
				libHandle = NativeLibrary.Load( $"{NetCore.NativeDllPath}/libSkiaSharp.so.116.0.0" );
				cachedLibHandles.Add( libraryName, libHandle );
				return libHandle;
			}
			else if ( OperatingSystem.IsMacOS() )
			{
				libHandle = NativeLibrary.Load( $"{NetCore.NativeDllPath}/libSkiaSharp.dylib" );
				cachedLibHandles.Add( libraryName, libHandle );
				return libHandle;
			}
		}
		return IntPtr.Zero;
	}

}
#endif
