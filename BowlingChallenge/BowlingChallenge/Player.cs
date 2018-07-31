using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BowlingChallenge
{
  class Player
  {
    private IList<int> rollSequence;
    private IList<FrameScore> frameScore;
    private int totalScore { get; set; }
    public int currentFrame { get; set; }
    public bool gameOver { get; set; } = false;
    public string playerName;
    public int playerLevel;

    public Player(string name)
    {
      playerName = name;
      rollSequence = new List<int>();
      frameScore = new List<FrameScore>();
      frameScore.Add(new FrameScore());
      totalScore = 0;
      currentFrame = 0;
    }

    public void AddRoll(int roll)
    {
      FrameScore frame = frameScore[currentFrame];
      if (roll < 0 || roll > 10) roll = -1;
      rollSequence.Add(roll);
      frame.AddRoll(roll);
      AddRollRetrogressively(roll);
      if(currentFrame < 9)
      {
        if (frame.Score >= 10 || frame.Rolls.Count == 2)
        {
          frameScore.Add(new FrameScore());
          currentFrame++;
          if (currentFrame == 9) frameScore[currentFrame].TenthFrame = true;
        }
      }
      else
      {
        if (frame.Rolls.Any(x => x != 10))
        {
          if (frame.Rolls.Count > 2)
          {
            gameOver = true;
          }
          if (frame.Rolls.Count > 1 && !frame.Spare && !frame.Strike)
          {
            gameOver = true;
          }
        }
      }
    }

    private void AddRollRetrogressively(int roll)
    {
      if(frameScore[currentFrame].Rolls.Count < 3)
      {
        if (currentFrame > 0)
        {
          if (frameScore[currentFrame].Rolls.Count > 1)
          {
            if (frameScore[currentFrame - 1].Strike)
            {
              frameScore[currentFrame - 1].Score += roll;
            }
          }
          else
          {
            if (frameScore[currentFrame - 1].Spare || frameScore[currentFrame - 1].Strike)
            {
              frameScore[currentFrame - 1].Score += roll;
            }
          }
        }

        if (currentFrame > 1)
        {
          if (frameScore[currentFrame].Rolls.Count == 1)
          {
            if (frameScore[currentFrame - 1].Strike)
            {
              if (frameScore[currentFrame - 2].Strike)
              {
                frameScore[currentFrame - 2].Score += roll;
              }
            }
          }
        }
      }
    }

    public int GetRemainingPins()
    {
      return 10 - (frameScore[currentFrame].Rolls.Sum() % 10);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(playerName + " (" + (playerLevel - 1) + ")");
      sb.Append("| ");
      foreach (FrameScore frame in frameScore)
      {
        if(frame.Rolls.Count > 0) frame.Rolls.ForEach(x => sb.Append(x + " "));
        sb.Append("| ");
      }
      sb.AppendLine();
      sb.Append("| ");
      foreach (FrameScore frame in frameScore)
      {
        sb.Append(frame.Score + " | ");
      }

      totalScore = frameScore.Sum(x => x.Score);
      sb.Append("        " + totalScore);

      return sb.ToString();
    }
  }

  internal class FrameScore
  {
    public bool Strike { get; set;  }
    public bool Spare { get; set; }
    public bool TenthFrame { get; set; }
    public int Score { get; set; }
    public List<int> Rolls { get; set; }

    public FrameScore()
    {
      Strike = false;
      Spare = false;
      Score = 0;
      Rolls = new List<int>();
    }

    public void AddRoll(int roll)
    {
      if (roll + Rolls.Sum() > 10 && !TenthFrame)
      {
        roll = -1;
      }
      else if (TenthFrame && Rolls.Count == 1)
      {
        if (!Strike && roll + Rolls[0] > 10) roll = -1;
      }
      else if (TenthFrame && Rolls.Count == 2)
      {
        if (Strike && Rolls[1] < 10 && !Spare && roll + Rolls[1] > 10) roll = -1;
      }
      Rolls.Add(roll);
      Score = Rolls.Sum();
      if (Rolls.Count > 3) Score = Rolls.Where(x => x == 10).Sum();
      if (Score == 10)
      {
        if (Rolls.Count == 1) Strike = true;
        else if (Rolls.Count == 2) Spare = true;
      }
    }
  }
}
