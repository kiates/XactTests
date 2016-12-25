// Copyright© 2016-2016 Chad C. Yates (cyates@dynfxdigital.com)

#region Using

using System;
using System.Diagnostics.Contracts;
using System.Text;
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

    public XnaSound(SoundEffect soundEffect, string name, bool looped, Keys key)
      : base(name, key)
    {
      this.soundEffect = soundEffect;
      Looped = looped;
      Instance = null;
    }

		public bool IsCreated
		{
			get
			{
				return false;
			}
		}

		public override bool IsActive { get { return Instance != null; } }

		public bool IsPreparing
		{
			get
			{
				return false;
			}
		}

		public override bool IsPrepared { get { return false; } }

    public override bool IsPlaying
    {
      get { return Instance != null && (Instance.State == SoundState.Playing || Instance.State == SoundState.Paused); }
    }

    public override bool IsStopping { get { return false; } }

    public override bool IsStopped
    {
      get { return Instance != null && !IsDisposed && Instance.State == SoundState.Stopped; }
    }

    public override bool IsDisposed { get {return IsActive && Instance.IsDisposed; } }

    public override bool Paused { get
    {
      return IsActive && Instance.State == SoundState.Paused;
    }
      set
      {
        if (value && !Paused)
        {
          Instance.Pause();
        }
        else if (!value && Paused)
        {
          Instance.Resume();
        }
      }
    }

    public override void Play(AudioListener listener, AudioEmitter emitter, bool positional)
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
	    if (IsPlaying)
	    {
		    Instance.Stop(true);
	    }

	    Instance.Dispose();
    }

    public override void Deactivate()
    {
      Instance = null;
    }

	  public override object Clone()
	  {
		  return new XnaSound(soundEffect, Name, Looped, Key);
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
			
			/*
      spriteBatch.DrawString(font,
        String.Format(
          "XnaSoundEffect: Name='{0}', IsPlaying={1}, IsStopping={2}, IsStopped={3}",
          Name, IsPlaying, IsStopping, IsStopped), position, clearColor);
      position.Y = position.Y + font.LineSpacing;

      spriteBatch.DrawString(font,
        String.Format("Volume={0}, Pitch={1}, Pan={2}", volume, pitch, pan),
        position, clearColor);
      position.Y = position.Y + font.LineSpacing;
*/

			StringBuilder sb = new StringBuilder(1000);

			sb.Clear();
			sb.Append(
				String.Format("XactCue: Nm='{0}',Crtd={1},Prpng={2},Prpd={3},Plyng={4},Psd={5},Stpng={6},Stpd={7},Dspd={8},Actv={9}",
					Name, IsCreated, IsPreparing, IsPrepared, IsPlaying, Paused, IsStopping, IsStopped, IsDisposed, IsActive));
			spriteBatch.DrawString(font, sb.ToString(), position, clearColor);
			position.Y = position.Y + font.LineSpacing;

			/*
			sb.Clear();
			sb.Append(
				String.Format(
					"#Cues={0},AtkTm={1},RlsTm={2},DpSclr={3},Ang={4},Dstnc={5}",
					numCueInstances, attackTime, releaseTime, dopplerPitchScalar,
					orientationAngle, distance));
			spriteBatch.DrawString(font, sb.ToString(), position, clearColor);
			position.Y = position.Y + font.LineSpacing;
			*/
		}
  }
}