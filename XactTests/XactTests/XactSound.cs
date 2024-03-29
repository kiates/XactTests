// Copyrightę 2016-2016 Chad C. Yates (cyates@dynfxdigital.com)

#region Using

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace XactTests
{
  class XactSound : Sound
  {
    public readonly SoundBank soundBank;
    public Cue Cue;

    public XactSound(SoundBank soundBank, string name, Keys key)
      : base(name, key)
    {
      this.soundBank = soundBank;
      Cue = null;
    }

    public override bool IsActive { get { return Cue != null; } }

    public override bool IsPrepared { get {return IsActive && Cue.IsPrepared;} }

    public override bool IsPlaying
    {
      get { return Cue != null && Cue.IsPlaying; }
    }

    public override bool IsStopping
    {
      get { return Cue != null && Cue.IsStopping; }
    }

    public override bool IsStopped
    {
      get { return Cue == null || Cue.IsStopped; }
    }

    public override bool IsDisposed { get { return Cue.IsDisposed; } }

    public override bool Paused
    {
      get { return Cue.IsPaused; }
      set
      {
        if (value && !Cue.IsPaused)
        {
          Cue.Pause();
        }
        else if (!value && Cue.IsPaused)
        {
          Cue.Resume();
        }
      }
    }

    public Cue GetCue()
    {
      if (Cue == null)
      {
        Cue = soundBank.GetCue(Name);
      }

      return Cue;
    }

    public override void Play(AudioListener listener, AudioEmitter emitter, bool positional)
    {
      Positional = positional;

      if (Cue == null)
      {
        Cue = soundBank.GetCue(Name);
      }

      if (positional)
      {
        Cue.Apply3D(listener, emitter);
      }

      Cue.Play();
    }

    public override void Apply3D(AudioListener listener, AudioEmitter emitter)
    {
      Cue.Apply3D(listener, emitter);
    }

    public override void Stop(AudioStopOptions asAuthored)
    {
      Cue.Stop(asAuthored);
    }

    public override void Dispose()
    {
      Cue.Dispose();
    }

    public override void Deactivate()
    {
      Cue = null;
    }

	  public override object Clone()
	  {
		  return new XactSound(soundBank, Name, Key);
	  }

	  public override void DrawCueInfo(SpriteBatch spriteBatch, SpriteFont font,
      ref Vector2 position)
    {
      float numCueInstances = 0;
      float attackTime = 0;
      float releaseTime = 0;
      float dopplerPitchScalar = 0;
      float orientationAngle = 0;
      float distance = 0;

      bool isCreated = false;
      bool isPreparing = false;
      bool isPrepared = false;
      bool isPlaying = false;
      bool isPaused = false;
      bool isStopping = false;
      bool isStopped = false;
      bool isDisposed = IsActive && Cue.IsDisposed;
      bool isActive = IsActive;

      Color clearColor = Color.LightGray;
      if (IsActive && !Cue.IsDisposed)
      {
        numCueInstances = Cue.GetVariable("NumCueInstances");
        attackTime = Cue.GetVariable("AttackTime");
        releaseTime = Cue.GetVariable("ReleaseTime");
        dopplerPitchScalar = Cue.GetVariable("DopplerPitchScalar");
        orientationAngle = Cue.GetVariable("OrientationAngle");
        distance = Cue.GetVariable("Distance");

        isCreated = Cue.IsCreated;
        isPreparing = Cue.IsPreparing;
        isPrepared = Cue.IsPrepared;
        isPlaying = Cue.IsPlaying;
        isPaused = Cue.IsPaused;
        isStopping = Cue.IsStopping;
        isStopped = Cue.IsStopped;
        isDisposed = Cue.IsDisposed;

        if (isPlaying)
        {
          clearColor = Color.White;
        }
        else if (isStopping)
        {
          clearColor = Color.LightGray;
        }
        else if (isStopped)
        {
          clearColor = Color.Gray;
        }
      }
      else
      {
        clearColor = Color.DarkGray;
      }

      StringBuilder sb = new StringBuilder(1000);

      sb.Clear();
      sb.Append(
        String.Format("XactCue: Nm='{0}',Crtd={1},Prpng={2},Prpd={3},Plyng={4},Psd={5},Stpng={6},Stpd={7},Dspd={8},Actv={9}",
          Name, isCreated, isPreparing, isPrepared, isPlaying, isPaused, isStopping, isStopped, isDisposed, isActive));
      spriteBatch.DrawString(font, sb.ToString(), position, clearColor);
      position.Y = position.Y + font.LineSpacing;

      sb.Clear();
      sb.Append(
        String.Format(
          "#Cues={0},AtkTm={1},RlsTm={2},DpSclr={3},Ang={4},Dstnc={5}",
          numCueInstances, attackTime, releaseTime, dopplerPitchScalar,
          orientationAngle, distance));
      spriteBatch.DrawString(font, sb.ToString(), position, clearColor);
      position.Y = position.Y + font.LineSpacing;
    }
  }
}