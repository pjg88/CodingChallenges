using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingChallenge
{
  class GameManager
  {
    private IList<Player> _players = new List<Player>();
    private IList<int> _rolls = new List<int>();
    private int playerTurn = 0;
    private int numOfPlayers = 0;
    private bool autoRoll = false;

    public GameManager(string[] args)
    {
      IList<string> rolls = new List<string>(args);

      SetupPlayers(rolls);
      RunGame(rolls);
    }

    private void RunGame(IList<string> rolls)
    {
      EnterRolls(rolls);

      while (_players.Any(x => !x.gameOver))
      {
        if(!_players[playerTurn].gameOver)
        {
          if (autoRoll)
          {
            int remainingPins = _players[playerTurn].GetRemainingPins();
            if (remainingPins <= 0 && _players[playerTurn].currentFrame == 9) remainingPins = 10;
            EnterRoll(RollGenerator.GenerateRoll(_players[playerTurn].playerLevel, remainingPins));
          }
          else
          {
            Console.WriteLine("Enter next roll: ");
            int roll = Convert.ToInt32(Console.ReadLine());
            if (roll < 0) autoRoll = true;
            else EnterRoll(roll);
          }

          PrintScores();
        }
        else
        {
          AdvanceTurn();
        }
      }
    }

    private void SetupPlayers(IList<string> rolls)
    {
      if (rolls.Count > 0)
      {
        numOfPlayers = Convert.ToInt32(rolls[0]);
        rolls.RemoveAt(0);
      }
      else
      {
        Console.WriteLine("At any point, enter -1 to switch from manual entry to auto-generation.");
        Console.WriteLine("Enter number of players:");
        numOfPlayers = Convert.ToInt32(Console.ReadLine());
        if(numOfPlayers < 0)
        {
          AutoCreatePlayers();
          return;
        }
      }

      for (int count = 0; count < numOfPlayers; count++)
      {
        _players.Add(new Player("Player " + (count + 1)));
        if (rolls.Any())
        {
          _players[count].playerLevel = Convert.ToInt32(rolls[0]) + 1;
          rolls.RemoveAt(0);
        }
        else
        {
          Console.WriteLine("Enter 0-9 rating for Player " + (count + 1) +
                            " (0 = beginner, 1 = amatuer, 2 = novice, etc...");
          int level = Convert.ToInt32(Console.ReadLine());
          if (level < 0)
          {
            AutoCreatePlayers();
            return;
          }

          _players[count].playerLevel = level + 1;
        }
      }
    }

    private void AutoCreatePlayers()
    {
      if (numOfPlayers <= 0) numOfPlayers = RollGenerator.GenerateNumberInRange(2, 5);
      if (_players.Any()) _players[_players.Count - 1].playerLevel = RollGenerator.GenerateNumberInRange(1, 3);
      for (int count = _players.Count; count < numOfPlayers; count++)
      {
        _players.Add(new Player("Player " + (count + 1)));
        _players[count].playerLevel = RollGenerator.GenerateNumberInRange(1, 5);
      }

      autoRoll = true;
    }

    private void EnterRolls(IList<string> rolls)
    {
      foreach (string roll in rolls)
      {
        EnterRoll(Convert.ToInt32(roll));
        PrintScores();
      }
    }

    private void EnterRoll(int roll)
    {
      _rolls.Add(roll);
      int frame = _players[playerTurn].currentFrame;
      _players[playerTurn].AddRoll(roll);
      if (frame < _players[playerTurn].currentFrame)
      {
        AdvanceTurn();
      }
    }

    private void AdvanceTurn()
    {
      if (playerTurn >= _players.Count - 1) playerTurn = 0;
      else playerTurn++;
    }

    private void PrintScores()
    {
      foreach (Player score in _players)
      {
        Console.WriteLine(score.ToString());
      }
      Console.WriteLine();
    }
  }
}
