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
    public readonly string Name;
    public readonly bool Positional;
    public readonly Keys Key;

    protected Sound(string name, bool positional, Keys key)
    {
      Name = name;
      Positional = positional;
      Key = key;
    }

    public abstract bool IsActive { get; }
    public abstract bool IsPlaying { get; }
    public abstract bool IsStopping { get; }
    public abstract bool IsStopped { get; }
    public abstract void Play(AudioListener listener, AudioEmitter emitter);
    public abstract void Apply3D(AudioListener listener, AudioEmitter emitter);

    public abstract void Stop(AudioStopOptions asAuthored);
    public abstract void Dispose();

    public abstract void DrawCueInfo(SpriteBatch spriteBatch, SpriteFont font,
      ref Vector2 position);
  }
}