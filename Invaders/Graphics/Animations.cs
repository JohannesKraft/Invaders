using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Invaders.GameGraphics
{
    public class Animation
    {
        public double Duration { get; set; }
        public Rectangle DestionationRectangle { get; set; }
        public int CurrentFrame { get; set; }
        public int CurrentIteration { get; set; }
        public float Rotation { get; set; }
        public bool IsActive { get; set; }
        public int Tag { get; set; }
        public Color Color { get; set; }

        private double millisPerFrame;
        private double prevTime;
        private double startTime;
        private int totalFrames;
        private int totalIterations;

        public Animation(double duration, int frames, int iterations, Rectangle destinationRect, float rotation, int tag, Color color)
        {
            Duration = duration;
            millisPerFrame = duration / frames;
            startTime = -1;
            totalFrames = frames;
            totalIterations = iterations;

            DestionationRectangle = destinationRect;
            Rotation = rotation;

            Tag = tag;
            Color = color;
            CurrentFrame = 0;
            IsActive = true;
        }

        public void Update(double milliseconds)
        {
            if (startTime == -1)
            {
                startTime = milliseconds;
                prevTime = milliseconds;
            }

            if (milliseconds - prevTime > millisPerFrame)
            {
                CurrentFrame++;
                prevTime = milliseconds;
            }

            if (CurrentFrame >= totalFrames)
            {
                CurrentIteration++;
                if (CurrentIteration >= totalIterations && totalIterations != -1)
                {
                    IsActive = false;
                }
                else
                {
                    CurrentFrame = 0;
                }
            }
        }
    }

    public class AnimationSequence
    {
        public Sequence[] Sequences { get => sequences.ToArray(); }
        public Sequence ActiveSequence { get => sequences[SequenceIndex]; }
        public int SequenceIndex { get; set; }
        public Rectangle DestionationRectangle { get; set; }
        public int CurrentFrame { get; set; }
        public float Rotation { get; set; }
        public int Tag { get; set; }
        public Color Color { get; set; }

        private List<Sequence> sequences;


        public AnimationSequence(Rectangle destinationRect, float rotation, int tag, Color color)
        {
            Tag = tag;
            CurrentFrame = 0;
            DestionationRectangle = destinationRect;
            Rotation = rotation;
            Color = color;
            sequences = new List<Sequence>();
            sequences.Add(new Sequence(0, 0, 0));
            SequenceIndex = 0;
        }

        public Sequence Add(int startIndex, int endIndex, double duration)
        {
            Sequence seq = new Sequence(startIndex, endIndex, duration);
            sequences.Add(seq);
            return seq;
        }

        public void Remove(Sequence sequence)
        {
            sequences.Remove(sequence);
            SequenceIndex = 0;
        }

        public void Play(Sequence sequence)
        {
            int index = sequences.IndexOf(sequence);
            Play(index);

        }

        public void Play(int index)
        {
            SequenceIndex = index;
            ActiveSequence.Reversed = false;
        }

        public void PlayReversed(int index)
        {
            SequenceIndex = index;
            ActiveSequence.Reversed = true;
        }

        public void Update(double milliseconds)
        {
            ActiveSequence.Update(milliseconds);
            CurrentFrame = ActiveSequence.CurrentFrame;
        }

        public class Sequence
        {
            public double Duration { get; set; }
            public int CurrentFrame { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public bool IsActive { get; set; }
            public bool Reversed { get; set; }

            private double millisPerFrame;
            private double prevTime;
            private double startTime;
            private int totalFrames;

            public Sequence(int startIndex, int endIndex, double duration)
            {
                this.Duration = duration;
                this.totalFrames = endIndex - startIndex;
                this.millisPerFrame = Math.Abs(duration / totalFrames);
                this.startTime = -1;

                this.StartIndex = startIndex;
                this.EndIndex = endIndex;

                this.CurrentFrame = StartIndex;
                IsActive = true;
                Reversed = false;
            }

            public void Update(double milliseconds)
            {
                if (startTime == -1)
                {
                    startTime = milliseconds;
                    prevTime = milliseconds;
                }

                if (milliseconds - prevTime > millisPerFrame)
                {
                    if (!Reversed)
                    {
                        if (CurrentFrame < EndIndex)
                        {
                            CurrentFrame++;
                        }
                    }
                    else
                    {
                        if (CurrentFrame > StartIndex)
                        {
                            CurrentFrame--;
                        }
                    }

                    prevTime = milliseconds;
                }
            }
        }
    }

    public class AnimatedSprite
    {
        public int Tag { get; set; }
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int totalFrames;

        public AnimatedSprite(Texture2D texture, int rows, int columns, int tag)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            totalFrames = Rows * Columns;
            Tag = tag;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, float rotation, int currentFrame, Color color)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Vector2 origin = new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2);
            destinationRectangle.Location = destinationRectangle.Center;

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteEffects.None, 0);
        }
    }

    public class AnimationManager
    {
        public Animation[] Animations { get => animations.ToArray(); }
        public AnimationSequence[] AnimationsSequences { get => animationSequences.ToArray(); }
        public AnimatedSprite[] AnimatedSprites { get => animatedSprites.ToArray(); }

        private List<Animation> animations;
        private List<AnimationSequence> animationSequences;
        private List<AnimatedSprite> animatedSprites;

        public AnimationManager()
        {
            animations = new List<Animation>();
            animatedSprites = new List<AnimatedSprite>();
            animationSequences = new List<AnimationSequence>();
        }

        public AnimatedSprite AddSprite(Texture2D texture, int rows, int columns, int tag)
        {
            for (int i = 0; i < animatedSprites.Count; i++)
            {
                AnimatedSprite spr = animatedSprites[i];
                if (spr.Tag == tag)
                {
                    animatedSprites.Remove(spr);
                    i--;
                }
            }

            AnimatedSprite sprite = new AnimatedSprite(texture, rows, columns, tag);
            animatedSprites.Add(sprite);
            return sprite;
        }

        public Animation AddAnimation(double duration, int totalFrames, Rectangle destinationRect, int tag, Color? color = null)
        {
            if (!color.HasValue)
            {
                color = Color.White;
            }

            Animation anim = new Animation(duration, totalFrames, 1, destinationRect, 0.0F, tag, color.Value);
            animations.Add(anim);
            return anim;
        }

        public Animation AddAnimation(double duration, int totalFrames, int totalIterations, Rectangle destinationRect, float rotation, int tag, Color? color = null)
        {
            if (!color.HasValue)
            {
                color = Color.White;
            }

            Animation anim = new Animation(duration, totalFrames, totalIterations, destinationRect, rotation, tag, color.Value);
            animations.Add(anim);
            return anim;
        }

        public void AddAnimationSequence(AnimationSequence sequence)
        {
            animationSequences.Add(sequence);
        }

        public void RemoveSprite(AnimatedSprite sprite)
        {
            animatedSprites.Remove(sprite);
        }

        public void RemoveAnimation(Animation animation)
        {
            animation.IsActive = false;
            animations.Remove(animation);
        }

        public void RemoveAnimation(AnimationSequence sequence)
        {
            animationSequences.Remove(sequence);
        }

        public void Update(double milliseconds)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                Animation anim = animations[i];
                if (!anim.IsActive)
                {
                    animations.Remove(anim);
                    i--;
                }
            }

            foreach (var anim in animations)
            {
                anim.Update(milliseconds);
            }

            foreach (var seq in animationSequences)
            {
                seq.Update(milliseconds);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                Animation anim = animations[i];
                foreach (var sprite in animatedSprites)
                {
                    if (sprite.Tag == anim.Tag && anim.IsActive)
                    {
                        sprite.Draw(batch, anim.DestionationRectangle, anim.Rotation, anim.CurrentFrame, anim.Color);
                    }
                }
            }

            for (int i = 0; i < animationSequences.Count; i++)
            {
                AnimationSequence seq = animationSequences[i];
                foreach (var sprite in animatedSprites)
                {
                    if (sprite.Tag == seq.Tag)
                    {
                        sprite.Draw(batch, seq.DestionationRectangle, seq.Rotation, seq.CurrentFrame, seq.Color);
                    }
                }
            }
        }
    }
}
