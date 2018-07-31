using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingChallenge
{
  static class RollGenerator
  {
    private static LehmerRng rand = new LehmerRng(new System.Random().Next());
    public static int GenerateRoll(int playerLevel, int remainingPins)
    {
      IList<int> rolls = new List<int>();

      for (int count = 0; count < playerLevel; count++)
      { 
        rolls.Add(RollWithGutterModifier(remainingPins));
      }

      return rolls.Max();
    }

    private static int RollWithGutterModifier(int remainingPins)
    {
      int gutter = ((int) rand.Next()) % 4;
      if (gutter > 0) gutter = 1;

      int roll = ((int) (rand.Next() % 11)) - (10 - remainingPins);
      if (roll < 0) roll = 0;

      int strike = (int) rand.Next() % 10;
      if (strike == 0) return remainingPins;

      return roll * gutter;
    }

    public static int GenerateNumberInRange(int min, int max)
    {
      return (int) (rand.Next() % (max - min + 1)) + min;
    }
  }

  public class LehmerRng
  {
    private const int a = 16807;
    private const int m = 2147483647;
    private const int q = 127773;
    private const int r = 2836;
    private int seed;
    public LehmerRng(int seed)
    {
      if (seed <= 0 || seed == int.MaxValue)
        throw new Exception("Bad seed");
      this.seed = seed;
    }
    public double Next()
    {
      int hi = seed / q;
      int lo = seed % q;
      seed = (a * lo) - (r * hi);
      if (seed <= 0)
        seed = seed + m;
      return ((seed * 1.0) / m) * 100000;
    }
  }
}
