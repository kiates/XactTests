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
  class XnaSound : Sound
  {
    public readonly SoundEffect soundEffect;
    public SoundEffectInstance Instance;
    public bool Looped;

    public XnaSound(SoundEffect soundEffect, string name, bool looped, bool positional, Keys key)
      : base(name, positional, key)
    {
      this.soundEffect = soundEffect;
      Looped = looped;
      Instance = null;
    }

    public override bool IsActive { get { return Instance != null; } }

    public override bool IsPlaying
    {
      get { return Instance != null && Instance.State == SoundState.Playing; }
    }

    public override bool IsStopping { get { return false; } }

    public override bool IsStopped
    {
      get { return Instance == null || Instance.State == SoundState.Stopped; }
    }

    public override void Play(AudioListener listener, AudioEmitter emitter)
    {
      Instance = soundEffect.CreateInstance();
      Instance.IsLooped = Looped;
      Instance.Play();
    }

    public override void Apply3D(AudioListener listener, AudioEmitter emitter)
    {
      Instance.Apply3D(listener, emitter);
    }

    public override void Stop(AudioStopOptions asAuthored)
    {
      Instance.Stop(asAuthored == AudioStopOptions.Immediate);
    }

    public override void Dispose()
    {
      Instance.Dispose();
      Instance = null;
    }

    public override void DrawCueInfo(SpriteBatch spriteBatch, SpriteFont font,
      ref Vector2 position)
    {
      float volume = 0;
      float pitch = 0;
      float pan = 0;

      Color clearColor = Color.LightGray;
      if (IsActive)
      {
        volume = Instance.Volume;
        pitch = Instance.Pitch;
        pan = Instance.Pan;

        if (IsPlaying)
        {
          clearColor = Color.White;
        }
        else if (IsStopped)
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
          "XnaSoundEffect: Name='{0}', IsPlaying={1}, IsStopping={2}, IsStopped={3}",
          Name, IsPlaying, IsStopping, IsStopped), position, clearColor);
      position.Y = position.Y + font.LineSpacing;

      spriteBatch.DrawString(font,
        String.Format("Volume={0}, Pitch={1}, Pan={2}", volume, pitch, pan),
        position, clearColor);
      position.Y = position.Y + font.LineSpacing;
    }
  }
}