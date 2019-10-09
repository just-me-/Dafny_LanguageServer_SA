using System;

namespace DafnyServer {
  public class VersionCheck {
    public static string CurrentVersion() {
      var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
      Console.WriteLine("VERSION:" + version);
      return "VERSION:" + version;
    }
  }
}
