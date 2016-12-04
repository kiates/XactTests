// Copyright© 2016-2016 Chad C. Yates (cyates@dynfxdigital.com)

#region Using

using System.Diagnostics.Contracts;

#endregion

namespace XactTests
{
#if WINDOWS || XBOX
  static class Program
  {
    /// <summary>The main entry point for the application.</summary>
    static void Main(string[] args)
    {
      using (Game1 game = new Game1())
      {
        game.Run();
      }
    }
  }
#endif
}