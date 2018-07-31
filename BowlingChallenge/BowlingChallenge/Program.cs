using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingChallenge
{
  class Program
  {
    static void Main(string[] args)
    {
      GameManager gm = new GameManager(args);
      Console.ReadLine();
    }
  }
}
