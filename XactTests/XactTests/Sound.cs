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
  abstract class Sound : IDisposable
  {
    public static Keys PositionalModifierKey = Keys.LeftControl;

    public readonly string Name;
    public bool Positional;
    public readonly Keys Key;

    protected Sound(string name, Keys key)
    {
      Name = name;
      Key = key;
    }

    public abstract bool IsActive { get; }
    public abstract bool IsPrepared { get; }
    public abstract bool IsPlaying { get; }
    public abstract bool IsStopping { get; }
    public abstract bool IsStopped { get; }
    public abstract bool IsDisposed { get; }
    public abstract bool Paused { get; set; }
    public abstract void Play(AudioListener listener, AudioEmitter emitter, bool positional);
    public abstract void Apply3D(AudioListener listener, AudioEmitter emitter);

    public abstract void Stop(AudioStopOptions asAuthored);
    public abstract void Dispose();

    public abstract void DrawCueInfo(SpriteBatch spriteBatch, SpriteFont font,
      ref Vector2 position);

    public abstract void Deactivate();
  }
}