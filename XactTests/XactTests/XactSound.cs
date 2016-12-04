// Copyright© 2016-2016 Chad C. Yates (cyates@dynfxdigital.com)

#region Using

using System;
using System.Diagnostics.Contracts;
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

    public XactSound(SoundBank soundBank, string name, bool positional, Keys key)
      : base(name, positional, key)
    {
      this.soundBank = soundBank;
      Cue = null;
    }

    public override bool IsActive { get { return Cue != null; } }

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

    public override void Play(AudioListener listener, AudioEmitter emitter)
    {
      Cue = soundBank.GetCue(Name);

      if (Positional)
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
      Cue = null;
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

      bool isPlaying = false;
      bool isStopping = false;
      bool isStopped = true;

      Color clearColor = Color.LightGray;
      if (IsActive)
      {
        numCueInstances = Cue.GetVariable("NumCueInstances");
        attackTime = Cue.GetVariable("AttackTime");
        releaseTime = Cue.GetVariable("ReleaseTime");
        dopplerPitchScalar = Cue.GetVariable("DopplerPitchScalar");
        orientationAngle = Cue.GetVariable("OrientationAngle");
        distance = Cue.GetVariable("Distance");

        isPlaying = Cue.IsPlaying;
        isStopping = Cue.IsStopping;
        isStopped = Cue.IsStopped;

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

      spriteBatch.DrawString(font,
        String.Format(
          "XactCue: Name='{3}', IsPlaying={0}, IsStopping={1}, IsStopped={2}",
          isPlaying, isStopping, isStopped, Name), position, clearColor);
      position.Y = position.Y + font.LineSpacing;

      spriteBatch.DrawString(font,
        String.Format(
          "NumCueInstances={0}, AttackTime={1}, ReleaseTime={2}, DopplerPitchScalar={3}, OrientationAngle={4}, Distance={5}",
          numCueInstances, attackTime, releaseTime, dopplerPitchScalar,
          orientationAngle, distance), position, clearColor);
      position.Y = position.Y + font.LineSpacing;
    }
  }
}